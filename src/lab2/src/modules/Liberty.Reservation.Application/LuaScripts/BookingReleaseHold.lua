local quantity = tonumber(ARGV[1])
local n = tonumber(ARGV[2])

for i = 1, n do
  local globalKey = KEYS[(i - 1) * 2 + 1]
  local userKey = KEYS[(i - 1) * 2 + 2]

  -- Check if key exists and get values safely
  local currentGlobal = tonumber(redis.call('get', globalKey) or '0')
  local currentUser = tonumber(redis.call('get', userKey) or '0')

  -- Nếu không còn gì để release thì coi như đã xử lý
  if currentGlobal > 0 and currentGlobal < quantity then
    return redis.error_reply('Global hold quantity is less than release quantity for ' .. globalKey)
  end

  if currentUser > 0 and currentUser < quantity then
    return redis.error_reply('User hold quantity is less than release quantity for ' .. userKey)
  end
end

for i = 1, n do
  local globalKey = KEYS[(i - 1) * 2 + 1]
  local userKey = KEYS[(i - 1) * 2 + 2]

  local currentGlobal = tonumber(redis.call('get', globalKey) or '0')
  if currentGlobal > 0 then
    if currentGlobal <= quantity then
      redis.call('del', globalKey)
    else
      redis.call('decrby', globalKey, quantity)
    end
  end

  local currentUser = tonumber(redis.call('get', userKey) or '0')
  if currentUser > 0 then
    if currentUser <= quantity then
      redis.call('del', userKey)
    else
      redis.call('decrby', userKey, quantity)
    end
  end
end

return 1
