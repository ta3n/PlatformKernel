local patterns = ARGV
local result = {}
local seen = {}

-- Loop through each pattern
for i = 1, #patterns do
  local cursor = "0"
  repeat
    -- SCAN keyspace with pattern
    local scanResult = redis.call("SCAN", cursor, "MATCH", patterns[i], "COUNT", 1000)
    cursor = scanResult[1]
    local keys = scanResult[2]

    for j = 1, #keys do
      local k = keys[j]
      if not seen[k] then
        seen[k] = true
        local v = redis.call('HGET', k, 'data')
        if v then
          table.insert(result, v)
        end
      end
    end
  until cursor == "0"
end

return result
