using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberty.Application.Utils
{
    public class MailBuilder
    {
        public class MaiBuilderException : Exception { }
        public class MaiBuilderNoSmtpException : MaiBuilderException { }
        public class MaiBuilderNoSmtpPortException : MaiBuilderException { }
        public class MaiBuilderNoSmtpUserNameException : MaiBuilderException { }
        public class MaiBuilderNoSmtpPasswordException : MaiBuilderException { }

        public enum EMailSendTypes
        {
            To = 1,
            CC = 1 << 1,
            BCC = 1 << 2,
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string POP3 { get; set; }
        public string Smtp { get; set; }
        public int? SmtpPort { get; set; }

        public MimeKit.Text.TextFormat TextFormat { get; set; } = MimeKit.Text.TextFormat.Plain;

        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        public List<MailBuilderAddress> Froms { get; set; } = new List<MailBuilderAddress>();
        public List<MailBuilderAddress> Tos { get; set; } = new List<MailBuilderAddress>();
        public List<MailBuilderAddress> CCs { get; set; } = new List<MailBuilderAddress>();
        public List<MailBuilderAddress> BCCs { get; set; } = new List<MailBuilderAddress>();
        public List<MailBuilderAttachment> Attachments { get; set; } = new List<MailBuilderAttachment>();





        public void AddFrom(string name, string address)
        {
            this.Froms.Add(new MailBuilderAddress(name, address));
        }


        public void AddTo(string email, EMailSendTypes type)
        {
            // to:name to:mail を同じにする
            switch (type)
            {
                case EMailSendTypes.To:
                    this.AddTo(email, email);
                    break;
                case EMailSendTypes.CC:
                    this.AddCC(email, email);
                    break;
                case EMailSendTypes.BCC:
                    this.AddBCC(email, email);
                    break;
            }
        }

        public void AddTo(string name, string address)
        {
            this.Tos.Add(new MailBuilderAddress(name, address));
        }
        public void AddCC(string name, string address)
        {
            this.CCs.Add(new MailBuilderAddress(name, address));
        }
        public void AddBCC(string name, string address)
        {
            this.BCCs.Add(new MailBuilderAddress(name, address));
        }

        public void AddAttachment(string name)
        {
            this.Attachments.Add(new MailBuilderAttachment(name));
        }

        public async Task SendAsync()
        {

            // check
            if (string.IsNullOrEmpty(this.Smtp)) throw new MaiBuilderNoSmtpException();
            if (this.SmtpPort == null) throw new MaiBuilderNoSmtpPortException();
            if (string.IsNullOrEmpty(this.UserName)) throw new MaiBuilderNoSmtpUserNameException();
            if (string.IsNullOrEmpty(this.Password)) throw new MaiBuilderNoSmtpPasswordException();

            // MimeMessageを作り、宛先やタイトルなどを設定する
            var message = new MimeKit.MimeMessage();

            message.From.AddRange(this.Froms.Select(a => a.MailboxAddress));
            message.To.AddRange(this.Tos.Select(a => a.MailboxAddress));
            message.Cc.AddRange(this.CCs.Select(a => a.MailboxAddress));
            message.Bcc.AddRange(this.BCCs.Select(a => a.MailboxAddress));
            //message.From.Add(new MimeKit.MailboxAddress("MailKit ユーザー", "ishiharatomoya@liberty-mail.net"));
            //message.To.Add(new MimeKit.MailboxAddress("MailKit 試験", "ishiharatomoya@liberty-mail.net"));
            // message.Cc.Add(……省略……);
            // message.Bcc.Add(……省略……);
            message.Subject = this.Subject;

            // 本文を作る
            //var textPart = new MimeKit.TextPart(this.TextFormat);
            //textPart.Text = this.Body;
            // MimeMessageを完成させる
            //message.Body = textPart;

            var multipart = new MimeKit.Multipart("mixed");
            multipart.Add(new MimeKit.TextPart(this.TextFormat)
            {
                Text = this.Body
            });
            foreach (var attachment in this.Attachments)
            {
                multipart.Add(attachment.MimePart);
            }
            message.Body = multipart;

            //foreach (var attachment in this.Attachments)
            //{
            //    message.Attachments.Append(attachment.MimePart);
            //}

            // SMTPサーバに接続してメールを送信する
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
#if DEBUG
                // 開発用のSMTPサーバが暗号化に対応していないときは、次の行を追加する
                //client.ServerCertificateValidationCallback = (s, c, h, e) => true;
#endif

                await client.ConnectAsync(this.Smtp, this.SmtpPort.Value);
                //WriteLine("接続完了");

                // SMTPサーバがユーザー認証を必要としない場合は、次の2行は不要
                await client.AuthenticateAsync(this.UserName, this.Password);
                //WriteLine("認証完了");

                await client.SendAsync(message);
                //WriteLine("送信完了");

                await client.DisconnectAsync(true);
                //WriteLine("切断");

            }
        }

        public class MailBuilderAddress
        {

            public string Name { get; set; }
            public string Address { get; set; }

            public MimeKit.MailboxAddress MailboxAddress => new MimeKit.MailboxAddress(this.Name, this.Address);

            public MailBuilderAddress(string name, string address)
            {
                this.Name = name;
                this.Address = address;
            }
        }

        public class MailBuilderAttachment
        {

            public string File { get; set; }
            public string Address { get; set; }

            public MimeKit.MimePart MimePart => new MimePart()
            {
                Content = new MimeContent(System.IO.File.OpenRead(this.File)),
                ContentDisposition = new MimeKit.ContentDisposition(),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(this.File)
            };

            public MailBuilderAttachment(string name)
            {
                this.File = name;
            }
        }
    }
}
