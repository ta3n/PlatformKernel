for i = 1, #ARGV do
    local pattern = ARGV[i]
    local cursor = "0"
    repeat
        local scanResult = redis.call("SCAN", cursor, "MATCH", pattern, "COUNT", 1000)
        cursor = scanResult[1]
        local keys = scanResult[2]

        if #keys > 0 then
            redis.call('DEL', unpack(keys))
        end
    until cursor == "0"
end
