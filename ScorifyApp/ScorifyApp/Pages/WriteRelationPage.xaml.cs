using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Models;
using ScorifyApp.Pages.EventTabbedPages;
using ScorifyApp.Pages.WriteRelationTabbedPages;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages
{
    public partial class WriteRelationPage : CarouselPage
    {
        private EventPageViewModel StreamViewModel;

        private WriteRelationPageViewModel ViewModel;

        public WriteRelationPage(Event evnt)
        {
            StreamViewModel = new EventPageViewModel {Event = evnt};
            ViewModel=new WriteRelationPageViewModel();
            InitializeComponent();
            Children.Add(new WriteMessageTab(ViewModel));
            Children.Add(new StreamTab(StreamViewModel));
            

            BindingContext = ViewModel;
        }
    }
}
