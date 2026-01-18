using MimeKit;

namespace Liberty.ApplicationShared.Utils;

public class MailBuilder
{
    public class MaiBuilderException : Exception;

    public class MaiBuilderNoSmtpException : MaiBuilderException;

    public class MaiBuilderNoSmtpPortException : MaiBuilderException;

    public class MaiBuilderNoSmtpUserNameException : MaiBuilderException;

    public class MaiBuilderNoSmtpPasswordException : MaiBuilderException;

    public enum EMailSendTypes
    {
        To = 1,
        Cc = 1 << 1,
        Bcc = 1 << 2
    }

    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Pop3 { get; set; }
    public string? Smtp { get; set; }
    public int? SmtpPort { get; set; }

    public MimeKit.Text.TextFormat TextFormat { get; set; } = MimeKit.Text.TextFormat.Html;

    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public List<MailBuilderAddress> Froms { get; set; } = [];
    public List<MailBuilderAddress> Tos { get; set; } = [];
    public List<MailBuilderAddress> CCs { get; set; } = [];
    public List<MailBuilderAddress> BCCs { get; set; } = [];
    public List<MailBuilderAttachment> Attachments { get; set; } = [];

    public void AddFrom(
        string name,
        string address
    )
    {
        Froms.Add(new MailBuilderAddress(name, address));
    }

    public void AddTo(
        string email,
        EMailSendTypes type
    )
    {
        // to:name to:mail を同じにする
        switch (type)
        {
            case EMailSendTypes.To:
                AddTo(email, email);
                break;
            case EMailSendTypes.Cc:
                AddCc(email, email);
                break;
            case EMailSendTypes.Bcc:
                AddBCC(email, email);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void AddTo(
        string name,
        string address
    )
    {
        Tos.Add(new MailBuilderAddress(name, address));
    }

    public void AddCc(
        string name,
        string address
    )
    {
        CCs.Add(new MailBuilderAddress(name, address));
    }

    public void AddBCC(
        string name,
        string address
    )
    {
        BCCs.Add(new MailBuilderAddress(name, address));
    }

    public void AddAttachment(
        string name
    )
    {
        Attachments.Add(new MailBuilderAttachment(name));
    }

    public async Task SendAsync()
    {
        // check
        if (string.IsNullOrEmpty(Smtp))
        {
            throw new MaiBuilderNoSmtpException();
        }

        if (SmtpPort == null)
        {
            throw new MaiBuilderNoSmtpPortException();
        }

        if (string.IsNullOrEmpty(UserName))
        {
            throw new MaiBuilderNoSmtpUserNameException();
        }

        if (string.IsNullOrEmpty(Password))
        {
            throw new MaiBuilderNoSmtpPasswordException();
        }

        // MimeMessageを作り、宛先やタイトルなどを設定する
        var message = new MimeMessage();

        message.From.AddRange(Froms.Select(a => a.MailboxAddress));
        message.To.AddRange(Tos.Select(a => a.MailboxAddress));
        message.Cc.AddRange(CCs.Select(a => a.MailboxAddress));
        message.Bcc.AddRange(BCCs.Select(a => a.MailboxAddress));
        //message.From.Add(new MimeKit.MailboxAddress("MailKit ユーザー", "ishiharatomoya@liberty-mail.net"));
        //message.To.Add(new MimeKit.MailboxAddress("MailKit 試験", "ishiharatomoya@liberty-mail.net"));
        // message.Cc.Add(……省略……);
        // message.Bcc.Add(……省略……);
        message.Subject = Subject;

        // 本文を作る
        //var textPart = new MimeKit.TextPart(this.TextFormat);
        //textPart.Text = this.Body;
        // MimeMessageを完成させる
        //message.Body = textPart;

        var multipart = new Multipart("mixed") { new TextPart(TextFormat) { Text = Body } };
        foreach (var attachment in Attachments)
        {
            multipart.Add(attachment.MimePart);
        }

        message.Body = multipart;

        //foreach (var attachment in this.Attachments)
        //{
        //    message.Attachments.Append(attachment.MimePart);
        //}

        // SMTPサーバに接続してメールを送信する
        using var client = new MailKit.Net.Smtp.SmtpClient();
#if DEBUG
        // 開発用のSMTPサーバが暗号化に対応していないときは、次の行を追加する
        //client.ServerCertificateValidationCallback = (s, c, h, e) => true;
#endif

        await client.ConnectAsync(Smtp, SmtpPort.Value);
        //WriteLine("接続完了");

        // SMTPサーバがユーザー認証を必要としない場合は、次の2行は不要
        await client.AuthenticateAsync(UserName, Password);
        //WriteLine("認証完了");

        await client.SendAsync(message);
        //WriteLine("送信完了");

        await client.DisconnectAsync(true);
        //WriteLine("切断");
    }

    public class MailBuilderAddress(
        string name,
        string address
    )
    {
        public string Name { get; set; } = name;
        public string Address { get; set; } = address;

        public MailboxAddress MailboxAddress => new(Name, Address);
    }

    public class MailBuilderAttachment(
        string name
    )
    {
        public string File { get; set; } = name;
        public string? Address { get; set; }

        public MimePart MimePart => new()
        {
            Content = new MimeContent(System.IO.File.OpenRead(File)),
            ContentDisposition = new ContentDisposition(),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(File)
        };
    }
}
