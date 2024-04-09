using Serilog;

namespace tamagotchi_pet.Utils
{
    public static class Logging
    {
        public static ILogger Logger { get; private set; }

        static Logging()
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/tamagotchi.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}