using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Core.Data;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages.EventTabbedPages
{
    public partial class DescriptionTab : ContentPage
    {
        private EventPageViewModel ViewModel;
        public DescriptionTab()
        {
            InitializeComponent();
        }

        public DescriptionTab(EventPageViewModel viewModel) : this()
        {
            ViewModel = viewModel;
            BindingContext = viewModel;

            var tapEvent = new TapGestureRecognizer();
            tapEvent.Tapped += tapEvent_Tapped;
            VoteUpClickableImage.GestureRecognizers.Add(tapEvent);
            VoteDownClickableImage.GestureRecognizers.Add(tapEvent);
        }

        private async void tapEvent_Tapped(object sender, EventArgs e)
        {
            var img = sender as Image;
            if (img == null)
            {
                return;
            }

            if (img == VoteUpClickableImage)
            {
                await MakeVote(true);
            }
            else if (img == VoteDownClickableImage)
            {
                await MakeVote(false);
            }
        }

        private bool isBusy = false;
        private async Task MakeVote(bool isPositive)
        {
            if (isBusy)
            {
                return;
            }
            isBusy = true;
            var response = await ApiClient.SendEventVote(ViewModel.Event, isPositive);
            if (response == null)
            {
                await DisplayAlert("Hey!", "You've already voted!", "OK");
            }
            else if (response == false)
            {
                await DisplayAlert("Sorry", "Could not vote up at the moment", "OK");
            }
            isBusy = false;
        }
    }
}
