using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ScorifyApp.Pages.EventTabbedPages
{
    public partial class StreamTab : ContentPage
    {
        public StreamTab()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await Task.Delay(4000);
            ActivityIndicator.IsVisible = false;
            MessageList.IsVisible = true;
        }
    }
}
