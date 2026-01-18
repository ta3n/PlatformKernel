using Newtonsoft.Json;
using System.Reflection;
using System.Resources;

namespace Liberty.Reservation.Employee.WebAPI;

public class I18n : Localizes.Resources.I18n
{
}

public class I18nE : Localizes.Resources.Errors.I18n
{
    private ResourceManager ResourceManager { get; set; } = new(
        $"{Assembly.GetExecutingAssembly().GetName().Name}.Localizes.Resources.Errors.I18n",
        Assembly.GetExecutingAssembly()
    );

    public I18nEData Localize(
        string val
    )
    {
        var res = ResourceManager.GetString(val);
        var result = JsonConvert.DeserializeObject<I18nEData>(res);

        return result;
    }

    // リフレクションを使用した単純なローカライザ
    public static string Localizer(
        string val
    )
    {
        var resourceManager = new ResourceManager(
            $"{Assembly.GetExecutingAssembly().GetName().Name}.Localizes.Resources.Errors.I18n",
            Assembly.GetExecutingAssembly()
        );
        var res = resourceManager.GetString(val);

        return res;
    }

    public static I18nEData Localizer2(
        string val
    )
    {
        var res = Localizer(val);
        var result = JsonConvert.DeserializeObject<I18nEData>(res);

        return result;
    }
}

public class I18nEData
{
    public string Title { get; set; }
    public string Description { get; set; }
}
