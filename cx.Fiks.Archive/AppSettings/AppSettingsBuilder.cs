using cx.Fiks.Archive.Services;
using Microsoft.Extensions.Configuration;

namespace cx.Fiks.Archive.AppSettings
{
    public static class AppSettingsBuilder
    {
        public static MessageSender? MessageSender { get; set; }
        public static ApplicationSettings CreateAppSettings(IConfiguration configuration)
        {
            IConfigurationSection configSection = configuration.GetSection("AppSettings");
            if (configSection == null) return new ApplicationSettings();
            var settings = configSection.Get<ApplicationSettings>();
            return settings ?? new ApplicationSettings();
        }
    }
}