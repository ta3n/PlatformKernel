namespace Liberty.Application.Settings
{
    public class AppSetting
    {
        public App App { get; set; }

        public ConnectionStrings ConnectionString { get; set; }

        public SMTPMail SMTPMail { get; set; }
    }

    public class SMTPMail
    {
        public SMTPMailSendMail SendMail { get; set; }
    }

    public class SMTPMailSendMail
    {
        public SMTPMailSendMailFrom From { get; set; }
    }
    public class SMTPMailSendMailFrom
    {
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string Smtp { get; set; }
        public int? SmtpPort { get; set; }
    }
    /// <summary></summary>
    public class App
    {
        /// <summary></summary>
        public string AppName { get; set; }

        /// <summary></summary>
        public string AppVersion { get; set; }
    }

    public class HealthOptions
    {
        public bool Enabled { get; set; } = true;
    }
    public class ConnectionStrings
    {
        public string? DataContextConnection { get; set; }
    }

}