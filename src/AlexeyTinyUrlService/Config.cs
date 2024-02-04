namespace Alexey.TinyUrlService
{
    // Extremely simple implementation
    internal class Config : ITinyUrlServiceConfig
    {
        public byte MaxAddAttemptsNumber { get; } = 10;

        /*
        * With 6-character short URLs and 62 symbols, we can create over 56 billion unique combinations.
        * However, for in memory storage implementation it depends from the environment.
        * todo@ define this number using the stress tests.
        */
        public int MaxTotalItemsNumber { get; } = 10000000; // 10M

        public bool MonitorMaxTotalItemsNumber { get; } = false; // disable it for now
    }
}
