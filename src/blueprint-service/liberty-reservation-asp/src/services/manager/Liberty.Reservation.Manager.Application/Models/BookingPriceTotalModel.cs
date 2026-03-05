namespace Liberty.Reservation.Manager.Application.Models;

public class BookingPriceTotalModel
{
    public decimal TotalRoomGroupPrice { get; set; }

    public decimal TotalSpaTax { get; set; }

    public decimal TotalOptionItemPrice { get; set; }

    public int IncomePoint { get; set; }

    public int UsedPoint { get; set; }

    public int RemainPoint { get; set; }

    public decimal TotalDiscount
    {
        get
        {
            var price = 0;
            price += UsedPoint;
            return price;
        }
    }

    public decimal TotalPrice
    {
        get
        {
            decimal price = 0;
            price += TotalRoomGroupPrice;
            price += TotalSpaTax;
            price += TotalOptionItemPrice;
            return price;
        }
    }

    public decimal AllTotalPrice
    {
        get
        {
            decimal price = 0;
            price += TotalPrice;
            price -= TotalDiscount;

            return price;
        }
    }
}
