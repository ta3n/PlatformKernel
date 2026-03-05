# Detail Design - Reservation Cancellation - Member - Online Payment

------------------------------------------------------------------

## 1. Activity Diagram

```mermaid
flowchart TD
  A((Start)) -- Request --> B["`Call ReservationsEndpoint
                                /api/reservations/[id]/cancellation`"]
  B --> C[Call BookingCancellationCommandHandler]
  C --> D[Call IBookingCheckAvailableService.GetReservationByUserAsync]
  D --> E{Check Reservation IsReserved?}
  E -- Yes --> F[Call IBookingCheckModifyInPriceService.IsModifyInPriceAsync]
  E -- No --> G[Throw ReservationInvalidException]
  F --> H{Check Reservation IsOnlinePayment?}
  H -- Yes --> I1[Call OnlinePaymentRefundAsync]
  H -- No --> J[Call BookingAbortCommandHandler]
  I1 --> I2[Call IBookingReservationService.FindOderIdOfOnlinePaymentAsync]
  I2 --> K[Call IGmoPaymentGatewayService.SearchTradeAsync]
  K --> L[Call IGmoPaymentGatewayService.CancelAsync]
  L --> J
  J --> M{Check if current booking can be cancelled?}
  M -- Yes --> N[IBookingReservationService.CancelAsync]
  M -- No --> O[Throw ReservationInvalidException]
  N --> NF{If there is an error update in database?}
  NF -- No --> AG[Call IUnitOfWork.RollbackAsync]
  NF -- Yes --> S[Register Send Mail Schedule Job]
  AG --> AH[Throw AppLibertyException]
  G --> Y[Return Response]
  O --> Y
  S --> Y
  AH --> Y
  Y -- Response --> Z(((Stop)))
```

> Note: The `Activity Diagram` illustrate the sequence of activities and interactions between different components during the booking
> cancellation process by member.
> The `ReservationsEndpoint` use input parameters `BookingCancellationRequest` from body and `id` from route to filter existing reservation
> and cancellation.
> The `id` in the `ReservationsEndpoint` is parameter from route.

### 1.1. Request (BookingCancellationRequest)

| No | Parameter | Description                           |
|----|-----------|---------------------------------------|
| 1  | AppDateId | The ID of the option item (required). |

### 1.2 Response

The `ReservationsEndpoint`  return code 204 no content with booking id in header when cancellation execute successfully.

## 2. Sequence Diagram

```mermaid
sequenceDiagram
  participant Client
  participant ReservationsEndpoint
  participant BookingCancellationCommandHandler
  participant IBookingCheckAvailableService
  participant BookingAbortCommandHandler
  participant IBookingReservationService
  participant IGmoPaymentGatewayService
  participant IUnitOfWork
  participant MailJobScheduler
  participant Reservation.MailService
  participant IRepository
  participant Database
  Client ->> ReservationsEndpoint: 1. Request Payload (BookingCancellationRequest)
  ReservationsEndpoint ->> BookingCancellationCommandHandler: 2. Send BookingCancellationCommand (payload)
  BookingCancellationCommandHandler ->> IBookingCheckAvailableService: 3. GetReservationByUserAsync (reservationId, userCode)
  IBookingCheckAvailableService ->> IRepository: 3.1 Query reservation data
  IRepository ->> Database: 3.1.1 Query data
  Database -->> IRepository: 3.1.2 Return data
  IRepository -->> IBookingCheckAvailableService: 3.2 Return reservation data
  IBookingCheckAvailableService -->> BookingCancellationCommandHandler: 3.3 Return reservation data
  BookingCancellationCommandHandler ->> BookingCancellationCommandHandler: 4. Check if reservation is reserved
  alt reservation is not reserved
    BookingCancellationCommandHandler -->> ReservationsEndpoint: 5. Throw ReservationInvalidException
    ReservationsEndpoint -->> Client: 6. Return Response
  end
  BookingCancellationCommandHandler ->> BookingCancellationCommandHandler: 7. Check if reservation is online payment
  alt reservation is online payment
    BookingCancellationCommandHandler ->> IBookingReservationService: 8. FindOrderIdOfOnlinePaymentAsync (reservationId)
    IBookingReservationService ->> IGmoPaymentGatewayService: 8.1 SearchTradeAsync (orderId)
    IGmoPaymentGatewayService -->> IBookingReservationService: 8.2 Return trade data
    IBookingReservationService ->> IGmoPaymentGatewayService: 8.3 CancelAsync (orderId)
    IGmoPaymentGatewayService -->> IBookingReservationService: 8.4 Return cancel data
  end
  BookingCancellationCommandHandler ->> BookingAbortCommandHandler: 9. Send BookingAbortCommand (ReservationStatus.UserCanceled, *params)
  BookingAbortCommandHandler ->> BookingAbortCommandHandler: 10. Check if current booking can be cancelled
  alt booking cannot be cancelled
    BookingAbortCommandHandler -->> ReservationsEndpoint: 11. Throw ReservationInvalidException
    ReservationsEndpoint -->> Client: 12. Return Response
  end
  BookingAbortCommandHandler ->> IBookingReservationService: 13. CancelAsync (reservationId)
  IBookingReservationService ->> IRepository: 13.1 Update reservation status
  IRepository ->> Database: 13.1.1 Save data
  Database -->> IRepository: 13.1.2 Return data
  IRepository -->> IBookingReservationService: 13.2 Return data
  IBookingReservationService -->> BookingAbortCommandHandler: 13.3 Return updated reservation
  BookingAbortCommandHandler ->> BookingAbortCommandHandler: 14. Check if there is an error updating the database
  alt error updating database
    BookingAbortCommandHandler ->> IUnitOfWork: 15. RollbackAsync
    IUnitOfWork -->> BookingAbortCommandHandler: 16. Rollback completed
    BookingAbortCommandHandler -->> ReservationsEndpoint: 17. Throw AppLibertyException
    ReservationsEndpoint -->> Client: 18. Return Response
  end
  BookingCancellationCommandHandler ->> MailJobScheduler: 19. Register Send Mail Schedule Job
  MailJobScheduler ->> Reservation.MailService: 19.1 Publish to queue RabbitMQ
  MailJobScheduler -->> BookingCancellationCommandHandler: 19.2 Return response
  BookingCancellationCommandHandler -->> ReservationsEndpoint: 20. Return Success Response
  ReservationsEndpoint -->> Client: 21. Return Response
```

