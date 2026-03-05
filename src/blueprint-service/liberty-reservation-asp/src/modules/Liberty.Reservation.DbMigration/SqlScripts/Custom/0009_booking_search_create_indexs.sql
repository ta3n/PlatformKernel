-- Delete exist indexes
-- DROP INDEX IF EXISTS idx_plan_room_site_date_enabled;
-- DROP INDEX IF EXISTS idx_price_data_enabled;
-- DROP INDEX IF EXISTS idx_room_group_app_date_is_not_selled_composite;
-- DROP INDEX IF EXISTS idx_room_group_deleted;
-- DROP INDEX IF EXISTS idx_plan_room_group_site_app_date_search;
-- DROP INDEX IF EXISTS idx_plan_room_group_site_app_date_plan_id;
-- DROP INDEX IF EXISTS idx_plan_room_group_site_app_date_room_group_id;
-- DROP INDEX IF EXISTS idx_plan_room_group_site_app_date_site_id;
-- DROP INDEX IF EXISTS idx_plan_room_group_site_app_date_date_calendar;
-- DROP INDEX IF EXISTS idx_plan_room_group_site_app_date_display_order;
-- DROP INDEX IF EXISTS idx_room_group_app_date_composite;
-- DROP INDEX IF EXISTS idx_facility_room_group_composite;
-- DROP INDEX IF EXISTS idx_room_group_enabled;
-- DROP INDEX IF EXISTS idx_plan_room_site_date_price_composite;
-- DROP INDEX IF EXISTS idx_room_group_app_date_main;
-- DROP INDEX IF EXISTS idx_room_group_app_date_order;
-- DROP INDEX IF EXISTS idx_room_group_active;
-- DROP INDEX IF EXISTS idx_plan_room_site_date_composite;
-- DROP INDEX IF EXISTS idx_plan_room_site_date_calendar;
-- DROP INDEX IF EXISTS idx_plan_room_site_display_order;
-- DROP INDEX IF EXISTS idx_plan_performance;
-- DROP INDEX IF EXISTS idx_plan_room_group_performance;
-- DROP INDEX IF EXISTS idx_facility_plan_performance;
-- DROP INDEX IF EXISTS idx_plan_site_performance;
-- DROP INDEX IF EXISTS idx_plan_lookup;
-- DROP INDEX IF EXISTS idx_plan_room_group_lookup;
-- DROP INDEX IF EXISTS idx_facility_plan_lookup;
-- DROP INDEX IF EXISTS idx_plan_site_lookup;
-- DROP INDEX IF EXISTS idx_room_group_app_date_optimized;
-- DROP INDEX IF EXISTS idx_room_group_optimized;
-- DROP INDEX IF EXISTS idx_facility_room_group_optimized;
-- DROP INDEX IF EXISTS idx_facility_optimized;


CREATE INDEX IF NOT EXISTS idx_plan_lookup
ON public.plan (is_deleted, is_enabled, is_on_line_payment, is_on_side_payment,
                cancellation_id, use_display_date, display_date_start, display_date_end,
                number_of_stay_limit_max, number_of_stay_limit_min, is_secret);

CREATE INDEX IF NOT EXISTS idx_cancellation_lookup
ON public.cancellation (id, is_deleted, is_enabled);

CREATE INDEX IF NOT EXISTS idx_plan_room_group_lookup
ON public.plan_room_group (plan_id, is_deleted, is_enabled);

CREATE INDEX IF NOT EXISTS idx_facility_plan_lookup
ON public.facility_plan (plan_id, facility_id, is_deleted);

CREATE INDEX IF NOT EXISTS idx_plan_site_lookup
ON public.plan_site (plan_id, site_id, is_deleted);

CREATE INDEX IF NOT EXISTS idx_plan_room_group_site_lookup
ON public.plan_room_group_site (plan_id, site_id, is_deleted, is_enabled, room_group_id);

CREATE INDEX IF NOT EXISTS idx_room_group_lookup
ON public.room_group (id, is_deleted, is_enabled);

CREATE INDEX IF NOT EXISTS idx_plan_complex_filter
ON public.plan (is_deleted, is_enabled, is_on_line_payment, is_on_side_payment,
                cancellation_id, is_secret, display_order);
