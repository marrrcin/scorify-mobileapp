using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Models;
using ScorifyApp.Pages.UserDetailsTabbedPages;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages
{
    public partial class UserDetailsPage : CarouselPage
    {
        private UserDetailsPageViewModel ViewModel;
        public UserDetailsPage(User user)
        {
            InitializeComponent();
            ViewModel = new UserDetailsPageViewModel
            {
                User = user
            };
            BindingContext = ViewModel;
            Children.Add(new UserProfileTab(ViewModel));
            Children.Add(new DisciplinePage("Your relations",user));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CurrentPage = Children.First();
        }
    }
}
