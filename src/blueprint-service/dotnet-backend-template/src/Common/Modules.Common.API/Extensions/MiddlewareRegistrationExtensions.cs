using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.API.Abstractions;

namespace Modules.Common.API.Extensions;

public static class MiddlewareRegistrationExtensions
{
	public static IApplicationBuilder UseModuleMiddlewares(
		this IApplicationBuilder app
	)
	{
		foreach (var configurator in app.ApplicationServices.GetServices<IModuleMiddlewareConfigurator>())
		{
			configurator.Configure(app);
		}

		return app;
	}
}
