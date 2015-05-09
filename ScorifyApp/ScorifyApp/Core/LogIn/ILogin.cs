using System.Threading.Tasks;

namespace ScorifyApp.Core.LogIn
{
    public interface ILogin
    {
        Task LoadLoginFromFile();

        bool LoggedIn { get; }

        string Token { get; }

        string TokenSecret { get; }

        string UserId { get; }

        string LoginRequestUrl { get; }

        Task ExtractCredentials(Flurl.Url responseUrl);

        Task Logout();
    }
}
