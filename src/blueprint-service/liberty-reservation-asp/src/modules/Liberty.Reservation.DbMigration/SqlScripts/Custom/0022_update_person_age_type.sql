UPDATE person_age_type
SET meta = jsonb_set(meta, '{PersonAgeGroup}', '8', true)
WHERE meta ->> 'GroupName' = '食事無・布団無';
