# Detail Design - C004 SetRooms

------------------------------------------------------------------

## 1. Activity Diagram

```mermaid
---
title: SetRooms API Activity Diagram
---

flowchart TD
  A[Start] --> A1[Check Authentication - JWT]
  A1 -- valid --> B[Send request to api/site-controller/SetRooms KakusanEndpoint/SetRooms]
  A1 -- invalid --> E1[Return HTTP 401 Unauthorized] --> Z[End]

  B --> C[Invoke KakusanSetRoomsCommandHandler]
  C --> D[IsValidRequest?]

  D --> D1[Check if hotelId belongs to user - from JWT]
  D1 --> D2[Validate request data]

  D2 --> Decision{Both checks valid?}
  Decision -- No --> E[Return HTTP 400 Bad Request] --> Z
  Decision -- Yes --> F[HandleAsync]
  F --> G[Return data in XML format with HTTP 200 OK]
  G --> Z[End]
```

## 2. Sequence Diagram

### SetRooms API Sequence Diagram

```mermaid
sequenceDiagram
  participant Client
  participant KakusanEndpoint
  participant KakusanSetRoomsCommandHandler
  participant KakusanSetRoomsCommandValidator
  participant ICheckFacilityService
  participant IRepository
  participant Database

  Client ->>+ KakusanEndpoint: 1. Send request (GetRoomType)
  KakusanEndpoint ->> KakusanEndpoint: 2. Validate JWT

  alt JWT Invalid
    KakusanEndpoint -->> Client: 3. Return 401 Unauthorized
  else JWT Valid
    KakusanEndpoint ->>+ KakusanSetRoomsCommandHandler: 3.1 Pass request with user info from JWT
    KakusanSetRoomsCommandHandler ->>+ ICheckFacilityService: 3.2 Check if hotelId belongs to user
    ICheckFacilityService -->>- KakusanSetRoomsCommandHandler: 3.3 Return ownership check result

    KakusanSetRoomsCommandHandler ->>+ KakusanSetRoomsCommandValidator: 3.4 Validate request data
    KakusanSetRoomsCommandValidator -->>- KakusanSetRoomsCommandHandler: 3.5 Return validation result

    alt Validation Failed or Hotel Not Owned
      KakusanSetRoomsCommandHandler -->>- KakusanEndpoint: 4. Return 400 Bad Request
      KakusanEndpoint -->> Client: 5. Return 400 Bad Request
    else Valid
      KakusanSetRoomsCommandHandler ->>+ IRepository: 6. GetRoomTypeAsync
      IRepository ->>+ Database: 7. Query room type data
      Database -->>- IRepository: 8. Return data
      IRepository ->>- KakusanSetRoomsCommandHandler: 9. Return room type data
      KakusanSetRoomsCommandHandler ->> KakusanEndpoint: 10. Return XML response
      KakusanEndpoint ->> Client: 11. Return HTTP 200 OK + XML
    end
  end
```
