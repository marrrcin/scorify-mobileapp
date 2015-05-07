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
    }
}
