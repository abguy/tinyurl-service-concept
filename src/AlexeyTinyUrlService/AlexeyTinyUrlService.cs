namespace Alexey.TinyUrlService
{
    public static class AlexeyTinyUrlService
    {
        private static object _lock = new object();

        private static ITinyUrlService _instance = null;

        public static ITinyUrlService GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        /*
                        * Select actual implementation due to the additonal requirements. Consider:
                        * 1) How many items/URLs need to be stored in total?
                        * 2) Available memory consumption.
                        * 3) Should we keep values forever or they can expire?
                        * 4) Should we reuse short URL for the same long URLs?
                        *
                        * For any persistent storage implementation we might need to add
                        * some extra cache layer to avoid many read requests to DB.
                        * However, it should depend from the additional requirements (Consistency/Availability/Partition tolerance), 
                        * so we can consider different transaction isolation levels.
                        */
                        IShortUriGenerator shortUriGenerator = new ShortUriGeneratorRandom(6);
                        Config config = new Config();
                        _instance = new TinyUrlServiceMemory(shortUriGenerator, config);
                    }
                }
            }
            // todo@ ensure that _instance object is not disposed

            return _instance;
        }
    }
}
