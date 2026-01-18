using Liberty.Application.Settings;
using Liberty.Application.Utils;
using Microsoft.Extensions.Configuration;

namespace Liberty.Application.Domains.Services.Mails
{


    public class SMTPMailService : IMailService
    {


        public SMTPMailServiceSetting Settings { get; } = new SMTPMailServiceSetting();
        private readonly IConfiguration _configuration;
        public SMTPMailService(

            IConfiguration configuration
            )
        {

            _configuration = configuration;


        }

        public async Task SendAsync(string[] tos, string subject, string body)
        {
            var settings = _configuration.Get<AppSetting>();

            if (settings != null)
            {
                var mb = new MailBuilder
                {
                    UserName = settings.SMTPMail.SendMail.From.UserName,
                    Smtp = settings.SMTPMail.SendMail.From.Smtp,
                    SmtpPort = settings.SMTPMail.SendMail.From.SmtpPort,
                    Password = settings.SMTPMail.SendMail.From.Password
                };
                mb.AddFrom(settings.SMTPMail.SendMail.From.Name, settings.SMTPMail.SendMail.From.Mail);


                foreach (var to in tos)
                {
                    mb.AddTo(to, to);
                }

                mb.Subject = subject;
                mb.Body = body;
                //
                await mb.SendAsync();
            }
        }



        public class SMTPMailServiceSetting
        {
            public SMTPMailServiceSettingSendMail SendMail { get; set; } = new SMTPMailServiceSettingSendMail();
            public class SMTPMailServiceSettingSendMail
            {
                public SMTPMailServiceSettingSendMailFrom From { get; set; } = new SMTPMailServiceSettingSendMailFrom();

                public class SMTPMailServiceSettingSendMailFrom
                {
                    public string Password { get; internal set; }
                    public string UserName { get; internal set; }
                    public string Mail { get; internal set; }
                    public string Name { get; internal set; }
                    public string Smtp { get; internal set; }
                    public int? SmtpPort { get; internal set; }
                }
            }

        }

    }
}
