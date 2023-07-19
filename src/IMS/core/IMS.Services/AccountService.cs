using IMS.BusinessModel.Entity;
using IMS.Dao;
using NHibernate;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    public interface IAccountService
    {
        Task CreateUserAsync(string name, long id, long creatorId);
        string GetUserName(long userId);
    }
    public class AccountService : BaseService, IAccountService
    {
        private readonly IApplicationUserDao _userDao;

        public AccountService(ISession session) : base(session)
        {
            _userDao = new ApplicationUserDao(session);
        }

        public async Task CreateUserAsync(string name, long id, long creatorId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var user = new ApplicationUser()
                    {
                        AspNetUsersId = id,
                        Name = name,
                        CreateBy = creatorId,
                        CreationDate = _timeService.Now,
                    };

                    await _userDao.AddAsync(user);
                    transaction.Commit();

                    _serviceLogger.Info("Data Saved!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error(ex.Message, ex);
                }
            }
        }

        public string GetUserName(long userId)
        {
            Expression<Func<ApplicationUser, bool>> filter = null;
            filter = x => x.AspNetUsersId.Equals(userId);

            var user =  _userDao.GetUser(filter);

            return user.Name + $"<{user.Id}>";
        }
    }
}
