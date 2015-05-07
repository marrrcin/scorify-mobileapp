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
    public class EventPageViewModel : INotifyPropertyChanged
    {
        public Event Event { set; get; }

        public IEnumerable<Message> Messages
        {
            get
            {
                var messages = Enumerable.Range(1, 40).Select(i =>
                {
                    var texts = new string[]
                    {
                        "Vivamus molestie mauris ut mauris aliquam, id ullamcorper lorem finibus.",
                        "Donec vitae faucibus nibh.",
                        "Cras vitae leo turpis.",
                        "Morbi pretium lectus vel lobortis finibus."
                    };
                    var msg = new Message
                    {
                        Content = texts[i%texts.Length],
                        Created = DateTime.Now
                    };
                    return msg;
                });
                return messages.ToArray();
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
