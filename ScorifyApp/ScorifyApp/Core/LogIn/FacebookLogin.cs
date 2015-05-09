using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;

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
                if (!string.IsNullOrEmpty(facebookLogin.Token))
                {
                    Token = facebookLogin.Token;
                    Code = facebookLogin.Code;
                    UserId = facebookLogin.UserId;
                    LoggedIn = true;
                }
            }
            triedFromFile = true;
        }

        public string LoginRequestUrl
        {
            get
            {
                return
                    @"https://www.facebook.com/dialog/oauth?client_id=" + FacebookApi.AppId + @"&redirect_uri=https://www.facebook.com/connect/login_success.html&response_type=code%20token";
            }
        }

        public async Task ExtractCredentials(Flurl.Url responseUrl)
        {
            Token = responseUrl.QueryParams.ContainsKey("access_token") ? responseUrl.QueryParams["access_token"].ToString() : null;

            Code = responseUrl.QueryParams.ContainsKey("code") ? responseUrl.QueryParams["code"].ToString() : null;

            UserId = await GetUserIdAsync();

            var toSave = JsonConvert.SerializeObject(this);
            if (! await FileStorage.SaveToFile(FileName, toSave))
            {
                Token = null;
                Code = null;
                LoggedIn = false;
            }
            else
            {
                LoggedIn = true;
            }
        }

        protected async Task<string> GetUserIdAsync()
        {
            string userId = "";
            if (!string.IsNullOrEmpty(Token))
            {
                var request = new Flurl.Url(@"https://graph.facebook.com/v2.3/me");
                request.QueryParams.Add("access_token", Token);
                var response = await request.GetJsonAsync<Dictionary<string, string>>();
                if (response.ContainsKey("id"))
                {
                    userId = response["id"];
                }
            }
            return userId;
        }

        public async Task Logout()
        {
            await FileStorage.SaveToFile(FileName, "");
            LoggedIn = false;
        }
    }
}
