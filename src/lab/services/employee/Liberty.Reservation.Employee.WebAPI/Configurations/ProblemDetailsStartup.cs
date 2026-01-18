namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class ProblemDetailsStartup
{
    public static IServiceCollection AddProblemDetailsModule(
        this IServiceCollection services
    )
    {
        services.AddProblemDetails();

        return services;
    }

    public static IApplicationBuilder UseApplicationProblemDetails(
        this IApplicationBuilder app,
        IHostEnvironment environment
    )
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        return app;
    }
}
