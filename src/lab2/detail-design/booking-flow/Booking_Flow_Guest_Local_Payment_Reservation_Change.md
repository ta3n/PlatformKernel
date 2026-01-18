# Detail Design - Reservation Change - Guest - Local Payment

------------------------------------------------------------------

## 1. Activity Diagram

```mermaid
flowchart TD
  A((Start)) -- Request --> B["`Call GuestEndpoint
                                /api/guest/booking/[code]/change-execution`"]
  B --> B1[Call IBookingSecureUrlService.DecryptAndValidate]
  B1 --> C[Call BookingChangeExecutionCommandHandler]
  C --> D[Call IBookingCheckAvailableService.GetReservationByUserAsync]
  D --> E{Check Reservation IsReserved?}
  E -- Yes --> F[Call IBookingCheckModifyInPriceService.IsModifyInPriceAsync]
  E -- No --> G[Throw ReservationInvalidException]
  F --> H{Check Reservation IsOnlinePayment and IsModifyInPrice?}
  H -- Yes --> I[Call IBookingReservationService.FindOderIdOfOnlinePaymentAsync]
  H -- No --> J[Call BookingAdjustCommandHandler]
  I --> K[Call IGmoPaymentGatewayService.SearchTradeAsync]
  K --> L[Call IGmoPaymentGatewayService.CancelAsync]
  L --> J
  J --> M{Check existing reservation can be modified?}
  M -- Yes --> N[Call IBookingCheckAvailableService.CheckAdjustAvailableAsync]
  M -- No --> G
  N --> P{Check valid booking?}
  P -- Yes --> Q[Call BookingAdjustHeaderDataCommand]
  P -- No --> G
  Q --> X[Call ReservationService.AdjustHeaderDataOfReservationAsync]
  X --> S[Register Send Mail Schedule Job]
  G --> Y[Return Response]
  S --> Y
  Y -- Response --> Z(((Stop)))
```

> **Note**: The **`Activity Diagram`** illustrate the sequence of activities and interactions between different components during the
> booking change process by guest.
> The **`GuestEndPoint` `ChangeExecutionOfReservation`** use input parameters **`BookingAdjustRequest`** from body and **`code`** from route
> to filter existing reservation and adjust.
> The **`code`** in the **`GuestEndPoint`** is parameter from route.

### 1.1. Request (BookingAdjustRequest)

#### **BookingAdjustRequest Detail**

| No | Parameter           | Description                                                      |
|----|---------------------|------------------------------------------------------------------|
| 1  | IsAgree             | Indicate the adjust is agreed or not                             |
| 2  | CheckInTime         | The check-in time.                                               |
| 3  | NumberOfNights      | The number of nights to stay.                                    |
| 4  | NumberOfRooms       | The number of rooms to stay.                                     |
| 5  | FreeInput           | The memo information of reservation (optional).                  |
| 6  | Reserver            | The information of the representative.                           |
| 7  | MainUser            | The information of the customer.                                 |
| 8  | NightPeoples        | The information of person age type group by room and night.      |
| 9  | NightOptions        | The information of option item age type group by room and night. |
| 10 | RoomRepresentatives | The information of rooms in reservation.                         |

> **Note**: Ensure that the **`CheckInTime`** are in the correct format **`mm:ss`**.
**`NightPeoples`** parameter indicates a list of **`NightPeopleOfReservationAdjustRequest`**.
**`NightOptions`** parameter indicates a list of **`NightOptionOfReservationAdjustRequest`**. **`RoomRepresentatives`** parameter indicates
> a list of **`RoomRepresentativeOfReservationAdjustRequest`**.

##### **NightPeopleOfReservationAdjustRequest Detail**

| No | Parameter | Description                                        |
|----|-----------|----------------------------------------------------|
| 1  | AppDateId | The app date of night.                             |
| 2  | Rooms     | The person age type information of rooms in night. |

> **Note**: **`Rooms`** parameter indicates a list of **`RoomNightOfReservationAdjustRequest`**.

###### **RoomNightOfReservationAdjustRequest Detail**

| No | Parameter | Description                                 |
|----|-----------|---------------------------------------------|
| 1  | RoomIndex | The index of the room in night.             |
| 2  | Peoples   | The information of person age type in room. |

> **Note**: **`Peoples`** parameter indicates a list of **`PeopleOfReservationAdjustRequest`**.

###### **PeopleOfReservationAdjustRequest Detail**

