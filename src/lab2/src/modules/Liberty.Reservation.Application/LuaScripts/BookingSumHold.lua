local cursor = '0'
local sum = 0
local pattern = ARGV[1]

repeat
  local result = redis.call('SCAN', cursor, 'MATCH', pattern, 'COUNT', 1000)
  cursor = result[1]
  local keys = result[2]

  for i = 1, #keys do
    local value = tonumber(redis.call('GET', keys[i]) or '0')
    sum = sum + value
  end
until cursor == '0'

return sum
