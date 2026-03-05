# Booking Concurrency Control—Preventing Overbooking

- [Booking Concurrency Control—Preventing Overbooking](#booking-concurrency-controlpreventing-overbooking)
  - [1. Problem Overview](#1-problem-overview)
  - [2. Implementation of Each Solution](#2-implementation-of-each-solution)
    - [Solution A – Using PostgreSQL Locking (`SELECT FOR UPDATE`)](#solution-a--using-postgresql-locking-select-for-update)
    - [Solution B – Redis + Lua Script](#solution-b--redis--lua-script)
  - [3. Critical Issues with PostgreSQL Locking](#3-critical-issues-with-postgresql-locking)
    - [A. Key Limitations](#a-key-limitations)
    - [B. Additional Practical Risks](#b-additional-practical-risks)
  - [4. Comparison Table: PostgreSQL Lock vs Redis + Lua](#4-comparison-table-postgresql-lock-vs-redis--lua)
  - [5. Conclusion](#5-conclusion)
  - [6. Appendix: Redis Hold System Implementation (Details in Full Document)](#6-appendix-redis-hold-system-implementation-details-in-full-document)
    - [7. References](#7-references)

## 1. Problem Overview

In a Property Management System (PMS), overbooking can occur when multiple users try to reserve the same room simultaneously. To prevent
this, a robust concurrency control mechanism is needed to ensure only one user can successfully hold a booking at a time.

This document presents two main solutions to this problem:

- **Solution A**: Using **Database Locking via PostgreSQL `SELECT ... FOR UPDATE`**
- **Solution B**: Using **Redis with Lua script** for atomic hold control

---

## 2. Implementation of Each Solution

### Solution A – Using PostgreSQL Locking (`SELECT FOR UPDATE`)

This approach utilizes PostgreSQL row-level locking to ensure no two transactions can update the same room record concurrently.
[Details see more in full document](solution-a/solution-db-locking.md)

**Sample implementation:**

```csharp
public async Task<int> LockAsync(
    DbContext dbContext,
    long roomGroupId,
    long checkInDate,
    CancellationToken cancellationToken = default)
{
    var conn = dbContext.Database.GetDbConnection();
    var transaction = dbContext.Database.CurrentTransaction;
    const string sql = """
        SELECT id FROM room_group WHERE id = @RoomGroupId FOR UPDATE
    """;
    var sellNumber = await conn.QueryFirstOrDefaultAsync<int?>(
        sql, new { RoomGroupId = roomGroupId },
        transaction!.GetDbTransaction());

    return sellNumber ?? 0;
}
```

---

### Solution B – Redis + Lua Script

This solution uses Redis and Lua scripting to manage room hold logic atomically:

- Keys are created per room/date/user
- Lua scripts validate room availability and apply holds atomically
- Each hold is assigned a TTL (e.g., 5 minutes) and expires automatically if unused
- If the user does not complete payment, the hold automatically expires

**Details include:**

- Implementation of `TryHoldRoomAsync`, `ReleaseHoldAsync`, and Lua scripts
- Key structure: `globalHoldKey` and `userHoldKey`
- TTL mechanism and partial release
- Testing using k6 for concurrent scenarios (e.g., 10 users attempting 1 room)

---

## 3. Critical Issues with PostgreSQL Locking

### A. Key Limitations

1. **Connection Limit**:

- PostgreSQL typically supports around 200 max connections
- In microservice systems with 10+ services, booking service may only get 10–20 connections
- If each booking request creates a new connection:
  - It’s easy to exceed the limit during high concurrency.
  - This leads to “too many connections” errors, potentially freezing the whole system.

2. **Lock Escalation**:

- Holding multiple room/date combinations can escalate to table-level locks, blocking other services:
  - When holding multiple bookings at once → locks multiple rows.
  - PostgreSQL may escalate to a full table lock.
  - This can cause deadlocks or block other services (like reporting or admin tasks).

3. **High Latency**:

- PostgreSQL is disk-based and inherently slower than Redis (memory-based)
- Requests can queue and time out under contention

### B. Additional Practical Risks

4. **No Fail-Fast Behavior**:

- Requests are blocked waiting for locks, leading to delayed failure or timeout

5. **Lack of Horizontal Scalability**:

- PostgreSQL has a single write-master → not horizontally scalable

6. **No Auto-Cleanup/TTL**:

- Manual cleanup of abandoned holds is required

7. **No Native Multi-Hold per User Support**:

- Requires custom logic to distinguish between multiple holds from the same user

8. **Incapable of Handling Burst Traffic**:

- 1000 simultaneous requests for the same room will overwhelm PostgreSQL

9. **Difficult to Test and Debug Lock Failures**:

- When locks deadlock, block, or wait, it’s hard to reproduce during testing.
- Redis with logs + TTL makes tracing issues much easier.

---

## 4. Comparison Table: PostgreSQL Lock vs Redis + Lua

| Criteria                     | PostgreSQL Locking        | Redis + Lua Script             |
|------------------------------|---------------------------|--------------------------------|
| Atomicity                    | ✅ Yes                    | ✅ Yes (Lua)                   |
| Auto-Cleanup                 | ❌ No                     | ✅ Yes (via TTL)               |
| Developer Experience         | ⚠️ Complex (transactions) | ✅ Simple (script + wrapper)   |
| Fail-Fast Capability         | ❌ No                     | ✅ Yes                         |
| High-Concurrency Performance | ❌ Poor                   | ✅ Excellent (>100K ops/s)     |
| Infra Cost                   | ✅ Low (if small)         | ✅ Low (if Redis in use)       |
| Multi-hold Support           | ❌ Difficult              | ✅ Easy via bookingTempCode    |
| Operational Complexity       | ⚠️ Medium-High            | ✅ Low                         |
| Scalability                  | ❌ Limited                | ✅ Scalable with Redis Cluster |
| TTL Support                  | ❌ No                     | ✅ Built-in                    |

---

## 5. Conclusion

While using PostgreSQL `SELECT FOR UPDATE` can be technically feasible, it becomes impractical in real-world deployments involving:

- High concurrency
- Limited DB connections
- Multi-service environments
- Low-latency expectations

**Redis + Lua script is a proven solution used by global-scale platforms like Agoda, Booking.com, and Airbnb**. It provides:

- Fast atomic operations
- TTL for auto-release
- Horizontal scalability
- Lightweight infrastructure cost

> **Recommendation**: Use Redis for the hold phase in concurrency control. Reserve PostgreSQL for final confirmation after payment.

---

## 6. Appendix: Redis Hold System Implementation (Details in [Full Document](solution-b/solution-redis-atomic-hold-layer.md))

- Key components: `TryHoldRoomAsync`, `ReleaseHoldAsync`, Lua scripts
- TTL and automatic expiration
- k6 stress test for concurrency
- Redis logs and debug strategy
- Enhancements: partial release, idempotency, Redis hash tags
- Key structure: `globalHoldKey`, `userHoldKey`

> The complete technical implementation is included in the second half of this document.

---

### 7. References

- [Concurrency Conundrum in Booking Systems](https://medium.com/%40abhishekranjandev/concurrency-conundrum-in-booking-systems-2e53dc717e8c)
- [System Design concept: Locking ticket/seat for Booking.com like apps (Very crisp)](https://medium.com/%40isha5415657/system-design-concept-locking-ticket-seat-very-crisp-951316e4f6cf)
- [Notable Organizations Using Redis](https://sealos.io/blog/what-is-redis?utm_source=chatgpt.com#notable-organizations-using-redi)
- [Hands-on Preventing Database Race Conditions with Redis](https://iniakunhuda.medium.com/hands-on-preventing-database-race-conditions-with-redis-2c94453c1e47)
- [Performance Considerations Between Lua Scripts and Basic Commands in Redis for Cloud Applications](https://www.linkedin.com/pulse/performance-considerations-between-lua-scripts-basic-commands-corder-ac4mc?utm_source=chatgpt.co)
