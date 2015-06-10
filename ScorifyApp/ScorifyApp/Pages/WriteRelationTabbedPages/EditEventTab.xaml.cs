using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Core.Data;
using ScorifyApp.Models;
using Xamarin.Forms;

namespace ScorifyApp.Pages.WriteRelationTabbedPages
{
    public partial class EditEventTab : ContentPage
    {
        private Event ViewModel;
        public EditEventTab(Event evnt)
        {
            InitializeComponent();
            ViewModel = evnt;
            BindingContext = ViewModel;
        }

        private async void EditButton_OnClicked(object sender, EventArgs e)
        {
            var errors = new LinkedList<string>();
            if (string.IsNullOrEmpty(ViewModel.Title) || ViewModel.Title.Length < 6 || ViewModel.Title.Length > 64)
            {
                errors.AddLast("Title has incorrect length");
            }

            if (string.IsNullOrEmpty(ViewModel.Description) || ViewModel.Description.Length < 10 || ViewModel.Description.Length > 256)
            {
                errors.AddLast("Description has incorrect length");
            }

            if (string.IsNullOrEmpty(ViewModel.Venue) || ViewModel.Venue.Length < 6 || ViewModel.Venue.Length > 64)
            {
                errors.AddLast("Venue has incorrect length");
            }

            if (errors.Count != 0)
            {
                await DisplayAlert("Some data is incorrect", string.Join("," + Environment.NewLine, errors), "Cancel");
            }
            else
            {
                var buttonText = EditButton.Text;
                EditButton.Text = "Editing ...";

                var response = await ApiClient.EditEventAsync(ViewModel);

                EditButton.Text = buttonText;

                if (!response)
                {
                    await DisplayAlert("Could not edit event", "Check your internet connection and try again later...", "Cancel");
                }
            }
        }
    }
}
