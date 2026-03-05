using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Templates.FormatModels;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Test.IntegrationTests;

public class MailTemplatesIo10001IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10001);

public class MailTemplatesIo10002IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10002);

public class MailTemplatesIo10003IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10003);

public class MailTemplatesIo10004IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10004);

public class MailTemplatesIo10005IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10005);

public class MailTemplatesIo10006IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10006);

public class MailTemplatesIo10007IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10007);

public class MailTemplatesIo10008IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10008);

public class MailTemplatesIo10009IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10009)
{
    protected override string GetFormat()
    {
        var templateFormat = new Io10009TemplateFormat
        {
            Subject = "Test",
            Body = "Test"
        };

        return JsonConvert.SerializeObject(templateFormat);
    }
}

public class MailTemplatesIo10010IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10010)
{
    protected override string GetFormat()
    {
        var templateFormat = new Io10010TemplateFormat
        {
            Subject = "Test",
            Body = "Test"
        };

        return JsonConvert.SerializeObject(templateFormat);
    }
}

public class MailTemplatesIo10101IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10101);

public class MailTemplatesIo10102IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10102);

public class MailTemplatesIo10201IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10201);

public class MailTemplatesIo10202IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10202);

public class MailTemplatesIo10203IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10203);

public class MailTemplatesIo10204IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10204);

public class MailTemplatesIo10205IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10205);

public class MailTemplatesIo10206IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10206);

public class MailTemplatesIo10207IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO10207);

public class MailTemplatesIo20001IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO20001);

public class MailTemplatesIo20002IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO20002);

public class MailTemplatesIo20003IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO20003);

public class MailTemplatesIo20004IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO20004)
{
    protected override string GetFormat()
    {
        var templateFormat = new Io20004TemplateFormat
        {
            Subject = "Test",
            Body = "Test"
        };

        return JsonConvert.SerializeObject(templateFormat);
    }
}

public class MailTemplatesIo30001IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO30001)
{
    protected override string GetFormat()
    {
        var templateFormat = new Io30001TemplateFormat
        {
            Subject = "Test",
            Body = "Test"
        };

        return JsonConvert.SerializeObject(templateFormat);
    }
}

public class MailTemplatesIo30002IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO30002)
{
    protected override string GetFormat()
    {
        var templateFormat = new Io30002TemplateFormat
        {
            Subject = "Test",
            Body = "Test"
        };

        return JsonConvert.SerializeObject(templateFormat);
    }
}

public class MailTemplatesIo30003IntTest() : BaseMailTemplatesBaseEndpointIntTest(IoType.IO30003)
{
    protected override string GetFormat()
    {
        var templateFormat = new Io30003TemplateFormat
        {
            Subject = "Test",
            Body = "Test"
        };

        return JsonConvert.SerializeObject(templateFormat);
    }
}
