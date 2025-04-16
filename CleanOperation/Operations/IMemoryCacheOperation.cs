namespace CleanOperation.Operations
{
    public interface IMemoryCacheOperation : ICleanOperation
    {
        T? Get<T>(string key);
        void Set(string key, object data, int seconds = 30);
    }
}
