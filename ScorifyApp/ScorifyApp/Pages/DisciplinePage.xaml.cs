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

        public DisciplinePage(Discipline discipline)
        {
            InitializeComponent();
            ViewModel = new DisciplinePageViewModel {Discipline = discipline};
            BindingContext = ViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.Events = await ApiClient.GetEventsAsync(ViewModel.Discipline.Title);
            ViewModel.Filtered = ViewModel.Events;
        }

        private async void EventsList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selected = e.SelectedItem as Event;
            EventsList.SelectedItem = null;
            if (selected != null)
            {
                selected.Discipline = ViewModel.Discipline;
                await Navigation.PushModalAsync(new EventPage(selected));
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
                    where ev.Title.ToLower().Contains(search)
                          || ev.Description.ToLower().Contains(search)
                          || ev.Venue.ToLower().Contains(search)
                          //|| ev.User.Name.ToLower().Contains(search) TODO PUT USERNAME HERE
                          || ev.StartDate.ToString().Contains(search)
                    select ev;
                return filtered.ToArray();
            }
            else
            {
                return toFilter;
            }
        }

        private async void NewRelationButton_OnClicked(object sender, EventArgs e)
        {
            var discipline = ViewModel.Discipline;
            await Navigation.PushModalAsync(new NewRelationPage(discipline));
        }
    }
}
