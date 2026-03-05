UPDATE person_age_type
SET meta = jsonb_set(meta, '{FoodBed}', to_jsonb(1::integer), true)
WHERE meta->>'GroupName' = '食事有・布団無';

UPDATE person_age_type
SET meta = jsonb_set(meta, '{FoodBed}', to_jsonb(2::integer), true)
WHERE meta->>'GroupName' = '食事無・布団有';
