# Detail Design - Check Changed - Detail Flow

## 1.Activity Diagram

```mermaid
flowchart TD
  A@{ shape: circle, label: "Start" } --> A0[Get last updated string]
  A0 --> A01[Get updated_at of facility, plan, room group, person age type]
  A01 --> A02[Create LastUpdatedTimeOfBoookingModel]
  A02 --> A03[Convert LastUpdatedTimeOfBoookingModel to json]
  A03 --> A04[Convert json to base64 string]
  A04 --> A05[Return LastUpdatedString base64]
  A05 --> A06[Request check changed]
  A06 -- Request --> A1[Call ICheckChangedService.GetLastUpdatedAt return LastUpdatedTimeOfBookingModel]
  A1 --> A2[Parse payload from base64 string to LastUpdatedTimeOfBookingModel]
  A2 --> A3{Parse successfully?}
  A3 -- No --> M[Throw LastUpdateStringInvalidException]
  A3 -- Yes --> B
  B[Compare last updated time from service and payload]
  B --> B1{Is changed?}
  B1 -- Yes --> B2[Check property change]
  B2 -->|Case facility change| C[Throw FacilityHasChangesException]
  B2 -->|Case site change| D[Throw SiteHasChangesException]
  B2 -->|Case plan change| E[Throw PlanHasChangesException]
  B2 -->|Case cancellation change| F[Throw CancellationHasChangesException]
  B2 -->|Case room group change| G[Throw RoomGroupHasChangesException]
  B2 -->|Case question change| H[Throw QuestionHasChangesException]
  B2 -->|Case file change| I[Throw FileHasChangesException]
  B2 -->|Case person age type change| K[Throw PersonAgeTypeHasChangesException]
  B2 -->|Default| L[Throw AppLibertyException]
  B1 -- No --> B3[Check option changed]
  B3 --> B4{Is changed?}
  B4 --> Yes --> O[Throw OptionItemHasChangesException]
  B4 --> No --> P[Return true]
  P --> N
  O --> N
  C --> N
  D --> N
  E --> N
  F --> N
  G --> N
  H --> N
  I --> N
  J --> N
  K --> N
  L --> N
  M --> N@{ shape: dbl-circ, label: "Stop" }
  ```

> **Note**: The `Activity Diagram` illustrate the sequence of activities and interactions between different components during the check changed process.
> The `BookingEndPoint` `CheckChangeAsync` and `CheckChangedOfRoomAsync` use input parameters `CheckChangedRequest` to check whether the plan or room information has been changed.

### 1.1. Request (CheckChangedRequest)

#### **CheckChangedRequest**

| No | Parameter         | Description                                         |
|----|-------------------|-----------------------------------------------------|
| 1  | LastUpdatedString | The last updated time model base64 encoded (required). |
| 2  | OptionItems       | List of option item response (required).            |

> **Note**: Ensure that the `LastUpdatedString` and `OptionItems` are all provided.
> `OptionItems` parameter is in type of `CheckUpdatedOptionItemRequest`.

##### **CheckUpdatedOptionItemRequest Parameters**

| No | Parameter     | Description                                     |
|----|---------------|-------------------------------------------------|
| 1  | Id            | Id of option item (required).                   |
| 2  | LastUpdatedAt | The last update time of option item (required). |

> **Note**: Ensure that `LastUpdatedAt` is provides in string format.

### 1.2. Response (RoomOfBookingResponse)

**Change in facility**: Throw `FacilityHasChangesException`
**Change in site**: Throw `SiteHasChangesException`
**Change in plan**: Throw `PlanHasChangesException`
**Change in cancellation**: Throw `CancellationHasChangesException`
**Change in room group**: Throw `RoomGroupHasChangesException`
**Change in question**: Throw `QuestionHasChangesException`
**Change in file**: Throw `FileHasChangesException`
**Change in person age type**: Throw `PersonAgeTypeHasChangesException`
**No change happened**: Return **true**

