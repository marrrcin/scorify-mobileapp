using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScorifyApp.Core
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

        public async Task Logout()
        {
            await FileStorage.SaveToFile(FileName, "");
            LoggedIn = false;
        }
    }
}
