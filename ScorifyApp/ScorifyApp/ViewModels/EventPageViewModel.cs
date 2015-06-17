using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Annotations;
using ScorifyApp.Models;
using Tweetinvi.Core.Extensions;

namespace ScorifyApp.ViewModels
{
    public class EventPageViewModel : INotifyPropertyChanged
    {
        public Event Event { set; get; }

        public string Contender1
        {
            get
            {
                if (Event.Contenders != null)
                {
                    var firstContender = FirstContender;
                    return firstContender != null ? firstContender["title"].ToString() : string.Empty;
                }
                return string.Empty;
            }
        }

        public Dictionary<string, object> FirstContender
        {
            get
            {
                var firstContender = Event.Contenders.FirstOrDefault();
                return firstContender;
            }
        }

        private string _score1;
        private string _score2;
        public string Score1
        {
            set
            {
                _score1 = value;
                OnPropertyChanged();
            }
            get { return _score1; }
        }

        public string Score2
        {
            set
            {
                _score2 = value;
                OnPropertyChanged();
            }
            get { return _score2; }
        }

        public string Contender2
        {
            get
            {
                if (Event.Contenders != null)
                {
                    var secondContender = SecondContender;
                    return secondContender != null ? secondContender["title"].ToString() : string.Empty;
                }
                return string.Empty;
            }
        }

        public Dictionary<string, object> SecondContender
        {
            get
            {
                var secondContender = Event.Contenders.Skip(1).FirstOrDefault();
                return secondContender;
            }
        }

        public IEnumerable<string> Contenders
        {
            get
            {
                if (Event.Contenders != null)
                {
                    return Event.Contenders.Select(c => c["title"]).Cast<string>();
                }
                return new string[0];
            }
        }

        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();

        private bool _requestActive;

        public bool IsRequesting
        {
            get { return _requestActive; }
            set { _requestActive = value; OnPropertyChanged(); }
        }


        public void UpdateMessages(IEnumerable<Message> newMessages)
        {
            newMessages.ForEach(msg => _messages.Insert(0, msg));
            OnPropertyChanged("Messages");
        }

        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
