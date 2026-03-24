local function split(value, separator)
    local result = {}

    if not value or value == '' then
        return result
    end

    for entry in string.gmatch(value, '([^' .. separator .. ']+)') do
        table.insert(result, entry)
    end

    return result
end

local reservationKey = KEYS[1]
local expiringReservationsKey = KEYS[2]

local releaseReason = ARGV[1]
local retentionTtl = tonumber(ARGV[2])
local releasedAt = tonumber(ARGV[3])

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

if status == 'released' then
    return cjson.encode({
        code = 'EXISTS',
        status = status,
        reservationId = reservationId,
        inventoryId = inventoryId,
        expiresAtUnix = expiresAtUnix
    })
end

if status == 'confirmed' then
    return cjson.encode({
        code = 'ALREADY_CONFIRMED',
        status = status,
        reservationId = reservationId,
        inventoryId = inventoryId,
        expiresAtUnix = expiresAtUnix
    })
end

local quantity = tonumber(redis.call('HGET', reservationKey, 'quantity') or '0')
local stockKeys = split(redis.call('HGET', reservationKey, 'stockKeys'), ',')

for _, stockKey in ipairs(stockKeys) do
    redis.call('INCRBY', stockKey, quantity)
end

redis.call(
    'HSET',
    reservationKey,
    'status', 'released',
    'releaseReason', releaseReason,
    'releasedAtUnix', releasedAt
)

redis.call('EXPIRE', reservationKey, retentionTtl)
redis.call('ZREM', expiringReservationsKey, reservationId)

return cjson.encode({
    code = 'RELEASED',
    status = 'released',
    reservationId = reservationId,
    inventoryId = inventoryId,
    expiresAtUnix = expiresAtUnix
})
