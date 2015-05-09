using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScorifyApp.Core
{
    public interface ITwitterApi
    {
        string ConsumerKey { get; }

        string ConsumerSecret { get; }

        string AccessToken { get; }

        string AccessSecret { get; }
    }
}
