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

namespace ScorifyApp.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable<Discipline> Disciplines
        {
            get
            {
                var disciplines = new[]
                {
                    new Discipline {Id = "1", Title = "Football"},
                    new Discipline {Id = "2", Title = "Volleyball"},
                    new Discipline {Id = "3", Title = "Basketball"}
                };
                return disciplines;
            }
        }
    }
}
