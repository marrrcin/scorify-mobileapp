using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Core.Data;
using ScorifyApp.Models;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages.UserDetailsTabbedPages
{
    public partial class UserEventsTab : ContentPage
    {
        private UserDetailsPageViewModel ViewModel;
        public UserEventsTab(UserDetailsPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        private async void UserEventsList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selected = e.SelectedItem as Event;
            UserEventsList.SelectedItem = null;
            if (selected != null)
            {
                var evnt = await ApiClient.GetEventDetails(selected);
                if (evnt != null)
                {
                    await Navigation.PushModalAsync(new WriteRelationPage(evnt));
                }
                else
                {
                    await DisplayAlert("Could get event details", "Check your internet connection and try again later...", "OK");
                }
            }
        }
    }
}
