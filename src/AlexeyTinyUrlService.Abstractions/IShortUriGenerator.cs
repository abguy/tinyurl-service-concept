namespace Alexey.TinyUrlService
{
    public interface IShortUriGenerator
    {
        string Generate(string originalUrl);
    }
}
