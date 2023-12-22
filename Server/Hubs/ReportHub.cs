using EstdCoReportApp.Application.Domain;
using Microsoft.AspNetCore.SignalR;

namespace EstdCoReportApp.Server.Hubs
{
    public class ReportHub : Hub<List<Report>> { }
}
