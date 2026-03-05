UPDATE reservation r
SET check_out_time = p.check_out
FROM plan p
WHERE r.plan_id = p.id
  AND p.check_out IS NOT NULL;
