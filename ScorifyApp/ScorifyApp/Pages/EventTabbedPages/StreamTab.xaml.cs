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

        private volatile bool IsPollingNow = false;

        private volatile bool ShouldUpdateScore = false;

        private CancellationTokenSource ApiRequestCancel;

        private int ApiPollDelayMilliseconds = 3000;

        private Task ApiPollTask = null;

        private EventPageViewModel ViewModel;

        private IEnumerable<Message> UpdatedMessages = null;

        private IEnumerable<Dictionary<string, object>> UpdatedContenders = null;

        private object locker = new object();

        public StreamTab(EventPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = viewModel;
            MessageList.BindingContext = ViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //first request
            ActivityIndicator.IsVisible = true;
            ViewModel.UpdateMessages(await ApiClient.GetEventMessagesAsync(ViewModel.Event));
            ActivityIndicator.IsVisible = false;

            //event polling thread
            ApiRequestCancel = new CancellationTokenSource();
            ApiPollTask = Task.Factory.StartNew(async () =>
            {
                await ApiRequest();
            }
            , ApiRequestCancel.Token);

            //UI refreshing thread
            Device.StartTimer(TimeSpan.FromMilliseconds(ApiPollDelayMilliseconds - 500.0),UpdateView);
            Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                ActivityIndicator.IsVisible = IsPollingNow;
                return ShouldPollApi;
            });

            InitializeScores();
        }

        private void InitializeScores()
        {
            if (ViewModel.Event.Discipline.Title.Contains("race"))
            {
                MultipleContendersStackLayout.IsVisible = true;
                TwoContendersBox.IsVisible = false;

                UpdateMultipleContendersPanel(MultipleContendersStackLayout);
            }
            else
            {
                const string scoreKey = "score";
                try
                {
                    ViewModel.Score1 = ViewModel.FirstContender[scoreKey].ToString();
                    ViewModel.Score2 = ViewModel.SecondContender[scoreKey].ToString();
                }
                catch (Exception e)
                {
                    // f*** null reference
                }
            }
        }

        private bool UpdateView()
        {
            UpdateMessagesView();
            UpdateScoreView();
            return ShouldPollApi;
        }

        private void UpdateScoreView()
        {
            if (ShouldUpdateScore && UpdatedContenders != null)
            {
                lock (locker)
                {
                    ViewModel.Event.Contenders = UpdatedContenders;
                    UpdatedContenders = null;
                    ShouldUpdateScore = false;
                    ViewModel.ScoreUpdateRequired = false;
                    InitializeScores();
                }
            }
        }

        private void UpdateMessagesView()
        {
            if (ShouldUpdateMessages && UpdatedMessages != null)
            {
                lock (locker)
                {
                    ViewModel.UpdateMessages(UpdatedMessages);
                    UpdatedMessages = null;
                    ShouldUpdateMessages = false;
                    MessageList.ScrollTo(ViewModel.Messages.FirstOrDefault(), ScrollToPosition.Start, true);
                    ForceLayout();
                }
            }
        }

        private void UpdateMultipleContendersPanel(StackLayout multipleContendersBox)
        {
            multipleContendersBox.Children.Clear();

            ViewModel.Contenders.ForEach(c =>
            {
                var time = GetTimeScore(c);
                var editScorePanel = PrepareScorePanel(c, time);
                multipleContendersBox.Children.Add(editScorePanel);
            });
        }

        private string GetTimeScore(string c)
        {
            var contenderData =
                ViewModel.Event.Contenders.FirstOrDefault(ct => (string)ct["title"] == c);
            string time = "0:00";
            if (contenderData.ContainsKey("total_time"))
            {
                time = contenderData["total_time"].ToString();
            }
            return time;
        }

        private static StackLayout PrepareScorePanel(string c, string time)
        {
            var editScorePanel = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            editScorePanel.Children.Add(new Label
            {
                Text = c,
                FontSize = 16.0
            });

            editScorePanel.Children.Add(new Label
            {
                Text = time + " seconds",
                FontSize = 16.0
            });
            return editScorePanel;
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
                try
                {
                    await Task.Delay(ApiPollDelayMilliseconds, ApiRequestCancel.Token);
                    IsPollingNow = true;
                    await PollMessages(viewModel, messageComparer);
                    await PollScore(viewModel);
                    IsPollingNow = false;
                    if (ApiRequestCancel.IsCancellationRequested)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    // :D
                }
            }
            viewModel.IsRequesting = false;
        }

        private async Task PollScore(EventPageViewModel viewModel)
        {
            var eventDetails = await ApiClient.GetEventDetailsAsync(viewModel.Event);
            if (eventDetails != null)
            {
                if (eventDetails.Contenders.Any(ScoreChanged(viewModel)) || ViewModel.ScoreUpdateRequired)
                {
                    lock (locker)
                    {
                        ShouldUpdateScore = true;
                        UpdatedContenders = eventDetails.Contenders;
                    }
                }
            }
        }

        private static Func<Dictionary<string, object>, bool> ScoreChanged(EventPageViewModel viewModel)
        {
            return c =>
            {
                var ctdr = viewModel.Event.Contenders.FirstOrDefault(cd => cd["title"].ToString() == c["title"].ToString());
                if (ctdr.ContainsKey("score"))
                {
                    return ctdr["score"].ToString() != c["score"].ToString();
                }
                else
                {
                    return ctdr["total_time"].ToString() != c["total_time"].ToString();
                }
            };
        }

        private async Task PollMessages(EventPageViewModel viewModel, MessageComparer messageComparer)
        {
            var lastMessage = viewModel.Messages.OrderBy(msg => msg.Timestamp).LastOrDefault();
            var newMessages =
                await ApiClient.GetEventMessagesAsync(viewModel.Event, lastMessage != null ? lastMessage.Timestamp : 0);
            newMessages = newMessages.Except(viewModel.Messages, messageComparer);
            lock (locker)
            {
                UpdatedMessages = newMessages.ToArray();
                ShouldUpdateMessages = UpdatedMessages.Any();
            }
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
