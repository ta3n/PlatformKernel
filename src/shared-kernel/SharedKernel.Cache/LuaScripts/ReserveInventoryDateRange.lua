local reservationKey = KEYS[#KEYS - 1]
local expiringReservationsKey = KEYS[#KEYS]
local dateKeyCount = #KEYS - 2

local reservationId = ARGV[1]
local inventoryId = ARGV[2]
local quantity = tonumber(ARGV[3])
local holdTtl = tonumber(ARGV[4])
local dataTtl = tonumber(ARGV[5])
local now = tonumber(ARGV[6])

local existingStatus = redis.call('HGET', reservationKey, 'status')
if existingStatus then
    return cjson.encode({
        code = 'EXISTS',
        status = existingStatus,
        reservationId = redis.call('HGET', reservationKey, 'reservationId'),
        inventoryId = redis.call('HGET', reservationKey, 'inventoryId'),
        expiresAtUnix = tonumber(redis.call('HGET', reservationKey, 'expiresAtUnix') or '0')
    })
end

local dates = {}

for index = 1, dateKeyCount do
    dates[index] = ARGV[6 + index]
    local available = tonumber(redis.call('GET', KEYS[index]) or '0')

    if available < quantity then
        return cjson.encode({
            code = 'INSUFFICIENT',
            status = 'rejected',
            reservationId = reservationId,
            inventoryId = inventoryId,
            failedDate = dates[index],
            available = available
        })
    end
end

for index = 1, dateKeyCount do
    redis.call('DECRBY', KEYS[index], quantity)
end

local expiresAt = now + holdTtl

redis.call(
    'HSET',
    reservationKey,
    'reservationId', reservationId,
    'inventoryId', inventoryId,
    'quantity', quantity,
    'status', 'reserved',
    'dates', table.concat(dates, ','),
    'stockKeys', table.concat(KEYS, ',', 1, dateKeyCount),
    'createdAtUnix', now,
    'expiresAtUnix', expiresAt
)

redis.call('EXPIRE', reservationKey, dataTtl)
redis.call('ZADD', expiringReservationsKey, expiresAt, reservationId)

return cjson.encode({
    code = 'RESERVED',
    status = 'reserved',
    reservationId = reservationId,
    inventoryId = inventoryId,
    expiresAtUnix = expiresAt
})
