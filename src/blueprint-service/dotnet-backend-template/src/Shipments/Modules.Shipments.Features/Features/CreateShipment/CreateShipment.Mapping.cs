using Modules.Shipments.Domain.Entities;
using Modules.Shipments.Features.Features.Shared.Responses;

namespace Modules.Shipments.Features.Features.CreateShipment;

internal static class CreateShipmentMappingExtensions
{
	public static Shipment MapToShipment(
		this CreateShipmentRequest request,
		string shipmentNumber
	)
	{
		return Shipment.Create(
			shipmentNumber,
			request.OrderId,
			request.Address,
			request.Carrier,
			request.ReceiverEmail,
			[.. request.Items
				.Select(
					x => new ShipmentItem
					{
						Product = x.Product,
						Quantity = x.Quantity
					}
				)]
		);
	}

	public static ShipmentResponse MapToResponse(
		this Shipment shipment
	)
	{
		return new ShipmentResponse(
			shipment.Number,
			shipment.OrderId,
			shipment.Address,
			shipment.Carrier,
			shipment.ReceiverEmail,
			shipment.Status,
			[.. shipment.Items.Select(x => new ShipmentItemResponse(x.Product, x.Quantity))]
		);
	}
}
