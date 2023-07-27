using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules.Enum;
using IMS.BusinessRules.Exceptions;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface IPaymentService
    {
        Task AddAsync(PaymentModel model);

        (int total, int totalDisplay, IList<PaymentDto> records) LoadAllPayments(string searchBy, int length, int start, string sortBy, string sortDir);
        Task<PaymentDto> GetPaymentDetailsAsync(long operationId, int operationType);
    }
    #endregion

    public class PaymentService : BaseService, IPaymentService
    {
        #region Initializtion
        private readonly IPaymentDao _paymentDao;
        private readonly IBankDao _bankDao;

        public PaymentService(ISession session) : base(session)
        {
            _paymentDao = new PaymentDao(session);
            _bankDao = new BankDao(session);
        }

        public async Task AddAsync(PaymentModel model)
        {
            using(var transaction = _session.BeginTransaction())
            {
                try
                {
                    var payment = new Payment
                    {                        
                        BankId = model.BankId,
                        Bank = await _bankDao.GetByIdAsync(model.BankId),
                        OperationId = model.OperationId,
                        OperationType = (int)model.OperationType,
                        PaymentMethod = (int)model.PaymentMethod,
                        TransactionId = model.TransactionId,
                        IsPaid = true,
                        Amount = model.Amount,
                        PaymentDate = _timeService.Now
                    };

                    await _paymentDao.EditAsync(payment);
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex.Message, ex);
                    throw new CustomException("Something went wron during payment");
                }
            }
        }
        #endregion

        #region Operational Function


        #endregion

        #region Single Instance Loading

        public async Task<PaymentDto> GetPaymentDetailsAsync(long operationId, int operationType)
        {
            var payment = await Task.Run(() => _paymentDao.Get(
                x => x.OperationId == operationId && 
                    x.OperationType == operationType).FirstOrDefault());

            var paymentDto = new PaymentDto
            {
                Id = payment.Id,
                OperationType = (OperationType)payment.OperationType,
                OperationId = payment.OperationId,
                BankId = payment.BankId,
                Amount = payment.Amount,
                PaymentMethod = (PaymentMethod)payment.PaymentMethod,
                IsPaid = payment.IsPaid,               
                TransactionId = payment.TransactionId,
                PaymentDate = _timeService.Now
            };

            return paymentDto;
        }

        #endregion

        #region List Loading Function
        public (int total, int totalDisplay, IList<PaymentDto> records) LoadAllPayments(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Payment, bool>> filter = null;               

                var result = _paymentDao.LoadAllPayments(filter, null, start, length, sortBy, sortDir);

                List<PaymentDto> payments = new List<PaymentDto>();
                foreach (Payment payment in result.data)
                {
                    payments.Add(
                        new PaymentDto
                        {
                            Id = payment.Id,
                            BankId = payment.Bank.Id,
                            OperationId = payment.OperationId,
                            OperationType = (OperationType)payment.OperationType,
                            PaymentMethod = (PaymentMethod)payment.PaymentMethod,
                            TransactionId = payment.TransactionId,
                            Amount = payment.Amount,
                            IsPaid = payment.IsPaid                            
                        });
                }

                return (result.total, result.totalDisplay, payments);
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion

    }
}
