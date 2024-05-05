using OptimaValue.Service;

IHost host = Host.CreateDefaultBuilder(args)
       .UseWindowsService()
    .ConfigureServices(services =>
    {
        services.AddHostedService<PlcWorker>();
    })
    .Build();

AppDomain.CurrentDomain.ProcessExit += (s, e) =>
{
    // Anropa kod f�r graci�s avst�ngning h�r
    host.Services.GetRequiredService<IHostApplicationLifetime>().StopApplication();
};

host.Run();
