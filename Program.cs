﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CalendarQuickstart
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


            //foreach(var cal in myCals)
            //{

            //}

            // Define parameters of request.
            // Get all of my calendars
            //CalendarListResource.ListRequest myCalsRequest = service.CalendarList.List();
            //var cals = myCalsRequest.Execute();
            //foreach()
            //Console.WriteLine("Id: {0}", cal.Id);
            //Console.WriteLine("Summary: {0}", cal.Summary);
            //Console.WriteLine("Description: {0}", cal.Description);
            List<calItem> familyEvents = new List<calItem>();

            List<string> cals = new List<string>()
            {
                "3ebumnacuflq2qoljg18rbavn8@group.calendar.google.com",
                "en.usa#holiday@group.v.calendar.google.com",
                "lloydturley2@gmail.com",
                "family14932423850734630239@group.calendar.google.com",
                "vkroqg925v3afevfe5t62aq3i8@group.calendar.google.com",
                "en.christian#holiday@group.v.calendar.google.com",
                "f2qeksj4vjbvvscqilpe0a5hn4e3h9ij@import.calendar.google.com",
                "waldentennessee@gmail.com",
                "s93rp3t2qo3k16kot7bd4nqs2s@group.calendar.google.com"
            };

            foreach(var cal in cals)
            {
                EventsResource.ListRequest request = service.Events.List(cal);
                request.TimeMin = DateTime.Now;
                request.TimeMax = DateTime.Now.AddDays(7);
                request.ShowDeleted = false;
                request.SingleEvents = true;
                //request.MaxResults = 10;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                // List events.
                Events events = request.Execute();
                //Console.WriteLine("Summary: {0}", events.Summary);

                if (events.Items != null && events.Items.Count > 0)
                {
                    //Console.WriteLine("Access role: {0}", events.AccessRole);
                    //Console.WriteLine("Description: {0}", events.Description);
                    //Console.WriteLine("ETage: {0}", events.ETag);
                    //Console.WriteLine("Kind: {0}", events.Kind);
                    //Console.WriteLine("Updated: {0}", events.Updated);
                    //Console.WriteLine("Upcoming events:");

                    bool allDay = false;

                    foreach (var eventItem in events.Items)
                    {
                        DateTime when;
                        if (eventItem.Start.DateTime==null)
                        {
                            when = DateTime.Parse(eventItem.Start.Date);
                            allDay = true;
                        } else
                        {
                            when = (DateTime)eventItem.Start.DateTime;
                            allDay = false;
                        }

                        calItem fe = new calItem
                        {
                            CalName = events.Summary,
                            EvtDate = when,
                            EvtName = eventItem.Summary,
                            AllDay = allDay
                        };

                        if (allDay == true)
                        {
                            string thing = $"All Day - {events.Summary} - {eventItem.Summary} - {when}";
                        } else
                        {
                            string thing = $"{events.Summary} - {eventItem.Summary} - {when}";
                        }
                        
                        familyEvents.Add(fe);

                        //Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                    }
                }
                else
                {
                    //Console.WriteLine("No upcoming events found.");
                }
                //Console.WriteLine("----------------------------");
            }
            foreach (var t in familyEvents.OrderBy(p => p.EvtDate))
            {
                Console.WriteLine($"{t.EvtDate} - {t.CalName} - {t.EvtName}");
            }
            Console.Read();

        }
    }
    public class calItem
    {
        public string CalName { get; set; }
        public string EvtName { get; set; }
        public DateTime EvtDate { get; set; }

        public bool AllDay { get; set; }
    }
}