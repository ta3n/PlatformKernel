-- KEYS[1]  : price calendar lock key
-- ARGV[1]  : startDate
-- ARGV[2]  : endDate
-- ARGV[3]  : token
-- ARGV[4]  : ttl (seconds)

local key = KEYS[1]
local newStart = tonumber(ARGV[1])
local newEnd = tonumber(ARGV[2])
local token = ARGV[3]
local ttl = tonumber(ARGV[4])

local items = redis.call('LRANGE', key, 0, -1)

for i, item in ipairs(items) do
    local p1 = string.find(item, '|')
    local p2 = string.find(item, '|', p1 + 1)

    local existStart = tonumber(string.sub(item, 1, p1 - 1))
    local existEnd = tonumber(string.sub(item, p1 + 1, p2 - 1))

    if (newStart <= existEnd) and (newEnd >= existStart) then
        return 0
    end
end

local newVal = newStart .. '|' .. newEnd .. '|' .. token
redis.call('RPUSH', key, newVal)

redis.call('EXPIRE', key, ttl)

return 1
