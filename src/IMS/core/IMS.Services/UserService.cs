using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using IMS.BusinessRules.Exceptions;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static NHibernate.Engine.Query.CallableParser;

namespace IMS.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(string name, string email, long aspid, long creatorId);
        Task UpdateUserAsync(string name, string email, long aspid, long creatorId);
        Task<bool> IsActiveUserAsync(string email);
        string GetUserName(long userId);
        Task BlockAsync(long userId);
        IList<(long, string)> LoadAllActiveUsers();
        (int total, int totalDisplay, IList<UserDto> records) LoadAllUsers(string searchBy = null,
            int length = 10, int start = 1, string sortBy = null, string sortDir = null);
    }
    public class UserService : BaseService, IUserService
    {
        private readonly IApplicationUserDao _userDao;

        public UserService(ISession session) : base(session)
        {
            _userDao = new ApplicationUserDao(session);
        }

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
                        Status = (int)Status.Active
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

            return user.Name;
        }

        public async Task BlockAsync(long userId)
        {
            try
            {
                using(var transaction = _session.BeginTransaction())
                {
                    Expression<Func<ApplicationUser, bool>> filter = null;
                    filter = x => x.AspNetUsersId.Equals(userId);

                    var user = await Task.Run( () => _userDao.GetUser(filter));
                    if(user  == null)
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

        public async Task<bool> IsActiveUserAsync(string email)
        {
            Expression<Func<ApplicationUser, bool>> filter = null;
            filter = x => x.Email.Equals(email) && x.Status == 1;

            var user = await Task.Run( () => _userDao.GetUser(filter));

            return user == null? false : true;
        }
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

        public (int total, int totalDisplay, IList<UserDto> records) LoadAllUsers(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<ApplicationUser, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Email.Contains(searchBy) && x.Status != (int)Status.Delete;
                }

                var result = _userDao.LoadAll(filter, null, start, length, sortBy, sortDir);

                List<UserDto> users = new List<UserDto>();
                foreach(ApplicationUser user in result.data)
                {
                    users.Add(
                        new UserDto
                        {
                            Id = user.AspNetUsersId,
                            Name = user.Name,
                            Email = user.Email,
                            CreateBy = user.CreateBy,
                            CreationDate = user.CreationDate,
                            Status = (Status)user.Status,
                            Rank = user.Rank,
                            VersionNumber = user.VersionNumber,
                            BusinessId = user.BusinessId,
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

        public async Task UpdateUserAsync(string name, string email, long aspid, long creatorId)
        {
            try
            {
                using(var transaction = _session.BeginTransaction())
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
            }
            catch (Exception ex)
            {
                _serviceLogger.Error($"{ex.Message}", ex);
                throw new CustomException("User information failed to update"); 
            }
        }
    }
}

