using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScorifyApp.Annotations;
using ScorifyApp.Core;
using ScorifyApp.Core.Data;
using ScorifyApp.Core.LogIn;
using ScorifyApp.Models;

namespace ScorifyApp.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected FacebookLogin _facebookLogin = new FacebookLogin();

        protected TwitterLogin _twitterLogin = new TwitterLogin();

        private bool isLoggedIn = false;
        public bool IsLoggedIn
        {
            set { isLoggedIn = value; OnPropertyChanged(); OnPropertyChanged("IsNotLoggedIn"); }
            get { return isLoggedIn; }
        }

        public bool IsNotLoggedIn
        {
            get { return !IsLoggedIn;}
        }

        public FacebookLogin FacebookLogin
        {
            get
            {
                return _facebookLogin;
            }
        }

        public TwitterLogin TwitterLogin
        {
            get
            {
                return _twitterLogin;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<Discipline> _disciplines;
        public IEnumerable<Discipline> Disciplines
        {
            get { return _disciplines ?? new Discipline[0]; }
            set { _disciplines = value; OnPropertyChanged(); }
        }

        public async Task LoadDisciplines()
        {
            Disciplines = await ApiClient.GetDisciplinesAsync();
        }
    }
}
