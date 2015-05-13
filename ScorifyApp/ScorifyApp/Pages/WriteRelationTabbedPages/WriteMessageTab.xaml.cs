using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Core.Data;
using ScorifyApp.Models;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages.WriteRelationTabbedPages
{
    public partial class WriteMessageTab : ContentPage
    {
        private WriteRelationPageViewModel ViewModel;
        private string _buttonText;
        private Color _buttonColor;
        private volatile bool IsActive = false;

        public WriteMessageTab(WriteRelationPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _buttonText = SendButton.Text;
            _buttonColor = SendButton.BackgroundColor;
        }

        private async void SendButton_OnClicked(object sender, EventArgs e)
        {
            if (IsActive)
            {
                await DisplayAlert("Not so fast", "Are you in a hurry, my friend?", "nope");
                return;
            }
            IsActive = true;
            UpdateActivity(true);
            if (string.IsNullOrEmpty(ViewModel.Content) || ViewModel.Content.Length < 4 ||
                ViewModel.Content.Length > 128)
            {
                await DisplayAlert("Message is incorrect", "Please write message with proper length", "Cancel");
            }
            else
            {
                var msg = new Message
                {
                    Content = ViewModel.Content,
                    Created = DateTime.Now,
                    Event = ViewModel.Event
                };
                var response = await ApiClient.PostEventMessageAsync(msg);
                if (response)
                {
                    ViewModel.Content = string.Empty;
                }
                else
                {
                    await DisplayAlert("Could not post message", "Check your internet connection and try again later...", "Cancel");
                }
            }
            UpdateActivity(false);
            IsActive = false;
        }

        private void UpdateActivity(bool isActive)
        {
            SendingIndicator.IsVisible = isActive;
            if (isActive)
            {
                SendButton.Text = "Sending...";
                SendButton.BackgroundColor = new Color(211/255.0, 183/255.0, 61/255.0,1.0);
            }
            else
            {
                SendButton.Text = _buttonText;
                SendButton.BackgroundColor = _buttonColor;
            }
            
        }
    }
}
