CREATE OR REPLACE FUNCTION get_root_reservation_datetime(start_id BIGINT)
RETURNS TIMESTAMPTZ AS $$
DECLARE
    current_id BIGINT := start_id;
    parent_id BIGINT;
    result TIMESTAMPTZ;
BEGIN
    LOOP
        SELECT r.parent_id, r.reservation_date_time
        INTO parent_id, result
        FROM reservation r
        WHERE r.id = current_id;

        EXIT WHEN parent_id IS NULL;
        current_id := parent_id;
    END LOOP;

    RETURN result;
END;
$$ LANGUAGE plpgsql STABLE;
