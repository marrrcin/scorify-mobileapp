﻿using System;
using System.Threading.Tasks;
using Autofac.Core;
using Flurl;
using Newtonsoft.Json;
using ScorifyApp.Core.Data;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Credentials;
using User = ScorifyApp.Models.User;

namespace ScorifyApp.Core.LogIn
{
    public class TwitterLogin : ILogin
    {
        //you have to implement own ITwitterApi, this class is not on repo due to sensitive data inside
        protected ITwitterApi TwitterApi = new TwitterApiData();

        private bool triedFromFile = false;
        public async Task LoadLoginFromFile()
        {
            if (triedFromFile)
                return;

            var fromFile = await FileStorage.LoadFromFile(FileName);
            if (!string.IsNullOrEmpty(fromFile))
            {
                var twitterLogin = JsonConvert.DeserializeObject<TwitterLogin>(fromFile);
                if (!string.IsNullOrEmpty(twitterLogin.Token))
                {
                    Token = twitterLogin.Token;
                    TokenSecret = twitterLogin.TokenSecret;
                    LoggedIn = true;
                    UserId = twitterLogin.UserId;
                    UserEmail = twitterLogin.UserEmail;
                    User = twitterLogin.User;
                    UserContext.Initialize(User);
                    TwitterCredentials.SetCredentials(Token, TokenSecret, TwitterApi.ConsumerKey, TwitterApi.ConsumerSecret);
                    LoggedIn = true;
                }
            }
            triedFromFile = true;
        }

        public bool LoggedIn { private set; get; }

        public string Token { set; get; }

        public string TokenSecret { set; get; }

        public string UserId { set; get; }

        public string UserEmail { set; get; }

        public string CallbackUrl = @"http://188.226.142.132/scorifymobileapp-callback";


        protected ITemporaryCredentials _CachedCredentials = null;
        protected ITemporaryCredentials ApplicationCredentials
        {
            get
            {
                if (_CachedCredentials == null)
                {
                    _CachedCredentials = CredentialsCreator.GenerateApplicationCredentials(TwitterApi.ConsumerKey,
                    TwitterApi.ConsumerSecret);    
                }
                return _CachedCredentials;
            }
        }

        public string LoginRequestUrl
        {
            get
            {
                var request = CredentialsCreator.GetAuthorizationURLForCallback(ApplicationCredentials, CallbackUrl);
                if (request == null)
                {
                    _CachedCredentials = CredentialsCreator.GenerateApplicationCredentials(TwitterApi.ConsumerKey,
                        TwitterApi.ConsumerSecret);
                }
                request = CredentialsCreator.GetAuthorizationURLForCallback(ApplicationCredentials, CallbackUrl);
                return request;
            }
        }

        protected string FileName = "twitter.json";

        public async Task ExtractCredentials(Url responseUrl)
        {
            try
            {
                var credentials = CredentialsCreator.GetCredentialsFromCallbackURL(responseUrl.ToString(),
                    ApplicationCredentials);
                TwitterCredentials.SetCredentials(credentials);
                Token = credentials.AccessToken;
                TokenSecret = credentials.AccessTokenSecret;
                var user = await UserAsync.GetLoggedUser();
                UserId = user.IdStr;
                UserEmail = user.ScreenName;
                User = await ApiClient.LogInUserAsync("twitter", Token, TokenSecret);
                if (User == null)
                {
                    LoggedIn = false;
                    return;
                }
                UserContext.Initialize(User);
                var toSave = JsonConvert.SerializeObject(this);
                if (!await FileStorage.SaveToFile(FileName, toSave))
                {
                    Token = null;
                    TokenSecret = null;
                    LoggedIn = false;
                }
                else
                {
                    LoggedIn = true;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public User User { set; get; }

        public async Task Logout()
        {
            await FileStorage.SaveToFile(FileName, "");
            LoggedIn = false;
        }
    }
}
