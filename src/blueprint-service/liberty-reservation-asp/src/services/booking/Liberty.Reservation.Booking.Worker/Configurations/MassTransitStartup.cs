using Liberty.MassTransit;
using Liberty.SysIntegrationEvent;
using MassTransit;
using System.Reflection;
using Liberty.Reservation.Booking.Worker.Consumers;
using Liberty.Reservation.Booking.Worker.Application.Options;

namespace Liberty.Reservation.Booking.Worker.Configurations;

public static class MassTransitStartup
{
    public static IServiceCollection AddTransportModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMassTransitCustom(
            configuration,
            configure =>
            {
                var entryAssembly = Assembly.GetExecutingAssembly();
                configure.AddConsumers(entryAssembly);
            },
            (
                context,
                busFactoryConfigurator
            ) =>
            {
                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.BookingReminderOfUpcomingCheckInDateQueue,
                    endpointConfigurator =>
                    {
                        endpointConfigurator.Durable = true;
                        endpointConfigurator.PrefetchCount = 16;
                        endpointConfigurator.UseMessageRetry(
                            retryConfig => retryConfig.Incremental(
                                5,
                                TimeSpan.FromSeconds(30),
                                TimeSpan.FromSeconds(30)
                            )
                        );
                        endpointConfigurator.Consumer<BookingReminderOfUpcomingCheckInDateConsumer>(context);
                    }
                );

                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.BookingReminderOfUpcomingCheckInDateSendMailQueue,
                    endpointConfigurator =>
                    {
                        endpointConfigurator.Durable = true;
                        endpointConfigurator.PrefetchCount = 16;
                        endpointConfigurator.UseMessageRetry(
                            retryConfig => retryConfig.Incremental(
                                5,
                                TimeSpan.FromSeconds(30),
                                TimeSpan.FromSeconds(30)
                            )
                        );
                        endpointConfigurator.Consumer<BookingReminderOfUpcomingCheckInDateSendMailConsumer>(context);
                    }
                );

                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.BookingCancellationFeeReminderQueue,
                    endpointConfigurator =>
                    {
                        endpointConfigurator.Durable = true;
                        endpointConfigurator.PrefetchCount = 16;
                        endpointConfigurator.UseMessageRetry(
                            configurator => configurator.Incremental(
                                5,
                                TimeSpan.FromSeconds(30),
                                TimeSpan.FromSeconds(30)
                            )
                        );
                        endpointConfigurator.Consumer<BookingCancellationFeeReminderConsumer>(context);
                    }
                );

                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.BookingCancellationFeeReminderSendMailQueue,
                    endpointConfigurator =>
                    {
                        endpointConfigurator.Durable = true;
                        endpointConfigurator.PrefetchCount = 16;
                        endpointConfigurator.UseMessageRetry(
                            configurator => configurator.Incremental(
                                5,
                                TimeSpan.FromSeconds(30),
                                TimeSpan.FromSeconds(30)
                            )
                        );
                        endpointConfigurator.Consumer<BookingCancellationFeeReminderSendMailConsumer>(context);
                    }
                );

                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.ImageResizeQueue,
                    endpointConfigurator =>
                    {
                        endpointConfigurator.Durable = true;
                        endpointConfigurator.PrefetchCount = 1;
                        endpointConfigurator.ConcurrentMessageLimit = 1;
                        endpointConfigurator.UseMessageRetry(
                            retryConfig => retryConfig.Incremental(
                                5,
                                TimeSpan.FromSeconds(30),
                                TimeSpan.FromSeconds(30)
                            )
                        );
                        endpointConfigurator.Consumer<ImageResizeConsumer>(context);
                    }
                );

                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.BookingSearchPrecomputeQueue,
                    endpointConfigurator =>
                    {
                        endpointConfigurator.Durable = true;
                        endpointConfigurator.PrefetchCount = 1;
                        endpointConfigurator.ConcurrentMessageLimit = 1;
                        endpointConfigurator.UseMessageRetry(
                            retryConfig => retryConfig.Incremental(
                                5,
                                TimeSpan.FromSeconds(30),
                                TimeSpan.FromSeconds(30)
                            )
                        );
                        endpointConfigurator.Consumer<BookingSearchPrecomputeConsumer>(context);
                    }
                );

                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.RoomGroupAppDateAggregationQueue,
                    endpointConfigurator =>
                    {
                        endpointConfigurator.Durable = true;
                        endpointConfigurator.PrefetchCount = 1;
                        endpointConfigurator.ConcurrentMessageLimit = 1;
                        endpointConfigurator.UseMessageRetry(
                            retryConfig => retryConfig.Incremental(
                                5,
                                TimeSpan.FromSeconds(30),
                                TimeSpan.FromSeconds(30)
                            )
                        );
                        endpointConfigurator.Consumer<RoomGroupAppDateAggregationConsumer>(context);
                    }
                );

                var options = configuration.GetSection("SetRoomAppDate").Get<RoomGroupAppDateOptions>();

                var roomGroupUpsertPartitionCount = options?.PartitionCount;
                for (var i = 0; i < roomGroupUpsertPartitionCount; i++)
                {
                    var queueName = $"{ReservationQueues.RoomGroupAppDateUpsertQueue}{i}";

                    busFactoryConfigurator.ReceiveEndpoint(
                        queueName,
                        e =>
                        {
                            e.ConfigureConsumer<RoomGroupAppDateUpsertConsumer>(context);
                            e.PrefetchCount = 1;
                            e.ConcurrentMessageLimit = 1;
                        }
                    );
                }
            }
        );

        return services;
    }
}
