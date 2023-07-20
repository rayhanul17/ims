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
    public interface ICustomerService
    {
        Task AddAsync(CustomerAddModel model, long userId);
        Task UpdateAsync(CustomerEditModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        Task<CustomerEditModel> GetByIdAsync(long id);
        IList<CustomerDto> LoadAllCustomers();
        (int total, int totalDisplay, IList<CustomerDto> records) LoadAllCustomers(string searchBy, int length, int start, string sortBy, string sortDir);
    }
    #endregion

    public class CustomerService : BaseService, ICustomerService
    {
        #region Initializtion
        private readonly ICustomerDao _customerDao;

        public CustomerService(ISession session) : base(session)
        {
            _customerDao = new CustomerDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(CustomerAddModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var count = _customerDao.GetCount(x => x.Email == model.Email && x.ContactNumber == model.ContactNumber);
                    if(count > 0)
                    {
                        throw new DuplicateException("Found another customer with this name");
                    }

                    var customer = new Customer()
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

                    await _customerDao.AddAsync(customer);
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

        public async Task UpdateAsync(CustomerEditModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var objectcount = _customerDao.GetCount(x => x.Id == model.Id);
                    var namecount = _customerDao.GetCount(x => x.Name == model.Name);

                    if (namecount < 1)
                    {
                        throw new InvalidOperationException("No record found with this id!");
                    }
                    if (namecount > 1)
                    {
                        throw new DuplicateException("Already exist customer with this name");
                    }

                    var customer = new Customer()
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

                    await _customerDao.EditAsync(customer);
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
                    //await _customerDao.RemoveByIdAsync(id);
                    var customer = await _customerDao.GetByIdAsync(id);
                    customer.Status = (int)Status.Delete;
                    customer.ModifyBy = userId;
                    customer.ModificationDate = _timeService.Now;
                    await _customerDao.EditAsync(customer);
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex);
                    transaction.Rollback();
                }
            }
        }

        public async Task<CustomerEditModel> GetByIdAsync(long id)
        {
            try
            {
                var customer = await _customerDao.GetByIdAsync(id);
                if (customer == null)
                {
                    throw new ArgumentNullException(nameof(customer)); 
                }
                return new CustomerEditModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Address = customer.Address,
                    ContactNumber = customer.ContactNumber,
                    Email = customer.Email,
                    CreateBy = customer.CreateBy,
                    CreationDate = (customer.CreationDate).ToString(),
                    ModifyBy = customer.ModifyBy,
                    ModificationDate = customer.ModificationDate,
                    Status = (Status)customer.Status,
                    Rank = customer.Rank,
                    VersionNumber = customer.VersionNumber,
                    BusinessId = customer.BusinessId,
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
        public (int total, int totalDisplay, IList<CustomerDto> records) LoadAllCustomers(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Customer, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) && x.Status != (int)Status.Delete;
                }

                var result = _customerDao.LoadAllCustomers(filter, null, start, length, sortBy, sortDir);

                List<CustomerDto> customers = new List<CustomerDto>();
                foreach (Customer customer in result.data)
                {
                    customers.Add(
                        new CustomerDto
                        {
                            Id = customer.Id,
                            Name = customer.Name,
                            Address = customer.Address,
                            ContactNumber = customer.ContactNumber,
                            Email = customer.Email,
                            CreateBy = customer.CreateBy,
                            CreationDate = customer.CreationDate,                            
                            Status = (Status)customer.Status,
                            Rank = customer.Rank,
                            VersionNumber = customer.VersionNumber,
                            BusinessId = customer.BusinessId,
                        });
                }

                return (result.total, result.totalDisplay, customers);
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }        

        public IList<CustomerDto> LoadAllCustomers()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
