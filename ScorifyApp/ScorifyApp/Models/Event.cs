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

        public string Description
        {
            get
            {
                return
                    @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam et eros et turpis blandit egestas. Sed sed turpis in tellus finibus dapibus in vitae leo. Fusce rhoncus nunc a velit mattis rhoncus. Vivamus aliquet libero sed augue condimentum, sit amet consequat sapien sodales. Nunc a tempor nisl. Vestibulum hendrerit lorem eget posuere semper. Sed dui mi, eleifend eget faucibus ac, rutrum ac diam. Praesent id risus in lacus cursus hendrerit id eget metus. Suspendisse vitae sapien dignissim, iaculis mi a, iaculis arcu. Donec quis lectus nec orci tristique venenatis non sit amet neque. Sed finibus aliquam volutpat. Sed sit amet aliquam augue. Phasellus a diam et sem suscipit ornare. Aenean malesuada sodales elit a iaculis.";
            }
        }

        public string Venue { set; get; }

        public DateTime StartDate { set; get; }

        public bool Finished { set; get; }
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
