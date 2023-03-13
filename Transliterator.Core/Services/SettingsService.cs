using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transliterator.Services;

namespace Transliterator.Core.Services
{
    public class SettingsService
    {
        public bool IsAutoStartEnabled { get; set; }

        private const string configurationFilePath = "Settings.json";
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            DefaultValueHandling = DefaultValueHandling.Include,
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public event EventHandler? SettingsLoadedEvent;

        public event EventHandler? SettingsResetEvent;

        public event EventHandler? SettingsSavedEvent;

        public SettingsService()
        {
            Load();
        }

        public void Load()
        {
            if (File.Exists(configurationFilePath))
            {
                var settings = File.ReadAllText(configurationFilePath);
                JsonConvert.PopulateObject(settings, this, JsonSerializerSettings);
            }
            else
            {
                Save();
            }

            SynchronizeJSONAndWindowsStartupSettings();

            SettingsLoadedEvent?.Invoke(this, EventArgs.Empty);
        }

        // TODO: Write the implementation
        public void Reset()
        {
            SettingsResetEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Save()
        {
            string settings = JsonConvert.SerializeObject(this, JsonSerializerSettings);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + configurationFilePath, settings);

            SynchronizeJSONAndWindowsStartupSettings();

            SettingsSavedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void SynchronizeJSONAndWindowsStartupSettings()
        {
            bool hasAutoStartEntry = AutostartMethods.HasAutostartEntry();

            if (IsAutoStartEnabled && !hasAutoStartEntry)
                AutostartMethods.WriteAutostartEntry();
            if (!IsAutoStartEnabled && hasAutoStartEntry)
                AutostartMethods.DeleteAutostartEntry();
        }
    }
}