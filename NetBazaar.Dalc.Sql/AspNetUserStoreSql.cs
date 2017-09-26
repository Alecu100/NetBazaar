using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NetBazaar.Dalc.Dtos;
using NetBazaar.Dalc.Interfaces;

namespace NetBazaar.Dalc.Sql
{
    public class AspNetUserStoreSql : BaseStoreSql, IAspNetUserStore
    {
        public AspNetUserStoreSql(NetBazaarDatabase netBazaarDatabase) : base(netBazaarDatabase)
        {
        }

        public Task CreateAsync(AspNetUser user)
        {
            var createAsync = new Task(() => Create(user));
            createAsync.Start();
            return createAsync;
        }


        public Task UpdateAsync(AspNetUser user)
        {
            var updateAsync = new Task(() => Update(user));
            updateAsync.Start();
            return updateAsync;
        }

        public Task DeleteAsync(AspNetUser user)
        {
            var deleteAsync = new Task(() => Delete(user));
            deleteAsync.Start();
            return deleteAsync;
        }

        public Task<AspNetUser> FindByIdAsync(long userId)
        {
            var findByIdAsync = new Task<AspNetUser>(() => FindById(userId));
            findByIdAsync.Start();
            return findByIdAsync;
        }

        public Task<AspNetUser> FindByNameAsync(string userName)
        {
            var findByNameAsync = new Task<AspNetUser>(() => FindByName(userName));
            findByNameAsync.Start();
            return findByNameAsync;
        }

        public Task SetPasswordHashAsync(AspNetUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(AspNetUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(AspNetUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetEmailAsync(AspNetUser user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(AspNetUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<AspNetUser> FindByEmailAsync(string email)
        {
            return _netBazaarDatabase.AspNetUsers.FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper());
        }

        public Task AddLoginAsync(AspNetUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            _netBazaarDatabase.AspNetUserLogins.Add(new AspNetUserLogin
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            });

            return _netBazaarDatabase.SaveChangesAsync();
        }

        public Task RemoveLoginAsync(AspNetUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userLogin =
                user.AspNetUserLogins.FirstOrDefault(
                    l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey);

            if (
                userLogin != null)
            {
                user.AspNetUserLogins.Remove(userLogin);
                return _netBazaarDatabase.SaveChangesAsync();
            }

            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(AspNetUser user)
        {
            var task = new Task<IList<UserLoginInfo>>(() => GetLogins(user));
            task.Start();

            return task;
        }

        public Task<AspNetUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var foundUsers = from user in _netBazaarDatabase.AspNetUsers
                join userLogin in _netBazaarDatabase.AspNetUserLogins on user.Id equals userLogin.UserId
                where userLogin.ProviderKey == login.LoginProvider && userLogin.ProviderKey == login.ProviderKey
                select user;


            return foundUsers.FirstOrDefaultAsync();
        }

        public Task<IList<Claim>> GetClaimsAsync(AspNetUser user)
        {
            var task = new Task<IList<Claim>>(() => GetClaims(user));
            task.Start();

            return task;
        }

        public Task RemoveClaimAsync(AspNetUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            var userClaim =
                user.AspNetUserClaims.FirstOrDefault(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);

            if (userClaim != null)
            {
                user.AspNetUserClaims.Remove(userClaim);
                return _netBazaarDatabase.SaveChangesAsync();
            }

            return Task.FromResult(0);
        }

        public Task AddClaimAsync(AspNetUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            _netBazaarDatabase.AspNetUserClaims.Add(new AspNetUserClaim
            {
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });

            return _netBazaarDatabase.SaveChangesAsync();
        }

        public Task AddToRoleAsync(AspNetUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value Cannot Be Null Or Empty", "roleName");
            }

            var userRoleDb = _netBazaarDatabase.AspNetRoles.FirstOrDefault(r => r.Name.Equals(roleName));

            if (userRoleDb != null && user.AspNetRoles.All(r => !r.Name.Equals(roleName)))
            {
                user.AspNetRoles.Add(userRoleDb);
                return _netBazaarDatabase.SaveChangesAsync();
            }

            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(AspNetUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value Cannot Be Null Or Empty", "roleName");
            }

            var userRole = user.AspNetRoles.FirstOrDefault(r => r.Name.Equals(roleName));

            if (userRole != null)
            {
                user.AspNetRoles.Remove(userRole);
                return _netBazaarDatabase.SaveChangesAsync();
            }

            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(AspNetUser user)
        {
            var task = new Task<IList<string>>(() => GetRoles(user));
            task.Start();

            return task;
        }


        public Task<bool> IsInRoleAsync(AspNetUser user, string roleName)
        {
            var task = new Task<bool>(() => IsInRole(user, roleName));
            task.Start();

            return task;
        }

        public Task SetSecurityStampAsync(AspNetUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.SecurityStamp);
        }


        public IQueryable<AspNetUser> Users
        {
            get { return _netBazaarDatabase.AspNetUsers; }
        }


        public Task SetPhoneNumberAsync(AspNetUser user, string phoneNumber)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (phoneNumber == null)
            {
                throw new ArgumentNullException("phoneNumber");
            }

            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(AspNetUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return
                Task.FromResult(user.LockoutEndDateUtc != null
                    ? new DateTimeOffset(user.LockoutEndDateUtc.Value)
                    : new DateTimeOffset());
        }

        public Task<int> IncrementAccessFailedCountAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(AspNetUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(AspNetUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(AspNetUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public void Dispose()
        {
        }

        public IList<string> GetRoles(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var userId = user.Id;
            var query = from u in _netBazaarDatabase.AspNetUsers
                where u.Id.Equals(userId)
                select u;

            return query.First().AspNetRoles.Select(r => r.Name).ToList();
        }

        private bool IsInRole(AspNetUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Value Cannot Be Null Or Empty", "roleName");
            }

            var role = _netBazaarDatabase.AspNetRoles.FirstOrDefault(r => r.Name.ToUpper() == roleName.ToUpper());

            if (role != null)
            {
                // ReSharper disable once PossibleNullReferenceException
                return
                    _netBazaarDatabase.AspNetRoles.FirstOrDefault(r => r.Name.ToUpper() == roleName.ToUpper())
                        .AspNetUsers.Any(u => u.Id == user.Id);
            }
            return false;
        }

        private IList<Claim> GetClaims(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return user.AspNetUserClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }


        private void Create(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _netBazaarDatabase.AspNetUsers.Add(user);
            _netBazaarDatabase.SaveChanges();

            var userRoleDb = _netBazaarDatabase.AspNetRoles.FirstOrDefault(r => r.Name.Equals("Member"));

            if (userRoleDb != null)
            {
                user.AspNetRoles.Add(userRoleDb);
                _netBazaarDatabase.SaveChangesAsync();
            }
        }


        private void Update(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (_netBazaarDatabase.Entry(user).State == EntityState.Detached)
            {
                _netBazaarDatabase.AspNetUsers.Attach(user);
                _netBazaarDatabase.Entry(user).State = EntityState.Modified;
            }

            _netBazaarDatabase.SaveChanges();
        }


        private void Delete(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            _netBazaarDatabase.AspNetUsers.Remove(user);
            _netBazaarDatabase.SaveChanges();
        }

        private AspNetUser FindById(long userId)
        {
            return _netBazaarDatabase.AspNetUsers.Find(userId);
        }


        private AspNetUser FindByName(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            return _netBazaarDatabase.AspNetUsers.FirstOrDefault(user => user.UserName == userName);
        }

        private IList<UserLoginInfo> GetLogins(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return user.AspNetUserLogins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();
        }
    }
}