| No | Parameter       | Description                                                          |
|----|-----------------|----------------------------------------------------------------------|
| 1  | PersonAgeTypeId | The id of person age type.                                           |
| 2  | NumberOfPeoples | The number of people in range of min and max age of person age type. |
| 2  | Gender          | The gender of person age type in room.                               |

##### **NightOptionOfReservationAdjustRequest Detail**

| No | Parameter | Description                                    |
|----|-----------|------------------------------------------------|
| 1  | AppDateId | The app date of night.                         |
| 2  | Rooms     | The option item information of rooms in night. |

> **Note**: **`Rooms`** parameter indicates a list of **`RoomOptionOfReservationAdjustRequest`**.

###### **RoomOptionOfReservationAdjustRequest Detail**

| No | Parameter   | Description                             |
|----|-------------|-----------------------------------------|
| 1  | RoomIndex   | The index of the room in night.         |
| 2  | OptionItems | The information of option item in room. |

> **Note**: **`OptionItems`** parameter indicates a list of **`OptionOfReservationAdjustRequest`**.

###### **OptionOfReservationAdjustRequest Detail**

| No | Parameter    | Description                        |
|----|--------------|------------------------------------|
| 1  | OptionItemId | The id of option item.             |
| 2  | Number       | The number of option item in room. |

##### **RoomRepresentativeOfReservationAdjustRequest Detail**

| No | Parameter | Description                                    |
|----|-----------|------------------------------------------------|
| 1  | AppDateId | The app date of night.                         |
| 2  | Rooms     | The option item information of rooms in night. |

### 1.2 Response

The **`GuestEndPoint` `ChangeExecutionOfReservation`** return code 204 no content with booking id in header when change execute
successfully.

## 2. Sequence Diagram

### Guest Booking Reservation Change Execute Sequence Diagram

```mermaid
sequenceDiagram
  participant Client
  participant GuestEndpoint
  participant BookingChangeExecutionCommandHandler
  participant BookingAdjustCommandHandler
  participant BookingAdjustPriceCommandHandler
  participant BookingAdjustHeaderDataCommandHandler
  participant IBookingSecureUrlService
  participant IBookingCheckAvailableService
  participant IBookingCheckModifyInPriceService
  participant IBookingReservationService
  participant IMailTemplateService
  participant IRepository
  participant Database
  participant MailJobScheduler(Liberty Batch)
  Client ->>+ GuestEndpoint: 1. Request Payload (BookingAdjustRequest)
  GuestEndpoint ->>+ IBookingSecureUrlService: 2. DecryptAndValidate(code)
  IBookingSecureUrlService -->>- GuestEndpoint: 3. Return BookingSecureUrlResponse
  GuestEndpoint ->>+ BookingChangeExecutionCommandHandler: 4. Send BookingChangeExecutionCommand (payload)
  BookingChangeExecutionCommandHandler ->>+ IBookingCheckAvailableService: 5. GetReservationByUserAsync (reservationId, userCode)
  IBookingCheckAvailableService ->>+ IRepository: 6. Query reservation data
  IRepository ->>+ Database: 7. Query data
  Database -->>- IRepository: 8. Return data
  IRepository -->>- IBookingCheckAvailableService: 9. Return reservation data
  IBookingCheckAvailableService -->>- BookingChangeExecutionCommandHandler: 10. Return existing reservation
  BookingChangeExecutionCommandHandler ->> BookingChangeExecutionCommandHandler: 11. Check if reservation is reserved
  alt reservation is not reserved
    BookingChangeExecutionCommandHandler -->> Client: 11.1. Throw ReservationInvalidException
  else reservation is reserved
    BookingChangeExecutionCommandHandler ->>+ IBookingCheckModifyInPriceService: 12. IsModifyInPriceAsync(existingReservation, adjustRequest)
    IBookingCheckModifyInPriceService ->>+ IRepository: 13. Query data
    IRepository ->>+ Database: 14. Query data
    Database -->>- IRepository: 15. Return data
    IRepository -->>- IBookingCheckModifyInPriceService: 16. Return data
    IBookingCheckModifyInPriceService -->>- BookingChangeExecutionCommandHandler: 17. Return isModifyInPrice
    BookingChangeExecutionCommandHandler ->> BookingChangeExecutionCommandHandler: 18. Check if reservation is online payment and modify in price
    BookingChangeExecutionCommandHandler ->>+ BookingAdjustCommandHandler: 19. Send BookingAdjustCommand
    BookingAdjustCommandHandler ->>+ IBookingCheckAvailableService: 20. CheckAdjustAvailableAsync(facilityId, planId, adjustRequest)
    IBookingCheckAvailableService ->>+ IRepository: 21. Query data
    IRepository ->>+ Database: 22. Query data
    Database -->>- IRepository: 23. Return data
    IRepository -->>- IBookingCheckAvailableService: 24. Return data
    IBookingCheckAvailableService -->>- BookingAdjustCommandHandler: 25. Return booking validity
    BookingAdjustCommandHandler ->> BookingAdjustCommandHandler: 26. Validate booking, check modify in price
    BookingAdjustCommandHandler ->>+ BookingAdjustHeaderDataCommandHandler: 27. Send BookingAdjustHeaderCommand
    BookingAdjustHeaderDataCommandHandler ->>+ IBookingReservationService: 28. AdjustHeaderDataOfReservationAsync(entityToUpdate)
    IBookingReservationService ->>+ IRepository: 29. Update reservation
    IRepository ->>+ Database: 30. Save data
    Database -->>- IRepository: 31. Return data
    IRepository -->>- IBookingReservationService: 32. Return data
    IBookingReservationService -->>- BookingAdjustHeaderDataCommandHandler: 33. Return updated reservation
    BookingAdjustHeaderDataCommandHandler -->>- BookingAdjustCommandHandler: 34. Return reservation ID
    BookingAdjustCommandHandler -->>- BookingChangeExecutionCommandHandler: 35. Return reservation ID
    BookingChangeExecutionCommandHandler ->>+ IMailTemplateService: 36. FindMailTemplateAsync()
    IMailTemplateService ->>+ IRepository: 37. Query mail template data
    IRepository ->>+ Database: 38. Query mail template data
    Database -->>- IRepository: 39. Return mail template data
    IRepository -->>- IMailTemplateService: 40. Return mail template data
    IMailTemplateService -->>- BookingChangeExecutionCommandHandler: 41. Return mail template data
    BookingChangeExecutionCommandHandler ->>+ MailJobScheduler: 42. Register Send Mail Job
    MailJobScheduler -->>- BookingChangeExecutionCommandHandler: 43. Return response
    BookingChangeExecutionCommandHandler -->>- GuestEndpoint: 44. Return reservation ID
    GuestEndpoint -->>- Client: 45. Return reservation ID
  end
```

