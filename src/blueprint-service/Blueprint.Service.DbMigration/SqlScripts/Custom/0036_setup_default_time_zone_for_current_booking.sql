UPDATE facility
SET
    time_zone_id = 'Asia/Tokyo',
    time_zone = TIME '09:00:00'
WHERE time_zone_id IS null;


UPDATE reservation AS r
SET booking_data = new_data.booking_data
FROM (
    SELECT
        r.id,
        r.booking_data
        || jsonb_build_object(
            'Facility',
            COALESCE(r.booking_data->'Facility', '{}'::jsonb)
            || jsonb_build_object(
                'TimeZoneId', 'Asia/Tokyo',
                'TimeZone', '09:00:00'
            )
        ) AS booking_data
    FROM reservation AS r
) AS new_data
WHERE r.id = new_data.id;
