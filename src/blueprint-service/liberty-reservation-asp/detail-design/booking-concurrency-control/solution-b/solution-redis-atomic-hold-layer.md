# Booking Concurrency Control—Preventing Overbooking / Solution B: Redis Atomic Hold Layer

- [Booking Concurrency Control—Preventing Overbooking / Solution B: Redis Atomic Hold Layer](#booking-concurrency-controlpreventing-overbooking--solution-b-redis-atomic-hold-layer)
  - [1. Activity Diagram](#1-activity-diagram)
  - [2. A room holding system is designed to](#2-a-room-holding-system-is-designed-to)
  - [3. Technologies Used](#3-technologies-used)
    - [3.1. TryHoldRoomAsync](#31-tryholdroomasync)
      - [Purpose: TryHoldRoomAsync](#purpose-tryholdroomasync)
      - [Usage context (TryHoldRoomAsync)](#usage-context-tryholdroomasync)
      - [Detailed logic (TryHoldRoomAsync)](#detailed-logic-tryholdroomasync)
      - [Lua Script](#lua-script)
      - [How the Lua Script Works](#how-the-lua-script-works)
      - [Result (TryHoldRoomAsync)](#result-tryholdroomasync)
    - [3.2. ReleaseHoldAsync](#32-releaseholdasync)
      - [Purpose: ReleaseHoldAsync](#purpose-releaseholdasync)
      - [Usage context (ReleaseHoldAsync)](#usage-context-releaseholdasync)
      - [Detailed logic](#detailed-logic)
      - [Script](#script)
      - [Description](#description)
      - [Result (ReleaseHoldAsync)](#result-releaseholdasync)
  - [4. Solution and Mechanism for Cache Key Generation](#4-solution-and-mechanism-for-cache-key-generation)
    - [4.1. Objectives of the Cache Key System](#41-objectives-of-the-cache-key-system)
    - [4.2. Main Types of Cache Keys](#42-main-types-of-cache-keys)
    - [4.3. Overall Operating Mechanism](#43-overall-operating-mechanism)
    - [4.4. TTL and Automatic Release Mechanism](#44-ttl-and-automatic-release-mechanism)
    - [4.5. Advantages of the Solution](#45-advantages-of-the-solution)
    - [4.6. Example Summary](#46-example-summary)
  - [5. Testing with k6](#5-testing-with-k6)
    - [Purpose](#purpose)
    - [Test Scenario](#test-scenario)
    - [Prerequisites](#prerequisites)
    - [k6 Test Script](#k6-test-script)
    - [Running the Test](#running-the-test)
    - [Analyzing Results](#analyzing-results)
    - [Advantages of k6 Testing](#advantages-of-k6-testing)
  - [6. Suggested Enhancements and Improvements](#6-suggested-enhancements-and-improvements)
    - [6.1. Idempotency Handling for Repeated Holds](#61-idempotency-handling-for-repeated-holds)
    - [6.2. Enhanced Lua Script: Partial Release Support](#62-enhanced-lua-script-partial-release-support)
    - [6.3. Redis Key HashTagging for Cluster Compatibility](#63-redis-key-hashtagging-for-cluster-compatibility)
    - [6.4. Audit Logging](#64-audit-logging)
    - [6.5. Finalize Hold After Payment](#65-finalize-hold-after-payment)

## 1. Activity Diagram

![img.png](../../../docs/images/Liberty%20SystemArchitect-Booking%20flow%20-%20Overview.drawio.png)

## 2. A room holding system is designed to

- **Prevent overbooking** when multiple users/customers attempt to book the same room at the same time.
- **Temporarily hold rooms** when a user initiates the payment process.
- **Release held rooms** when the user cancels, the payment fails, or the hold time expires.

## 3. Technologies Used

| Technology              | Purpose                                                                                                            |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------ |
| **Redis**               | High-speed in-memory database used to store temporary room hold states.                                            |
| **Lua script in Redis** | Ensures synchronous logic execution, prevents race conditions, and enhances performance through atomic operations. |

### 3.1. TryHoldRoomAsync

#### Purpose: TryHoldRoomAsync

The `TryHoldRoomAsync` method in C# is responsible for invoking a Redis Lua script to hold rooms, applied across multiple consecutive dates, and ensuring data is validated before proceeding with the hold.

#### Usage context (TryHoldRoomAsync)

- Uses a Redis script to call `BookingHoldRoom.lua`.
- Applied to booking data from the user.
- Retrieves availability (`GetAvailabilityForUserAsync`)  before actually holding the rooms.

#### Detailed logic (TryHoldRoomAsync)

1. Calls `GetAvailabilityForUserAsync()` to retrieve the number of rooms available for each date.
2. Verifies that all dates have sufficient availability (`min >= NumberOfRooms`).
3. Constructs Redis keys including:

- `globalHoldKey`:  total rooms held globally for a given date.
- `userHoldKey`: number of rooms held by a specific user for a date.

4. Prepares Redis arguments:


- `NumberOfRooms`: the number of rooms to hold
- `TTL`: time-to-live for the hold.
- `n`: number of dates.
- `[maxPerDay]`: max number of rooms available for each date.

5. Calls `Redis.ScriptEvaluateAsync(...)` with the `BookingHoldRoom.lua` script.

#### Lua Script

```lua
local quantity = tonumber(ARGV[1])
local ttl = tonumber(ARGV[2])
local n = tonumber(ARGV[3])

-- Debug: Total number of handling days
redis.log(redis.LOG_NOTICE, "=== Start BookingHoldRoom ===")
redis.log(redis.LOG_NOTICE, "Request: quantity=" .. quantity .. ", ttl=" .. ttl .. ", nights=" .. n)

-- 1. VALIDATION PHASE
for i = 1, n do
  local globalHoldKey = KEYS[(i - 1) * 2 + 1]
  local userHoldKey = KEYS[(i - 1) * 2 + 2]
  local currentGlobal = tonumber(redis.call('get', globalHoldKey) or '0')
  local max = tonumber(ARGV[3 + i])

  redis.log(redis.LOG_NOTICE, string.format("[Check] globalHoldKey=%s, current=%d, max=%d, quantity=%d",
    globalHoldKey, currentGlobal, max, quantity))

  if (currentGlobal + quantity) > max then
    redis.log(redis.LOG_WARNING, "[Rejected] Not enough room on " .. globalHoldKey)
    return 0
  end
end

-- 2. APPLY PHASE (write hold keys)
for i = 1, n do
  local globalHoldKey = KEYS[(i - 1) * 2 + 1]
  local userHoldKey = KEYS[(i - 1) * 2 + 2]

  -- Update global key
  redis.call('incrby', globalHoldKey, quantity)
  redis.call('expire', globalHoldKey, ttl)

  -- Update user-specific hold key
  redis.call('set', userHoldKey, quantity, 'EX', ttl)

  redis.log(redis.LOG_NOTICE, string.format("[Held] globalHoldKey=%s, userHoldKey=%s, quantity=%d, ttl=%d",
    globalHoldKey, userHoldKey, quantity, ttl))
end

redis.log(redis.LOG_NOTICE, "=== BookingHoldRoom Completed Successfully ===")
return 1
```

#### How the Lua Script Works

- Iterate through each day from 1 to n.
- Retrieve the number of rooms currently held (currentGlobal) for that day.
- Compare it with the maximum allowed — if it exceeds the limit, return 0 (failure).
- Log detailed information if there are not enough rooms available.

#### Result (TryHoldRoomAsync)

- Returns `true` if the room hold is successful (`result == 1`).
- Returns `false` if there is not enough availability or a validation error occurs (`result == 0`).

### 3.2. ReleaseHoldAsync

#### Purpose: ReleaseHoldAsync

The `ReleaseHoldAsync` method in C# is Cancel the room hold if the user does not complete the payment or perform a release after a successful booking.

#### Usage context (ReleaseHoldAsync)

- Uses a Redis script to call `ReleaseHoldRoom.lua`.
- Applied to booking data from the user.

#### Detailed logic

1. Retrieve the list of dates and the number of rooms held based on the provided input data.
2. Constructs Redis keys including:

- `globalHoldKey`:  total rooms held globally for a given date.
- `userHoldKey`: number of rooms held by a specific user for a date.

3. Prepares Redis arguments:

- `NumberOfRooms`: the number of rooms to hold
- `dateList.Count`: Total dates

4. Calls `Redis.ScriptEvaluateAsync(...)` with the `ReleaseHoldRoom.lua` script.

#### Script

```lua
local quantity = tonumber(ARGV[1])
local n = tonumber(ARGV[2])

for i = 1, n do
  local globalKey = KEYS[(i - 1) * 2 + 1]
  local userKey = KEYS[(i - 1) * 2 + 2]

  -- Check current holds
  local currentGlobal = tonumber(redis.call('get', globalKey) or '0')
  local currentUser = tonumber(redis.call('get', userKey) or '0')

  if currentGlobal < quantity then
    return redis.error_reply('Global hold quantity is less than release quantity for ' .. globalKey)
  end

  if currentUser < quantity then
    return redis.error_reply('User hold quantity is less than release quantity for ' .. userKey)
  end
end

for i = 1, n do
  local globalKey = KEYS[(i - 1) * 2 + 1]
  local userKey = KEYS[(i - 1) * 2 + 2]

  -- Decrease or delete global hold
  local currentGlobal = tonumber(redis.call('get', globalKey) or '0')
  if currentGlobal == quantity then
    redis.call('del', globalKey)
  else
    redis.call('decrby', globalKey, quantity)
  end

  -- Decrease or delete user hold
  local currentUser = tonumber(redis.call('get', userKey) or '0')
  if currentUser == quantity then
    redis.call('del', userKey)
  else
    redis.call('decrby', userKey, quantity)
  end
end

return 1
```

#### Description

- Both the global hold and the user hold have sufficient quantity to release.
- If not enough, immediately return an error.
- Decrease the number of held tickets.
- If the remaining hold count becomes 0 → delete the Redis key to save memory.

#### Result (ReleaseHoldAsync)

- Returns `true` if the room release is successful (`result == 1`).

## 4. Solution and Mechanism for Cache Key Generation

### 4.1. Objectives of the Cache Key System

The cache key system is designed to:

- Manage the number of rooms currently held by date, user, plan, and room group.
- Ensure consistency and atomicity when multiple users hold rooms simultaneously.
- Optimize performance and reduce database load by using Redis as a temporary cache store.

### 4.2. Main Types of Cache Keys

1. globalHoldKey (Total rooms currently held)

- Purpose: Track the total number of rooms held for a specific date, regardless of user.

- Format:

  Fac{facilityId}:Site{siteId}:Plan{planId}:Room{roomId}\:D{appDateId}:BkgHold

  Example: Fac1:Site2:Plan5:Room8:D20240515:BkgHold

- Creation Mechanism: Generated in the GetBookingHoldKey() method.
  Called by Lua scripts when holding or releasing rooms.

1. userHoldKey (Number of rooms held by a specific user)

- Purpose: Track the number of rooms held by a specific user for each date.

- Format:

  Fac{facilityId}:Site{siteId}:Plan{planId}:Room{roomId}\:D{appDateId}:BkgHold:Usr{userCode}:{bookingTempCode}

**Example**: `Fac1:Site2:Plan5:Room8:D20240515:BkgHold:UsrABC123:TMP999`

- **Creation Mechanism**: Generated in the `GetUserHoldKey()` method, based on the user code (`UserCode`) and temporary booking code (`BookingTempCode`).
- **BookingTempCode Explanation**:
- The `BookingTempCode` is a unique identifier generated for each hold attempt to ensure that multiple hold requests by the same user for the same room group do not reuse the same Redis `userHoldKey`.
- It is typically a 32-character hexadecimal string created using `Guid.NewGuid().ToString("N")` in C#, guaranteeing uniqueness with negligible collision risk.
- This prevents unintended TTL (time-to-live) renewal of previous holds, as each new hold attempt creates a distinct `userHoldKey` with its own TTL, allowing earlier holds to expire independently.
- Example: For two hold attempts by user `UsrABC123`, the `BookingTempCode` might be `a8c9f2d67e32434bafbc8f5b620d197c` for the first attempt and `b7d8e3c56f21423a9eab7g4c730e286d` for the second, resulting in separate keys like:

  ```txt
  Fac1:Site2:Plan5:Room8:D20240515:BkgHold:UsrABC123:a8c9f2d67e32434bafbc8f5b620d197c
  Fac1:Site2:Plan5:Room8:D20240515:BkgHold:UsrABC123:b7d8e3c56f21423a9eab7g4c730e286d
  ```

---

### 4.3. Overall Operating Mechanism

|Action                                               | Lua Script              | Affected Keys                | Notes                                       |
|---------------------------------------------------- | ----------------------- | -----------------------------| --------------------------------------------|
|Hold rooms (TryHoldRoomAsync)                        | BookingHoldRoom.lua     | globalHoldKey, userHoldKey   | Increases hold count, sets TTL              |
|Release hold (ReleaseHoldAsync)                      | BookingReleaseHold.lua  | globalHoldKey, userHoldKey   | Decreases hold count, deletes key if zero   |
|Get current hold count (GetHeldQuantityForDateAsync) | BookingSumHold.lua      | globalHoldKey:*              | Uses Redis pattern matching to sum totals   |

---

### 4.4. TTL and Automatic Release Mechanism

- TTL (Time to Live):
  Each key is set with a TTL (default 300 seconds) immediately after holding the room.

- Benefits:

  - Automatically releases keys when users do not complete payment.
  - Prevents stale or "orphaned" holds from lingering in Redis.

- Set via: Lua script with the Redis expire command.

---

### 4.5. Advantages of the Solution

- Atomic and race-condition free thanks to Redis Lua scripting.
- Separate holds per user for accurate booking confirmation display.
- Automatic cleanup of Redis resources after TTL expires.
- Easy scalability to extend for other room types, add-on services, etc.

---

### 4.6. Example Summary

User USR123 wants to hold 2 rooms from 2025-05-15 to 2025-05-16

Generated keys:

Fac1:Site1:Plan10:Room22:D20250515:BkgHold
Fac1:Site1:Plan10:Room22:D20250515:BkgHold:UsrUSR123:a12b3c4d5e6f7890a1b2c3d4e5f67890
Fac1:Site1:Plan10:Room22:D20250516:BkgHold
Fac1:Site1:Plan10:Room22:D20250516:BkgHold:UsrUSR123:3f5a8c8e1d2f4a2c8e9e8d7d4e0b2a5c

After TTL expires or the user cancels:

All keys above will be deleted from Redis.

## 5. Testing with k6

### Purpose

The purpose of performance testing with **k6** is to simulate high-concurrency booking scenarios to verify that the room holding system prevents overbooking. Specifically, this test ensures that when **10 users simultaneously attempt to book the last available room**, only **one user** successfully holds the room, while the others receive appropriate failure responses, maintaining system integrity and fairness.

### Test Scenario

This test simulates **10 virtual users (VUs)** concurrently sending booking requests for a single available room. The system should:

- Allow only **one user** to successfully hold the room.
- Return failure responses (e.g., HTTP 400 or 409) for the other 9 users due to insufficient availability.
- Maintain atomicity and prevent race conditions using Redis and Lua scripts (as described in Section 3).

### Prerequisites

Before running the test, ensure the following:

- **k6 Installed**: Install k6 on your system. Download from [k6.io](https://k6.io/docs/getting-started/installation/) or use:

  ```bash
  brew install k6        # macOS
  choco install k6       # Windows
  sudo apt-get install k6 # Linux
  ```

- **Setup Steps**:
  Follow these steps to prepare the environment for the k6 script (`script.js`):

1. **Obtain the Booking API URL**:

- The k6 script uses the URL `http://localhost:7088/api/booking/plans/75/rooms/64?api-version=1` to target the booking API for `PlanId: 75`, `RoomId: 64`.
- Ensure the API server is running locally on port `7088`. If the server is hosted elsewhere (e.g., a staging server), obtain the correct URL from the API documentation or development team.
- Update the `url` variable in `script.js` if necessary:

    ```javascript
    const url = 'http://your-server:7088/api/booking/plans/75/rooms/64?api-version=1';
    ```

- Verify the endpoint is accessible by sending a test request:

    ```bash
    curl http://localhost:7088/api/booking/plans/75/rooms/64?api-version=1
    ```

    A response (even an error) confirms the server is reachable. If it fails, check the server status or consult the API team.

2. **Create headers.json with Headers**:

- The k6 script loads headers from `headers.json`to configure API requests.
- Obtain the required headers from the API documentation or a sample request (e.g., via Postman or a previous successful request). Common headers include `x-facility-code`, `x-site-code`, and `content-type`.
- Create a file named `headers.json` in the same directory as `script.js` with the following content:

    ```json
    {
      "accept": "application/json, text/plain, */*",
      "accept-language": "en-US",
      "access_code": "",
      "content-type": "application/json-patch+json",
      "dnt": "1",
      "origin": "https://stage.site.reservation.liberty-service.com",
      "priority": "u=1, i",
      "referer": "https://stage.site.reservation.liberty-service.com/",
      "sec-ch-ua": "\"Not:A-Brand\";v=\"24\", \"Chromium\";v=\"134\"",
      "sec-ch-ua-mobile": "?0",
      "sec-ch-ua-platform": "\"macOS\"",
      "sec-fetch-dest": "empty",
      "sec-fetch-mode": "cors",
      "sec-fetch-site": "same-site",
      "time-zone-offset": "7",
      "user-agent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36",
      "x-facility-code": "facility-acaae1f8-85cd-442f-b400-bc59190f1eb2",
      "x-site-code": "a8dbd27bd3084cf8a26b23abfe80287c"
    }
    ```

- Save `headers.json`. If using a different file path, specify it
- Validate headers with the API team to ensure `x-facility-code` and `x-site-code` match the test environment.

3. **Create payload.json with Payload**:

- The k6 script loads the payload from `payload.json` to define the booking request for 1 room (`PlanId: 75`, `RoomId: 64`, `2025-05-28`).
- Obtain the payload structure from the API documentation or a sample booking request. It must include fields like `checkInDate`, `adjust`, and `planQuestions`.
- Create a file named `payload.json` in the same directory as `script.js` with the following content:

    ```json
    {
      "checkInDate": 20250528,
      "checkInTime": "00:10:00",
      "paymentType": "OnSidePayment",
      "adjust": {
        "isAgree": true,
        "checkInTime": "00:10:00",
        "numberOfNights": 1,
        "numberOfRooms": 1,
        "freeInput": "",
        "reserver": {
          "fullName": "test-1",
          "kana": "aasd",
          "email": "hieudm@yopaz.dev",
          "postCode": "123",
          "address1": "青森県",
          "address2": "123",
          "address3": "asd",
          "phoneNumber": "1231454624",
          "countryCode": "AG"
        },
        "nightPeoples": [
          {
            "appDateId": 20250528,
            "rooms": [
              {
                "roomIndex": 0,
                "peoples": [
                  {
                    "personAgeTypeId": 25,
                    "numberOfPeoples": 1,
                    "gender": "Female"
                  },
                  {
                    "personAgeTypeId": 25,
                    "numberOfPeoples": 1,
                    "gender": "Male"
                  }
                ]
              }
            ]
          }
        ],
        "nightOptions": [
          {
            "appDateId": 20250528,
            "rooms": [
              {
                "roomIndex": 0,
                "optionItems": []
              }
            ]
          }
        ],
        "roomRepresentatives": null,
        "id": 0,
        "checkInDateId": 20250528
      },
      "planQuestions": [
        {
          "questionId": 6,
          "answerData": "3"
        },
        {
          "questionId": 7,
          "answerData": "2"
        },
        {
          "questionId": 8,
          "answerData": "4"
        },
        {
          "questionId": 9,
          "answerData": "abc\nabc\nabc"
        },
        {
          "questionId": 10,
          "answerData": "abc\nabc\nabc"
        }
      ],
      "optionsQuestions": [],
      "bookingDate": "2025-05-15T09:59:03.783Z"
    }
    ```

- **Room Availability**: Configure the system to have **exactly 1 room available** for the target room group (e.g., `PlanId: 69`, `RoomId: 51`) on the test date (e.g., `2025-05-15`).

### k6 Test Script

The following k6 script simulates 10 virtual users attempting to book 1 room concurrently. The script sends HTTP POST requests to the booking API, checks for successful responses, and tracks errors.

```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Custom metric to track error rate (requests with status != 200)
export const errorRate = new Rate('errors');

export const options = {
  vus: 10,           // Number of virtual users running concurrently
  iterations: 10,    // Total iterations per VU (each user sends 1 request)
  thresholds: {
    // 95% of requests must complete within 20 seconds
    http_req_duration: ['p(95)<20000'],

    // Expect error rate to be between 89% and 91%
    // Because only 1 out of 10 requests can succeed when 1 room is available
    http_req_failed: ['rate > 0.89', 'rate < 0.91'],
  },
};

// API endpoint to test
const url = 'http://localhost:7088/api/booking/plans/75/rooms/64?api-version=1';

// Load payload and headers from external JSON files for easy modification
const payloadRaw = open('payload.json');
const headersRaw = open('headers.json');

const payload = JSON.parse(payloadRaw);
const headers = JSON.parse(headersRaw);

export default function () {
  // Send POST request to the API with provided payload and headers
  let res = http.post(url, JSON.stringify(payload), { headers });

  // Check if the response status is 200 OK
  // If not, mark this request as an error
  check(res, {
    'is status 200': (r) => r.status === 200,
  }) || errorRate.add(1);

  // Pause for 0.3 seconds between iterations to simulate realistic user behavior
  sleep(0.3);
}
```

### Running the Test

1. Save the script as `script.js`.
2. Execute the script using k6:

   ```bash
   k6 run script.js
   ```

3. Expected output:

- k6 will display a summary of the test, including:
  - **HTTP request duration**: Time taken for each request.
  - **Success rate**: Number of HTTP 200 responses (should be ~1/10).
  - **Error rate**: Number of non-200 responses (should be ~9/10 due to insufficient rooms).
- Example output:

    ```txt
    ✓ is status 200: 1/10 (10%)
    ✗ errors: 9/10 (90%)

    http_req_duration........: avg=150ms p(95)=250ms
    http_req_failed..........: 0.00%
    errors...................: 90.00%
    ```

### Analyzing Results

- **Success Criteria**:
  - Exactly **1 user** receives HTTP 200, indicating a successful hold.
  - **9 users** receive non-200 responses (e.g., HTTP 400 or 409), indicating insufficient room availability.
  - Redis should show:
    - One `userHoldKey` with a value of `1` (e.g., `redis_instance:Fac1:Site1:Plan69:Room51:D20250515:BkgHold:UsrTest-TVV-<random>:TMP999`).
    - One `globalHoldKey` with a value of `1` (e.g., `redis_instance:Fac1:Site1:Plan69:Room51:D20250515:BkgHold`).
  - No unexpected errors (e.g., HTTP 500).
- **Failure Cases**:
  - **Multiple successes**: If more than one user receives HTTP 200, the system has failed to prevent overbooking, indicating a race condition.
  - **No successes**: If all requests fail, check room inventory or API configuration.
  - **Unexpected errors**: HTTP 500 or timeouts suggest server or Redis issues.
- **Debugging**:
  - Check Redis logs for Lua script execution details (e.g., `BookingHoldRoom.lua` logs).
  - Inspect API response bodies for error details (e.g., `"message": "Insufficient rooms available"`).

### Advantages of k6 Testing

- Validates system behavior under high-concurrency scenarios.
- Ensures atomic operations (via Redis Lua scripts) prevent overbooking.
- Provides clear metrics for success, failure, and performance.
- Facilitates debugging with detailed logging and response checks.

---

## 6. Suggested Enhancements and Improvements

### 6.1. Idempotency Handling for Repeated Holds

To prevent a single user from repeatedly holding rooms (causing resource starvation), implement a cache-based lock using Redis:

- Before holding a room, check if a similar `userHoldKey` (with same user and date) already exists.
- If it exists and TTL has not expired, reject new holds or return success if the quantity matches.

### 6.2. Enhanced Lua Script: Partial Release Support

To allow partial release (releasing fewer rooms than held), update `ReleaseHoldRoom.lua` as follows:

```lua
-- Partial release supported Lua script
local quantity = tonumber(ARGV[1])
local n = tonumber(ARGV[2])

for i = 1, n do
  local globalKey = KEYS[(i - 1) * 2 + 1]
  local userKey = KEYS[(i - 1) * 2 + 2]

  local currentGlobal = tonumber(redis.call('get', globalKey) or '0')
  local currentUser = tonumber(redis.call('get', userKey) or '0')

  if currentGlobal < quantity then
    return redis.error_reply('Global hold < release quantity for ' .. globalKey)
  end
  if currentUser < quantity then
    return redis.error_reply('User hold < release quantity for ' .. userKey)
  end
end

for i = 1, n do
  local globalKey = KEYS[(i - 1) * 2 + 1]
  local userKey = KEYS[(i - 1) * 2 + 2]

  redis.call('decrby', globalKey, quantity)
  redis.call('decrby', userKey, quantity)

  if tonumber(redis.call('get', globalKey)) <= 0 then redis.call('del', globalKey) end
  if tonumber(redis.call('get', userKey)) <= 0 then redis.call('del', userKey) end
end

return 1
```

### 6.3. Redis Key HashTagging for Cluster Compatibility

Ensure all Redis keys are grouped in the same hash slot by using hash tags:

- Instead of:
  `Fac1:Site2:Plan5:Room8:D20240515:BkgHold`
- Use:
  `{Fac1:Site2:Plan5:Room8:D20240515}:BkgHold`

This ensures that all keys sharing the same `{...}` part are stored in the same Redis slot, essential for Redis Cluster mode.

### 6.4. Audit Logging

For better observability and debugging:

- Log every `TryHoldRoom` and `ReleaseHoldRoom` request in a persistent store (DB or ELK).
- Suggested structure:

  ```json
  {
    "timestamp": "2025-05-18T10:03:21Z",
    "action": "TryHoldRoom",
    "userCode": "USR123",
    "room": "Room22",
    "dates": ["2025-05-15", "2025-05-16"],
    "quantity": 2,
    "result": "Success"
  }
  ```

### 6.5. Finalize Hold After Payment

To prevent inconsistency (payment success but booking DB insert fails):

- After successful payment, verify hold again using Redis before confirming booking.
- Or introduce `FinalizeHoldRoom` API to mark the hold as committed (and optionally remove the temporary hold).
