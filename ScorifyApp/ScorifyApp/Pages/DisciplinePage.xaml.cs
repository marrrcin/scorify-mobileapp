using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private async void EventsList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selected = e.SelectedItem as Event;
            if (selected != null)
            {
                await Navigation.PushModalAsync(new EventPage(selected));
            }
        }
    }
}
