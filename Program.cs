using Google.Apis.Auth.OAuth2;
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
            CalendarListResource.ListRequest myCalsRequest = service.CalendarList.List();
            myCalsRequest.PrettyPrint = false;
            var cals = myCalsRequest.Execute();
            foreach(var cal in cals.Items)
            {
                Console.WriteLine("Id: {0}", cal.Id);
                Console.WriteLine("Summary: {0}", cal.Summary);
                Console.WriteLine("Description: {0}", cal.Description);

                EventsResource.ListRequest request = service.Events.List(cal.Id);
                request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 10;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                // List events.
                Events events = request.Execute();
            
                if (events.Items != null && events.Items.Count > 0)
                {
                    Console.WriteLine("Access role: {0}", events.AccessRole);
                    Console.WriteLine("Description: {0}", events.Description);
                    Console.WriteLine("ETage: {0}", events.ETag);
                    Console.WriteLine("Kind: {0}", events.Kind);
                    Console.WriteLine("Summary: {0}", events.Summary);
                    Console.WriteLine("Updated: {0}", events.Updated);
                    Console.WriteLine("Upcoming events:");

                    foreach (var eventItem in events.Items)
                    {
                        string when = eventItem.Start.DateTime.ToString();
                        if (String.IsNullOrEmpty(when))
                        {
                            when = eventItem.Start.Date;
                        }
                        Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                    }
                }
                else
                {
                    Console.WriteLine("No upcoming events found.");
                }
                Console.WriteLine("----------------------------");
            }
            Console.Read();

        }
    }
}