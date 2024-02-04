namespace Alexey.TinyUrlService
{
    public interface ITinyUrlService
    {
        string CreateShortUri(string longUrl, string customShortUri = null);

        string GetLongUrl(string shortUri);

        void DeleteShortUri(string shortUri);

        ulong GetClickCount(string shortUri);

        int GetTotalItemsNumber();
    }
}
