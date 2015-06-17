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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                var parent = Parent as CarouselPage;
                if (parent != null)
                {
                    parent.CurrentPage = parent.Children[0];
                    parent.ForceLayout();
                }
            }
            catch (Exception)
            {
                // :D
            }
        }
    }
}
