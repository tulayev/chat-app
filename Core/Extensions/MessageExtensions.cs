using Core.Models;
using System.Linq.Expressions;

namespace Core.Extensions
{
    public static class MessageExtensions
    {
        public static Expression<Func<Message, bool>> BetweenUsersPredicate(int userId1, int userId2)
        {
            return x =>
                (x.SenderId == userId1 && x.ReceiverId == userId2) ||
                (x.SenderId == userId2 && x.ReceiverId == userId1);
        }
    }
}
