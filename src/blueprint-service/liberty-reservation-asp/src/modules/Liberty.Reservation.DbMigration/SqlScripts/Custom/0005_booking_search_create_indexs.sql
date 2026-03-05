CREATE INDEX IF NOT EXISTS idx_plan_room_site_date_enabled
ON plan_room_group_site_app_date_price_data
(plan_id, room_group_id, site_id, date_calendar, is_enabled, is_deleted)
WHERE is_deleted = false AND is_enabled = true;

CREATE INDEX IF NOT EXISTS idx_price_data_enabled
ON price_data (id, is_deleted, is_enabled)
WHERE is_deleted = false AND is_enabled = true;

CREATE INDEX IF NOT EXISTS idx_room_group_app_date_is_not_selled_composite
ON room_group_app_date (room_group_id, app_date_id, is_deleted, is_not_selled, display_order);

CREATE INDEX IF NOT EXISTS idx_room_group_deleted
ON room_group (is_deleted) WHERE NOT is_deleted;

CREATE INDEX IF NOT EXISTS idx_plan_room_group_site_app_date_search
ON public.plan_room_group_site_app_date
(is_deleted, plan_id, room_group_id, site_id, date_calendar, display_order DESC);

CREATE INDEX IF NOT EXISTS idx_plan_room_group_site_app_date_plan_id
ON public.plan_room_group_site_app_date (plan_id)
WHERE NOT is_deleted;

CREATE INDEX IF NOT EXISTS idx_plan_room_group_site_app_date_room_group_id
ON public.plan_room_group_site_app_date (room_group_id)
WHERE NOT is_deleted;

CREATE INDEX IF NOT EXISTS idx_plan_room_group_site_app_date_site_id
ON public.plan_room_group_site_app_date (site_id)
WHERE NOT is_deleted;

CREATE INDEX IF NOT EXISTS idx_plan_room_group_site_app_date_date_calendar
ON public.plan_room_group_site_app_date (date_calendar)
WHERE NOT is_deleted;

CREATE INDEX IF NOT EXISTS idx_plan_room_group_site_app_date_display_order
ON public.plan_room_group_site_app_date (display_order DESC)
WHERE NOT is_deleted;

CREATE INDEX IF NOT EXISTS idx_room_group_app_date_composite
ON room_group_app_date (room_group_id, app_date_id, is_enabled, is_deleted)
WHERE is_deleted = false AND is_enabled = true;

CREATE INDEX IF NOT EXISTS idx_facility_room_group_composite
ON facility_room_group (facility_id, room_group_id, is_enabled, is_deleted)
WHERE is_deleted = false AND is_enabled = true;

CREATE INDEX IF NOT EXISTS idx_room_group_enabled
ON room_group (id, is_enabled, is_deleted)
WHERE is_deleted = false AND is_enabled = true;

CREATE INDEX IF NOT EXISTS idx_plan_room_site_date_price_composite
ON public.plan_room_group_site_app_date_price_data
(plan_id, room_group_id, site_id, date_calendar, is_deleted, is_enabled, price_data_id);

CREATE INDEX IF NOT EXISTS idx_room_group_app_date_main
ON room_group_app_date (room_group_id, app_date_id, is_deleted, is_not_selled);

CREATE INDEX IF NOT EXISTS idx_room_group_app_date_order
ON room_group_app_date (display_order DESC, room_group_id, app_date_id);

CREATE INDEX IF NOT EXISTS idx_room_group_active
ON room_group (is_deleted, id) WHERE NOT is_deleted;

CREATE INDEX IF NOT EXISTS idx_plan_room_site_date_composite
ON public.plan_room_group_site_app_date
(is_deleted, plan_id, room_group_id, site_id, date_calendar, display_order DESC);

CREATE INDEX IF NOT EXISTS idx_plan_room_site_date_calendar
ON public.plan_room_group_site_app_date (date_calendar)
WHERE NOT is_deleted;

CREATE INDEX IF NOT EXISTS idx_plan_room_site_display_order
ON public.plan_room_group_site_app_date (display_order DESC)
WHERE NOT is_deleted;

CREATE INDEX IF NOT EXISTS idx_plan_performance ON public.plan
(is_deleted, is_enabled, is_on_line_payment, is_on_side_payment, is_secret,
 use_display_date, display_date_start, display_date_end);

CREATE INDEX IF NOT EXISTS idx_plan_room_group_performance ON public.plan_room_group
(is_deleted, plan_id, is_enabled);

CREATE INDEX IF NOT EXISTS idx_facility_plan_performance ON public.facility_plan
(is_deleted, plan_id, facility_id);

CREATE INDEX IF NOT EXISTS idx_plan_site_performance ON public.plan_site
(is_deleted, plan_id, site_id);

CREATE INDEX IF NOT EXISTS idx_plan_lookup ON public.plan(is_deleted, is_enabled, is_on_line_payment, is_on_side_payment, use_display_date, display_date_start, display_date_end, number_of_stay_limit_max, number_of_stay_limit_min, is_secret);

CREATE INDEX IF NOT EXISTS idx_plan_room_group_lookup ON public.plan_room_group(is_deleted, plan_id, is_enabled);

CREATE INDEX IF NOT EXISTS idx_facility_plan_lookup ON public.facility_plan(is_deleted, plan_id, facility_id);

CREATE INDEX IF NOT EXISTS idx_plan_site_lookup ON public.plan_site(is_deleted, plan_id, site_id);

CREATE INDEX IF NOT EXISTS idx_room_group_app_date_optimized
ON room_group_app_date (room_group_id, app_date_id, is_deleted, is_enabled);

CREATE INDEX IF NOT EXISTS idx_room_group_optimized
ON room_group (id, is_deleted, is_enabled, display_order);

CREATE INDEX IF NOT EXISTS idx_facility_room_group_optimized
ON facility_room_group (room_group_id, facility_id, is_deleted, is_enabled);

CREATE INDEX IF NOT EXISTS idx_facility_optimized
ON facility (id, is_deleted, is_enabled);
