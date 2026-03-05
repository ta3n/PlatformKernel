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
