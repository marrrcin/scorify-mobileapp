using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Models;
using ScorifyApp.Pages.EventTabbedPages;
using ScorifyApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ScorifyApp.Pages
{
    public partial class EventPage : CarouselPage
    {
        private EventPageViewModel ViewModel;
        public EventPage(Event evnt)
        {
            InitializeComponent();
            ViewModel = new EventPageViewModel {Event = evnt};
            BindingContext = ViewModel;
            Children.Add(new StreamTab(ViewModel));
            Children.Add(new DescriptionTab(ViewModel));
            Children.Add(new CommentsTab(ViewModel));

            foreach (var tab in Children)
            {
                tab.BindingContext = BindingContext;
            }

            int i = 0;
            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                ForceLayout();
                OnCurrentPageChanged();
                ++i;
                return i <= 3;
            });
        }

        protected override void OnAppearing()
        {
            //workaround - tab did load correctly, so first tab was not displayed until CurrentPage was changed
            //base.OnAppearing();
            //var streamPage = Children.FirstOrDefault(c => c is StreamTab);
            //if (streamPage != null)
            //{
            //    CurrentPage = streamPage;
            //}
        }
    }
}