### Description

| No.  | Activity                                                                    | Description                                                                                                    |
|------|-----------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------|
| 1    | Client → GuestEndpoint                                                      | The client sends a request payload (`BookingAdjustRequest`) to the GuestEndpoint.                              |
| 2    | GuestEndpoint → IBookingSecureUrlService                                    | The GuestEndpoint calls `DecryptAndValidate(code)` to decrypt and validate the secure URL code.                |
| 3    | IBookingSecureUrlService → GuestEndpoint                                    | The service returns a `BookingSecureUrlResponse` to the GuestEndpoint.                                         |
| 4    | GuestEndpoint → BookingChangeExecutionCommandHandler                        | The GuestEndpoint sends a `BookingChangeExecutionCommand` with the payload to the handler.                     |
| 5    | BookingChangeExecutionCommandHandler → IBookingCheckAvailableService        | The handler calls `GetReservationByUserAsync` to retrieve reservation details by reservation ID and user code. |
| 6    | IBookingCheckAvailableService → IRepository                                 | The service queries the repository for reservation data.                                                       |
| 7    | IRepository → Database                                                      | The repository queries the database for reservation data.                                                      |
| 8    | Database → IRepository                                                      | The database returns the reservation data to the repository.                                                   |
| 9    | IRepository → IBookingCheckAvailableService                                 | The repository returns the reservation data to the service.                                                    |
| 10   | IBookingCheckAvailableService → BookingChangeExecutionCommandHandler        | The service returns the reservation details to the handler.                                                    |
| 11   | BookingChangeExecutionCommandHandler → BookingChangeExecutionCommandHandler | The handler checks if the reservation is reserved.                                                             |
| 11.1 | BookingChangeExecutionCommandHandler → GuestEndpoint                        | If the reservation is not reserved, the handler throws an exception to the GuestEndpoint.                      |
| 12   | BookingChangeExecutionCommandHandler → IBookingCheckModifyInPriceService    | If the reservation is reserved, the handler calls `IsModifyInPriceAsync` to check modification pricing.        |
| 13   | IBookingCheckModifyInPriceService → IRepository                             | The service queries the repository for data.                                                                   |
| 14   | IRepository → Database                                                      | The repository queries the database for data.                                                                  |
| 15   | Database → IRepository                                                      | The database returns the queried data to the repository.                                                       |
| 16   | IRepository → IBookingCheckModifyInPriceService                             | The repository returns the queried data to the service.                                                        |
| 17   | IBookingCheckModifyInPriceService → BookingChangeExecutionCommandHandler    | The service returns the modification pricing status to the handler.                                            |
| 18   | BookingChangeExecutionCommandHandler → BookingChangeExecutionCommandHandler | The handler checks if the reservation involves online payment and requires a price modification.               |
| 19   | BookingChangeExecutionCommandHandler → BookingAdjustCommandHandler          | The handler sends a `BookingAdjustCommand` to the BookingAdjustCommandHandler.                                 |
| 20   | BookingAdjustCommandHandler → IBookingCheckAvailableService                 | The handler calls `CheckAdjustAvailableAsync` to validate the adjustment.                                      |
| 21   | IBookingCheckAvailableService → IRepository                                 | The service queries the repository for adjustment data.                                                        |
| 22   | IRepository → Database                                                      | The repository queries the database for adjustment data.                                                       |
| 23   | Database → IRepository                                                      | The database returns the adjustment data to the repository.                                                    |
| 24   | IRepository → IBookingCheckAvailableService                                 | The repository returns the adjustment data to the service.                                                     |
| 25   | IBookingCheckAvailableService → BookingAdjustCommandHandler                 | The service returns the booking validity status to the handler.                                                |
| 26   | BookingAdjustCommandHandler → BookingAdjustCommandHandler                   | The handler validates the booking and checks for price modification.                                           |
| 27   | BookingAdjustCommandHandler → BookingAdjustHeaderDataCommandHandler         | The handler sends a `BookingAdjustHeaderCommand` to adjust the reservation header data.                        |
| 28   | BookingAdjustHeaderDataCommandHandler → IBookingReservationService          | The handler calls `AdjustHeaderDataOfReservationAsync` to update reservation details.                          |
| 29   | IBookingReservationService → IRepository                                    | The service updates the reservation data in the repository.                                                    |
| 30   | IRepository → Database                                                      | The repository saves the updated reservation data to the database.                                             |
| 31   | Database → IRepository                                                      | The database returns the saved data to the repository.                                                         |
| 32   | IRepository → IBookingReservationService                                    | The repository returns the saved data to the service.                                                          |
| 33   | IBookingReservationService → BookingAdjustHeaderDataCommandHandler          | The service returns the updated reservation to the handler.                                                    |
| 34   | BookingAdjustHeaderDataCommandHandler → BookingAdjustCommandHandler         | The handler returns the reservation ID to the BookingAdjustCommandHandler.                                     |
| 35   | BookingAdjustCommandHandler → BookingChangeExecutionCommandHandler          | The handler returns the reservation ID to the BookingChangeExecutionCommandHandler.                            |
| 36   | BookingChangeExecutionCommandHandler → IMailTemplateService                 | The handler calls `FindMailTemplateAsync` to fetch the appropriate mail template.                              |
| 37   | IMailTemplateService → IRepository                                          | The service queries the repository for mail template data.                                                     |
| 38   | IRepository → Database                                                      | The repository queries the database for mail template data.                                                    |
| 39   | Database → IRepository                                                      | The database returns the mail template data to the repository.                                                 |
| 40   | IRepository → IMailTemplateService                                          | The repository returns the mail template data to the service.                                                  |
| 41   | IMailTemplateService → BookingChangeExecutionCommandHandler                 | The service returns the mail template data to the handler.                                                     |
| 42   | BookingChangeExecutionCommandHandler → MailJobScheduler                     | The handler registers a mail job to send an email.                                                             |
| 43   | MailJobScheduler → BookingChangeExecutionCommandHandler                     | The scheduler returns the mail job response to the handler.                                                    |
| 44   | BookingChangeExecutionCommandHandler → GuestEndpoint                        | The handler returns the reservation ID to the GuestEndpoint.                                                   |
| 45   | GuestEndpoint → Client                                                      | The GuestEndpoint returns the reservation ID to the client.                                                    |

> **Note**: The `Sequence Diagram` illustrates the interactions between the client, guest endpoint, command handler, services, repository
> and database during the booking change process by guest.
