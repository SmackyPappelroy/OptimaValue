using FileLogger;
using OptimaValue.Config;

namespace OptimaValue.Service
{
    public class PlcWorker : BackgroundService
    {
        private FileLog LoggerInstance;
        private readonly ILogger<PlcWorker> _logger;
        private readonly IHostApplicationLifetime applicationLifetime;

        public PlcWorker(ILogger<PlcWorker> logger, IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            this.applicationLifetime = applicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && !Master.Stopping)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(1000, stoppingToken);
                if (!Master.IsStarted && !Master.Stopping)
                {
                    Settings.Load();
                    LoggerInstance = CreateFileLogger();

                    PlcConfig.PopulateDataTable();
                    if (await Master.StartLog().ConfigureAwait(false))
                    {
                        Logger.LogInfo("Loggnings-service startad");
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
            await Task.Delay(1000, stoppingToken);
            applicationLifetime.StopApplication();
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Master.StopLog(true);
            Logger.LogError($"Worker stopped at: {DateTimeOffset.Now}");
            await Task.Delay(2000);
            Logger.Dispose();
        }

        public FileLog CreateFileLogger()
        {
            return new Logger.LoggerBuilder()
                .WithDirectoryPath(@"C:\OptimaValue\")
                .WithServiceLogger(_logger)
                .WithLogDelay(5)
                .EnableFileLogging(true)
                .EnableSqlLogging(Settings.Server, Settings.Databas, Settings.User, Settings.Password)
                .Build();
        }

    }
}