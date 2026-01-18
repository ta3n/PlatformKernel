CREATE OR REPLACE FUNCTION add_days_to_yyyymmdd(
    input_date BIGINT,
    add_days   INTEGER
)
RETURNS BIGINT
LANGUAGE sql
IMMUTABLE
AS $$
SELECT
  to_char(
    make_date(
      (input_date / 10000)::int,
      ((input_date / 100) % 100)::int,
            (input_date % 100)::int
        )
      + add_days,
    'YYYYMMDD'
  )::BIGINT;
$$;
