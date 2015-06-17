using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScorifyApp.Core.Data;
using ScorifyApp.Models;
using ScorifyApp.ViewModels;
using Xamarin.Forms;

namespace ScorifyApp.Pages.EventTabbedPages
{
    public partial class CommentsTab : ContentPage
    {
        private EventPageViewModel ViewModel;

        private volatile bool ShouldPollApi = true;

        private volatile bool ShouldUpdateComments = false;

        private volatile bool IsPollingNow = false;

        private CancellationTokenSource ApiRequestCancel;

        private int ApiPollDelayMilliseconds = 3670;

        private Task ApiPollTask = null;

        private IEnumerable<Comment> UpdatedComments = null;

        private object locker = new object();

        public CommentsTab(EventPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //ui
            _buttonColor = PostCommentButton.BackgroundColor;
            _buttonText = PostCommentButton.Text;

            //first request
            ViewModel.UpdateComments(await ApiClient.GetEventCommentsAsync(ViewModel.Event));

            //message polling thread
            ApiRequestCancel = new CancellationTokenSource();
            ApiPollTask = Task.Factory.StartNew(async () =>
            {
                await ApiRequest();
            }
            , ApiRequestCancel.Token);

            //UI refreshing thread
            Device.StartTimer(TimeSpan.FromMilliseconds(ApiPollDelayMilliseconds - 500.0), UpdateComments);
            Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                ActivityIndicator.IsVisible = IsPollingNow;
                return ShouldPollApi;
            });
        }

        private bool UpdateComments()
        {
            if (ShouldUpdateComments && UpdatedComments != null)
            {
                lock (locker)
                {
                    ViewModel.UpdateComments(UpdatedComments);
                    UpdatedComments = null;
                    ShouldUpdateComments = false;
                    CommentList.ScrollTo(ViewModel.Comments.FirstOrDefault(), ScrollToPosition.Start, true);
                    ForceLayout();
                }
            }
            return ShouldPollApi;
        }

        private async Task ApiRequest()
        {
            var viewModel = BindingContext as EventPageViewModel;
            if (viewModel == null)
            {
                await DisplayAlert("Sorry", "Something went wrong and I cannot get comments", "Cancel");
                return;
            }
            var commentComparer = new CommentComparer();
            while (ShouldPollApi)
            {
                await Task.Delay(ApiPollDelayMilliseconds, ApiRequestCancel.Token);
                IsPollingNow = true;
                var lastComment = viewModel.Comments.OrderBy(msg => msg.Timestamp).LastOrDefault();
                var newMessages = await ApiClient.GetEventCommentsAsync(viewModel.Event, lastComment != null ? lastComment.Timestamp : 0);
                newMessages = newMessages.Except(viewModel.Comments, commentComparer);
                lock (locker)
                {
                    UpdatedComments = newMessages.ToArray();
                    ShouldUpdateComments = UpdatedComments.Any();
                }
                IsPollingNow = false;
                if (ApiRequestCancel.IsCancellationRequested)
                {
                    break;
                }
            }
            viewModel.IsRequesting = false;
        }

        private class CommentComparer : IEqualityComparer<Comment>
        {
            public bool Equals(Comment x, Comment y)
            {
                return x.Timestamp == y.Timestamp && y.Content == x.Content;
            }

            public int GetHashCode(Comment obj)
            {
                return 1;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ShouldPollApi = false;
            ApiRequestCancel.Cancel();
            ApiPollTask.Wait();
        }

        private bool IsActive = false;
        private Color _buttonColor;
        private string _buttonText;

        private void UpdateActivity(bool isActive)
        {
            if (isActive)
            {
                PostCommentButton.Text = "Sending...";
                PostCommentButton.BackgroundColor = new Color(211 / 255.0, 183 / 255.0, 61 / 255.0, 1.0);
            }
            else
            {
                PostCommentButton.Text = _buttonText;
                PostCommentButton.BackgroundColor = _buttonColor;
            }

        }

        private async void PostCommentButton_OnClicked(object sender, EventArgs e)
        {
            if (IsActive)
            {
                await DisplayAlert("Not so fast", "Are you in a hurry, my friend?", "nope");
                return;
            }
            IsActive = true;
            UpdateActivity(IsActive);
            var comment = CommentTextBox.Text.Trim();
            if (string.IsNullOrEmpty(comment) || comment.Length < 3 || comment.Length > 128)
            {
                await DisplayAlert("Comment is incorrect", "Please write comment with proper length", "Cancel");
            }

            var response = await ApiClient.PostCommentAsync(ViewModel.Event, comment);
            if (response == false)
            {
                await DisplayAlert("Could not post comment", "Check your internet connection and try again later...", "Cancel");
            }
            else if (response == null)
            {
                await DisplayAlert("Not so fast", "You can't post two comments one after another", "nope");
            }
            else
            {
                await DisplayAlert("Success", "Comment added!", "ok");
                CommentTextBox.Text = string.Empty;
            }

            IsActive = false;
            UpdateActivity(IsActive);
        }
    }
}
