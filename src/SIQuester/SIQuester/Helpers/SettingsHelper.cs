﻿using Polly;
using SIQuester.Model;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace SIQuester.Helpers;

internal static class SettingsHelper
{
    public const string SettingsFolderName = "Settings";

    private static readonly string SettingsFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        AppSettings.ManufacturerName,
        AppSettings.ProductName,
        SettingsFolderName);

    /// <summary>
    /// User settings file name.
    /// </summary>
    internal const string UserSettingsFileName = "usersettings.json";

    /// <summary>
    /// Loads user settings.
    /// </summary>
    public static AppSettings? LoadUserSettings()
    {
        try
        {
            var settingsFile = Path.Combine(SettingsFolder, UserSettingsFileName);

            if (File.Exists(settingsFile))
            {
                try
                {
                    using var stream = File.Open(settingsFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var settings = JsonSerializer.Deserialize<AppSettings>(stream);

                    if (settings != null)
                    {
                        settings.HasChanges = false;
                    }

                    return settings;
                }
                catch { }
            }
        }
        catch { }

        return null;
    }

    /// <summary>
    /// Saves user settings.
    /// </summary>
    internal static void SaveUserSettings(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(SettingsFolder);
            var settingsFile = Path.Combine(SettingsFolder, UserSettingsFileName);

            var retryPolicy = Policy
                .Handle<IOException>(exc => exc.HResult == -2147024864) // File being used by another process
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt)));

            retryPolicy.Execute(() =>
            {
                using var stream = File.Create(settingsFile);
                JsonSerializer.Serialize(stream, settings);
            });
        }
        catch (Exception exc)
        {
            MessageBox.Show(
                $"{Properties.Resources.SettingsSavingError}: {exc.Message}",
                AppSettings.ProductName,
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
        }
    }
}
