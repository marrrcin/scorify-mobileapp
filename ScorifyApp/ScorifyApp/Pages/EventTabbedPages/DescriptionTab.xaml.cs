using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages.EventTabbedPages
{
    public partial class DescriptionTab : ContentPage
    {
        private EventPageViewModel ViewModel;
        public DescriptionTab()
        {
            InitializeComponent();
        }

        public DescriptionTab(EventPageViewModel viewModel) : this()
        {
            ViewModel = viewModel;
            BindingContext = viewModel;
        }
    }
}
