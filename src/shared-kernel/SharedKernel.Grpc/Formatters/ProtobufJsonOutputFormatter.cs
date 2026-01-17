using System.Text;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace SharedKernel.Grpc.Formatters;

/// <summary>
/// A custom output formatter for serializing Protobuf messages into JSON format.
/// </summary>
public sealed class ProtobufJsonOutputFormatter : TextOutputFormatter
{
    private readonly JsonFormatter _formatter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtobufJsonOutputFormatter"/> class.
    /// </summary>
    /// <param name="formatter">
    /// An optional <see cref="JsonFormatter"/> instance. If not provided, a default formatter is created
    /// with settings to exclude default values.
    /// </param>
    public ProtobufJsonOutputFormatter(
        JsonFormatter? formatter = null
    )
    {
        _formatter = formatter ?? new JsonFormatter(new JsonFormatter.Settings(false));

        // Add supported media types for JSON
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/json"));

        // Add supported encodings
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    /// <summary>
    /// Determines whether the formatter can write the specified type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>
    /// <c>true</c> if the type implements <see cref="IMessage"/> (Protobuf message); otherwise, <c>false</c>.
    /// </returns>
    protected override bool CanWriteType(
        Type? type
    )
    {
        return type is not null && typeof(IMessage).IsAssignableFrom(type);
    }

    /// <summary>
    /// Writes the Protobuf message as a JSON string to the response body.
    /// </summary>
    /// <param name="context">The output formatter context.</param>
    /// <param name="selectedEncoding">The encoding to use for the response body.</param>
    /// <returns>
    /// A task that represents the asynchronous write operation.
    /// </returns>
    public override async Task WriteResponseBodyAsync(
        OutputFormatterWriteContext context,
        Encoding selectedEncoding
    )
    {
        // Cast the object to IMessage (Protobuf message)
        var msg = (IMessage)context.Object!;

        // Serialize the Protobuf message to JSON
        var json = _formatter.Format(msg);

        // Write the JSON string to the response body
        await context.HttpContext.Response.WriteAsync(json, selectedEncoding);
    }
}
