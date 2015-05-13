using System;
using System.Collections.Generic;
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
            if (selected != null)
            {
                await Navigation.PushModalAsync(new EventPage(selected));
            }
        }

        private async void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                if (e.NewTextValue != e.OldTextValue)
                {
                    IEnumerable<Event> filtered = null;
                    await Task.Factory.StartNew(() => filtered = FilterEvents(e.NewTextValue,ViewModel.Events.ToArray()));
                    ViewModel.Filtered = filtered;
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
                return (from ev in toFilter
                                      where ev.Title.Contains(search)
                                            || ev.Description.ToLower().Contains(search)
                                            || ev.Venue.ToLower().Contains(search)
                                            || ev.User.Name.ToLower().Contains(search)
                                            || ev.StartDate.ToString().Contains(search)
                                      select ev).ToArray();
            }
            else
            {
                return toFilter;
            }
        }
    }
}
