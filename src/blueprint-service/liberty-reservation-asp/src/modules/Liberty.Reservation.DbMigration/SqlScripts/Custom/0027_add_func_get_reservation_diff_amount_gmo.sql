CREATE OR REPLACE FUNCTION public.reservation_diff_amount_gmo_online_payment(p_reservation_id BIGINT)
RETURNS boolean
LANGUAGE sql
STABLE
AS $func$
SELECT COALESCE((
          SELECT COALESCE(
                   (
                     CASE
                       WHEN jsonb_typeof(r.booking_data->'AllTotalPrice') = 'number'
                         THEN (r.booking_data->>'AllTotalPrice')::numeric
          WHEN (r.booking_data->>'AllTotalPrice') ~ '^-?\d+(\.\d+)?$'
            THEN (r.booking_data->>'AllTotalPrice')::numeric
          ELSE NULL
        END
        <>
        CASE
          WHEN g.request IS NULL
            THEN NULL
          WHEN jsonb_typeof(g.request->'Amount') = 'number'
            THEN (g.request->>'Amount')::numeric
          WHEN (g.request->>'Amount') ~ '^-?\d+(\.\d+)?$'
            THEN (g.request->>'Amount')::numeric
          ELSE NULL
        END
                   ),
                 FALSE
               )
        FROM public.reservation r
               LEFT JOIN LATERAL (
            SELECT request
            FROM public.gmo_change_tran_report
            WHERE reservation_id = r.parent_id
            ORDER BY id DESC
              LIMIT 1
         ) g ON TRUE
    WHERE r.parent_id = p_reservation_id
    LIMIT 1
  ), FALSE);
$func$;
