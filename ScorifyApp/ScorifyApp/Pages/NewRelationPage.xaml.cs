using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Core.Data;
using ScorifyApp.Models;
using ScorifyApp.ViewModels;
using Tweetinvi.Core.Events.EventArguments;
using Xamarin.Forms;

namespace ScorifyApp.Pages
{
    public partial class NewRelationPage : ContentPage
    {
        private NewEventPageViewModel ViewModel;
        public NewRelationPage(Discipline discipline)
        {
            InitializeComponent();
            ViewModel = new NewEventPageViewModel {Discipline = discipline};
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var now = DateTime.Now;
            StartTimePicker.Time = new TimeSpan(now.Hour,now.Minute,seconds:now.Second);
            EndTimePicker.Time = new TimeSpan(now.Hour, now.Minute + 1, seconds: now.Second);
            StartDatePicker.Date = now;
            EndDatePicker.Date = now;
            ViewModel.StartDate = now;
            ViewModel.EndDate = now;

            //TODO delete this before release:
            ViewModel.Title = "Lorem title dolor";
            ViewModel.Description = "Ipsum dolor sit amet? Yolo swag";
            ViewModel.Contenders = "Arsenal,Lech Poznan";
            ViewModel.Venue = "PUT Gym";
            ViewModel.User = new User { Id = "554de8c63731620003000000", Email = "Marcin" };

        }

        private async void CreateButton_OnClicked(object sender, EventArgs e)
        {
            var errors = new LinkedList<string>();
            //validation
            Validate(ViewModel, errors);

            if (errors.Count != 0)
            {
                await DisplayAlert("Some data is incorrect", string.Join("," + Environment.NewLine,errors), "Cancel");
            }
            else
            {
                var buttonText = CreateButton.Text;
                CreateButton.Text = "Creating ...";

                var evnt = PrepareEventToSend();
                var response = await ApiClient.CreateEventAsync(evnt);

                CreateButton.Text = buttonText;
                
                if (response)
                {
                    await Navigation.PushModalAsync(new WriteRelationPage(evnt));
                }
                else
                {
                    await DisplayAlert("Could not create event", "Check your internet connection and try again later...", "Cancel");
                }
            }

        }

        private Event PrepareEventToSend()
        {
            var startDate = FormatDate(ViewModel.StartDate, ViewModel.StartTime);
            var endDate = FormatDate(ViewModel.EndDate, ViewModel.EndTime);
            var evnt = new Event
            {
                Title = ViewModel.Title,
                Discipline = ViewModel.Discipline,
                Contenders =
                    ViewModel.Contenders.Split(',').Select(c => new Dictionary<string, object> {{"title", c}}).ToArray(),
                Description = ViewModel.Description,
                StartDateTime = startDate,
                EndDateTime = endDate,
                User = ViewModel.User,
                Venue = ViewModel.Venue
            };
            return evnt;
        }

        private DateTime FormatDate(DateTime date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month,
                date.Day, time.Hours, time.Minutes,
                time.Seconds);
        }

        private void Validate(NewEventPageViewModel evnt, LinkedList<string> errors)
        {
            if (string.IsNullOrEmpty(evnt.Title) || evnt.Title.Length < 6 || evnt.Title.Length > 64)
            {
                errors.AddLast("Title has incorrect length");
            }

            if (string.IsNullOrEmpty(evnt.Description) || evnt.Description.Length < 10 || evnt.Description.Length > 256)
            {
                errors.AddLast("Description has incorrect length");
            }

            if (string.IsNullOrEmpty(evnt.Venue) || evnt.Title.Length < 6 || evnt.Title.Length > 64)
            {
                errors.AddLast("Venue has incorrect length");
            }

            if (string.IsNullOrEmpty(ViewModel.Contenders) || ViewModel.Contenders.Length < 6 ||
                ViewModel.Contenders.Length > 256)
            {
                errors.AddLast("Incorrect number of contenders");
            }

            if (evnt.StartDate > evnt.EndDate || (evnt.StartDate == evnt.EndDate && ViewModel.StartTime >= ViewModel.EndTime))
            {
                errors.AddLast("End date is incorrect");
            }
        }

        private void StartDatePicker_OnDateSelected(object sender, DateChangedEventArgs e)
        {
            ViewModel.StartDate = e.NewDate;
        }

        private void EndDatePicker_OnDateSelected(object sender, DateChangedEventArgs e)
        {
            ViewModel.EndDate = e.NewDate;
        }
    }
}
