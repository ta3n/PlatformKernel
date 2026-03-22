using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Hangfire;
using SharedKernel.Hangfire.Models;
using SharedKernel.Hangfire.Utils;

namespace SharedKernel.Hangfire.Test;

public class UnitTest1
{
    [Fact]
    public void GetDefaultRecurringJobOptions_UsesTokyoTimeZone()
    {
        var options = JobUtil.GetDefaultRecurringJobOptions();

        Assert.IsType<RecurringJobOptions>(options);
        Assert.Equal(JobUtil.DefaultTimeZone, options.TimeZone.Id);
    }

    [Fact]
    public void UseHangfireDashboardCustom_ReturnsSameBuilderWhenDisabled()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(services);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("HangfireDashboard:Enabled", "false"),
                new KeyValuePair<string, string?>("HangfireDashboard:DashboardUrl", "hangfire"),
                new KeyValuePair<string, string?>("HangfireDashboard:Username", "user"),
                new KeyValuePair<string, string?>("HangfireDashboard:Password", "pass"),
                new KeyValuePair<string, string?>("HangfireDashboard:IsReadOnly", "false")
            ])
            .Build();

        var result = app.UseHangfireDashboardCustom(configuration);

        Assert.Same(app, result);
    }

    [Fact]
    public void JobRequestRecords_ExposeInheritedValues()
    {
        var recurring = new RegisterRecurringJobRequest("ImageResize", "nightly", "{}", "0 0 * * *");
        var delayed = new RegisterScheduleJobDelayRequest("ImageResize", "delay", "{}", TimeSpan.FromMinutes(5));

        Assert.Equal("ImageResize", recurring.EventName);
        Assert.Equal("nightly", recurring.JobName);
        Assert.Equal("0 0 * * *", recurring.CronExpression);
        Assert.Equal(TimeSpan.FromMinutes(5), delayed.Delay);
    }
}
