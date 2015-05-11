using System;
using System.Diagnostics;
using System.Windows;
using Discovr.Classes.Core;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;

namespace Discovr.Agent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private Geoposition PreviousPosition { get; set; }


        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected async override void OnInvoke(ScheduledTask task)
        {
            var geolocator = new Geolocator();

            var maximumAge = TimeSpan.FromMinutes(1);
            var timeOut = TimeSpan.FromSeconds(30);

            try
            {

                var geoposition = await geolocator.GetGeopositionAsync(maximumAge, timeOut);

                if (geoposition != PreviousPosition)
                {
                    var userLocation = GeoConverter.ConvertGeocoordinate(geoposition.Coordinate);
                    PreviousPosition = geoposition;

                    // TODO: Send update to cloud server
                    //var toast = new ShellToast
                    //{
                    //    Title = "Location DiscovR'd",
                    //    Content = string.Format("{0}:{1} {2} accuracy", userLocation.Latitude, userLocation.Longitude, GeoConverter.MetersToReadableString(geoposition.Coordinate.Accuracy, true))
                    //};
                    //toast.Show();
                }
                else
                {
                    //var toast = new ShellToast { Title = "DiscovR", Content = string.Format("You haven't moved!") };
                    //toast.Show();
                }
            }
            catch (Exception ex)
            {
                if ((uint) ex.HResult == 0x80004004)
                {
                    var toast = new ShellToast { Title = "DiscovR", Content = "Location is disabled on this device :(" };
                    toast.Show();
                }
            }
            

            NotifyComplete();
        }
    }
}