## 2. Sequence Diagram
```mermaid
sequenceDiagram
  participant Client
  participant BookingEndpoint
  participant CheckChangedCommandHandler
  participant ICheckChangedService
  participant IOptionItemRepository
  participant IPlanRepository
  participant IRoomGroupRepository
  participant IFacilityRepository
  participant IPersonAgeTypeRepo as IPlanRoomGroupSitePersonAgeTypeRepository
  participant Database
  Client ->> BookingEndpoint: 1. Request Payload (CheckChangedRequest)
  BookingEndpoint ->> CheckChangedCommandHandler: 2. CheckChangedCommand (payload)
  CheckChangedCommandHandler ->> ICheckChangedService: 3. GetLastUpdatedAt(planId, roomGroupId)
  ICheckChangedService ->> IPlanRepository: 4 Query plan updated_at
  IPlanRepository ->> Database: 5 Query plan updated_at
  Database -->> IPlanRepository: 6 Return plan updated_at
  IPlanRepository -->> ICheckChangedService: 7 Return room group updated_at
  ICheckChangedService ->> IRoomGroupRepository: 8 Query room group updated_at
  IRoomGroupRepository ->> Database: 9 Query room group updated_at
  Database -->> IRoomGroupRepository: 10 Return room group updated_at
  IFacilityRepository -->> ICheckChangedService: 11 Return facility updated_at
  ICheckChangedService ->> IFacilityRepository: 12 Query facility updated_at
  IFacilityRepository ->> Database: 13 Query facility updated_at
  Database -->> IFacilityRepository: 14 Return facility updated_at
  IFacilityRepository -->> ICheckChangedService: 15 Return facility updated_at
  ICheckChangedService ->> IPersonAgeTypeRepo: 16 Query person age type updated_at
  IPersonAgeTypeRepo ->> Database: 17 Query person age type updated_at
  Database -->> IPersonAgeTypeRepo: 18 Return person age type updated_at
  IPersonAgeTypeRepo -->> ICheckChangedService: 19 Return person age type updated_at
  ICheckChangedService -->> CheckChangedCommandHandler: 20 Return LastUpdatedTimeOfBoookingModel
  CheckChangedCommandHandler ->> CheckChangedCommandHandler: 21 Parse payload LastUpdatedString to LastUpdatedTimeOfBoookingModel
  alt Parse Failed
    CheckChangedCommandHandler -->> BookingEndpoint: 22.1 Throw LastUpdateStringInvalidException
    BookingEndpoint --> Client: 23.1 Throw LastUpdateStringInvalidException
  else Parse Successfully
    CheckChangedCommandHandler ->> CheckChangedCommandHandler: 22.2 Compare last update time from payload and service
    alt Not equal
      CheckChangedCommandHandler -->> BookingEndpoint: 23.2 Throw change exception
      BookingEndpoint -->> Client: 23.2 Throw change exception
    else equal
      CheckChangedCommandHandler ->> CheckChangedCommandHandler: 23.3 Parse option's LastUpdatedString to long
      alt false
        CheckChangedCommandHandler -->> BookingEndpoint: 24.1 Throw OptionItemUpdatedTimeInvalidException
        BookingEndpoint -->> Client: 25.1 Throw OptionItemUpdatedTimeInvalidException
      else
        CheckChangedCommandHandler ->> CheckChangedCommandHandler: 24.2 Check option's LastUpdatedTime change
        alt changed
          CheckChangedCommandHandler -->> BookingEndpoint: 25.2 Throw OptionItemHasChangesException
          BookingEndpoint -->> Client: 26.1 Throw OptionItemUpdatedTimeInvalidException
        end
      end
    end
  end

  CheckChangedCommandHandler -->> BookingEndpoint: 25.3 Return true
  BookingEndpoint -->> Client: 26.2 Return true
```

### Description

