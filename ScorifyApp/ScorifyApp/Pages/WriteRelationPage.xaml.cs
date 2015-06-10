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
            InitializeComponent();
            StreamViewModel = new EventPageViewModel {Event = evnt};
            ViewModel=new WriteRelationPageViewModel {Event = evnt};
            Children.Add(new WriteMessageTab(ViewModel));
            Children.Add(new StreamTab(StreamViewModel));
            Children.Add(new DescriptionTab(StreamViewModel));
            Children.Add(new EditEventTab(evnt));
            BindingContext = ViewModel;
            int i = 0;
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                ForceLayout();
                OnCurrentPageChanged();
                ++i;
                return i <= 3;
            });
        }

    }
}
