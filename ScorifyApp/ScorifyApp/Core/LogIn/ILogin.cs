using System.Threading.Tasks;
using ScorifyApp.Models;

namespace ScorifyApp.Core.LogIn
{
    public interface ILogin
    {
        Task LoadLoginFromFile();

        bool LoggedIn { get; }

        string Token { get; }

        string TokenSecret { get; }

        string UserId { get; }

        string UserEmail { set; get; }

        string LoginRequestUrl { get; }

        Task ExtractCredentials(Flurl.Url responseUrl);

        User User { set; get; }

        Task Logout();
    }
}
