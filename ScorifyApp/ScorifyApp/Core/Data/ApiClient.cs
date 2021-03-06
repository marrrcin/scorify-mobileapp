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
using ScorifyApp.Core.LogIn;
using ScorifyApp.Models;
using Tweetinvi.Core.Helpers;
using Tweetinvi.Core.Interfaces;

namespace ScorifyApp.Core.Data
{
    public class ApiClient
    {
        public static string ApiUrl = @"http://46.101.33.201:4000/api/";

        protected static FlurlClient GetFlurlClientForRequest(string apiRequest)
        {
            var client = new Flurl.Url(ApiUrl + apiRequest).WithHeader("Accept", @"application/vnd.scorify.v1");
            return client;
        }

        public static async Task<User> LogInUserAsync(string provider, string token, string secret = null)
        {
            try
            {
                var request = GetFlurlClientForRequest("sessions");
                object requestData;
                if (provider == "facebook")
                {
                    requestData = new
                    {
                        session = new
                        {
                            provider = provider,
                            provider_token = token
                        }
                    };
                }
                else
                {
                    requestData = new
                    {
                        session = new
                        {
                            provider = provider,
                            provider_token = token,
                            provider_secret = secret
                        }
                    };
                }
                

                var response = await request.PostJsonAsync(requestData);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserWrapper>(responseContent);
                return user.User;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public async static Task<IEnumerable<Discipline>> GetDisciplinesAsync()
        {
            var request = GetFlurlClientForRequest("disciplines");
            var result = await request.GetJsonAsync<DisciplinesCollection>();
            return result.Disciplines;
        }

        public async static Task<IEnumerable<Event>> GetEventsAsync(Discipline discipline)
        {
            try
            {
                var request = GetFlurlClientForRequest("disciplines/" + discipline.Id + "/events");
                var response = await request.GetJsonAsync<EventsCollection>();
                foreach (var evnt in response.Events)
                {
                    ParseDates(evnt);
                    evnt.Discipline = discipline;
                }
                return response.Events;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Event[0];
            }
        }

        private static void ParseDates(Event evnt)
        {
            DateTime d;
            if (DateTime.TryParse(evnt.Start_Date, out d))
            {
                evnt.StartDateTime = d;
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
                    {"start_date" , newEvent.StartDateTime.ToApiDateTimeFormat()},
                    {"end_date" , newEvent.EndDateTime.ToApiDateTimeFormat()},
                    {"contenders" , newEvent.Contenders.ToArray()}
                };
                var requestData = new {@event = eventData, user_id = newEvent.User.Id};
                var request = GetFlurlClientForRequest(@"disciplines/" + newEvent.Discipline.Id + @"/events");

                var response = await request
                       .WithHeader("Authorization", UserContext.Current.AuthorizationToken)
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
                    .WithHeader("Authorization", UserContext.Current.AuthorizationToken)
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
        public static async Task<IEnumerable<Message>> GetEventMessagesAsync(Event evnt,long? afterTimestamp = null)
        {
            try
            {
                var request = GetFlurlClientForRequest(@"disciplines/" + evnt.Discipline.Id + @"/events/" + evnt.Id + @"/messages");
                var response = await request.Url
                    .SetQueryParam("after",afterTimestamp ?? 0)
                    .WithHeader("Accept", @"application/vnd.scorify.v1")
                    .GetJsonAsync<MessageCollection>();
                foreach (var message in response.Messages)
                {
                    message.Created = message.Timestamp.ToDateTime();
                }
                return response.Messages;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Message[0];
            }
        }

        public static async Task<bool> UpdateScoreAsync(Event evnt, Dictionary<string,object> contender,string score)
        {
            try
            {
                object requestData;
                if (evnt.Discipline.Title.Contains("race"))
                {
                    requestData = new
                    {
                        contender = new
                        {
                            title = contender["title"],
                            total_time = score
                        }
                    };
                }
                else
                {
                    requestData = new
                    {
                        contender = new
                        {
                            title = contender["title"],
                            score = score
                        }
                    };
                }

                var request =
                    GetFlurlClientForRequest(@"disciplines/" + evnt.Discipline.Id + "/events/" + evnt.Id + "/contenders/" +
                                             contender["id"])
                        .WithHeader("Authorization", UserContext.Current.AuthorizationToken);
                var response = await request.PatchJsonAsync(requestData);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        public static async Task<User> GetUserDetailsAsync(string userId)
        {
            try
            {
                var request = GetFlurlClientForRequest(@"users/" + userId);
                var response = await request
                    .WithHeader("Accept", @"application/vnd.scorify.v1")
                    .GetJsonAsync<UserWrapper>();

                var user = response.User;

                if (user.Events.Any())
                {
                    var disciplinesMapping = await GetDisciplinesMapping();
                    foreach (var evnt in user.Events)
                    {
                        ParseDates(evnt);
                        evnt.User = user;
                        evnt.Discipline = new Discipline
                        {
                            Id = evnt.Discipline_Id,
                            Title = disciplinesMapping[evnt.Discipline_Id]
                        };
                    }
                }


                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private static async Task<Dictionary<string, string>> GetDisciplinesMapping()
        {
            var disciplinesMapping = (await ApiClient.GetDisciplinesAsync()).ToDictionary(kvp => kvp.Id, kvp => kvp.Title);
            return disciplinesMapping;
        }

        public static async Task<Event> GetEventDetailsAsync(Event evnt)
        {
            try
            {
                var disciplineId = evnt.Discipline_Id ?? evnt.Discipline.Id;
                var evntId = evnt.Id;
                var request = GetFlurlClientForRequest(@"disciplines/" + disciplineId + @"/events/" + evntId);
                var response = await request.GetJsonAsync<EventWrapper>();
                if (response.Event != null)
                {
                    var disciplinesMapping = await GetDisciplinesMapping();
                    var eventDetails = response.Event;
                    eventDetails.Discipline = new Discipline
                    {
                        Id = disciplineId,
                        Title = disciplinesMapping[disciplineId]
                    };
                    return response.Event;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        private class UserWrapper
        {
            public User User { set; get; }
        }

        public static async Task<bool> EditEventAsync(Event evnt)
        {
            try
            {
                var eventData = new Dictionary<string, object>
                {
                    {"title" , evnt.Title},
                    {"description" , evnt.Description},
                    {"venue" , evnt.Venue},
                    {"finished",evnt.Finished}
                };
                var requestData = new { @event = eventData, user_id = evnt.User.Id };
                var request = GetFlurlClientForRequest(@"disciplines/" + evnt.Discipline.Id + @"/events/"+evnt.Id);

                var response = await request
                       .WithHeader("Authorization", UserContext.Current.AuthorizationToken)
                       .PatchJsonAsync(requestData);

                if (response.StatusCode != HttpStatusCode.Created)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public static async Task<bool?> SendEventVote(Event evnt, bool isPositive)
        {
            try
            {
                var requestData = new
                {
                    vote = new Dictionary<string,bool>
                    {
                        {"positive?",isPositive}
                    }
                };

                var request =
                    GetFlurlClientForRequest(@"disciplines/" + evnt.Discipline.Id + "/events/" + evnt.Id + "/votes")
                        .WithHeader("Authorization", UserContext.Current.AuthorizationToken);

                var response = await request.PostJsonAsync(requestData);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
            return false;
        }

        public static async Task<IEnumerable<Comment>> GetEventCommentsAsync(Event evnt, long? afterTimestamp = null)
        {
            try
            {
                var request = GetFlurlClientForRequest(@"disciplines/" + evnt.Discipline.Id + @"/events/" + evnt.Id + @"/comments");
                var response = await request.Url
                    .SetQueryParam("after", afterTimestamp ?? 0)
                    .WithHeader("Accept", @"application/vnd.scorify.v1")
                    .GetJsonAsync<CommentCollection>();

                foreach (var comment in response.Comments)
                {
                    comment.Created = comment.Timestamp.ToDateTime();
                }
                return response.Comments;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new Comment[0];
            }
        }

        public static async Task<bool?> PostCommentAsync(Event evnt, string commentText)
        {
            try
            {
                var requestData = new
                {
                    comment = new
                    {
                        content = commentText
                    }
                };
                var request = GetFlurlClientForRequest(@"disciplines/" + evnt.Discipline.Id + @"/events/" + evnt.Id + @"/comments");
                var response = await request.Url
                    .WithHeader("Accept", @"application/vnd.scorify.v1")
                    .WithHeader("Authorization", UserContext.Current.AuthorizationToken)
                    .PostJsonAsync(requestData);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
            return false;
        }
    }

}
