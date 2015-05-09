using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScorifyApp.Models
{
    public class Discipline
    {
        public string Id { set; get; }

        public string Title { set; get; }
    }

    public class DisciplinesCollection
    {
        public IEnumerable<Discipline> Disciplines { set; get; }
    }
}
