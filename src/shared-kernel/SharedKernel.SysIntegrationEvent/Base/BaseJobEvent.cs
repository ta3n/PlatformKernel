using System.Text.Json;
using MassTransit;

namespace SharedKernel.SysIntegrationEvent.Base;

/// <summary>
/// Represents the base class for job-related events in the system.
/// </summary>
/// <remarks>
/// This abstract class provides common properties and methods for events
/// related to jobs, including serialization and deserialization of event data.
/// It is used as the base type for various specific job events.
/// </remarks>
public abstract class BaseJobEvent : CorrelatedBy<Guid>
{
    /// <summary>
    /// Gets the unique correlation identifier associated with the event.
    /// This identifier is used to track and correlate events in the system,
    /// ensuring consistent identification across different components or processes.
    /// </summary>
    public Guid CorrelationId { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the event. This is typically used to identify
    /// the specific type of event being handled or processed.
    /// </summary>
    public string? EventName { get; set; }

    /// <summary>
    /// Gets or sets the type of the event, typically represented by the name of the specific event class.
    /// </summary>
    /// <remarks>
    /// This property is used to distinguish between different event types derived from the base class.
    /// It is automatically assigned the name of the specific event class in derived constructors,
    /// such as <c>ScheduleJobEvent</c> or <c>RecurringJobEvent</c>.
    /// </remarks>
    public string? EventType { get; set; }

    /// <summary>
    /// Represents a JSON-formatted data string associated with the event.
    /// This property is commonly used to store serialized data that can be
    /// deserialized and processed during event handling.
    /// </summary>
    public string? JsonData { get; set; }

    /// Deserializes the JSON string contained in the JsonData property into an object of the specified type.
    /// If the JsonData property is null or empty, the method returns the default value for the specified type.
    /// <typeparam name="T">The type into which the JSON data should be deserialized.</typeparam>
    /// <return>Returns the deserialized object of type T, or the default value of T if JsonData is null or empty.</return>
    public T? DeserializeJsonData<T>()
    {
        return string.IsNullOrEmpty(JsonData)
            ? default
            : JsonSerializer.Deserialize<T>(JsonData);
    }

    /// Serializes the provided data into a JSON string and assigns it to the JsonData property.
    /// <typeparam name="T">The type of the data to be serialized.</typeparam>
    /// <param name="data">The data to serialize into JSON format.</param>
    public void SerializeJsonData<T>(
        T data
    )
    {
        JsonData = JsonSerializer.Serialize(data);
    }
}
