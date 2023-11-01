using cx.Fiks.Archive;
using cx.Fiks.Archive.AppSettings;
using cx.Fiks.Archive.Services;
using cx.Fiks.Archive.Services.FiksIO;
using KS.Fiks.IO.Client;
using Serilog;

var configurationRoot = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true).Build();

var loggerFactory = InitSerilogConfiguration(configurationRoot);
////var appSettings = AppSettingsBuilder.CreateAppSettings(configurationRoot);
////var configuration = FiksIoConfigurationBuilder.CreateConfiguration(appSettings);
////var fiksIoClient = await FiksIOClient.CreateAsync(configuration, loggerFactory);

// Creating messageSender as a local instance
////AppSettingsBuilder.MessageSender = new MessageSender(fiksIoClient, appSettings);

// Setting the account to send messages to. In this case the same as sending account
////var toAccountId = appSettings.FiksIOConfig.FiksIoAccountId;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration((configHost) =>
    {
        configHost.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton(loggerFactory);
        ////services.AddScoped<SampleService>();
        services.AddSingleton<PeriodicHostedService>();
        services.AddHostedService(
            provider => provider.GetRequiredService<PeriodicHostedService>());

    });



IHost host = builder.Build();

await host.RunAsync();

static ILoggerFactory InitSerilogConfiguration(IConfiguration configuration)
{
    var loggerConfiguration = new LoggerConfiguration();
    loggerConfiguration.ReadFrom.Configuration(configuration);

    var logger = loggerConfiguration.CreateLogger();
    Log.Logger = logger;

    return LoggerFactory.Create(logging => logging.AddSerilog(logger));
}