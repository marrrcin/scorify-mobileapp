using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Core.Data;
using ScorifyApp.Models;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages
{
    public partial class DisciplinePage : ContentPage
    {
        private DisciplinePageViewModel ViewModel;

        private User User = null;

        private bool ShowFinished = false;

        public DisciplinePage(string pageTitle, User user) : this(null,pageTitle)
        {
            User = user;

            //windows phone nested layout padding fix
            var oldPadding = WrapperStackLayout.Padding;
            var newPadding = new Thickness(oldPadding.Left, oldPadding.Top, oldPadding.Right + 5, oldPadding.Bottom + 10);
            WrapperStackLayout.Padding = newPadding;

            if (ViewModel.Discipline == null)
            {
                NewRelationButton.IsVisible = false;
            }
        }

        public DisciplinePage(Discipline discipline,string pageTitle)
        {
            InitializeComponent();
            ViewModel = new DisciplinePageViewModel {Discipline = discipline};
            BindingContext = ViewModel;
            PageTitleLabel.Text = pageTitle;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (User == null)
            {
                ViewModel.Events = await ApiClient.GetEventsAsync(ViewModel.Discipline);
            }
            else
            {
                var userProfile = await ApiClient.GetUserDetailsAsync(User.Id);
                ViewModel.Events = userProfile.Events;
            }
            ViewModel.Filtered = ViewModel.Events.Where(ev => ev.Finished == ShowFinished);
        }

        private async void EventsList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selected = e.SelectedItem as Event;
            EventsList.SelectedItem = null;
            if (selected != null)// null;//finished here - i wanted to use discipline page both as discipline page and user events page
            {
                if (selected.Discipline == null)
                {
                    selected.Discipline = ViewModel.Discipline;
                }

                if (selected.Discipline_Id == null)
                {
                    selected.Discipline_Id = ViewModel.Discipline.Id;
                }

                var evnt = await ApiClient.GetEventDetailsAsync(selected);
                if (evnt == null)
                {
                    await DisplayAlert("Could get event details", "Check your internet connection and try again later...", "OK");
                    return;
                }

                if (User != null)
                {
                    await Navigation.PushModalAsync(new WriteRelationPage(evnt));
                }
                else
                {
                    await Navigation.PushModalAsync(new EventPage(evnt));    
                }
                
            }
        }

        private async void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var current = e.NewTextValue;
            await Task.Delay(300);
            if (current != SearchBox.Text) //do not search while user is typing
            {
                return;
            }

            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                if (e.NewTextValue != e.OldTextValue)
                {
                    IEnumerable<Event> filtered = null;
                    try
                    {
                        await Task.Factory.StartNew(() => filtered = FilterEvents(e.NewTextValue, ViewModel.Events.ToArray()));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    
                    ViewModel.Filtered = filtered ?? new Event[0];
                }
            }
            else
            {
                ViewModel.Filtered = ViewModel.Events;
            }
            
        }

        protected IEnumerable<Event> FilterEvents(string search, IEnumerable<Event> toFilter)
        {
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                var filtered = from ev in toFilter
                               where ev.Finished == ShowFinished
                                       && (ev.Title.ToLower().Contains(search)
                                          || ev.Description.ToLower().Contains(search)
                                          || ev.Venue.ToLower().Contains(search)
                                   || ev.User.Email.ToLower().Contains(search)
                                          || ev.StartDateTime.ToString().Contains(search))
                               select ev;
                return filtered.ToArray();
            }
            else
            {
                return toFilter.Where(ev => ev.Finished == ShowFinished).ToArray();
            }
        }

        private async void NewRelationButton_OnClicked(object sender, EventArgs e)
        {
            var discipline = ViewModel.Discipline;
            await Navigation.PushModalAsync(new NewRelationPage(discipline));
        }


        private Color OldFinishedColor;
        private async void Button_OnClicked(object sender, EventArgs e)
        {
            ShowFinished = !ShowFinished;
            var btn = sender as Button;
            if (ShowFinished)
            {
                OldFinishedColor = btn.BackgroundColor;
                btn.BackgroundColor = new Color(254/255.0,95/255.0,85/255.0);
            }
            else
            {
                btn.BackgroundColor = OldFinishedColor;
            }

            IEnumerable<Event> filtered = null;
            try
            {
                await Task.Factory.StartNew(() => filtered = FilterEvents(SearchBox.Text, ViewModel.Events.ToArray()));
            }
            catch (Exception)
            {
            }
            ViewModel.Filtered = filtered ?? new Event[0];
        }

        private Color OldVoteColor;
        private bool OrderByVotes;
        private async void OrderByVotesButton_OnClicked(object sender, EventArgs e)
        {
            OrderByVotes = !OrderByVotes;
            var btn = sender as Button;
            if (OrderByVotes)
            {
                OldVoteColor = btn.BackgroundColor;
                btn.BackgroundColor = new Color(254 / 255.0, 95 / 255.0, 85 / 255.0);
            }
            else
            {
                btn.BackgroundColor = OldVoteColor;
            }

            IEnumerable<Event> filtered = null;
            try
            {
                if (OrderByVotes)
                {
                    await Task.Factory.StartNew(() => filtered = FilterEvents(SearchBox.Text, ViewModel.Events.ToArray())
                     .OrderByDescending(ev => ev.Positive_Votes / (ev.Negative_Votes + 1)).ToArray());
                }
                else
                {
                    await Task.Factory.StartNew(() => filtered = FilterEvents(SearchBox.Text, ViewModel.Events.ToArray())
                    .OrderByDescending(ev => ev.StartDateTime).ToArray());
                }
                
                
            }
            catch (Exception)
            {
            }
            ViewModel.Filtered = filtered ?? new Event[0];
        }
    }
}
