using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules.Enum;
using IMS.BusinessRules.Exceptions;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface IBankService
    {
        Task AddAsync(BankAddModel model, long userId);
        Task UpdateAsync(BankEditModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        Task<BankEditModel> GetByIdAsync(long id);
        IList<(long, string)> LoadAllBanks();
        IList<(long, string)> LoadAllActiveBanks();
        (int total, int totalDisplay, IList<BankDto> records) LoadAllBanks(string searchBy, int length, int start, string sortBy, string sortDir);
    }
    #endregion

    public class BankService : BaseService, IBankService
    {
        #region Initializtion
        private readonly IBankDao _bankDao;  
        private readonly IPaymentDao _paymentDao;

        public BankService(ISession session) : base(session)
        {
            _bankDao = new BankDao(session);  
            _paymentDao = new PaymentDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(BankAddModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var count = _bankDao.GetCount(x => x.Name == model.Name);
                    if(count > 0)
                    {
                        throw new CustomException("Found another Bank with this name");
                    }

                    var bank = new Bank()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Status = (int)model.Status,
                        Rank = await _bankDao.GetMaxRank("Bank") + 1,
                        CreateBy = userId,
                        CreationDate = _timeService.Now,
                    };

                    await _bankDao.AddAsync(bank);
                    transaction.Commit();
                }
                catch(CustomException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error(ex.Message, ex);

                    throw;
                }
            }
        }

        public async Task UpdateAsync(BankEditModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var bank = await _bankDao.GetByIdAsync(model.Id);
                    var namecount = _bankDao.GetCount(x => x.Name == model.Name);

                    if (bank == null)
                    {
                        throw new CustomException("No record found with this id!");
                    }
                    if (namecount > 1)
                    {
                        throw new CustomException("Already exist Bank with this name");
                    }

                    bank.Name = model.Name;
                    bank.Description = model.Description;
                    bank.Status = (int)model.Status;
                    bank.ModifyBy = userId;
                    bank.ModificationDate = _timeService.Now;                    

                    await _bankDao.EditAsync(bank);
                    transaction.Commit();

                    _serviceLogger.Info("Data Saved!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error(ex.Message, ex);

                    throw;
                }
            }
        }
        
        public async Task RemoveByIdAsync(long id, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    //var count = _paymentDao.GetCount(x => x.PaymentDetails.);
                    //if (count > 0)
                    //{
                    //    throw new CustomException("Found payment under this bank");
                    //}
                    //await _bankDao.RemoveByIdAsync(id);
                    var bank = await _bankDao.GetByIdAsync(id);
                    bank.Status = (int)Status.Delete;
                    bank.ModifyBy = userId;
                    bank.ModificationDate = _timeService.Now;
                    await _bankDao.EditAsync(bank);
                    transaction.Commit();

                }
                catch(CustomException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex);
                    transaction.Rollback();
                }
            }
        }

        public async Task<BankEditModel> GetByIdAsync(long id)
        {
            try
            {
                var bank = await _bankDao.GetByIdAsync(id);
                if (bank == null)
                {
                    throw new ArgumentNullException(nameof(bank));
                }
                return new BankEditModel
                {
                    Id = bank.Id,
                    Name = bank.Name,
                    Description = bank.Description,
                    CreateBy = bank.CreateBy,
                    CreationDate = bank.CreationDate,
                    ModifyBy = bank.ModifyBy,
                    ModificationDate = bank.ModificationDate,
                    Status = (Status)bank.Status,
                    Rank = bank.Rank,
                    VersionNumber = bank.VersionNumber,
                    BusinessId = bank.BusinessId,
                };
            }
            catch(Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion

        #region Single Instance Loading
        #endregion

        #region List Loading Function
        public (int total, int totalDisplay, IList<BankDto> records) LoadAllBanks(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Bank, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) || x.Description.Contains(searchBy);
                }

                var result = _bankDao.LoadAllBanks(filter, null, start, length, sortBy, sortDir);

                List<BankDto> banks = new List<BankDto>();
                foreach (Bank bank in result.data)
                {
                    banks.Add(
                        new BankDto
                        {
                            Id = bank.Id,
                            Name = bank.Name,
                            Description =bank.Description,
                            CreateBy = bank.CreateBy,
                            CreationDate = bank.CreationDate,                            
                            Status = (Status)bank.Status,
                            Rank = bank.Rank,
                            VersionNumber = bank.VersionNumber,
                            BusinessId = bank.BusinessId,
                        });
                }

                return (result.total, result.totalDisplay, banks);
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }
        
        public IList<(long,string)> LoadAllBanks()
        {
            List<(long, string)> banks = new List<(long, string)> ();
            var allBanks = _bankDao.GetBank(x => x.Status != (int)Status.Delete);
            foreach (var bank in allBanks)
            {
                banks.Add((bank.Id, bank.Name));
            }
            return banks;
        }

        public IList<(long, string)> LoadAllActiveBanks()
        {
            List<(long, string)> banks = new List<(long, string)>();
            var allBanks = _bankDao.GetBank(x => x.Status == (int)Status.Active);
            foreach (var bank in allBanks)
            {
                banks.Add((bank.Id, bank.Name));
            }
            return banks;
        }
        #endregion

    }
}
