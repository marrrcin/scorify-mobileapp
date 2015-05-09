namespace ScorifyApp.Core.LogIn
{
    public interface ITwitterApi
    {
        string ConsumerKey { get; }

        string ConsumerSecret { get; }

        string AccessToken { get; }

        string AccessSecret { get; }
    }
}
