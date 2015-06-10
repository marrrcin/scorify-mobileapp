using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using ScorifyApp.Core.Data;
using ScorifyApp.Models;

namespace ScorifyApp.Core.LogIn
{
    public class FacebookLogin : ILogin
    {
        //you have to implement own IFacebookApi, this class is not on repo due to sensitive data inside
        protected IFacebookApi FacebookApi = new FacebookApiData();

        protected string FileName = "facebook.json";

        public string Token { set; get; }

        public string TokenSecret
        {
            get { return Code; }
        }

        public string UserId { set; get; }

        public string UserEmail { set; get; }

        public string Code { set; get; }


        private bool triedFromFile = false;

        public bool LoggedIn { private set; get; }

        public async Task LoadLoginFromFile()
        {
            if (triedFromFile)
                return;

            var fromFile = await FileStorage.LoadFromFile(FileName);
            if (!string.IsNullOrEmpty(fromFile))
            {
                var facebookLogin = JsonConvert.DeserializeObject<FacebookLogin>(fromFile);
                if (facebookLogin.User != null && !string.IsNullOrEmpty(facebookLogin.User.Auth_Token))
                {
                    Token = facebookLogin.Token;
                    Code = facebookLogin.Code;
                    UserId = facebookLogin.UserId;
                    UserEmail = facebookLogin.UserEmail;
                    User = facebookLogin.User;
                    UserContext.Initialize(User);
                    LoggedIn = true;
                }
            }
            triedFromFile = true;
        }

        private string ApiScopes
        {
            get { return "email"; }
        }

        public string LoginRequestUrl
        {
            get
            {
                return
                    @"https://www.facebook.com/dialog/oauth?client_id=" + FacebookApi.AppId + @"&redirect_uri=https://www.facebook.com/connect/login_success.html&response_type=code%20token" + @"&scope=" + ApiScopes;
            }
        }

        public async Task ExtractCredentials(Flurl.Url responseUrl)
        {
            Token = responseUrl.QueryParams.ContainsKey("access_token") ? responseUrl.QueryParams["access_token"].ToString() : null;

            Code = responseUrl.QueryParams.ContainsKey("code") ? responseUrl.QueryParams["code"].ToString() : null;

            var userInfo = await GetUserInfoAsync();
            UserEmail = userInfo.Email;
            UserId = userInfo.Id;
            User = await ApiClient.LogInUser("facebook", Token);
            if (string.IsNullOrEmpty(User.Auth_Token))
            {
                LoggedIn = false;
                return;
            }
            UserContext.Initialize(User);
            var toSave = JsonConvert.SerializeObject(this);
            if (!await FileStorage.SaveToFile(FileName, toSave))
            {
                Token = null;
                Code = null;
                LoggedIn = false;
                UserId = null;
                UserEmail = null;
            }
            else
            {
                LoggedIn = true;
            }
        }

        public User User { set; get; }

        protected struct UserInfo
        {
            public string Id;
            public string Email;
        }

        protected async Task<UserInfo> GetUserInfoAsync()
        {
            var userInfo = new UserInfo();
            if (!string.IsNullOrEmpty(Token))
            {
                var request = new Flurl.Url(@"https://graph.facebook.com/v2.3/me?fields=id,email");
                request.QueryParams.Add("access_token", Token);
                var response = await request.GetJsonAsync<Dictionary<string, string>>();

                if (response.ContainsKey("id"))
                {
                    userInfo.Id = response["id"];
                }
                if (response.ContainsKey("email"))
                {
                    userInfo.Email = response["email"];
                }
            }
            return userInfo;
        }

        public async Task Logout()
        {
            await FileStorage.SaveToFile(FileName, "");
            LoggedIn = false;
        }
    }
}
