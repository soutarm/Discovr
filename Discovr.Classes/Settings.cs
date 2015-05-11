using System;
using System.IO.IsolatedStorage;

namespace Discovr.Classes.Core
{
    public class AppSettings
    {
        // Our settings
        private readonly IsolatedStorageSettings _settings;

        // The key names of our settings
        private const string LocationConsentSettingKeyName = "LocationConsent";
        private const string ShowMetricSettingKeyName = "ShowMetric";
        private const string LastKnownLatitudeSettingKeyName = "LastKnownLatitudeSetting";
        private const string LastKnownLongitudeSettingKeyName = "LastKnownLongitudeSetting";

        // The default value of our settings
        private const bool LocationConsentDefault = false;
        private const bool ShowMetricSettingDefault = true;
        private const double LastKnownLatitudeSettingDefault = 40.77;
        private const double LastKnownLongitudeSettingDefault = -73.98;

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            // Get the settings for this application.
            _settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (_settings.Contains(key))
            {
                // If the value has changed
                if (_settings[key] != value)
                {
                    // Store the new value
                    _settings[key] = value;
                    valueChanged = true;
                }
            }
                // Otherwise create the key.
            else
            {
                _settings.Add(key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (_settings.Contains(key))
            {
                value = (T) _settings[key];
            }
                // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            _settings.Save();
        }


        /// <summary>
        /// Property to get and set the LocationConsent
        /// </summary>
        public bool LocationConsent
        {
            get { return GetValueOrDefault(LocationConsentSettingKeyName, LocationConsentDefault); }
            set { if (AddOrUpdateValue(LocationConsentSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the ShowMetricSetting
        /// </summary>
        public bool ShowMetric
        {
            get { return GetValueOrDefault(ShowMetricSettingKeyName, ShowMetricSettingDefault); }
            set { if (AddOrUpdateValue(ShowMetricSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the LastKnownLatitude
        /// </summary>
        public double LastKnownLatitude
        {
            get { return GetValueOrDefault(LastKnownLatitudeSettingKeyName, LastKnownLatitudeSettingDefault); }
            set { if (AddOrUpdateValue(LastKnownLatitudeSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the LastKnownLongitude
        /// </summary>
        public double LastKnownLongitude
        {
            get { return GetValueOrDefault(LastKnownLongitudeSettingKeyName, LastKnownLongitudeSettingDefault); }
            set { if (AddOrUpdateValue(LastKnownLongitudeSettingKeyName, value)) Save(); }
        }

    }
}
