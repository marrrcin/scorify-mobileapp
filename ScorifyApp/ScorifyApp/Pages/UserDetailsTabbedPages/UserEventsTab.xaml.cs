using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Models;
using Xamarin.Forms;

namespace ScorifyApp.Pages.UserDetailsTabbedPages
{
    public partial class UserEventsTab : ContentPage
    {
        public UserEventsTab()
        {
            InitializeComponent();
        }

        private void UserEventsList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selected = e.SelectedItem as Event;
            UserEventsList.SelectedItem = null;
            if (selected != null)
            {
                   
            }
        }
    }
}
