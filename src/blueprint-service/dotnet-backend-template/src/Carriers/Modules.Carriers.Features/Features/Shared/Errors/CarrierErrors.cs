using Modules.Common.Domain.Results;

namespace Modules.Carriers.Features.Features.Shared.Errors;

internal static class CarrierErrors
{
	private const string ErrorPrefix = "Carriers";

	internal static Error NotFound(
		string carrierName
	)
	{
		return Error.NotFound($"{ErrorPrefix}.{nameof(NotFound)}", $"Active carrier with Name {carrierName} not found");
	}

	internal static Error ValidationError(
		string propertyName,
		string errorMessage
	)
	{
		return Error.Validation($"{ErrorPrefix}.{nameof(ValidationError)}", $"{propertyName}: {errorMessage}");
	}

	internal static Error AlreadyExists(
		string carrierName
	)
	{
		return Error.Conflict($"{ErrorPrefix}.{nameof(AlreadyExists)}", $"Carrier with name {carrierName} already exists");
	}
}
