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

        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();

        private bool _requestActive;

        public bool IsRequesting
        {
            get { return _requestActive; }
            set { _requestActive = value; OnPropertyChanged(); }
        }


        public void UpdateMessages(IEnumerable<Message> newMessages)
        {
            newMessages.ForEach(msg=>_messages.Insert(0,msg));
            OnPropertyChanged("Messages");    
        }

        public ObservableCollection<Message> Messages
        {
            get { return _messages;}
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
