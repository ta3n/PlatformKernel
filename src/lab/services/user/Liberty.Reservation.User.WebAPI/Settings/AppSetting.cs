namespace Liberty.Reservation.Employee.WebAPI.Settings;

public class AppSetting
{
    public App App { get; set; }

    public ConnectionStrings ConnectionStrings { get; set; }
    public Web Web { get; set; }
}

/// <summary></summary>
public class App
{
    /// <summary></summary>
    public string AppName { get; set; }

    /// <summary></summary>
    public string AppVersion { get; set; }
}

public class ConnectionStrings
{
    public string InMemoryDatabase { get; set; }

    public string MigrationsAssembly { get; set; }

    /// <summary></summary>
    public string DataContextConnection { get; set; }
}

public class Web
{
    /// <summary></summary>
    public WebCors Cors { get; set; }
}

public class WebCors
{
    /// <summary></summary>
    public string PolicyName { get; set; }
}
