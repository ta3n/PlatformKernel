SELECT *
FROM   pg_settings
WHERE  name = 'max_connections';

-- List all detailed connections (status + queries)
SELECT
    pid,
    usename AS user,
    datname AS database,
    application_name,
    client_addr,
    state,
    wait_event_type,
    wait_event,
    query_start,
    state_change,
    now() - query_start AS duration,
    LEFT(query, 100) AS query
FROM
    pg_stat_activity
WHERE
    pid <> pg_backend_pid()  -- Ignore the current connection
ORDER BY
    query_start DESC;

-- List of queries running for too long (for example> 30 seconds)
SELECT
    pid,
    now() - query_start AS runtime,
    usename,
    datname,
    client_addr,
    state,
    LEFT(query, 150) AS query
FROM
    pg_stat_activity
WHERE
    state = 'active'
    AND now() - query_start > interval '30 seconds'
ORDER BY
    runtime DESC;

-- The total number of connectors by database
SELECT
    datname,
    COUNT(*) AS connection_count
FROM
    pg_stat_activity
GROUP BY
    datname
ORDER BY
    connection_count DESC;

-- Number of connects according to Application Name (if you configure ApplicationName in Connection String)
SELECT
    application_name,
    datname,
    usename,
    COUNT(*) AS connection_count
FROM
    pg_stat_activity
GROUP BY
    application_name,
    datname,
    usename
ORDER BY
    connection_count DESC;
