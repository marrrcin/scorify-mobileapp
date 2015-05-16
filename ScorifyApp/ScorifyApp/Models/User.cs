using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScorifyApp.Models
{
    public class User
    {
        public string Email { set; get; }
        public string Id { get; set; }

        public string Auth_Token { set; get; }

        public IEnumerable<Event> Events { set; get; }
    }
}
