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
    public class NewEventPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public User User { set; get; }

        private string _contenders;
        public string Contenders { set { _contenders = value; OnPropertyChanged();} get{return _contenders;} }

        public Discipline Discipline { set; get; }

        private string _title;
        private string _description;
        private string _venue;

        public string Title
        {
            set { _title = value;OnPropertyChanged(); }
            get { return _title; }
        }

        public string Description
        {
            set { _description = value;OnPropertyChanged(); }
            get { return _description; }
        }

        public string Venue
        {
            set { _venue = value;OnPropertyChanged(); }
            get { return _venue;}
        }

        public DateTime StartDate { set; get; }

        public DateTime EndDate { set; get; }


        private TimeSpan _startTime;
        private TimeSpan _endTime;
        public TimeSpan StartTime { set { _startTime = value; OnPropertyChanged(); } get { return _startTime; } }

        public TimeSpan EndTime { set { _endTime = value; OnPropertyChanged(); } get { return _endTime; } }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
