CREATE OR REPLACE FUNCTION jsonb_search_all_values(json_data jsonb, search_term text)
RETURNS boolean AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1
        FROM jsonb_each_text(json_data)
        WHERE value ILIKE '%' || search_term || '%'
    );
END;
$$ LANGUAGE plpgsql IMMUTABLE;
