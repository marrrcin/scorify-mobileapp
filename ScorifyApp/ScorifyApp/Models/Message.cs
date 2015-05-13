using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScorifyApp.Models
{
    public class Message
    {
        public Event Event { set; get; }

        public string Content { set; get; }

        public string AttachmentUrl { set; get; }

        public DateTime Created { set; get; }

        public long Timestamp { set; get; }

        public string Id { set; get; }
    }

    public class MessageCollection
    {
        public IEnumerable<Message> Messages { set; get; }
    }
}
