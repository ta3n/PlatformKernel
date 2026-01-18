-- KEYS[1] : price calendar lock key
-- ARGV[1] : token

local key = KEYS[1]
local token = ARGV[1]

local items = redis.call('LRANGE', key, 0, -1)

for i, item in ipairs(items) do
    local p1 = string.find(item, '|')
    local p2 = string.find(item, '|', p1 + 1)

    -- Lấy chính xác phần token
    local existToken = string.sub(item, p2 + 1)

    if existToken == token then
        redis.call('LREM', key, 1, item)
        return 1
    end
end

return 0
