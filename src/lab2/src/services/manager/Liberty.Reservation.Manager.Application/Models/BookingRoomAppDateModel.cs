using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Manager.Application.Models;

public class BookingRoomAppDateModel
{
    public long SiteId { get; set; }

    public long PlanId { get; set; }
    public long AppDateId { get; set; }

    public int? Price { get; set; }

    public int? RemainNumber { get; set; }

    public bool IsNotSelled { get; set; }

    public int ReservedNumber { get; set; }

    public int ReservationPairs { get; set; }

    public bool CanRest => Exceptions.Count == 0;

    public RoomGroupInfoModel? RoomGroupInfo { get; set; }

    public List<ReservationServiceException> Exceptions { get; set; } = [];

    public class RoomGroupInfoModel
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public int? CapacityMax { get; set; }

        public int? CapacityMin { get; set; }

        public bool InRange(
            int? persons
        )
        {
            var min = CapacityMin;
            var max = CapacityMax;

            if (min == null)
            {
                return false;
            }

            if (max == null)
            {
                return false;
            }

            if (min > persons)
            {
                return false;
            }

            return !(max < persons);
        }
    }
}
