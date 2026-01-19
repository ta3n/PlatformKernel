using Microsoft.AspNetCore.Identity;
using Modules.Common.Domain.Results;

namespace Modules.Users.Domain.Errors;

public static class UserErrors
{
	private const string ErrorPrefix = "Users";

	public static Error NotFound(
		string userId
	)
	{
		return Error.NotFound($"{ErrorPrefix}.{nameof(NotFound)}", $"User with ID {userId} not found");
	}

	public static Error NotFoundByEmail(
		string email
	)
	{
		return Error.NotFound($"{ErrorPrefix}.{nameof(NotFound)}", $"User with email {email} not found");
	}

	public static Error RegistrationFailed(
		IEnumerable<IdentityError> identityErrors
	)
	{
		return Error.NotFound($"{ErrorPrefix}.{nameof(RegistrationFailed)}", string.Join(", ", identityErrors.Select(e => e.Description)));
	}

	public static Error UpdateFailed(
		IEnumerable<IdentityError> identityErrors
	)
	{
		return Error.NotFound($"{ErrorPrefix}.{nameof(UpdateFailed)}", string.Join(", ", identityErrors.Select(e => e.Description)));
	}

	public static Error DeleteFailed(
		IEnumerable<IdentityError> identityErrors
	)
	{
		return Error.NotFound($"{ErrorPrefix}.{nameof(DeleteFailed)}", string.Join(", ", identityErrors.Select(e => e.Description)));
	}

	public static Error RefreshFailed(
		IEnumerable<IdentityError> identityErrors
	)
	{
		return Error.NotFound($"{ErrorPrefix}.{nameof(RefreshFailed)}", string.Join(", ", identityErrors.Select(e => e.Description)));
	}

	public static Error RoleNotFound(
		string roleName
	)
	{
		return Error.NotFound($"{ErrorPrefix}.{nameof(RoleNotFound)}", $"Role '{roleName}' not found");
	}

	public static Error UpdateRoleFailed(
		IEnumerable<IdentityError> identityErrors
	)
	{
		return Error.NotFound($"{ErrorPrefix}.{nameof(UpdateRoleFailed)}", string.Join(", ", identityErrors.Select(e => e.Description)));
	}

	public static Error InvalidCredentials()
	{
		return Error.Validation($"{ErrorPrefix}.{nameof(InvalidCredentials)}", "Invalid email or password");
	}

	public static Error InvalidToken()
	{
		return Error.Validation($"{ErrorPrefix}.{nameof(InvalidToken)}", "Invalid token");
	}
}
