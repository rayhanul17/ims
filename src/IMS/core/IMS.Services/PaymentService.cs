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
    public interface IPaymentService
    {
        #region Opperational Function
        Task AddAsync(long operationId, OperationType operationType, decimal Amount, long userId, string voucherId);
        Task MakePaymentAsync(PaymentViewModel model, long userId);
        #endregion

        #region Single Instance Loading Function
        Task<PaymentViewModel> GetPaymentByIdAsync(long paymentId);
        Task<PaymentReportDto> GetPaymentDetailsAsync(long paymentId);
        #endregion

        #region List Loading Function
        (int total, int totalDisplay, IList<PaymentReportDto> records) LoadAllPayments(string searchBy, int length, int start, string sortBy, string sortDir);
        #endregion
    }

    public class PaymentService : BaseService, IPaymentService
    {
        #region Initialization
        private readonly IPaymentDao _paymentDao;
        private readonly IBankDao _bankDao;

        public PaymentService(ISession session) : base(session)
        {
            _paymentDao = new PaymentDao(session);
            _bankDao = new BankDao(session);
        }

        public async Task AddAsync(long operationId, OperationType operationType, decimal Amount, long userId, string voucherId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var payment = new Payment
                    {
                        PurchaseId = operationId,
                        SaleId = operationId,
                        VoucherId = voucherId,
                        OperationType = (int)operationType,
                        TotalAmount = Amount,
                        PaidAmount = 0,
                        CreateBy = userId,
                        CreationDate = _timeService.Now,
                        Rank = await _paymentDao.GetMaxRank() + 1,
                        Status = (int)Status.Active
                    };

                    var paymentId = await _paymentDao.AddAsync(payment);

                    var tableName = Convert.ToString((OperationType)payment.OperationType);
                    var query = $"UPDATE {tableName} SET PaymentId = {paymentId} WHERE Id = {payment.PurchaseId};";
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

        public async Task MakePaymentAsync(PaymentViewModel model, long userId)
        {

            try
            {
                var payment = await _paymentDao.GetByIdAsync(model.PaymentId);
                if (payment == null)
                {
                    throw new CustomException("Wrong payment id found");
                }
                else
                {
                    if (payment.TotalAmount == model.TotalAmount && model.Amount > 0 && model.Amount <= payment.TotalAmount - payment.PaidAmount)
                    {
                        var bank = await _bankDao.GetByIdAsync(model.BankId);
                        if (model.PaymentMethod == PaymentMethod.Bank && bank == null)
                        {
                            throw new CustomException("Bank not found");
                        }

                        var paymentDetails = new PaymentDetails
                        {
                            TransactionId = model.TransactionId,
                            Amount = model.Amount,
                            PaymentDate = _timeService.Now,
                            PaymentMethod = (int)model.PaymentMethod,
                            Payment = payment,
                            Bank = bank,
                            CreateBy = userId,
                            CreationDate = _timeService.Now,
                            Status = (int)Status.Active,
                            Rank = await _paymentDao.GetMaxRank(typeof(PaymentDetails).Name) + 1
                        };
                        
                        payment.PaidAmount += model.Amount;
                        payment.PaymentDetails.Add(paymentDetails);

                        using (var transaction = _session.BeginTransaction())
                        {
                            try
                            {
                                await _paymentDao.EditAsync(payment);

                                if (payment.PaidAmount == payment.TotalAmount)
                                {
                                    var tableName = Convert.ToString((OperationType)payment.OperationType);
                                    var query = $"UPDATE {tableName} SET IsPaid = 1 WHERE Id = {payment.PurchaseId};";
                                    await _paymentDao.ExecuteUpdateDeleteQuery(query);
                                }

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                        }
                    }
                    else
                    {
                        throw new CustomException("Modified payment information");
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
        #endregion

        #region Single Instance Loading Function
        public async Task<PaymentViewModel> GetPaymentByIdAsync(long paymentId)
        {
            try
            {
                var payment = await Task.Run(() => _paymentDao.Get(x => x.Id == paymentId).FirstOrDefault());
                if (payment == null)
                {
                    throw new CustomException("Invalid payment id");
                }
                var model = new PaymentViewModel
                {
                    PaymentId = payment.Id,
                    TotalAmount = payment.TotalAmount,
                    DueAmount = payment.TotalAmount - payment.PaidAmount,
                    Amount = payment.TotalAmount - payment.PaidAmount
                };

                return model;
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error($"{ex.Message}", ex);
                throw ex;
            }
        }

        public async Task<PaymentReportDto> GetPaymentDetailsAsync(long paymentId)
        {
            try
            {
                var payment = await Task.Run(() => _paymentDao.Get(
                    x => x.Id == paymentId).FirstOrDefault());
                
                if (payment == null)
                {
                    throw new CustomException("Invalid payment id");
                }

                var paymentDetails = new List<PaymentInformation>();

                foreach (var item in payment.PaymentDetails)
                {
                    Bank bank = null;
                    if (item.Bank != null)
                    {
                        bank = await _bankDao.GetByIdAsync(item.Bank.Id);
                    }
                    paymentDetails.Add(
                        new PaymentInformation
                        {
                            PaymentMethod = ((PaymentMethod)item.PaymentMethod).ToString(),
                            Amount = item.Amount.ToString(),
                            TransactionId = item.TransactionId,
                            PaymentDate = item.PaymentDate.ToString(),
                            Bank = bank?.Name
                        });
                }

                var paymentDto = new PaymentReportDto
                {
                    OperationType = ((OperationType)payment.OperationType).ToString(),
                    PaidAmount = payment.PaidAmount.ToString(),
                    TotalAmount = payment.TotalAmount.ToString(),
                    DueAmount = (payment.TotalAmount - payment.PaidAmount).ToString(),
                    PaymentDetails = paymentDetails
                };

                return paymentDto;
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

        #endregion

        #region List Loading Function
        public (int total, int totalDisplay, IList<PaymentReportDto> records) LoadAllPayments(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {                
                Expression<Func<Payment, bool>> filter = null;

                if (!string.IsNullOrWhiteSpace(searchBy))
                {
                    searchBy = searchBy.Trim();
                    filter = x => x.VoucherId.Contains(searchBy);
                }

                var result = _paymentDao.LoadAllPayments(filter, null, start, length, sortBy, sortDir);

                List<PaymentReportDto> payments = new List<PaymentReportDto>();
                foreach (Payment payment in result.data)
                {
                    payments.Add(
                        new PaymentReportDto
                        {
                            Id = payment.Id.ToString(),
                            OperationType = ((OperationType)payment.OperationType).ToString(),
                            TotalAmount = payment.TotalAmount.ToString(),
                            PaidAmount = payment.PaidAmount.ToString(),
                            DueAmount = (payment.TotalAmount - payment.PaidAmount).ToString(),
                            Rank = payment.Rank.ToString(),
                            VoucherId = payment.VoucherId
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
