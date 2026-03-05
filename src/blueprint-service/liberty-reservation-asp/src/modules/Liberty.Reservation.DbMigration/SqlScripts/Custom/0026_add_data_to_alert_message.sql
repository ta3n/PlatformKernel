DO $$
DECLARE
    create_date BIGINT;
BEGIN
    -- Tạo giá trị timestamp BIGINT theo format: YYYYMMDDHH24MISS + millisecond (3 số)
    create_date := (
        to_char(NOW(), 'YYYYMMDDHH24MISS') ||
        lpad( (EXTRACT(milliseconds FROM clock_timestamp())::INT % 1000)::text, 3, '0' )
    )::BIGINT;

    -- Insert dữ liệu mẫu
   INSERT INTO alert_message
(title, "content", icon, color, is_enabled, is_visible, record_memo, is_deleted, display_order, created_at, created_by, updated_by, updated_at, deleted_by, deleted_at, code)
VALUES('{"en": ""}'::jsonb, '{"en": ""}'::jsonb, NULL, NULL, false, false, NULL, false, 1, create_date, NULL, NULL, NULL, NULL, NULL, '1');
END $$;
