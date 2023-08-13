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
    public interface ISupplierService
    {
        #region Opperational Function
        Task AddAsync(SupplierAddViewModel model, long userId);
        Task UpdateAsync(SupplierEditViewModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        Task<string> GetNameByIdAsync(long id);
        #endregion

        #region Single Instance Loading Function
        Task<SupplierEditViewModel> GetByIdAsync(long id);
        #endregion

        #region List Loading Function
        IList<(long, string)> LoadAllActiveSuppliers();
        Task<(int total, int totalDisplay, IList<SupplierDto> records)> LoadAllSuppliers(string searchBy, int length, int start, string sortBy, string sortDir);
        #endregion
    }

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
        public async Task AddAsync(SupplierAddViewModel model, long userId)
        {
            try
            {
                var count = _supplierDao.GetCount(x => x.Email == model.Email && x.ContactNumber == model.ContactNumber);

                if (count > 0)
                {
                    throw new CustomException("Found another supplier with this name");
                }

                var supplier = new Supplier()
                {
                    Name = model.Name,
                    Address = model.Address,
                    ContactNumber = model.ContactNumber,
                    Email = model.Email,
                    Status = (int)model.Status,
                    Rank = await _supplierDao.GetMaxRank(typeof(Supplier).Name) + 1,
                    CreateBy = userId,
                    CreationDate = _timeService.Now,
                };

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _supplierDao.AddAsync(supplier);
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
                throw;            
            }
        }

        public async Task UpdateAsync(SupplierEditViewModel model, long userId)
        {
            try
            {
                var supplier = await _supplierDao.GetByIdAsync(model.Id);
                var namecount = _supplierDao.GetCount(x => x.Name == model.Name);

                if (supplier == null)
                {
                    throw new CustomException("No record found with this id!");
                }
                if (namecount > 1)
                {
                    throw new CustomException("Already exist supplier with this name");
                }

                supplier.Name = model.Name;
                supplier.Address = model.Address;
                supplier.ContactNumber = model.ContactNumber;
                supplier.Email = model.Email;
                supplier.Status = (int)model.Status;
                supplier.ModifyBy = userId;
                supplier.ModificationDate = _timeService.Now;

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _supplierDao.EditAsync(supplier);
                        transaction.Commit();
                    }
                    catch(Exception ex)
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
                var supplier = await _supplierDao.GetByIdAsync(id);
                if (supplier == null)
                {
                    throw new CustomException("No object with this id");
                }
                supplier.Status = (int)Status.Delete;
                supplier.ModifyBy = userId;
                supplier.ModificationDate = _timeService.Now;

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _supplierDao.EditAsync(supplier);
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
                throw ex;
            }            
        }        

        public async Task<string> GetNameByIdAsync(long id)
        {
            try
            {
                var supplier = await _supplierDao.GetByIdAsync(id);
                return supplier.Name;
            }
            catch(Exception ex)
            {
                _serviceLogger.Error($"{ex.Message}", ex);
                throw ex;
            }
        }

        #endregion

        #region Single Instance Loading
        public async Task<SupplierEditViewModel> GetByIdAsync(long id)
        {
            try
            {
                var supplier = await _supplierDao.GetByIdAsync(id);
                if (supplier == null)
                {
                    throw new ArgumentNullException(nameof(supplier));
                }
                return new SupplierEditViewModel
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    Address = supplier.Address,
                    ContactNumber = supplier.ContactNumber,
                    Email = supplier.Email,
                    CreateBy = supplier.CreateBy,                    
                    Status = (Status)supplier.Status,                    
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
        public async Task<(int total, int totalDisplay, IList<SupplierDto> records)> LoadAllSuppliers(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Supplier, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) || x.Email.Contains(searchBy) || x.ContactNumber.Contains(searchBy)
                                    || x.Address.Contains(searchBy);
                }

                var result = _supplierDao.GetDynamic(filter, null, start, length, sortBy, sortDir);

                List<SupplierDto> suppliers = new List<SupplierDto>();
                foreach (Supplier supplier in result.data)
                {
                    suppliers.Add(
                        new SupplierDto
                        {
                            Id = supplier.Id.ToString(),
                            Name = supplier.Name,
                            Address = supplier.Address,
                            ContactNumber = supplier.ContactNumber,
                            Email = supplier.Email,
                            CreateBy = await _userService.GetUserNameAsync(supplier.CreateBy),
                            CreationDate = supplier.CreationDate.ToString(),
                            Status = ((Status)supplier.Status).ToString(),
                            Rank = supplier.Rank.ToString()
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

        public IList<(long, string)> LoadAllActiveSuppliers()
        {
            List<(long, string)> suppliers = new List<(long, string)>();
            var allSuppliers = _supplierDao.Get(x => x.Status == (int)Status.Active);
            foreach (var supplier in allSuppliers)
            {
                suppliers.Add((supplier.Id, supplier.Name));
            }
            return suppliers;
        }
        #endregion

    }
}
