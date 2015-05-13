using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
            var client =  new FlurlClient(ApiUrl + apiRequest);
            client.WithHeader("Content-Type", "application/json");
            client.WithHeader("Accept", "application/vnd.scorify.v1");
            return client;
        }

        public async static Task<IEnumerable<Discipline>> GetDisciplinesAsync()
        {
            var request = GetFlurlClientForRequest("disciplines");
            var result = await request.GetJsonAsync<DisciplinesCollection>();
            return result.Disciplines;
        }

        public async static Task<IEnumerable<Event>> GetEventsAsync(string discipline)
        {
            //var request = GetFlurlClientForRequest("disciplines/" + discipline + "/events");
            await Task.Delay(1);
            var rand = new Random();
            var events = Enumerable.Range(0, 15).Select(i => new Event
            {
                Title = "Event #" + (i + 1),
                User = new User { Name = "User" + (i % 4)},
                StartDate = DateTime.Now.AddHours(rand.Next(1,7666)),
                Finished = rand.Next() % 2 == 0,
                Venue = "Venue " + rand.Next()
            });

            return events.ToArray();
        }

        public static async Task<bool> CreateEventAsync(Event newEvent)
        {
            try
            {
                var eventData = new Dictionary<string,object>
                {
                    {"title" , newEvent.Title},
                    {"description" , newEvent.Description},
                    {"venue" , newEvent.Venue},
                    {"start_date" , newEvent.StartDate.ToApiDateTimeFormat()},
                    {"end_date" , newEvent.EndDate.ToApiDateTimeFormat()},
                    {"contenders" , newEvent.Contenders.ToArray()}
                };

                var requestData = new {@event = eventData, user_id = newEvent.User.Id};
                var requestJson = JsonConvert.SerializeObject(requestData);
                var request = GetFlurlClientForRequest(@"disciplines/" + newEvent.Discipline.Id + @"/events");
                var response = await request.PostJsonAsync(requestData);
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    return false;
                }

                var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ToString());
                newEvent.Id = responseData["id"];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
          
            return true;
        }

        public static async Task<bool> PostEventMessageAsync(Event evnt)
        {
            await Task.Delay(1);
            return true;
        }
        public static async Task<IEnumerable<Message>> GetEventMessages(Event evnt,DateTime? afterDate = null)
        {
            await Task.Delay(1);
            var rand = new Random();
            var texts = new string[]
            {
                "Vivamus molestie mauris ut mauris aliquam, id ullamcorper lorem finibus.",
                "Donec vitae faucibus nibh.",
                "Cras vitae leo turpis.",
                "Morbi pretium lectus vel lobortis finibus."
            };
            var messages = Enumerable.Range(1, 1).Select(i => new Message
            {
                Created = DateTime.Now,
                Content = texts[rand.Next()%texts.Length]
            });
            return messages.ToArray();
        }
    }

}
