using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkBro.Configuration;
using ForkBro.Bookmakers;
using ForkBro.Configuration;
using ForkBro.Daemons;
using ForkBro.Mediator;
using ForkBro.Scanner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ForkBro.Common;

namespace ForkBro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostContext, logging) =>
                {
                logging.AddFile(hostContext.Configuration.GetSection("Logging"));
                })
                .ConfigureServices((hostContext, services) =>
                {
        #region ISetting [HostBuilderContext]
        services.AddTransient<ISetting, AppSettings>();
        #endregion

        #region Mediator [ISetting]
        services.AddSingleton<HubMediator>();
        services.AddSingleton<IScannerMediator>(x => x.GetRequiredService<HubMediator>());
        services.AddSingleton<IBookmakerMediator>(x => x.GetRequiredService<HubMediator>());
        services.AddSingleton<IDaemonMasterMediator>(x => x.GetRequiredService<HubMediator>());
        services.AddSingleton<IApiMediator>(x => x.GetRequiredService<HubMediator>());
        #endregion

        #region LiveScanner [ILogger,IScannerMediator,ISetting]
        services.AddHostedService<LiveScanner>();
        #endregion

        #region BookmakerService [ILogger,IBookmakerMediator]
        foreach (var item in (new AppSettings(hostContext)).Companies)
            switch (item.id)
            {
                case Bookmaker._favbet:
                    services.AddHostedService<Service_favbet>(); break;
                case Bookmaker._1xbet:
                    services.AddHostedService<Service_1xbet>(); break;
                default: throw new Exception("Неизвесный тип букмекера: " + item.id.ToString());
            }
        #endregion

        #region DaemonMaster [ILogger,ICalcDaemonMediator]
        services.AddHostedService<DaemonMaster>();
                    #endregion

        #region Worker [ILogger,HubMediator]
        //services.AddHostedService<Worker>(); //TODO Worker
        #endregion
                });
    }
    //[ProviderAlias("File")]
    //public class FileLoggerProvider : BatchingLoggerProvider
    //{
    //    private readonly string _path;
    //    private readonly string _fileName;
    //    private readonly int? _maxFileSize;
    //    private readonly int? _maxRetainedFiles;

    //    public FileLoggerProvider(IOptions<FileLoggerOptions> options) : base(options)
    //    {
    //        var loggerOptions = options.Value;
    //        _path = loggerOptions.LogDirectory;
    //        _fileName = loggerOptions.FileName;
    //        _maxFileSize = loggerOptions.FileSizeLimit;
    //        _maxRetainedFiles = loggerOptions.RetainedFileCountLimit;
    //    }

    //    // Write the provided messages to the file system
    //    protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken)
    //    {
    //        Directory.CreateDirectory(_path);

    //        // Group messages by log date
    //        foreach (var group in messages.GroupBy(GetGrouping))
    //        {
    //            var fullName = GetFullName(group.Key);
    //            var fileInfo = new FileInfo(fullName);
    //            // If we've exceeded the max file size, don't write any logs
    //            if (_maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > _maxFileSize)
    //            {
    //                return;
    //            }

    //            // Write the log messages to the file
    //            using (var streamWriter = File.AppendText(fullName))
    //            {
    //                foreach (var item in group)
    //                {
    //                    await streamWriter.WriteAsync(item.Message);
    //                }
    //            }
    //        }

    //        RollFiles();
    //    }

    //    // Get the file name
    //    private string GetFullName((int Year, int Month, int Day) group)
    //    {
    //        return Path.Combine(_path, $"{_fileName}{group.Year:0000}{group.Month:00}{group.Day:00}.txt");
    //    }

    //    private (int Year, int Month, int Day) GetGrouping(LogMessage message)
    //    {
    //        return (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day);
    //    }

    //    // Delete files if we have too many
    //    protected void RollFiles()
    //    {
    //        if (_maxRetainedFiles > 0)
    //        {
    //            var files = new DirectoryInfo(_path)
    //                .GetFiles(_fileName + "*")
    //                .OrderByDescending(f => f.Name)
    //                .Skip(_maxRetainedFiles.Value);

    //            foreach (var item in files)
    //            {
    //                item.Delete();
    //            }
    //        }
    //    }
    //}
}
