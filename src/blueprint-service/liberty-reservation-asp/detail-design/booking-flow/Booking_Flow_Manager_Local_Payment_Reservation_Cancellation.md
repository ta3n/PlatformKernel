# Detail Design - Reservation Cancellation - Guest - Local Payment

------------------------------------------------------------------

## 1. Activity Diagram

```mermaid
flowchart TD
    A((Start)) -- Request --> B["`Call GuestEndpoint
                                /api/reservations/[id]/cancellation`"]
    B --> C[Call BookingCancellationCommandHandler]
    C --> D[Call IBookingCheckAvailableService.GetReservationByUserAsync]
    D --> E{Check Reservation IsReserved?}
    E -- Yes --> F[Call IBookingReservationService.GetCancellationPriceAsync]
    E -- No --> G[Throw ReservationInvalidException]
    F --> I[Call BookingAbortCommandHandler]
    I --> M{Check existing reservation can be modified?}
    M -- Yes --> N[Call IReservationService.CancelAsync]
    M -- No --> G
    G --> Y[Return Response]
    N --> Y
    Y -- Response --> Z(((Stop)))
```

> **Note**: The **`Activity Diagram`** illustrate the sequence of activities and interactions between different components during the booking change process by guest.
> The **`GuestEndPoint` `CancellationOfReservation`** use input parameters **`BookingCancellationRequest`** from body and **`code`** from route to filter existing reservation and adjust.
> The **`code`** in the **`GuestEndPoint`** is parameter from route.

### 1.1. Request (BookingCancellationRequest)

#### **BookingCancellationRequest Detail**

| No | Parameter | Description                                  |
|----|-----------|----------------------------------------------|
| 1  | Id        | Indicate the ID of the reservation to cancel |

### 1.2 Response

The **`GuestEndPoint` `CancellationOfReservation`** return code 204 no content with booking id in header when change execute successfully.

## 2. Sequence Diagram

### Guest Booking Reservation Cancellation Sequence Diagram

```mermaid
sequenceDiagram
  participant Client
  participant GuestEndpoint
  participant BookingCancellationCommandHandler
  participant BookingAbortCommandHandler
  participant IBookingCheckAvailableService
  participant IBookingReservationService
  participant IMailTemplateService
  participant MailJobScheduler
  participant IRepository
  participant Database

  Client->>+GuestEndpoint: 1. Request Payload (BookingAdjustRequest)
  GuestEndpoint->>+BookingCancellationCommandHandler: 2. Send BookingChangeExecutionCommand (payload)
  BookingCancellationCommandHandler->>+IBookingCheckAvailableService: 3. GetReservationByUserAsync (reservationId, userCode)
  IBookingCheckAvailableService->>+IRepository: 4. Query reservation data
  IRepository->>+Database: 5. Query data
  Database-->>-IRepository: 6. Return data
  IRepository-->>-IBookingCheckAvailableService: 7. Return reservation data
  IBookingCheckAvailableService-->>-BookingCancellationCommandHandler: 8. Return existing reservation
  BookingCancellationCommandHandler->>BookingCancellationCommandHandler: 9. Check if reservation is reserved

  alt reservation is not reserved
    BookingCancellationCommandHandler-->>Client: 10. Throw ReservationInvalidException
  else reservation is reserved
    BookingCancellationCommandHandler->>+IBookingReservationService: 11. GetCancellationPriceAsync(cancellationDatetime)
    IBookingReservationService->>+IRepository: 12. Query reservation cancellation data
    IRepository->>+Database: 13. Query reservation cancellation data
    Database-->>-IRepository: 14. Return reservation cancellation data
    IRepository-->>-IBookingReservationService: 15. Return reservation cancellation data
    IBookingReservationService-->>-BookingCancellationCommandHandler: 16. Return cancellation price
    BookingCancellationCommandHandler->>+BookingAbortCommandHandler: 17. Send BookingAbortCommand
    BookingAbortCommandHandler->>BookingAbortCommandHandler: 18. Check if reservation can be modified

    alt reservation cannot be modified
      BookingAbortCommandHandler-->>Client: 19. Throw ReservationInvalidException
    else reservation can be modified
      BookingAbortCommandHandler->>+IBookingReservationService: 20. CancelAsync(entityToUpdate, cancelPrice)
      IBookingReservationService->>+IRepository: 21. Cancel reservation
      IRepository->>+Database: 22. Cancel reservation
      Database-->>-IRepository: 23. Return updated reservation
      IRepository-->>-IBookingReservationService: 24. Return updated reservation
      IBookingReservationService-->>-BookingAbortCommandHandler: 25. Return updated reservation
      BookingAbortCommandHandler-->>-BookingCancellationCommandHandler: 26. Return BookingAbortResponse
      BookingCancellationCommandHandler->>+IMailTemplateService: 27. FindMailTemplateAsync()
      IMailTemplateService->>+IRepository: 28. Query mail template data
      IRepository->>+Database: 29. Query mail template data
      Database-->>-IRepository: 30. Return mail template data
      IRepository-->>-IMailTemplateService: 31. Return mail template data
      IMailTemplateService-->>-BookingCancellationCommandHandler: 32. Return mail template data
      BookingCancellationCommandHandler->>+MailJobScheduler: 33. Register Send Mail Job
      MailJobScheduler-->>-BookingCancellationCommandHandler: 34. Return response
      BookingCancellationCommandHandler-->>-GuestEndpoint: 35. Return response(bookingId)
      GuestEndpoint-->>-Client: 36. Return response(bookingId)
    end
  end

```

