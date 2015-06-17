using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using ScorifyApp.Core;
using ScorifyApp.Core.Data;
using ScorifyApp.Core.LogIn;
using ScorifyApp.Models;
using ScorifyApp.ViewModels;
using Tweetinvi;
using Xamarin.Forms;

namespace ScorifyApp.Pages
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel ViewModel;

        private volatile bool TwitterClicked = false;
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainPageViewModel();
            BindingContext = ViewModel;
        }

        private async void DisciplinesList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selected = e.SelectedItem as Discipline;
            DisciplinesList.SelectedItem = null;
            if (selected != null)
            {
                await Navigation.PushModalAsync(new DisciplinePage(selected,"Available relations"), true);
            }
        }

        protected override async void OnAppearing()
        {
            //var request = new FlurlClient("http://188.226.142.132/ftp/test.json");
            //var resp = await request.GetJsonAsync<Test>();
            //TempLabel.Text = resp.title;
            bool retry;
            do
            {
                bool exception = false;
                retry = false;
                try
                {
                    await ViewModel.FacebookLogin.LoadLoginFromFile();
                    await ViewModel.TwitterLogin.LoadLoginFromFile();
                    ViewModel.IsLoggedIn = ViewModel.FacebookLogin.LoggedIn || ViewModel.TwitterLogin.LoggedIn;
                    if (ViewModel.IsLoggedIn)
                    {
                        //ViewModel.User = 
                    }
                    await ViewModel.LoadDisciplines();
                }
                catch (Exception e)
                {
                    exception = true;
                }
                if (exception)
                {
                    retry = await DisplayAlert("Sorry",
                                "Could not detect internet connection or the application service is unavailable",
                                "retry", "cancel");
                }
            } while (retry);
        }

        private async void WebView_OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            var url = new Flurl.Url(e.Url.Replace(@"#","/?"));
            var webUrl = e.Url;
            if (webUrl.Contains("facebook") && webUrl.Contains("access_token"))
            {
                await ViewModel.FacebookLogin.ExtractCredentials(url);
                ViewModel.IsLoggedIn = ViewModel.FacebookLogin.LoggedIn;
            }
        }

        private void FacebookWebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!ViewModel.IsLoggedIn && e.Url.Contains("facebook") && !e.Url.Contains("access_token"))
            {
                FacebookWebView.IsVisible = true;
                TwitterWebView.IsVisible = false;
            }
            else
            {
                FacebookWebView.IsVisible = false;
            }
        }

        private async void LogoutButton_OnClicked(object sender, EventArgs e)
        {
            await ViewModel.FacebookLogin.Logout();
            await ViewModel.TwitterLogin.Logout();
            UserContext.Reset();
            ViewModel.IsLoggedIn = false;
        }

        private void FacebookLoginButton_OnClicked(object sender, EventArgs e)
        {
            FacebookWebView.Source = new UrlWebViewSource { Url = ViewModel.FacebookLogin.LoginRequestUrl };
        }

        private async void TwitterLoginButton_OnClicked(object sender, EventArgs e)
        {
            if (TwitterClicked)
            {
                await DisplayAlert("Please wait", "Signing in with Twitter is in progress", "ok");
                return;
            }
            TwitterClicked = true;
            string url = null;
            url = await TwitterSignInUrl();

            if(!string.IsNullOrEmpty(url))
            {
                TwitterWebView.Source = new UrlWebViewSource { Url = url };
            }
            else
            {
                TwitterWebView.IsVisible = false;
                await DisplayAlert("Sorry", "Could not login using twitter at the time, try again later...", "Cancel");
            }
            TwitterClicked = false;
        }

        private async Task<string> TwitterSignInUrl()
        {
            string url = null;
            //WORKAROUND => INVOKING THIS TWICE SEEMS TO RESOLVE ISSUE WITH NULL URL :-O
            await Task.Factory.StartNew(() => url = ViewModel.TwitterLogin.LoginRequestUrl);
            if (string.IsNullOrEmpty(url))
            {
                await Task.Factory.StartNew(() => url = ViewModel.TwitterLogin.LoginRequestUrl);
            }
            return url;
        }

        private async void TwitterWebView_OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            var url = new Flurl.Url(e.Url);
            if (e.Url.Contains(ViewModel.TwitterLogin.CallbackUrl) && e.Url.Contains("oauth_token"))
            {
                await ViewModel.TwitterLogin.ExtractCredentials(url);
                ViewModel.IsLoggedIn = ViewModel.TwitterLogin.LoggedIn;
            }
        }

        private void TwitterWebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!ViewModel.IsLoggedIn && !e.Url.Contains(ViewModel.TwitterLogin.CallbackUrl))
            {
                TwitterWebView.IsVisible = true;
                FacebookWebView.IsVisible = false;
            }
            else
            {
                TwitterWebView.IsVisible = false;
            }
        }

        private async void UserRelationsButton_OnClicked(object sender, EventArgs e)
        {
            var success = true;
            try
            {
                var user = await ApiClient.GetUserDetailsAsync(UserContext.Current.User.Id); //TODO pass user id here
                if (user != null)
                {
                    await Navigation.PushModalAsync(new UserDetailsPage(user));
                }
            }
            catch (Exception ex)
            {
                success = false;
            }

            if (!success)
            {
                await DisplayAlert("Sorry", "Something went wrong and I cannot get your events", "Cancel");
            }
        }
    }
}