| No.  | Activity                                                | Description                                                                                                  |
|------|---------------------------------------------------------|--------------------------------------------------------------------------------------------------------------|
| 1    | Client → BookingEndpoint                                | The client sends a `CheckChangedRequest` payload to the `BookingEndpoint`.                                   |
| 2    | BookingEndpoint → CheckChangedCommandHandler            | `BookingEndpoint` forwards the request as a `CheckChangedCommand` with the payload.                          |
| 3    | CheckChangedCommandHandler → ICheckChangedService       | The `CheckChangedCommandHandler` calls `GetLastUpdatedAt(planId, roomGroupId)`.                              |
| 4    | ICheckChangedService → IPlanRepository                  | The `ICheckChangedService` queries the `plan updated_at`.                                                    |
| 5    | IPlanRepository → Database                              | The `IPlanRepository` queries the database for `plan updated_at`.                                            |
| 6    | Database → IPlanRepository                              | The database returns the `plan updated_at` data.                                                             |
| 7    | IPlanRepository → ICheckChangedService                  | The `IPlanRepository` returns the `plan updated_at` data.                                                    |
| 8    | ICheckChangedService → IRoomGroupRepository             | The `ICheckChangedService` queries the `room group updated_at`.                                              |
| 9    | IRoomGroupRepository → Database                         | The `IRoomGroupRepository` queries the database for `room group updated_at`.                                 |
| 10   | Database → IRoomGroupRepository                         | The database returns the `room group updated_at` data.                                                       |
| 11   | IFacilityRepository → ICheckChangedService              | The `IFacilityRepository` returns the `facility updated_at` data.                                            |
| 12   | ICheckChangedService → IFacilityRepository              | The `ICheckChangedService` queries the `facility updated_at`.                                                |
| 13   | IFacilityRepository → Database                          | The `IFacilityRepository` queries the database for `facility updated_at`.                                    |
| 14   | Database → IFacilityRepository                          | The database returns the `facility updated_at` data.                                                         |
| 15   | IFacilityRepository → ICheckChangedService              | The `IFacilityRepository` returns the `facility updated_at` data.                                            |
| 16   | ICheckChangedService → IPersonAgeTypeRepo               | The `ICheckChangedService` queries the `person age type updated_at`.                                         |
| 17   | IPersonAgeTypeRepo → Database                           | The `IPersonAgeTypeRepo` queries the database for `person age type updated_at`.                              |
| 18   | Database → IPersonAgeTypeRepo                           | The database returns the `person age type updated_at` data.                                                  |
| 19   | IPersonAgeTypeRepo → ICheckChangedService               | The `IPersonAgeTypeRepo` returns the `person age type updated_at` data.                                      |
| 20   | ICheckChangedService → CheckChangedCommandHandler       | The `ICheckChangedService` returns `LastUpdatedTimeOfBoookingModel`.                                         |
| 21   | CheckChangedCommandHandler → CheckChangedCommandHandler | The `CheckChangedCommandHandler` parses the payload `LastUpdatedString` to `LastUpdatedTimeOfBoookingModel`. |
| 22.1 | CheckChangedCommandHandler → BookingEndpoint            | If parsing fails, throws `LastUpdateStringInvalidException`.                                                 |
| 23.1 | BookingEndpoint → Client                                | The `BookingEndpoint` returns `LastUpdateStringInvalidException`.                                            |
| 22.2 | CheckChangedCommandHandler → CheckChangedCommandHandler | If parsing succeeds, compares last update time from payload and service.                                     |
| 23.2 | CheckChangedCommandHandler → BookingEndpoint            | If times are not equal, throws `change exception`.                                                           |
| 23.2 | BookingEndpoint → Client                                | The `BookingEndpoint` returns `change exception`.                                                            |
| 23.3 | CheckChangedCommandHandler → CheckChangedCommandHandler | If times are equal, parses option’s `LastUpdatedString` to long.                                             |
| 24.1 | CheckChangedCommandHandler → BookingEndpoint            | If parsing fails, throws `OptionItemUpdatedTimeInvalidException`.                                            |
| 25.1 | BookingEndpoint → Client                                | The `BookingEndpoint` returns `OptionItemUpdatedTimeInvalidException`.                                       |
| 24.2 | CheckChangedCommandHandler → CheckChangedCommandHandler | If parsing succeeds, checks option’s `LastUpdatedTime` change.                                               |
| 25.2 | CheckChangedCommandHandler → BookingEndpoint            | If the option has changed, throws `OptionItemHasChangesException`.                                           |
| 26.1 | BookingEndpoint → Client                                | The `BookingEndpoint` returns `OptionItemUpdatedTimeInvalidException`.                                       |
| 25.3 | CheckChangedCommandHandler → BookingEndpoint            | If no changes were detected, returns `true`.                                                                 |
| 26.2 | BookingEndpoint → Client                                | The `BookingEndpoint` sends `true` back to the client.                                                       |
