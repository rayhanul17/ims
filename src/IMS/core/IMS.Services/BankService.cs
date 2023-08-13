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
    public interface IBankService
    {
        #region Opperational Function
        Task AddAsync(BankAddViewModel model, long userId);
        Task UpdateAsync(BankEditViewModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        #endregion

        #region Single Instance Loading Function
        Task<BankEditViewModel> GetByIdAsync(long id);
        #endregion

        #region List Loading Function
        IList<(long, string)> LoadAllBanks();
        IList<(long, string)> LoadAllActiveBanks();
        Task<(int total, int totalDisplay, IList<BankDto> records)> LoadAllBanks(string searchBy, int length, int start, string sortBy, string sortDir);
        #endregion
    }

    public class BankService : BaseService, IBankService
    {
        #region Initializtion
        private readonly IBankDao _bankDao;

        public BankService(ISession session) : base(session)
        {
            _bankDao = new BankDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(BankAddViewModel model, long userId)
        {
            try
            {
                var count = _bankDao.GetCount(x => x.Name == model.Name);
                if (count > 0)
                {
                    throw new CustomException("Found another Bank with this name");
                }

                var bank = new Bank()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Status = (int)model.Status,
                    Rank = await _bankDao.GetMaxRank(typeof(Bank).Name) + 1,
                    CreateBy = userId,
                    CreationDate = _timeService.Now,
                };

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _bankDao.AddAsync(bank);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;
            }
        }

        public async Task UpdateAsync(BankEditViewModel model, long userId)
        {
            try
            {
                var bank = await _bankDao.GetByIdAsync(model.Id);
                var nameCount = _bankDao.GetCount(x => x.Name == model.Name);

                if (bank == null)
                {
                    throw new CustomException("No record found with this id!");
                }
                if (nameCount > 1)
                {
                    throw new CustomException("Already exist Bank with this name");
                }

                bank.Name = model.Name;
                bank.Description = model.Description;
                bank.Status = (int)model.Status;
                bank.ModifyBy = userId;
                bank.ModificationDate = _timeService.Now;

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _bankDao.EditAsync(bank);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;
            }
        }

        public async Task RemoveByIdAsync(long id, long userId)
        {
            try
            {
                var bank = await _bankDao.GetByIdAsync(id);
                bank.Status = (int)Status.Delete;
                bank.ModifyBy = userId;
                bank.ModificationDate = _timeService.Now;

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _bankDao.EditAsync(bank);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex);
                throw;
            }
        }
        #endregion

        #region Single Instance Loading Function
        public async Task<BankEditViewModel> GetByIdAsync(long id)
        {
            try
            {
                var bank = await _bankDao.GetByIdAsync(id);
                if (bank == null)
                {
                    throw new CustomException("No bank found with this id");
                }
                return new BankEditViewModel
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
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion

        #region List Loading Function
        public async Task<(int total, int totalDisplay, IList<BankDto> records)> LoadAllBanks(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Bank, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) || x.Description.Contains(searchBy);
                }

                var result = _bankDao.GetDynamic(filter, null, start, length, sortBy, sortDir);

                List<BankDto> banks = new List<BankDto>();
                foreach (Bank bank in result.data)
                {
                    banks.Add(
                        new BankDto
                        {
                            Id = bank.Id.ToString(),
                            Name = bank.Name,
                            Description = bank.Description,
                            CreateBy = await _userService.GetUserNameAsync(bank.CreateBy),
                            CreationDate = bank.CreationDate.ToString(),
                            Status = ((Status)bank.Status).ToString(),
                            Rank = bank.Rank.ToString(),

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

        public IList<(long, string)> LoadAllBanks()
        {
            List<(long, string)> banks = new List<(long, string)>();
            try
            {
                var allBanks = _bankDao.Get(x => x.Status != (int)Status.Delete);
                foreach (var bank in allBanks)
                {
                    banks.Add((bank.Id, bank.Name));
                }
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }

            return banks;
        }

        public IList<(long, string)> LoadAllActiveBanks()
        {
            List<(long, string)> banks = new List<(long, string)>();
            try
            {
                var allBanks = _bankDao.Get(x => x.Status == (int)Status.Active);
                foreach (var bank in allBanks)
                {
                    banks.Add((bank.Id, bank.Name));
                }
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }

            return banks;
        }
        #endregion
    }
}
