CREATE OR REPLACE FUNCTION apply_timezone_offset(
    ts timestamptz,
    tz text
)
RETURNS timestamptz AS
$$
    SELECT ts + COALESCE(tz::interval, interval '9 hours') * -1;
$$ LANGUAGE sql IMMUTABLE;
