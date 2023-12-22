namespace EstdCoReportApp.Application.Contract
{
    public interface IWordCloudDataService
    {
        Task<string> GenerateWordCloud(string url);
    }
}