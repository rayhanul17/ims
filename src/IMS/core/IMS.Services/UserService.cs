using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using IMS.BusinessRules.Exceptions;
using IMS.Dao;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface IUserService
    {
        Task CreateUserAsync(string name, string email, long aspid, long creatorId);
        Task UpdateUserAsync(string name, string email, long aspid, long creatorId);
        Task<bool> IsActiveUserAsync(string email);
        Task<string> GetUserNameAsync(long userId);
        Task BlockAsync(long userId);
        IList<(long, string)> LoadAllActiveUsers();
        Task<(int total, int totalDisplay, IList<UserDto> records)> LoadAllUsers(string searchBy = null,
            int length = 10, int start = 1, string sortBy = null, string sortDir = null);
    }
    #endregion

    public class UserService : IUserService
    {
        #region Initialization
        private readonly IApplicationUserDao _userDao;
        private readonly ISession _session;
        private readonly ITimeService _timeService;
        private readonly ILog _serviceLogger = LogManager.GetLogger("ServiceLogger");
        public UserService(ISession session)
        {
            _session = session;
            _userDao = new ApplicationUserDao(session);
            _timeService = new TimeService();
        }
        #endregion

        #region Operational Function
        public async Task CreateUserAsync(string name, string email, long id, long creatorId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var user = new ApplicationUser()
                    {
                        AspNetUsersId = id,
                        Name = name,
                        Email = email,
                        CreateBy = creatorId,
                        CreationDate = _timeService.Now,
                        Status = (int)Status.Active,
                        Rank = await _userDao.GetMaxRank() + 1,
                    };

                    await _userDao.AddAsync(user);
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error(ex.Message, ex);
                }
            }
        }

        public async Task<string> GetUserNameAsync(long userId)
        {
            Expression<Func<ApplicationUser, bool>> filter = null;
            filter = x => x.AspNetUsersId.Equals(userId);

            var user = await Task.Run( () => _userDao.Get(filter).FirstOrDefault());

            return user?.Name;
        }

        public async Task BlockAsync(long userId)
        {
            try
            {
                using (var transaction = _session.BeginTransaction())
                {
                    Expression<Func<ApplicationUser, bool>> filter = null;
                    filter = x => x.AspNetUsersId.Equals(userId);

                    var user = await Task.Run(() => _userDao.GetUser(filter));
                    if (user == null)
                    {
                        throw new CustomException("No user found with this id");
                    }

                    user.Status = (int)Status.Delete;
                    await _userDao.EditAsync(user);

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;
            }
        }

        public async Task UpdateUserAsync(string name, string email, long aspid, long creatorId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {

                    Expression<Func<ApplicationUser, bool>> filter = null;
                    filter = x => x.AspNetUsersId.Equals(aspid);

                    var user = await Task.Run(() => _userDao.GetUser(filter));
                    user.Name = name;
                    user.Email = email;
                    user.ModificationDate = _timeService.Now;
                    user.ModifyBy = creatorId;

                    await _userDao.EditAsync(user);

                    transaction.Commit();
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error($"{ex.Message}", ex);
                    throw new CustomException("User information failed to update");
                }
            }
        }

        public async Task<bool> IsActiveUserAsync(string email)
        {
            Expression<Func<ApplicationUser, bool>> filter = null;
            filter = x => x.Email.Equals(email) && x.Status == (int)Status.Active;

            var user = await Task.Run(() => _userDao.GetUser(filter));

            return user == null ? false : true;
        }
        #endregion

        #region List Loading Function
        public IList<(long, string)> LoadAllActiveUsers()
        {
            List<(long, string)> users = new List<(long, string)>();
            var allUsers = _userDao.GetUsers(x => x.Status == (int)Status.Active);
            foreach (var user in allUsers)
            {
                users.Add((user.AspNetUsersId, user.Name));
            }
            return users;
        }

        public async Task<(int total, int totalDisplay, IList<UserDto> records)> LoadAllUsers(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<ApplicationUser, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Email.Contains(searchBy) || x.Name.Contains(searchBy);
                }

                var result = _userDao.LoadAllUsers(filter, null, start, length, sortBy, sortDir);

                List<UserDto> users = new List<UserDto>();
                foreach (ApplicationUser user in result.data)
                {
                    users.Add(
                        new UserDto
                        {
                            Id = user.AspNetUsersId.ToString(),
                            Name = user.Name,
                            Email = user.Email,
                            CreateBy = await GetUserNameAsync(user.CreateBy),
                            CreationDate = user.CreationDate.ToString(),
                            Status = ((Status)user.Status).ToString(),
                            Rank = user.Rank.ToString()
                        });
                }

                return (result.total, result.totalDisplay, users);
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

