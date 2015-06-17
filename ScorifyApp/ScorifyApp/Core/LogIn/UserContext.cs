using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Models;

namespace ScorifyApp.Core.LogIn
{
    public class UserContext
    {
        public static UserContext Current { private set; get; }

        public User User { private set; get; }

        public string AuthorizationToken
        {
            get
            {
                if (User != null)
                {
                    return User.Auth_Token;
                }
                return string.Empty;
            }
        }

        private UserContext(User user)
        {
            User = user;
        }

        public static void Initialize(User user)
        {
            if (Current == null)
            {
                Current = new UserContext(user);
            }
        }

        public static void Reset()
        {
            Current = null;
        }
    }
}
