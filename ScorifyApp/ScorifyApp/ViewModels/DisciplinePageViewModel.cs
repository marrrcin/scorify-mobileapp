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

        public IEnumerable<Event> Events
        {
            get
            {
                var rand = new Random();
                var events = Enumerable.Range(0,40).Select(i => new Event
                {
                    Title = "Event #" + (i+1),
                    User = new User{Name="User"+rand.Next()},
                    StartDate = DateTime.Now,
                    Discipline = Discipline,
                    Finished = rand.Next()%2 == 0,
                    Venue = "Venue " + rand.Next()
                });

                return events.ToArray();
            }
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
