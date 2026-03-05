WITH meal_data AS (
  SELECT
    r.id AS reservation_id,
    jsonb_agg(
      jsonb_build_object(
        'id', mt.id,
        'name', mt.name,
        'meal_type_eat_type', pmt.meal_type_eat_type
      )
    ) AS meals_json
  FROM
    public.reservation r
  JOIN public.plan_meal_type pmt ON pmt.plan_id = r.plan_id
  JOIN public.meal_type mt ON mt.id = pmt.meal_type_id
  GROUP BY
    r.id
)
UPDATE public.reservation r
SET booking_data = jsonb_set(
  r.booking_data,
  '{Plan,Meals}',
  md.meals_json,
  true  -- create if missing
)
FROM meal_data md
WHERE r.id = md.reservation_id;
