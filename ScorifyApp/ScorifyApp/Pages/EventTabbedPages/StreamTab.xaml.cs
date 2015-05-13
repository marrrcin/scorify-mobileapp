using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScorifyApp.Core.Data;
using ScorifyApp.Models;
using ScorifyApp.ViewModels;
using Tweetinvi.Core.Extensions;
using Xamarin.Forms;
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ConvertToConstant.Local

namespace ScorifyApp.Pages.EventTabbedPages
{
    public partial class StreamTab : ContentPage
    {
        private volatile bool ShouldPollApi = true;

        private volatile bool ShouldUpdateMessages = false;

        private CancellationTokenSource ApiRequestCancel;

        private int ApiPollDelayMilliseconds = 5500;

        private Task ApiPollTask = null;

        private EventPageViewModel ViewModel;

        private IEnumerable<Message> UpdatedMessages = null;

        private object locker = new object();

        public StreamTab(EventPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = viewModel;
            MessageList.BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ApiRequestCancel = new CancellationTokenSource();
            ApiPollTask = Task.Factory.StartNew(async () =>
            {
                await ApiRequest();
            }
            , ApiRequestCancel.Token);
            
            Device.StartTimer(TimeSpan.FromMilliseconds(ApiPollDelayMilliseconds - 500.0),UpdateMessages);
        }

        private bool UpdateMessages()
        {
            if (ShouldUpdateMessages && UpdatedMessages != null)
            {
                lock (locker)
                {
                    ViewModel.UpdateMessages(UpdatedMessages);
                    UpdatedMessages = null;
                    ShouldUpdateMessages = false;
                    MessageList.ScrollTo(ViewModel.Messages.FirstOrDefault(),ScrollToPosition.Start,true);
                    ForceLayout();
                }
            }
            return ShouldPollApi;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ShouldPollApi = false;
            ApiRequestCancel.Cancel();
            ApiPollTask.Wait();
        }

        private async Task ApiRequest()
        {
            var viewModel = BindingContext as EventPageViewModel;
            if(viewModel == null)
            {
                await DisplayAlert("Sorry", "Something went wrong and I cannot get messages", "Cancel");
                return;
            }
            var messageComparer = new MessageComparer();
            while (ShouldPollApi)
            {
                await Task.Delay(ApiPollDelayMilliseconds, ApiRequestCancel.Token);
                var lastMessage = viewModel.Messages.OrderBy(msg => msg.Timestamp).LastOrDefault();
                var newMessages = await ApiClient.GetEventMessages(viewModel.Event, lastMessage != null ? lastMessage.Timestamp : 0);
                newMessages = newMessages.Except(viewModel.Messages, messageComparer);
                lock (locker)
                {
                    UpdatedMessages = newMessages.ToArray();
                    ShouldUpdateMessages = UpdatedMessages.Any();
                }
                if (ApiRequestCancel.IsCancellationRequested)
                {
                    break;
                }
            }
            viewModel.IsRequesting = false;
        }

        private class MessageComparer : IEqualityComparer<Message>
        {
            public bool Equals(Message x, Message y)
            {
                return x.Timestamp == y.Timestamp && y.Content == x.Content;
            }

            public int GetHashCode(Message obj)
            {
                return 1;
            }
        }
    }
}
