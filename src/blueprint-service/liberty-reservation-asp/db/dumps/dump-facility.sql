CREATE DATABASE liberty_facility ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';

-- Use dynamic variables for user and password
CREATE USER user_facility_readonly;

-- Grant connect permission
GRANT CONNECT ON DATABASE liberty_facility TO user_facility_readonly;

-- Grant USAGE permission
GRANT USAGE ON SCHEMA public TO user_facility_readonly;

-- Grant SELECT permission
GRANT SELECT ON ALL TABLES IN SCHEMA public TO user_facility_readonly;

-- Grant default SELECT permission for new tables
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO user_facility_readonly;
