using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using ScorifyApp.Models;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel ViewModel;
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainPageViewModel();
            BindingContext = ViewModel;
        }

        private async void DisciplinesList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selected = e.SelectedItem as Discipline;
            if (selected != null)
            {
                await Navigation.PushModalAsync(new DisciplinePage(selected), true);
            }
        }

        protected override async void OnAppearing()
        {
            //var request = new FlurlClient("http://188.226.142.132/ftp/test.json");
            //var resp = await request.GetJsonAsync<Test>();
            //TempLabel.Text = resp.title;
            await ViewModel.FacebookLogin.LoadLoginFromFile();
            ViewModel.IsLoggedIn = ViewModel.FacebookLogin.LoggedIn; //add twitter here as || 
        }

        private async void WebView_OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            var url = new Flurl.Url(e.Url.Replace(@"#","/?"));
            if (e.Url.Contains("facebook") && e.Url.Contains("access_token"))
            {
                await ViewModel.FacebookLogin.ExtractCredentials(url);
                ViewModel.IsLoggedIn = ViewModel.FacebookLogin.LoggedIn;
            }
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            FacebookWebView.Source = new UrlWebViewSource{Url = ViewModel.FacebookLogin.LoginRequestUrl};
        }

        private void FacebookWebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (!ViewModel.IsLoggedIn && e.Url.Contains("facebook") && !e.Url.Contains("access_token"))
            {
                FacebookWebView.IsVisible = true;
            }
            else
            {
                FacebookWebView.IsVisible = false;
            }
        }

        private async void LogoutButton_OnClicked(object sender, EventArgs e)
        {
            await ViewModel.FacebookLogin.Logout();
            ViewModel.IsLoggedIn = ViewModel.FacebookLogin.LoggedIn;
        }
    }
}
