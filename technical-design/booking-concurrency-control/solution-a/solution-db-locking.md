# Booking Concurrency Control—Preventing Overbooking / Solution A: Using database lockin

- [Booking Concurrency Control—Preventing Overbooking / Solution A: Using database lockin](#booking-concurrency-controlpreventing-overbooking--solution-a-using-database-lockin)
  - [1. Database (Pessimistic Locking - using Transaction)](#1-database-pessimistic-locking---using-transaction)
    - [Lock Table](#lock-table)
    - [SQL Script](#sql-script)
    - [Advantages / Disadvantages](#advantages--disadvantages)
  - [2. Redis Lock (Distributed lock)](#2-redis-lock-distributed-lock)
    - [Using Redis Lock on total booking](#using-redis-lock-on-total-booking)
    - [️Implementation](#️implementation)
    - [Advantages/Disadvantages](#advantagesdisadvantages)

## 1. Database (Pessimistic Locking - using Transaction)

### Lock Table

- Create a table, for example, `RoomInventory`, which has only one row recording the total number of rooms left.
- When booking, use a transaction and lock the table or lock that single row.
- Other transactions, when reaching the step `SELECT ... FOR UPDATE`, will have to wait for the previous transaction to finish (serialization).
- Set timeout for Transaction (3 → 5s), when timeout return message to user  →  “Someone is booking now, please try again in a few seconds.”

### SQL Script

```postgresql
START TRANSACTION;
SELECT available_rooms FROM RoomInventory FOR UPDATE;

UPDATE RoomInventory SET available_rooms = available_rooms - 1 WHERE available_rooms > 0;
COMMIT;
```

### Advantages / Disadvantages

| Advantages                                     | Disadvantages                                                                                                                      |
|------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------|
| Absolutely ensures data safety                 | Easy to cause deadlock if not handled well; blocks all bookings                                                                    |
| Simple and easy to implement for small systems | If there are many concurrent requests, transactions will wait for each other (may timeout, reduces performance)                    |
|                                                | Using locks on the database is only suitable for small systems with low concurrency.                                               |
|                                                | The database cannot handle high loads and is prone to congestion, blocking, timeouts, and mass failures when there are many users. |

## 2. Redis Lock (Distributed lock)

### Using Redis Lock on total booking

- When there is a booking request:
  - Try to acquire the lock booking:global.
  - If the lock is acquired → Proceed with booking.
  - If the lock cannot be acquired → Return “Someone is booking now, please try again in a few seconds.”

### ️Implementation

- Can use LuaScript or the RedLock library.
- It is necessary to set an expiration time to release the lock.

### Advantages/Disadvantages

| Advantages                           | Disadvantages                                                                                         |
|--------------------------------------|-------------------------------------------------------------------------------------------------------|
| Absolutely guarantees no overbooking | The whole system allows only one booking at a time; at peak times, this causes congestion and retries |
| Simple and easy to implement         | User experience may “fail” often if many users try to book at the same time                           |
