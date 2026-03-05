CREATE OR REPLACE FUNCTION get_root_reservation_code(start_id BIGINT)
RETURNS TEXT AS $$
WITH RECURSIVE reservation_chain AS (
    SELECT r.id, r.parent_id, r.code
    FROM reservation r
    WHERE r.id = start_id

    UNION ALL

    SELECT r2.id, r2.parent_id, r2.code
    FROM reservation r2
    INNER JOIN reservation_chain rc ON rc.parent_id = r2.id
)
SELECT rc.code
FROM reservation_chain rc
WHERE rc.parent_id IS NULL
LIMIT 1;
$$ LANGUAGE sql STABLE;