### Description

| No.  | Activity                                                              | Description                                                                                                                         |
|------|-----------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------|
| 1    | Client → GuestEndpoint                                                | The client sends a request payload (`BookingAdjustRequest`) to the GuestEndpoint.                                                   |
| 2    | GuestEndpoint → BookingCancellationCommandHandler                     | The GuestEndpoint sends a `BookingChangeExecutionCommand` with the payload to the command handler.                                  |
| 3    | BookingCancellationCommandHandler → IBookingCheckAvailableService     | The command handler calls `GetReservationByUserAsync` to retrieve reservation details by reservation ID and user code.              |
| 4    | IBookingCheckAvailableService → IRepository                           | The service queries the repository for reservation data.                                                                            |
| 5    | IRepository → Database                                                | The repository queries the database for reservation data.                                                                           |
| 6    | Database → IRepository                                                | The database returns the reservation data to the repository.                                                                        |
| 7    | IRepository → IBookingCheckAvailableService                           | The repository returns the reservation data to the service.                                                                         |
| 8    | IBookingCheckAvailableService → BookingCancellationCommandHandler     | The service returns the reservation details to the command handler.                                                                 |
| 9    | BookingCancellationCommandHandler → BookingCancellationCommandHandler | The handler checks if the reservation is reserved.                                                                                  |
| 10   | BookingCancellationCommandHandler → Client                            | If the reservation is not reserved, the handler throws a `ReservationInvalidException`.                                             |
| 11   | BookingCancellationCommandHandler → IBookingReservationService        | If the reservation is reserved, the handler calls `GetCancellationPriceAsync` to retrieve the cancellation price based on the date. |
| 12   | IBookingReservationService → IRepository                              | The service queries the repository for cancellation data.                                                                           |
| 13   | IRepository → Database                                                | The repository queries the database for cancellation data.                                                                          |
| 14   | Database → IRepository                                                | The database returns the cancellation data to the repository.                                                                       |
| 15   | IRepository → IBookingReservationService                              | The repository returns the cancellation data to the service.                                                                        |
| 16   | IBookingReservationService → BookingCancellationCommandHandler        | The service returns the cancellation price to the handler.                                                                          |
| 17   | BookingCancellationCommandHandler → BookingAbortCommandHandler        | The handler sends a `BookingAbortCommand` to the `BookingAbortCommandHandler` to proceed with cancellation.                         |
| 18   | BookingAbortCommandHandler → BookingAbortCommandHandler               | The handler checks if the existing reservation can be modified.                                                                     |
| 19   | BookingAbortCommandHandler → Client                                   | If the reservation cannot be modified, the handler throws a `ReservationInvalidException`.                                          |
| 20   | BookingAbortCommandHandler → IBookingReservationService               | If the reservation can be modified, the handler calls `CancelAsync` to cancel the reservation.                                      |
| 21   | IBookingReservationService → IRepository                              | The service calls the repository to cancel the reservation.                                                                         |
| 22   | IRepository → Database                                                | The repository sends the cancel request to the database to update the reservation status.                                           |
| 23   | Database → IRepository                                                | The database returns the updated reservation data to the repository.                                                                |
| 24   | IRepository → IBookingReservationService                              | The repository returns the updated reservation data to the service.                                                                 |
| 25   | IBookingReservationService → BookingAbortCommandHandler               | The service returns the updated reservation to the handler.                                                                         |
| 26   | BookingAbortCommandHandler → BookingCancellationCommandHandler        | The handler returns the `BookingAbortResponse` to the `BookingCancellationCommandHandler`.                                          |
| 27   | BookingCancellationCommandHandler → IMailTemplateService              | The handler calls `FindMailTemplateAsync` to fetch the appropriate mail template.                                                   |
| 28   | IMailTemplateService → IRepository                                    | The service queries the repository for mail template data.                                                                          |
| 29   | IRepository → Database                                                | The repository queries the database for mail template data.                                                                         |
| 30   | Database → IRepository                                                | The database returns the mail template data to the repository.                                                                      |
| 31   | IRepository → IMailTemplateService                                    | The repository returns the mail template data to the service.                                                                       |
| 32   | IMailTemplateService → BookingCancellationCommandHandler              | The service returns the mail template data to the handler.                                                                          |
| 33   | BookingCancellationCommandHandler → MailJobScheduler                  | The handler registers a mail job to send an email.                                                                                  |
| 34   | MailJobScheduler → BookingCancellationCommandHandler                  | The scheduler returns the response after registering the mail job.                                                                  |
| 35   | BookingCancellationCommandHandler → GuestEndpoint                     | The handler returns the booking ID to the GuestEndpoint.                                                                            |
| 36   | GuestEndpoint → Client                                                | The GuestEndpoint returns the booking ID to the client.                                                                             |

> **Note**: The `Sequence Diagram` illustrates the interactions between the client, guest endpoint, command handler, services, repository and database during the booking cancellation process by guest.
