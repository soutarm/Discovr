
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discovr.Classes.Core
{
    public class LocationEntity
    {
        public int EntityId { get; set; }
        public string Tags { get; set; }
        public string EntityLabel { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AvatarUrl { get; set; }

        public static Task<ICollection<LocationEntity>> GetFriends(int myEntityId, string tagName = "")
        {
            if (myEntityId < 1) return null;

            var taskCompletion = new TaskCompletionSource<ICollection<LocationEntity>>();
            var webClient = new WebClient();

            webClient.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    taskCompletion.TrySetException(e.Error);
                }
                else if (e.Cancelled)
                {
                    taskCompletion.TrySetCanceled();
                }
                else
                {
                    var fakeFriends = new List<LocationEntity>();
                    fakeFriends.Add(new LocationEntity { EntityId = 2, Tags = "friends,workmates", EntityLabel = "Jussy", Latitude = -37.756404, Longitude = 145.058048, AvatarUrl = "https://lh3.googleusercontent.com/-P3KcoaodZXg/AAAAAAAAAAI/AAAAAAAAADc/zcihOy34pIk/s80-c-k-no/photo.jpg" });
                    fakeFriends.Add(new LocationEntity { EntityId = 3, Tags = "friends,workmates", EntityLabel = "Ryan", Latitude = -37.864168, Longitude = 144.973086, AvatarUrl = "https://lh5.googleusercontent.com/-eUNuNiyv_zs/AAAAAAAAAAI/AAAAAAAAAJI/x_a8FEigKBI/s80-c-k-no/photo.jpg" });
                    fakeFriends.Add(new LocationEntity { EntityId = 4, Tags = "friends,workmates", EntityLabel = "Ottey", Latitude = -37.854037, Longitude = 144.993857, AvatarUrl = "https://lh5.googleusercontent.com/-ohzGFwW-HR8/AAAAAAAAAAI/AAAAAAAAAec/4u001Y5PCBE/s80-c-k-no/photo.jpg" });
                    fakeFriends.Add(new LocationEntity { EntityId = 5, Tags = "friends,family", EntityLabel = "Nate", Latitude = -37.687999, Longitude = 145.109702, AvatarUrl = "" });
                    fakeFriends.Add(new LocationEntity { EntityId = 6, Tags = "friends,family", EntityLabel = "Indy", Latitude = -37.724313, Longitude = 145.141057, AvatarUrl = "" });
                    //fakeFriends.Add(new LocationEntity { EntityId = 7, EntityLabel = "Tim", Latitude = -31.986048, Longitude = 115.872459, AvatarUrl = "" });

                    if (!string.IsNullOrEmpty(tagName))
                    {
                        taskCompletion.TrySetResult(fakeFriends.Where(ff =>  ff.Tags.Contains(tagName.ToLower())).ToList());
                    }
                    else
                    {
                        taskCompletion.TrySetResult(fakeFriends);
                    }
                }
            };

            // fake URL as a placeholder
            webClient.DownloadStringAsync(new Uri("http://www.google.com"));

            return taskCompletion.Task;
        }
    }
}
