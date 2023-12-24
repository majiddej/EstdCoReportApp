using EstdCoReportApp.Application;
using EstdCoReportApp.Application.Contract;
using EstdCoReportApp.Application.HttpClient;
using EstdCoReportApp.Repository.Sqlite;
using EstdCoReportApp.Server.Hubs;
using EstdCoReportApp.Server.Workers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=crypto.db");
});

var httpClient = builder.Services.AddHttpClient("ETC", client =>
{
    var baseUrl = "https://rest.coinapi.io";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSingleton<IHttpClientHelperAsync, HttpClientHelperAsync>();



builder.Services.AddTransient<IReportDataService, ReportDataService>();
builder.Services.AddTransient<ICryptoCurrencyRateDataService, CryptoCurrencyRateDataService>();
builder.Services.AddTransient<IWordCloudDataService, WordCloudDataService>();
builder.Services.AddHostedService<ReportDataWorker>();
builder.Services.AddHostedService<CryproRateDataWorker>();
builder.Services.AddHostedService<ChartDataWorker>();
builder.Services.AddHostedService<WordCloudDataWorker>();
builder.Services.AddHostedService<MostVolatileCryptoDataWorker>();







var app = builder.Build();
app.MapHub<ReportHub>("/reports");
app.MapHub<ChartHub>("/charts");
app.MapHub<CryptoRateHub>("/cryptoRates");
app.MapHub<WordCloudHub>("/wordCloud");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
