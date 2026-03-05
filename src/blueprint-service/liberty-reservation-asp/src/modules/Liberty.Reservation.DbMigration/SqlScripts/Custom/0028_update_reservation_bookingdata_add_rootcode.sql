WITH RECURSIVE chain AS (
  SELECT
    r.id AS leaf_id,
    r.id,
    r.parent_id,
    r.code
  FROM public.reservation r

  UNION ALL

  SELECT
    ch.leaf_id,
    p.id,
    p.parent_id,
    p.code
  FROM chain ch
         JOIN public.reservation p
              ON p.id = ch.parent_id
),
  root_code AS (
    SELECT
      ch.leaf_id,
      ch.code AS root_code
    FROM chain ch
    WHERE ch.parent_id IS NULL
  )
UPDATE public.reservation AS r
SET booking_data = jsonb_set(
  COALESCE(r.booking_data, '{}'::jsonb),
  '{RootCode}',
  to_jsonb(rc.root_code),
  true)
  FROM root_code rc
WHERE rc.leaf_id = r.id AND (r.booking_data->>'RootCode') IS DISTINCT FROM rc.root_code;
