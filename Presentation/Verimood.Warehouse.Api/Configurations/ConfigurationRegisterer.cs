namespace Verimood.Warehouse.Api.Configurations;

public static class ConfigurationRegisterer
{
    const string configurationDirectory = "Configurations";

    internal static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var env = builder.Environment;

        config.AddJsonFile("appsettings", env.EnvironmentName)
              .AddJsonFile("security", env.EnvironmentName);


        config.AddEnvironmentVariables();
        return builder;

    }

    private static ConfigurationManager AddJsonFile(this ConfigurationManager manager, string file, string? environment = null)
    {
        manager.AddJsonFile($"{configurationDirectory}/{file}.json", optional: true, reloadOnChange: false);

        if (environment != null)
        {
            manager.AddJsonFile($"{configurationDirectory}/{file}.{environment}.json", optional: true, reloadOnChange: false);
        }

        return manager;
    }
}
