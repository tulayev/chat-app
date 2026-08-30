using ChatApp.Domain.Models;

namespace ChatApp.Application.Common.Extensions
{
    public static class UserQueryableExtensions
    {
        /// <summary>
        /// Filters the query to only include users with confirmed emails.
        /// </summary>
        public static IQueryable<TUser> WhereEmailConfirmed<TUser>(this IQueryable<TUser> query)
            where TUser : AppUser
        {
            return query.Where(x => x.EmailConfirmed);
        }
    }
}
