using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Core.Data;
using ScorifyApp.ViewModels;
using Tweetinvi.Core.Extensions;
using Xamarin.Forms;

namespace ScorifyApp.Pages.WriteRelationTabbedPages
{
    public partial class UpdateScoreTab : ContentPage
    {
        private EventPageViewModel ViewModel;
        private string _buttonText;
        private Color _buttonColor;
        private Color ActiveColor = new Color(211 / 255.0, 183 / 255.0, 61 / 255.0, 1.0);
        public UpdateScoreTab(EventPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;

            _buttonText = UpdateButton.Text;
            _buttonColor = UpdateButton.BackgroundColor;


            InitializePage();
        }

        private void InitializePage()
        {
            if (ViewModel.Event.Discipline.Title.Contains("race"))
            {
                MultipleContendersBox.IsVisible = true;
                TwoContendersBox.IsVisible = false;

                UpdateMultipleContendersPanel(MultipleContendersBox);
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

        private void UpdateMultipleContendersPanel(StackLayout multipleContendersBox)
        {
            multipleContendersBox.Children.Clear();
            
            ViewModel.Contenders.ForEach(c =>
            {
                var contenderData =
                    ViewModel.Event.Contenders.FirstOrDefault(ct => (string) ct["title"] == c);
                string time = "0.00";
                if (contenderData.ContainsKey("total_time"))
                {
                    time = contenderData["total_time"].ToString();
                }

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
                    Text = time,
                    FontSize = 16.0
                });

                editScorePanel.Children.Add(new Entry
                {
                    Placeholder = "0:00"
                });
                multipleContendersBox.Children.Add(editScorePanel); 
            });
        }

        private async void UpdateButton_OnClicked(object sender, EventArgs e)
        {
            if (IsActive)
            {
                await DisplayAlert("Not so fast", "Are you in a hurry, my friend?", "nope");
                return;
            }
            IsActive = true;
            UpdateActivity(IsActive);
            bool success = true;
            if (MultipleContendersBox.IsVisible)
            {
                var toUpdate = MultipleContendersBox.Children.Where(c => c is StackLayout).Cast<StackLayout>().Select(sp =>
                {
                    var label = sp.Children.FirstOrDefault(x => x is Label) as Label;
                    var score = sp.Children.FirstOrDefault(x => x is Entry) as Entry;
                    var contenderData = ViewModel.Event.Contenders.FirstOrDefault(ct => (string)ct["title"] == label.Text);
                    double scoreTime = 0.0;
                    double.TryParse(score.Text, out scoreTime);
                    return new
                    {
                        contender = contenderData,
                        score = scoreTime.ToString()
                    };
                });

                foreach (var update in toUpdate)
                {
                    success &= await ApiClient.UpdateScoreAsync(ViewModel.Event, update.contender, update.score);
                    if (!success)
                    {
                        break;
                    }
                }
            }
            else
            {
                success &= await ApiClient.UpdateScoreAsync(ViewModel.Event, ViewModel.FirstContender, Score1Entry.Text);
                success &= await ApiClient.UpdateScoreAsync(ViewModel.Event, ViewModel.SecondContender, Score2Entry.Text);
            }

            if (!success)
            {
                await DisplayAlert("Sorry", "Could not update score", "OK");
            }
            else
            {
                var updated = await ApiClient.GetEventDetailsAsync(ViewModel.Event);
                if (updated != null)
                {
                    ViewModel.Event.Contenders = updated.Contenders;
                    InitializePage();
                }
                
            }
            IsActive = false;
            UpdateActivity(IsActive);

        }

        public bool IsActive { get; set; }

        private void UpdateActivity(bool isActive)
        {
            if (isActive)
            {
                UpdateButton.Text = "Updating...";
                UpdateButton.BackgroundColor = new Color(211 / 255.0, 183 / 255.0, 61 / 255.0, 1.0);
            }
            else
            {
                UpdateButton.Text = _buttonText;
                UpdateButton.BackgroundColor = _buttonColor;
            }

        }
    }
}
