using Serilog;
using System;
using System.IO;

namespace tamagotchi_pet.Utils
{
    public static class Logging
    {
        public static ILogger Logger { get; private set; }

        static Logging()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var myAppFolder = Path.Combine(appDataPath, "Tamagotchi");
            var logFilePath = Path.Combine(myAppFolder, "logs");

            Directory.CreateDirectory(logFilePath);

            Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(logFilePath, "tamagotchi.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}