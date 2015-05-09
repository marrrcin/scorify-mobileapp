using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScorifyApp.Models;
using Tweetinvi.Core.Interfaces;

namespace ScorifyApp.Core.Data
{
    public class ApiClient
    {
        public static string ApiUrl = @"http://boiling-citadel-6747.herokuapp.com/api/";

        protected static FlurlClient GetFlurlClientForRequest(string apiRequest)
        {
            return new FlurlClient(ApiUrl + apiRequest);
        }

        public async static Task<IEnumerable<Discipline>> GetDisciplinesAsync()
        {
            var request = GetFlurlClientForRequest("disciplines");
            var result = await request.GetJsonAsync<DisciplinesCollection>();
            return result.Disciplines;
        }

        //public async static Task<IEnumerable<Event>> GetEvents(string discipline)
        //{
        //    var request = GetFlurlClientForRequest("disciplines/" + discipline + "/events");
        //}
    }

}
