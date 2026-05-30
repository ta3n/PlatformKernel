using System.Text;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace SharedKernel.Grpc.Formatters;

/// <summary>
/// A custom input formatter for handling JSON requests and deserializing them into Protobuf messages.
/// </summary>
public sealed class ProtobufJsonInputFormatter : TextInputFormatter
{
    private readonly JsonParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtobufJsonInputFormatter"/> class.
    /// </summary>
    /// <param name="parser">
    /// An optional <see cref="JsonParser"/> instance. If not provided, a default parser is created
    /// with settings to ignore unknown fields.
    /// </param>
    public ProtobufJsonInputFormatter(
        JsonParser? parser = null
    )
    {
        _parser = parser ?? new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

        // Add supported media types for JSON
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/json"));

        // Add supported encodings
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    /// <summary>
    /// Determines whether the formatter can read the specified type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>
    /// <c>true</c> if the type implements <see cref="IMessage"/> (Protobuf message); otherwise, <c>false</c>.
    /// </returns>
    protected override bool CanReadType(
        Type type
    )
    {
        return typeof(IMessage).IsAssignableFrom(type);
    }

    /// <summary>
    /// Reads the request body and deserializes it into a Protobuf message.
    /// </summary>
    /// <param name="context">The input formatter context.</param>
    /// <param name="encoding">The encoding of the request body.</param>
    /// <returns>
    /// A task that represents the asynchronous read operation. The task result contains the deserialized Protobuf message.
    /// </returns>
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context,
        Encoding encoding
    )
    {
        // Read the request body as a JSON string
        using var reader = new StreamReader(context.HttpContext.Request.Body, encoding);
        var json = await reader.ReadToEndAsync();

        var type = context.ModelType;

        // Use reflection to call JsonParser.Parse<T>(json) since T is determined at runtime
        var parseGeneric = typeof(JsonParser)
            .GetMethods()
            .First(
                m => m.Name == nameof(JsonParser.Parse) && m.IsGenericMethodDefinition && m.GetParameters().Length == 1
            );
        var parse = parseGeneric.MakeGenericMethod(type);

        // Parse the JSON into a Protobuf message
        var message = (IMessage)parse.Invoke(_parser, [json])!;

        return await InputFormatterResult.SuccessAsync(message);
    }
}
