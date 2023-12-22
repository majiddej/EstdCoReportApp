using EstdCoReportApp.Application.Domain;

namespace EstdCoReportApp.Application.Contract
{
    public interface IReportDataService
    {
        Task<List<ChartData>> GetChartData();
        Task<List<Report>> GetReports();
    }
}