using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Liberty.Application.Domains.Services.Mails
{
    public interface IMailService
    {

        Task SendAsync(string[] tos, string subject, string body);

    }
}
