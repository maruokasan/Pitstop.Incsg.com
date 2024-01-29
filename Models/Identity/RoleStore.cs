using Pitstop.Models.PitstopData;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Pitstop.Models.Identity
{
    public class RoleStore : IQueryableRoleStore<Role>, IRoleClaimStore<Role>
    {
        private readonly PitstopContext _PitstopContext;
        public RoleStore(PitstopContext PitstopContext)
        {
            _PitstopContext = PitstopContext;
        }
        public IQueryable<Role> Roles
        {
            get
            {
                return _PitstopContext.Roles.AsQueryable();
            }
        }

        public Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var claimEntity = new RolePermission
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                RoleId = role.Id
            };

            _PitstopContext.RolePermissions.Add(claimEntity);
            _PitstopContext.SaveChanges();

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await _PitstopContext.Roles.AddAsync(role);

            bool isSuccess = await _PitstopContext.SaveChangesAsync() > 0;

            return isSuccess ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = $"Could not insert role {role.Name}" });
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _PitstopContext.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (roleId == null)
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            return await _PitstopContext.Roles.FindAsync(roleId);
        }

        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (normalizedRoleName == null)
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            return Task.FromResult(_PitstopContext.Roles.FirstOrDefault(e => e.NormalizedName == normalizedRoleName.ToUpper()));
        }

        public Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = default)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            IList<Claim> result = _PitstopContext.RolePermissions.Where(e => e.RoleId == role.Id).Select(e => new Claim(e.ClaimType, e.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        public Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var claimEntity = _PitstopContext.RolePermissions.Where(e => e.ClaimType == claim.Type && e.ClaimValue == claim.Value && e.RoleId == role.Id).FirstOrDefault();

            if (claimEntity != null)
            {
                _PitstopContext.RolePermissions.Remove(claimEntity);
                _PitstopContext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (roleName == null)
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            role.Name = roleName;
            return Task.FromResult<object>(null);
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            _PitstopContext.Roles.Update(role);

            bool isSuccess = await _PitstopContext.SaveChangesAsync() > 0;
            return isSuccess ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = $"Could not update role {role.Name}" });
        }
    }
}
