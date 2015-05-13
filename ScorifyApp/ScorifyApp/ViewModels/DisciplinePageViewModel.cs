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
    class DisciplinePageViewModel : INotifyPropertyChanged
    {
        
        public Discipline Discipline { set; get; }

        private IEnumerable<Event> _events;
        public IEnumerable<Event> Events
        {
            get { return _events; }
            set { _events = value; OnPropertyChanged(); }
        }

        private IEnumerable<Event> _filtered;
        public IEnumerable<Event> Filtered
        {
            get { return _filtered;}
            set { _filtered = value; OnPropertyChanged(); }
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
