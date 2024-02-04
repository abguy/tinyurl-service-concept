namespace Alexey.TinyUrlService
{
    public interface ITinyUrlServiceConfig
    {
        byte MaxAddAttemptsNumber { get; }

        int MaxTotalItemsNumber { get; }

        bool MonitorMaxTotalItemsNumber { get; }
    }
}
