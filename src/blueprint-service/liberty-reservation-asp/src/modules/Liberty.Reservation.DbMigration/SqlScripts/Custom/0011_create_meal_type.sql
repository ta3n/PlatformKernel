DO $$
DECLARE
create_date BIGINT;
BEGIN
    create_date := (
        to_char(NOW(), 'YYYYMMDDHH24MISS') ||
        lpad( (EXTRACT(milliseconds FROM clock_timestamp())::INT % 1000)::text, 3, '0' )
    )::BIGINT;

UPDATE "public"."meal_type" SET "display_order" = 1 WHERE "id" = 1;

UPDATE "public"."meal_type" SET "display_order" = 3 WHERE "id" = 2;

INSERT INTO
  "public"."meal_type"
("name", "is_enabled", "is_visible", "record_memo", "is_deleted", "display_order", "created_at", "code")
VALUES
  ('昼食', 't', 't', '1742385257', 'f', 2, create_date, '0d9cdfc5d8654e55ac9e6d37aa75e657');
END $$;
