using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Gravity.Configuration
{
    public class AppConfigSettingsProvider : ISettingsProvider
    {
        private readonly IConfiguration _configuration;

        public AppConfigSettingsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string GetConnectionString(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            var connectionString = _configuration.GetConnectionString(key);
            
            if (string.IsNullOrWhiteSpace(connectionString)) NotFound(key);
            
            return connectionString;
        }

        public string GetAppSetting(string key)
        {
            var appSetting = ConfigurationManager.AppSettings.Get(key);
            if (string.IsNullOrWhiteSpace(appSetting)) NotFound(key);
            return appSetting;
        }

        public string GetAppSetting(string key, string defaultValue)
        {
            var appSetting = ConfigurationManager.AppSettings.Get(key);
            if (string.IsNullOrWhiteSpace(appSetting)) return defaultValue;
            return appSetting;
        }

        public T GetNumericAppSetting<T>(string key) where T : struct
        {
            var appSetting = GetAppSetting(key);
            if (string.IsNullOrWhiteSpace(appSetting))
            {
                throw NotFound(key);
            }
            var result = (T) Convert.ChangeType(decimal.Parse(appSetting), typeof(T));
            return result;
        }

        public T GetNumericAppSetting<T>(string key, T defaultValue) where T : struct
        {
            var appSetting = GetAppSetting(key);
            if (string.IsNullOrWhiteSpace(appSetting)) return defaultValue;
            var result = (T) Convert.ChangeType(decimal.Parse(appSetting), typeof(T));
            return result;
        }

        public int GetIntAppSetting(string key)
        {
            var appSetting = GetAppSetting(key);
            if (string.IsNullOrWhiteSpace(appSetting))
            {
                throw NotFound(key);
            }
            var result = int.Parse(appSetting);
            return result;
        }

        public int GetIntAppSetting(string key, int defaultValue)
        {
            var appSetting = GetAppSetting(key);
            if (string.IsNullOrWhiteSpace(appSetting)) return defaultValue;
            var result = int.Parse(appSetting);
            return result;
        }

        public T GetEnumAppSetting<T>(string key, T defaultValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) return defaultValue;
            var appSetting = GetAppSetting(key);
            if (string.IsNullOrWhiteSpace(appSetting)) return defaultValue;
            var enu = (T) Enum.Parse(typeof(T), appSetting);
            var result = enu;
            return result;
        }

        public bool GetBooleanAppSetting(string key)
        {
            var appSetting = GetAppSetting(key);
            if (string.IsNullOrWhiteSpace(appSetting))
            {
                throw NotFound(key);
            }
            var result = bool.Parse(appSetting);
            return result;
        }

        public bool GetBooleanAppSetting(string key, bool defaultValue)
        {
            var appSetting = GetAppSetting(key);
            if (string.IsNullOrWhiteSpace(appSetting)) return defaultValue;
            var result = bool.Parse(appSetting);
            return result;
        }

        public T GetGenericSection<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            var section = ConfigurationManager.GetSection(key);
            if (section == null)
            {
                throw NotFound(key);
            }
            return (T) section;
        }
        
        public IConfigurationSection GetSection(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            var section = _configuration.GetSection(key);
            if (section == null)
            {
                throw NotFound(key);
            }
            return section;
        }

        private Exception NotFound(string key)
        {
            return new SettingsPropertyNotFoundException($"AppSetting '{key}' not found or empty.");
        }
    }
}