using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScorifyApp.Models
{
    public class Comment
    {
        public string Id { set; get; }

        public string User_Email { set; get; }

        public string Content { set; get; }

        public DateTime Created { set; get; }

        public long Timestamp { set; get; }
    }

    public class CommentCollection
    {
        public IEnumerable<Comment> Comments { set; get; }
    }
}
