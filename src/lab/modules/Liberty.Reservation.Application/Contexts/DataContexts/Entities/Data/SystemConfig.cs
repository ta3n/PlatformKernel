using Liberty.Entity;
using Liberty.Reservation.Application.Templates;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class SystemConfig : EntityData
{
    public string TemplateFormatJson { get; set; } = "{}";

    public TemplateFormatData? TemplateFormatData
    {
        get
        {
            try
            {
                var data = JsonConvert.DeserializeObject<TemplateFormatData>(TemplateFormatJson);
                // JSONになくNullになってしまうオブジェクトを補完
                data?.Fill();
                //
                return data;
            }
            catch
            {
                // 変換できない場合、新しいオブジェクトを返す
                return new TemplateFormatData();
            }
        }
        set => TemplateFormatJson = JsonConvert.SerializeObject(value);
    }
}
