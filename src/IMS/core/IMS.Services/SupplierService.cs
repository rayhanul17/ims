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
using System.Reflection;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface ISupplierService
    {
        Task AddAsync(SupplierAddModel model, long userId);
        Task UpdateAsync(SupplierEditModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        Task<SupplierEditModel> GetByIdAsync(long id);
        IList<SupplierDto> LoadAllSupliers();
        (int total, int totalDisplay, IList<SupplierDto> records) LoadAllSuppliers(string searchBy, int length, int start, string sortBy, string sortDir);
    }
    #endregion

    public class SupplierService : BaseService, ISupplierService
    {
        #region Initializtion
        private readonly ISupplierDao _supplierDao;

        public SupplierService(ISession session) : base(session)
        {
            _supplierDao = new SupplierDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(SupplierAddModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var count = _supplierDao.GetCount(x => x.Email == model.Email && x.ContactNumber == model.ContactNumber);
                    if (count > 0)
                    {
                        throw new NameDuplicateException("Found another supplier with this name");
                    }

                    var supplier = new Supplier()
                    {
                        Name = model.Name,
                        Address = model.Address,
                        ContactNumber = model.ContactNumber,
                        Email = model.Email,
                        Status = (int)model.Status,
                        Rank = model.Rank,
                        CreateBy = userId,
                        CreationDate = _timeService.Now,
                    };

                    await _supplierDao.AddAsync(supplier);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error(ex.Message, ex);

                    throw;
                }
            }
        }

        public async Task UpdateAsync(SupplierEditModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var objectcount = _supplierDao.GetCount(x => x.Id == model.Id);
                    var namecount = _supplierDao.GetCount(x => x.Name == model.Name);

                    if (namecount < 1)
                    {
                        throw new InvalidOperationException("No record found with this id!");
                    }
                    if (namecount > 1)
                    {
                        throw new NameDuplicateException("Already exist supplier with this name");
                    }

                    var supplier = new Supplier()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Address = model.Address,
                        ContactNumber = model.ContactNumber,
                        Email = model.Email,
                        Status = (int)model.Status,
                        CreateBy = model.CreateBy,
                        CreationDate = DateTime.Parse(model.CreationDate),
                        ModifyBy = userId,
                        ModificationDate = _timeService.Now
                    };

                    await _supplierDao.EditAsync(supplier);
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
                    //await _supplierDao.RemoveByIdAsync(id);
                    var supplier = await _supplierDao.GetByIdAsync(id);
                    supplier.Status = (int)Status.Delete;
                    supplier.ModifyBy = userId;
                    supplier.ModificationDate = _timeService.Now;
                    await _supplierDao.EditAsync(supplier);
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex);
                    transaction.Rollback();
                }
            }
        }

        public async Task<SupplierEditModel> GetByIdAsync(long id)
        {
            try
            {
                var supplier = await _supplierDao.GetByIdAsync(id);
                if (supplier == null)
                {
                    throw new ArgumentNullException(nameof(supplier)); 
                }
                return new SupplierEditModel
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    Address = supplier.Address,
                    ContactNumber = supplier.ContactNumber,
                    Email = supplier.Email,
                    CreateBy = supplier.CreateBy,
                    CreationDate = (supplier.CreationDate).ToString(),
                    ModifyBy = supplier.ModifyBy,
                    ModificationDate = supplier.ModificationDate,
                    Status = (Status)supplier.Status,
                    Rank = supplier.Rank,
                    VersionNumber = supplier.VersionNumber,
                    BusinessId = supplier.BusinessId,
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
        public (int total, int totalDisplay, IList<SupplierDto> records) LoadAllSuppliers(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Supplier, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) && x.Status != (int)Status.Delete;
                }

                var result = _supplierDao.LoadAllSuppliers(filter, null, start, length, sortBy, sortDir);

                List<SupplierDto> suppliers = new List<SupplierDto>();
                foreach (Supplier supplier in result.data)
                {
                    suppliers.Add(
                        new SupplierDto
                        {
                            Id = supplier.Id,
                            Name = supplier.Name,
                            Address = supplier.Address,
                            ContactNumber = supplier.ContactNumber,
                            Email = supplier.Email,
                            CreateBy = supplier.CreateBy,
                            CreationDate = supplier.CreationDate,                            
                            Status = (Status)supplier.Status,
                            Rank = supplier.Rank,
                            VersionNumber = supplier.VersionNumber,
                            BusinessId = supplier.BusinessId,
                        });
                }

                return (result.total, result.totalDisplay, suppliers);
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }
        
        public IList<SupplierDto> LoadAllSupliers()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
