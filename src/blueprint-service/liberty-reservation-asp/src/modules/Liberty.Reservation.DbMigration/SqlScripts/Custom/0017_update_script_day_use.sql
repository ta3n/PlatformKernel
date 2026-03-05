UPDATE reservation
SET booking_data = jsonb_set(
  reservation.booking_data,
  '{Plan,DayUse}',
  to_jsonb(plan.day_use),
  true
                   )
  FROM plan
WHERE
  (reservation.booking_data->'Plan'->>'Id')::bigint = plan.id;
