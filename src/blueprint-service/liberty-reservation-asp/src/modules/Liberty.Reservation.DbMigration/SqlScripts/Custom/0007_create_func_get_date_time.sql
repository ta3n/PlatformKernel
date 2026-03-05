CREATE OR REPLACE FUNCTION get_date_time(
    app_date_id BIGINT,
    check_out_time INTERVAL
)
RETURNS TIMESTAMP AS $$
BEGIN
  RETURN TO_TIMESTAMP(app_date_id::TEXT, 'YYYYMMDD') + COALESCE(check_out_time, INTERVAL '00:00');
END;
$$ LANGUAGE plpgsql IMMUTABLE;
