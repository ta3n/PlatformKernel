WITH ordered_data AS (
  SELECT *
  FROM (
         VALUES
           ('大人', 1),
           ('子供A', 2),
           ('子供B', 3),
           ('食事有・布団有', 4),
           ('食事有・布団無', 5),
           ('食事無・布団有', 6),
           ('食事無・布団無', 7)
       ) AS t(group_name, display_order)
)
UPDATE person_age_type
SET display_order = od.display_order
  FROM ordered_data AS od
WHERE (person_age_type.meta ->> 'GroupName') = od.group_name;
