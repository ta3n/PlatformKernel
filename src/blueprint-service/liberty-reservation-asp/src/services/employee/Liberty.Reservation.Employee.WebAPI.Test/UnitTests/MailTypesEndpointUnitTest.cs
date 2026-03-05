using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;
using Microsoft.AspNetCore.Mvc;

namespace Liberty.Reservation.Employee.WebAPI.Test.UnitTests;

public class MailTypesEndpointTests : BaseUnitTest
{
    [Fact]
    public void GetAllMailTypes_ReturnsOkResponse()
    {
        // Arrange
        var mailTypes = new List<(string IoType, string Name)>
        {
            (IoType.IO10001, "IO10001 一般向け 予約確認"),
            (IoType.IO10003, "IO10003 予約成立(施設)"),
            (IoType.IO10004, "IO10004 予約成立(ユーザ)"),
            (IoType.IO10005, "IO10005 予約キャンセル(施設)"),
            (IoType.IO10006, "IO10006 予約キャンセル(ユーザ)"),
            (IoType.IO10007, "IO10007 予約変更(施設)"),
            (IoType.IO10008, "IO10008 予約変更(ユーザ)"),
            (IoType.IO10010, "IO10010 予約キャンセル(ゲスト)"),
            (IoType.IO10011, "IO10011 予約キャンセル（ゲスト）"),
            (IoType.IO10012, "IO10012 予約成立（ゲスト）"),
            (IoType.IO10101, "IO10101 予約リマインダ(ユーザ)"),
            (IoType.IO10102, "IO10102 キャンセル料発生リマインダ(ユーザ)"),
            (IoType.IO10103, "IO10103 予約リマインダ(ゲスト)"),
            (IoType.IO10104, "IO10104 キャンセル料発生リマインダ(ゲスト)")
        };

        var mockMailTypeService = MockServices.MockMailTypeService(mailTypes);
        var controller = new MailTypesEndpoint(MockMapper, MockMediator, mockMailTypeService);
        var result = controller.GetAllMailTypes();
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult);
    }
}
