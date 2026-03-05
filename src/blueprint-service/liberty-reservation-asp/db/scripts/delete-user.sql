DO $$
DECLARE
  database_name TEXT := 'liberty_membership_facility';
  schema_names TEXT[] := ARRAY['public'];
  delete_user TEXT := 'readonly_facility';
  obj RECORD;
  schema_name TEXT;
BEGIN
  -- Loop through each schema and revoke permissions
  FOR i IN array_lower(schema_names, 1)..array_upper(schema_names, 1)
  LOOP
    schema_name := schema_names[i];
    -- 1. Revoke all privileges on all tables in the schema
    EXECUTE format('REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA %I FROM %I', schema_name, delete_user);

    -- 2. Revoke usage privileges on the schema
    EXECUTE format('REVOKE ALL PRIVILEGES ON SCHEMA %I FROM %I', schema_name, delete_user);

    -- 3. Revoke default privileges on newly created tables
    EXECUTE format('ALTER DEFAULT PRIVILEGES IN SCHEMA %I REVOKE ALL ON TABLES FROM %I', schema_name, delete_user);
  END LOOP;

  -- 4. Revoke access privileges to the database
  EXECUTE format('REVOKE ALL PRIVILEGES ON DATABASE %I FROM %I', database_name, delete_user);

  -- 5. Drop the role
  EXECUTE format('DROP ROLE %I', delete_user);
END $$;