### Description

| No.    | Activity                                                          | Description                                                                                    |
|--------|-------------------------------------------------------------------|------------------------------------------------------------------------------------------------|
| 1      | Client → ReservationsEndpoint                                     | The `Client` sends a `BookingCancellationRequest` payload to the `ReservationsEndpoint`.       |
| 2      | ReservationsEndpoint → BookingCancellationCommandHandler          | `ReservationsEndpoint` forwards the request as a `BookingCancellationCommand`.                 |
| 3      | BookingCancellationCommandHandler → IBookingCheckAvailableService | Calls `GetReservationByUserAsync` with `reservationId` and `userCode` to get reservation data. |
| 3.1    | IBookingCheckAvailableService → IRepository                       | Queries the reservation data from `IRepository`.                                               |
| 3.1.1  | IRepository → Database                                            | Queries the data from `Database`.                                                              |
| 3.1.2  | Database → IRepository                                            | Returns the data to `IRepository`.                                                             |
| 3.2    | IRepository → IBookingCheckAvailableService                       | Returns the reservation data to `IBookingCheckAvailableService`.                               |
| 3.3    | IBookingCheckAvailableService → BookingCancellationCommandHandler | Returns the reservation data to `BookingCancellationCommandHandler`.                           |
| 4      | BookingCancellationCommandHandler                                 | Checks if the reservation is reserved.                                                         |
| 5      | BookingCancellationCommandHandler → ReservationsEndpoint          | Throws `ReservationInvalidException` if the reservation is not reserved.                       |
| 6      | ReservationsEndpoint → Client                                     | Returns the exception to the client.                                                           |
| 7      | BookingCancellationCommandHandler                                 | Checks if the reservation is an online payment.                                                |
| 8      | BookingCancellationCommandHandler → IBookingReservationService    | Calls `FindOrderIdOfOnlinePaymentAsync` with `reservationId`.                                  |
| 8.1    | IBookingReservationService → IGmoPaymentGatewayService            | Calls `SearchTradeAsync` with `orderId`.                                                       |
| 8.2    | IGmoPaymentGatewayService → IBookingReservationService            | Returns the trade data to `IBookingReservationService`.                                        |
| 8.3    | IBookingReservationService → IGmoPaymentGatewayService            | Calls `CancelAsync` with `orderId`.                                                            |
| 8.4    | IGmoPaymentGatewayService → IBookingReservationService            | Returns the cancel data to `IBookingReservationService`.                                       |
| 9      | BookingCancellationCommandHandler → BookingAbortCommandHandler    | Sends the `BookingAbortCommand` with `ReservationStatus.UserCanceled` and parameters.          |
| 10     | BookingAbortCommandHandler                                        | Checks if the current booking can be cancelled.                                                |
| 11     | BookingAbortCommandHandler → ReservationsEndpoint                 | Throws `ReservationInvalidException` if the booking cannot be cancelled.                       |
| 12     | ReservationsEndpoint → Client                                     | Returns the exception to the client.                                                           |
| 13     | BookingAbortCommandHandler → IBookingReservationService           | Calls `CancelAsync` with `reservationId`.                                                      |
| 13.1   | IBookingReservationService → IRepository                          | Updates the reservation status in `IRepository`.                                               |
| 13.1.1 | IRepository → Database                                            | Saves the data in `Database`.                                                                  |
| 13.1.2 | Database → IRepository                                            | Returns the data to `IRepository`.                                                             |
| 13.2   | IRepository → IBookingReservationService                          | Returns the data to `IBookingReservationService`.                                              |
| 13.3   | IBookingReservationService → BookingAbortCommandHandler           | Returns the updated reservation to `BookingAbortCommandHandler`.                               |
| 14     | BookingAbortCommandHandler                                        | Checks if there is an error updating the database.                                             |
| 15     | BookingAbortCommandHandler → IUnitOfWork                          | Calls `RollbackAsync` if there is an error.                                                    |
| 16     | IUnitOfWork → BookingAbortCommandHandler                          | Returns the rollback completion to `BookingAbortCommandHandler`.                               |
| 17     | BookingAbortCommandHandler → ReservationsEndpoint                 | Throws `AppLibertyException` if there is an error.                                             |
| 18     | ReservationsEndpoint → Client                                     | Returns the exception to the client.                                                           |
| 19     | BookingCancellationCommandHandler → MailJobScheduler              | Registers the send mail schedule job.                                                          |
| 19.1   | MailJobScheduler → Reservation.MailService                        | Publishes the mail job to the RabbitMQ queue.                                                  |
| 19.2   | MailJobScheduler → BookingCancellationCommandHandler              | Returns the response to `BookingCancellationCommandHandler`.                                   |
| 20     | BookingCancellationCommandHandler → ReservationsEndpoint          | Returns the success response to `ReservationsEndpoint`.                                        |
| 21     | ReservationsEndpoint → Client                                     | Returns the final response to the client.                                                      |

> Note: The `Sequence Diagram` illustrates the interactions between the client, `ReservationsEndpoint`, command handler, services,
> repository and database during the booking cancellation process by member.
