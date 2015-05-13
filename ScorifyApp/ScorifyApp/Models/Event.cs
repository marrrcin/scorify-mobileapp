using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScorifyApp.Models
{
    public class Event
    {
        public Discipline Discipline { set; get; }

        public User User { set; get; }

        public string Title { set; get; }

        public string Description { set; get; }

        public string Venue { set; get; }

        public DateTime StartDate { set; get; }

        public DateTime EndDate { set; get; }

        public bool Finished { set; get; }

        public IEnumerable<Dictionary<string,object>> Contenders { set; get; }
        public string Id { get; set; }
    }

    public class EventsCollection
    {
        public IEnumerable<Event> Events { set; get; }

        public EventsMeta Meta { set; get; }

    }

    public class EventsMeta
    {
        public Pagination Pagination { set; get; }
    }

    public struct Pagination
    {
        public int per_page;

        public int total_pages;

        public int total_objects;
    }
}
