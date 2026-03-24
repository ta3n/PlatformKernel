local reservationKey = KEYS[1]
local expiringReservationsKey = KEYS[2]

local retentionTtl = tonumber(ARGV[1])
local confirmedAt = tonumber(ARGV[2])

local status = redis.call('HGET', reservationKey, 'status')

if not status then
    return cjson.encode({
        code = 'NOT_FOUND',
        status = 'missing'
    })
end

local reservationId = redis.call('HGET', reservationKey, 'reservationId')
local inventoryId = redis.call('HGET', reservationKey, 'inventoryId')
local expiresAtUnix = tonumber(redis.call('HGET', reservationKey, 'expiresAtUnix') or '0')

if status == 'confirmed' then
    return cjson.encode({
        code = 'EXISTS',
        status = status,
        reservationId = reservationId,
        inventoryId = inventoryId,
        expiresAtUnix = expiresAtUnix
    })
end

if status == 'released' then
    return cjson.encode({
        code = 'ALREADY_RELEASED',
        status = status,
        reservationId = reservationId,
        inventoryId = inventoryId,
        expiresAtUnix = expiresAtUnix
    })
end

if expiresAtUnix > 0 and confirmedAt > expiresAtUnix then
    return cjson.encode({
        code = 'EXPIRED',
        status = 'expired',
        reservationId = reservationId,
        inventoryId = inventoryId,
        expiresAtUnix = expiresAtUnix
    })
end

redis.call(
    'HSET',
    reservationKey,
    'status', 'confirmed',
    'confirmedAtUnix', confirmedAt
)

redis.call('EXPIRE', reservationKey, retentionTtl)
redis.call('ZREM', expiringReservationsKey, reservationId)

return cjson.encode({
    code = 'CONFIRMED',
    status = 'confirmed',
    reservationId = reservationId,
    inventoryId = inventoryId,
    expiresAtUnix = expiresAtUnix
})
