-- This script stores data in hash format with "data" field
-- Parameters:
--   ARGV[1]: Time to live (TTL) for keys
--   ARGV[2], ARGV[4], ARGV[6], ...: Keys
--   ARGV[3], ARGV[5], ARGV[7], ...: Corresponding values
-- Returns number of key-value pairs processed

local ttl = tonumber(ARGV[1])
local count = 0

for i = 2, #ARGV, 2 do
  -- Store data as hash with "data" field
  redis.call('HSET', ARGV[i], 'data', ARGV[i + 1])

  -- Set TTL only if > 0
  if ttl > 0 then
    redis.call('EXPIRE', ARGV[i], ttl)
  end

  count = count + 1
end

return count
