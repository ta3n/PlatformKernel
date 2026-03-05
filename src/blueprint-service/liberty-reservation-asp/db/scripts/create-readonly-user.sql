DO $$
DECLARE
  database_name TEXT := 'liberty_membership_facility';
  schema_names TEXT[] := ARRAY['public'];
  readonly_user_name TEXT := 'readonly';
  readonly_user_password TEXT := '01jj10kbc1pgxw6hchebqaqqv0';
  schema_name TEXT;
BEGIN
  -- Create read-only user
  EXECUTE format('CREATE USER %I WITH PASSWORD %L', readonly_user_name, readonly_user_password);

  -- Grant connect permission to the database
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I', database_name, readonly_user_name);

  -- Loop through each schema and grant permissions
  FOR i IN array_lower(schema_names, 1)..array_upper(schema_names, 1)
  LOOP
    schema_name := schema_names[i];
    -- Grant USAGE permission on the schema
    EXECUTE format('GRANT USAGE ON SCHEMA %I TO %I', schema_name, readonly_user_name);

    -- Grant SELECT permission on all existing tables
    EXECUTE format('GRANT SELECT ON ALL TABLES IN SCHEMA %I TO %I', schema_name, readonly_user_name);

    -- Grant default SELECT permission when new tables are created
    EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I GRANT SELECT ON TABLES TO %I', schema_name, readonly_user_name);
  END LOOP;
END $$;
