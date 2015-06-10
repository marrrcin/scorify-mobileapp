using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Annotations;
using ScorifyApp.Models;

namespace ScorifyApp.ViewModels
{
    public class UserDetailsPageViewModel : INotifyPropertyChanged
    {
        public User User { set; get; }

        public int EventsCount
        {
            get { return User.Events.Count(); }
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
