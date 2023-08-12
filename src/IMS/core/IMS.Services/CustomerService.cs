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
        Task AddAsync(CustomerAddViewModel model, long userId);
        Task UpdateAsync(CustomerEditViewModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        Task<CustomerEditViewModel> GetByIdAsync(long id);
        string GetNameById(long id);        
        IList<CustomerDto> LoadAllCustomers();
        IList<(long, string)> LoadAllActiveCustomers();
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
        public async Task AddAsync(CustomerAddViewModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var count = _customerDao.GetCount(x => x.Email == model.Email && x.ContactNumber == model.ContactNumber);
                    if(count > 0)
                    {
                        throw new CustomException("Found another customer with this name");
                    }

                    var customer = new Customer()
                    {
                        Name = model.Name,
                        Address = model.Address,
                        ContactNumber = model.ContactNumber,
                        Email = model.Email,
                        Status = (int)model.Status,
                        Rank = await _customerDao.GetMaxRank("Customer") + 1,
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

        public async Task UpdateAsync(CustomerEditViewModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var customer = await _customerDao.GetByIdAsync(model.Id);
                    var namecount = _customerDao.GetCount(x => x.Name == model.Name);

                    if (customer == null)
                    {
                        throw new CustomException("No record found with this id!");
                    }
                    if (namecount > 1)
                    {
                        throw new CustomException("Already exist customer with this name");
                    }

                    customer.Name = model.Name;
                    customer.Address = model.Address;
                    customer.ContactNumber = model.ContactNumber;
                    customer.Email = model.Email;                    
                    customer.Status = (int)model.Status;
                    customer.ModifyBy = userId;
                    customer.ModificationDate = _timeService.Now;

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
                    if (customer == null)
                    {
                        throw new CustomException("No object with this id");
                    }
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

        public async Task<CustomerEditViewModel> GetByIdAsync(long id)
        {
            try
            {
                var customer = await _customerDao.GetByIdAsync(id);
                if (customer == null)
                {
                    throw new CustomException("No object with this id"); 
                }
                return new CustomerEditViewModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Address = customer.Address,
                    ContactNumber = customer.ContactNumber,
                    Email = customer.Email,
                    CreateBy = customer.CreateBy,
                    CreationDate = customer.CreationDate,
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

        public string GetNameById(long id)
        {
            var customer = _customerDao.GetById(id);
            return customer.Name;
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
                    filter = x => x.Name.Contains(searchBy) || x.Email.Contains(searchBy) || x.ContactNumber.Contains(searchBy)
                                    || x.Address.Contains(searchBy);
                }

                var result = _customerDao.LoadAllCustomers(filter, null, start, length, sortBy, sortDir);

                List<CustomerDto> customers = new List<CustomerDto>();
                foreach (Customer customer in result.data)
                {
                    customers.Add(
                        new CustomerDto
                        {
                            Id = customer.Id.ToString(),
                            Name = customer.Name,
                            Address = customer.Address,
                            ContactNumber = customer.ContactNumber,
                            Email = customer.Email,
                            CreateBy = _userService.GetUserName(customer.CreateBy),
                            CreationDate = customer.CreationDate.ToString(),                            
                            Status = ((Status)customer.Status).ToString(),
                            Rank = customer.Rank.ToString()                            
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
        

        public IList<(long, string)> LoadAllActiveCustomers()
        {
            List<(long, string)> customers = new List<(long, string)>();
            var allCustomers = _customerDao.GetCustomers(x => x.Status == (int)Status.Active);
            foreach (var supplier in allCustomers)
            {
                customers.Add((supplier.Id, supplier.Name));
            }
            return customers;
        }
        #endregion
    }
}
