local result = {}

for i = 1, #KEYS do
  local v = redis.call('HGET', KEYS[i], 'data')
  if v then
    table.insert(result, v)
  end
end

return result
