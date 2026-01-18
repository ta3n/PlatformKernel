using Liberty.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();

var mysql = builder.AddMySql("mysql")
    .WithImageTag("8.0.29");

var reservationDb = mysql.AddDatabase(
    "reservation-db"
);

builder.AddProject<Projects.Liberty_Reservation_Employee_WebAPI>("employee-api");
    // .WithReference(reservationDb);

await builder.Build().RunAsync();
