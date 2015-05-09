using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScorifyApp.Core
{
    public interface ILogin
    {
        Task LoadLoginFromFile();

        bool LoggedIn { get; }

        string Token { get; }

        string TokenSecret { get; }

        string LoginRequestUrl { get; }

        Task ExtractCredentials(Flurl.Url responseUrl);

        Task Logout();
    }
}
