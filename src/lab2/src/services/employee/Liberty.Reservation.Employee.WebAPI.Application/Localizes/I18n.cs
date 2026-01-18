using Newtonsoft.Json;
using System.Reflection;
using System.Resources;

namespace Liberty.Reservation.Employee.WebAPI.Application.Localizes;

// public class I18n : Localizes.Resources.I18n; <- Config i18n

public class I18nE : Resources.Errors.I18n
{
    private new ResourceManager ResourceManager { get; set; } = new(
        $"{Assembly.GetExecutingAssembly().GetName().Name}.Localizes.Resources.Errors.I18n",
        Assembly.GetExecutingAssembly()
    );

    public I18nEData? Localize(
        string val
    )
    {
        var res = ResourceManager.GetString(val) ?? string.Empty;
        var result = JsonConvert.DeserializeObject<I18nEData>(res);

        return result;
    }

    // リフレクションを使用した単純なローカライザ
    public static string? Localized(
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

    public static I18nEData? LocalizedWithData(
        string val
    )
    {
        var res = Localized(val) ?? string.Empty;
        var result = JsonConvert.DeserializeObject<I18nEData>(res);

        return result;
    }
}
