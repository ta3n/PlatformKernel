using Liberty.Reservation.Application.Domains.Repositories;
using Liberty.Reservation.Application.Contexts.DataContexts;
using Liberty.Reservation.Employee.WebAPI.Application.Controllers;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Test.Mocks.DataContexts;
using Liberty.Reservation.Employee.WebAPI.Handlers;
using Liberty.Reservation.Employee.WebAPI.Initializations;
using Liberty.Reservation.Employee.WebAPI.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

//
var builder = WebApplication.CreateBuilder(args);

//
var config = builder.Configuration.Get<AppSetting>() ?? throw new AppSettingNotfoundException();

//
Action<DbContextOptionsBuilder> dbContextOptionsAction = dbContextOptions =>
{
    //// InMemoryDatabase
    //dbContextOptions.UseInMemoryDatabase(config.ConnectionStrings.InMemoryDatabase);

    //MySQL
    dbContextOptions
        //.UseLazyLoadingProxies()
        .UseMySql(
            config.ConnectionStrings.DataContextConnection,
            new MySqlServerVersion(new Version(8, 0, 29)),
            b => b
                //.EnableRetryOnFailure()
                .MigrationsAssembly(config.ConnectionStrings.MigrationsAssembly)
        )
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
};
builder.Services.AddDbContext<DataContext>(dbContextOptionsAction);
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
builder.Services.AddCors(
    o => o.AddPolicy(
        config.Web.Cors.PolicyName,
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    )
);

//
builder.Services.AddScoped<ITestController, TestController>();
//
builder.Services.AddScoped<IFileRepository, FileRepository>();
//

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHostedService<InitializationService>();

builder.Services.AddHostedService<InitializationService>();

//builder.Services
//               .AddIdentity<Employee, IdentityRole>()
//               .AddEntityFrameworkStores<DataContext>()
//               .AddUserManager<UserManager<Employee>>()
//               .AddDefaultTokenProviders();

// authorize
//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//services.AddAuthentication("Bearer")
    .AddJwtBearer(
        options =>
        {
            options.Authority = "https://localhost:7038"; // FIXME: �n�[�h�R�[�f�B���O��
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };

            //options.ClaimsIssuer = appSettings.JwtSetting.JwtIssuer;
            //options.TokenValidationParameters = AuthService.CreateValidationParameters(appSettings.JwtSetting);

            //options.TokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidIssuer = appSettings.JwtSetting.JwtIssuer,
            //    ValidAudience = appSettings.JwtSetting.JwtAudience,
            //    ValidateAudience = appSettings.JwtSetting.ValidateAudience,
            //    ValidateIssuerSigningKey = appSettings.JwtSetting.ValidateIssuerSigningKey,
            //    ClockSkew = TimeSpan.FromMinutes(15),
            //    IssuerSigningKey = new SymmetricSecurityKey(
            //        Encoding.UTF8.GetBytes(
            //            appSettings.JwtSetting.JwtKey)),
            //};
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
    dbContextOptionsAction(optionsBuilder);
    await DataContextMock.Create(optionsBuilder.Options);
}

// Error Handler
app.UseExceptionHandler(
    exceptionHandlerApp => { exceptionHandlerApp.Run(async context => await ErrorHandler.HandleRequest(context)); }
);
//
app.UseCors(config.Web.Cors.PolicyName);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
