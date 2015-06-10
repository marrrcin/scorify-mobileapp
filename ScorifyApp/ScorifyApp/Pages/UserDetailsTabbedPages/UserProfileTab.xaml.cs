using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages.UserDetailsTabbedPages
{
    public partial class UserProfileTab : ContentPage
    {
        private UserDetailsPageViewModel ViewModel;
        public UserProfileTab(UserDetailsPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }
    }
}
