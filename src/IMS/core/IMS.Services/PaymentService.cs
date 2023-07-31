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
        Task AddAsync(long operationId, OperationType operationType, decimal Amount);
        Task MakePaymentAsync(PaymentModel model);
        (int total, int totalDisplay, IList<PaymentReportDto> records) LoadAllPayments(string searchBy, int length, int start, string sortBy, string sortDir);
        Task<PaymentModel> GetPaymentByIdAsync(long paymentId);
        Task<PaymentReportDto> GetPaymentDetailsAsync(long paymentId);
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

        public async Task AddAsync(long operationId, OperationType operationType, decimal Amount)
        {
            using(var transaction = _session.BeginTransaction())
            {
                try
                {
                    var payment = new Payment
                    {                        
                        OperationId = operationId,
                        OperationType = (int)operationType,
                        TotalAmount = Amount,
                        PaidAmount = 0
                    };

                    var paymentId = await _paymentDao.AddAsync(payment);

                    var tableName = Convert.ToString((OperationType)payment.OperationType);
                    var query = $"UPDATE TableName SET PaymentId = {paymentId} WHERE Id = {payment.OperationId};";
                    query = query.Replace("TableName", tableName);

                    await _paymentDao.ExecuteUpdateDeleteQuery(query);

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex.Message, ex);
                    throw new CustomException("Something went wrong during payment creation");
                }
            }
        }

        public async Task MakePaymentAsync(PaymentModel model)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var payment = await _paymentDao.GetByIdAsync(model.PaymentId);
                    var bank = await _bankDao.GetByIdAsync(model.BankId);

                    var paymentDetails = new PaymentDetails
                    {
                        BankId = model.BankId,
                        TransactionId = model.TransactionId,
                        Amount = model.Amount,
                        PaymentDate = _timeService.Now,
                        PaymentMethod = (int)model.PaymentMethod,
                        Payment = payment,
                        Bank = bank,                       
                        
                    };
                    payment.PaidAmount += model.Amount;
                    
                    payment.PaymentDetails.Add(paymentDetails);
                    await _paymentDao.EditAsync(payment);

                    if(payment.PaidAmount == payment.TotalAmount)
                    {
                        var tableName = Convert.ToString((OperationType)payment.OperationType);
                        var query = $"UPDATE TableName SET IsPaid = 1 WHERE Id = {payment.OperationId};";
                        query = query.Replace("TableName", tableName);

                        await _paymentDao.ExecuteUpdateDeleteQuery(query);
                    }

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex.Message, ex);
                    transaction.Rollback();
                    throw new CustomException("Something went wrong during payment creation");
                }
            }
        }
        #endregion

        #region Operational Function


        #endregion

        #region Single Instance Loading

        public async Task<PaymentModel> GetPaymentByIdAsync(long paymentId)
        {
            var payment = await Task.Run(() => _paymentDao.Get(x => x.Id == paymentId).FirstOrDefault());

            var model = new PaymentModel
            {
                PaymentId = payment.Id,
                TotalAmount = payment.TotalAmount,
                DueAmount = payment.TotalAmount - payment.PaidAmount,
                Amount = payment.TotalAmount - payment.PaidAmount
            };

            return model;
        }
        public async Task<PaymentReportDto> GetPaymentDetailsAsync(long paymentId)
        {
            var payment = await Task.Run(() => _paymentDao.Get(
                x => x.Id == paymentId).FirstOrDefault());

            var paymentDetails = new List<PaymentInformation>();

            foreach( var item in payment.PaymentDetails)
            {
                var bank = await _bankDao.GetByIdAsync(item.Bank.Id);
                paymentDetails.Add(
                    new PaymentInformation
                    {
                        PaymentMethod = (PaymentMethod)item.PaymentMethod,
                        Amount = item.Amount,
                        TransactionId = item.TransactionId,
                        PaymentDate = item.PaymentDate,
                        Bank = bank.Name
                    });
            }

            var paymentDto = new PaymentReportDto
            {
                OperationType = (OperationType)payment.OperationType,
                PaidAmount = payment.PaidAmount,
                TotalAmount = payment.TotalAmount,
                DueAmount = payment.TotalAmount - payment.PaidAmount,
                PaymentDetails = paymentDetails
            };

            return paymentDto;
        }

        #endregion

        #region List Loading Function
        public (int total, int totalDisplay, IList<PaymentReportDto> records) LoadAllPayments(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Payment, bool>> filter = null;
                
                if (!string.IsNullOrWhiteSpace(searchBy))
                {
                    filter = x => x.Id == Convert.ToInt64(searchBy) || x.OperationId == Convert.ToInt64(searchBy);
                }

                var result = _paymentDao.LoadAllPayments(filter, null, start, length, sortBy, sortDir);

                List<PaymentReportDto> payments = new List<PaymentReportDto>();
                foreach (Payment payment in result.data)
                {
                    payments.Add(
                        new PaymentReportDto
                        {
                            Id = payment.Id,                      
                            OperationType = (OperationType)payment.OperationType,
                            TotalAmount = payment.TotalAmount,
                            PaidAmount = payment.PaidAmount,
                            DueAmount = payment.TotalAmount - payment.PaidAmount,
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
