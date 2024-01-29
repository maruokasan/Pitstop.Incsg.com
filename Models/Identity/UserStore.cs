using Pitstop.Models.PitstopData;
using Microsoft.AspNetCore.Identity;

namespace Pitstop.Models.Identity
{
    public class UserStore : IUserPasswordStore<User>, IUserRoleStore<User>
    {
        private readonly PitstopContext _PitstopContext;
        public UserStore(PitstopContext PitstopContext)
        {
            _PitstopContext = PitstopContext;
        }

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (roleName == null)
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var roleData = _PitstopContext.Roles.FirstOrDefault(e => e.Name.ToUpper() == roleName.ToUpper());

            var userRoles = new UserRoles
            {
                RoleId = roleData.Id,
                UserId = user.Id
            };

            _PitstopContext.UserRoles.Add(userRoles);

            return Task.FromResult<object>(null);
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _PitstopContext.Users.AddAsync(user);

            bool isSuccess = (await _PitstopContext.SaveChangesAsync() > 0);

            return isSuccess ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = $"Failed to insert user {user.UserName}" });
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _PitstopContext.Dispose();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _PitstopContext.Users.FindAsync(userId);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedUserName))
            {
                throw new ArgumentNullException(nameof(normalizedUserName));
            }

            return Task.FromResult(_PitstopContext.Users.FirstOrDefault(e => e.NormalizedEmail == normalizedUserName.ToUpper()
            && e.IsUserActive == true));
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roleList = _PitstopContext.Roles.Where(e => e.Name != null).ToList();
            var getUserRoleId = _PitstopContext.Users.Find(user.Id);

            List<string> userRoleNames = new List<string>();

            foreach (var item in getUserRoleId.Roles)
            {
                var role = roleList.FirstOrDefault(e => e.Id == item.Id);
                userRoleNames.Add(role.Name);
            }

            return userRoleNames;
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName);
        }

        public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (roleName == null)
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var role = _PitstopContext.Roles.FirstOrDefault(e => e.NormalizedName == roleName.ToUpper());

            var result = user.Roles.Where(e => e.Id == role.Id).ToList();

            return result.Count > 0 ? Task.FromResult(true) : Task.FromResult(false);
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.UserName = userName;
            return Task.FromResult<object>(null);
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _PitstopContext.Users.Update(user);

            bool isSuccess = await _PitstopContext.SaveChangesAsync() > 0;

            return isSuccess ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = $"Failed update user {user.UserName}" });
        }
    }
}
