using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Models.Responses;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class GmoChangeTranReportConfiguration : BaseDataEntityTypeConfiguration<GmoChangeTranReport>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<GmoChangeTranReport> builder
    )
    {
        builder.ToTable("gmo_change_tran_report", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Request)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ChangeOrderRequest>(v)
            );

        builder.Property(e => e.Response)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<OnlinePaymentChangeResponse>(v)
            );
    }
}
