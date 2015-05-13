﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var client = new Flurl.Url(ApiUrl + apiRequest).WithHeader("Accept", @"application/vnd.scorify.v1");
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
            try
            {
                var request = GetFlurlClientForRequest("disciplines/" + discipline + "/events");
                var response = await request.GetJsonAsync<EventsCollection>();
                return response.Events;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Event[0];
            }
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
                var response = await request
                    .PostJsonAsync(requestData);
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    return false;
                }

                var responseData = await GetResponseData(response);
                newEvent.Id = responseData["event"]["id"] as string;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
          
            return true;
        }

        private static async Task<Dictionary<string, Dictionary<string, object>>> GetResponseData(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(responseContent);
            return responseData;
        }

        public static async Task<bool> PostEventMessageAsync(Message msg)
        {
            try
            {
                var requestData = new {message = new{content = msg.Content}};
                var request = GetFlurlClientForRequest(@"disciplines/" + msg.Event.Discipline.Id + @"/events/" + msg.Event.Id + @"/messages");
                var response = await request
                    .PostJsonAsync(requestData);
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    return false;
                }
                var responseData = await GetResponseData(response);
                msg.Id = responseData["message"]["id"] as string;
                msg.Timestamp = Convert.ToInt64(responseData["message"]["timestamp"]);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public static async Task<IEnumerable<Message>> GetEventMessages(Event evnt,long? afterTimestamp = null)
        {
            try
            {
                var request = GetFlurlClientForRequest(@"disciplines/" + evnt.Discipline.Id + @"/events/" + evnt.Id + @"/messages");
                var response = await request.Url
                    .SetQueryParam("after",afterTimestamp ?? 0)
                    .WithHeader("Accept", @"application/vnd.scorify.v1")
                    .GetJsonAsync<MessageCollection>();
                return response.Messages;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Message[0];
            }
        }
    }

}
