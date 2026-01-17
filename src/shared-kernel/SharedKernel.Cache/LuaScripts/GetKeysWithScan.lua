local pattern = ARGV[1]
local result = {}
local cursor = "0"
repeat
    local scanResult = redis.call("SCAN", cursor, "MATCH", pattern, "COUNT", 1000)
    cursor = scanResult[1]
    local keys = scanResult[2]

    for i = 1, #keys do
        table.insert(result, keys[i])
    end
until cursor == "0"

return result
