namespace ThirdTask.Service
{
    public interface ILogProcessor
    {
        Task ProcessAsync(string inputPath, string outputPath, string problemPath);
    }
}