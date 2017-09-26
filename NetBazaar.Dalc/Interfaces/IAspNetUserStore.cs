using Microsoft.AspNet.Identity;
using NetBazaar.Dalc.Dtos;

namespace NetBazaar.Dalc.Interfaces
{
    public interface IAspNetUserStore :
        IUserPasswordStore<AspNetUser, long>,
        IUserEmailStore<AspNetUser, long>,
        IUserLoginStore<AspNetUser, long>,
        IUserClaimStore<AspNetUser, long>,
        IUserRoleStore<AspNetUser, long>,
        IUserSecurityStampStore<AspNetUser, long>,
        IQueryableUserStore<AspNetUser, long>,
        IUserPhoneNumberStore<AspNetUser, long>,
        IUserLockoutStore<AspNetUser, long>,
        IUserTwoFactorStore<AspNetUser, long>
    {
    }
}