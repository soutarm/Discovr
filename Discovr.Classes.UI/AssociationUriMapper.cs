using System;
using System.Windows.Navigation;

namespace Discovr.Classes.UI
{
    public class AssociationUriMapper : UriMapperBase
    {
        private string _tempUri;

        public override Uri MapUri(Uri uri)
        {
            _tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

            // URI association launch for Discovr.
            if (_tempUri.Contains("discovr:location?Data="))
            {
                // Get the LatLong (after "LatLong=").
                var locationIndex = _tempUri.IndexOf("Data=") + "Data=".Length;
                var location = _tempUri.Substring(locationIndex);

                // Map the show products request to ShowProducts.xaml
                return new Uri("/MainPage.xaml?Location=" + location, UriKind.Relative);
            }

            // Otherwise perform normal launch.
            return uri;
        }
    }
}
