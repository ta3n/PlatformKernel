UPDATE reservation r
SET
  booking_data = jsonb_set(
    COALESCE(r.booking_data, '{}'::jsonb),
    '{Plan}',
    jsonb_set(
      (COALESCE(r.booking_data -> 'Plan', '{}'::jsonb)) - 'CancellationDataPolicy',
      '{CancellationDataPolicy}',
      jsonb_build_object(
        'Id', c.id,
        'Name', c.name,
        'CanOnLinePayment', c.can_on_line_payment,
        'PaymentLimit', c.payment_limit,
        'TableSource', c.table_source,
        'Description', c.description,
        'CancellationData', COALESCE((SELECT
            jsonb_agg(
              jsonb_build_object(
                'DayStart', cd.day_start,
                'DayEnd', cd.day_end,
                'Rate', cd.rate
              )
            )
          FROM
            cancellation_cancellation_data ccd
              JOIN cancellation_data cd ON ccd.cancellation_data_id = cd.id
          WHERE
            ccd.cancellation_id = c.id), '[]' ::jsonb)
      ),
      TRUE
    ),
    TRUE
    ) FROM plan p
JOIN cancellation C
ON p.cancellation_id = C.id
WHERE
  r.plan_id = p.id;
