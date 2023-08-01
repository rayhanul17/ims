using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(string name, long id, long creatorId);
        Task<bool> IsActiveUserAsync(string email);
        string GetUserName(long userId);
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
                        Status = 1
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
                    filter = x => x.Name.Contains(searchBy) && x.Status != (int)Status.Delete;
                }

                var result = _userDao.LoadAll(filter, null, start, length, sortBy, sortDir);

                List<UserDto> users = new List<UserDto>();
                foreach(ApplicationUser user in result.data)
                {
                    users.Add(
                        new UserDto
                        {
                            Id = user.Id,
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
    }
}

