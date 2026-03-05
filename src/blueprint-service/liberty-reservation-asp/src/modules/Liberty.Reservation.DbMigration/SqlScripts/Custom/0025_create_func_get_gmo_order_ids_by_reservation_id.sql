CREATE OR REPLACE FUNCTION public.get_gmo_order_ids_by_reservation_id(start_id bigint)
 RETURNS text
 LANGUAGE plpgsql
AS $function$
	BEGIN
 RETURN (
        SELECT string_agg(DISTINCT grr.order_id, ', ')
        FROM order_reservation orr
        JOIN "order" o ON o.id = orr.order_id
        JOIN order_gmo_payment_result_request ogprr ON ogprr.order_id = o.id
        JOIN gmo_payment_result_request grr ON grr.id = ogprr.gmo_payment_result_request_id
        WHERE orr.reservation_id = start_id
          AND grr.order_id IS NOT NULL
          AND trim(grr.order_id) <> ''
    );
	END;
$function$
;
