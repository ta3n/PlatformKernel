using System.Reflection;
using Liberty.MassTransit;
using Liberty.Reservation.Booking.Worker.Application.Options;
using Liberty.Reservation.Booking.Worker.Consumers;
using Liberty.SysIntegrationEvent;
using MassTransit;

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

                var bookingSearchPrePrecomputeOption =
                    configuration.GetSection("PartitionQueue:BookingSearchPrePrecompute").Get<BookingSearchPrePrecomputeOption>();
                var bookingSearchPrePrecomputePartitionCount = bookingSearchPrePrecomputeOption?.PartitionCount;
                for (var i = 0; i < bookingSearchPrePrecomputePartitionCount; i++)
                {
                    var queueName = $"{ReservationQueues.BookingSearchPrecomputePartitionQueue}{i}";

                    busFactoryConfigurator.ReceiveEndpoint(
                        queueName,
                        e =>
                        {
                            e.PrefetchCount = 1;
                            e.ConcurrentMessageLimit = 1;
                            e.UseMessageRetry(
                                retryConfig => retryConfig.Incremental(
                                    5,
                                    TimeSpan.FromSeconds(30),
                                    TimeSpan.FromSeconds(30)
                                )
                            );
                            e.ConfigureConsumer<BookingSearchPrecomputePartitionConsumer>(context);
                        }
                    );
                }

                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.EmailSentAuditLogQueue,
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
                        endpointConfigurator.Consumer<EmailSentAuditLogConsumer>(context);
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

                var roomGroupAppDateOptions = configuration.GetSection("PartitionQueue:SetRoomAppDate").Get<RoomGroupAppDateOptions>();
                var roomGroupUpsertPartitionCount = roomGroupAppDateOptions?.PartitionCount;
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

                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.BookingAggregationAuditLogQueue,
                    endpointConfigurator =>
                    {
                        endpointConfigurator.Durable = true;
                        endpointConfigurator.PrefetchCount = 3;
                        endpointConfigurator.ConcurrentMessageLimit = 1;
                        endpointConfigurator.UseMessageRetry(
                            retryConfig => retryConfig.Incremental(
                                3,
                                TimeSpan.FromSeconds(30),
                                TimeSpan.FromSeconds(30)
                            )
                        );
                        endpointConfigurator.Consumer<BookingAggregateAuditLogConsumer>(context);
                    }
                );

                busFactoryConfigurator.ReceiveEndpoint(
                    ReservationQueues.BookingAggregateFaxAuditLogQueue,
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
                        endpointConfigurator.Consumer<BookingAggregateFaxAuditLogConsumer>(context);
                    }
                );

                var aggregateAuditLogOption = configuration.GetSection("PartitionQueue:AggregateAuditLog").Get<AggregateAuditLogOption>();
                var aggregateAuditLogPartitionCount = aggregateAuditLogOption?.PartitionCount;
                for (var i = 0; i < aggregateAuditLogPartitionCount; i++)
                {
                    var queueName = $"{ReservationQueues.BookingAuditLogPartitionQueue}{i}";

                    busFactoryConfigurator.ReceiveEndpoint(
                        queueName,
                        e =>
                        {
                            e.PrefetchCount = 1;
                            e.ConcurrentMessageLimit = 1;
                            e.UseMessageRetry(
                                retryConfig => retryConfig.Incremental(
                                    2,
                                    TimeSpan.FromSeconds(30),
                                    TimeSpan.FromSeconds(30)
                                )
                            );
                            e.Consumer<BookingAuditLogPartitionConsumer>(context);
                        }
                    );
                }
            }
        );

        return services;
    }
}
