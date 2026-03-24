# Hướng dẫn sử dụng TimescaleDB

- [Hướng dẫn sử dụng TimescaleDB](#hướng-dẫn-sử-dụng-timescaledb)
  - [1. Mục tiêu của tài liệu](#1-mục-tiêu-của-tài-liệu)
  - [2. TimescaleDB là gì và khác gì PostgreSQL](#2-timescaledb-là-gì-và-khác-gì-postgresql)
  - [3. Khi nào nên dùng TimescaleDB](#3-khi-nào-nên-dùng-timescaledb)
  - [4. Các khái niệm quan trọng](#4-các-khái-niệm-quan-trọng)
  - [5. Cài đặt và cấu hình self-host](#5-cài-đặt-và-cấu-hình-self-host)
  - [6. Kết nối ứng dụng với TimescaleDB](#6-kết-nối-ứng-dụng-với-timescaledb)
  - [7. Thiết kế schema cho dữ liệu time-series](#7-thiết-kế-schema-cho-dữ-liệu-time-series)
  - [8. Tạo hypertable](#8-tạo-hypertable)
  - [9. Ghi dữ liệu](#9-ghi-dữ-liệu)
  - [10. Truy vấn dữ liệu theo thời gian với time_bucket](#10-truy-vấn-dữ-liệu-theo-thời-gian-với-time_bucket)
  - [11. Continuous aggregates](#11-continuous-aggregates)
  - [12. Data retention](#12-data-retention)
  - [13. Columnstore và compression](#13-columnstore-và-compression)
  - [14. Tiering dữ liệu cũ](#14-tiering-dữ-liệu-cũ)
  - [15. Indexing và tối ưu truy vấn](#15-indexing-và-tối-ưu-truy-vấn)
  - [16. Theo dõi hypertable, chunk và jobs](#16-theo-dõi-hypertable-chunk-và-jobs)
  - [17. Chuyển đổi bảng PostgreSQL hiện có sang hypertable](#17-chuyển-đổi-bảng-postgresql-hiện-có-sang-hypertable)
  - [18. Sử dụng với .NET, Npgsql và EF Core](#18-sử-dụng-với-net-npgsql-và-ef-core)
  - [19. Ví dụ end-to-end](#19-ví-dụ-end-to-end)
  - [20. Khuyến nghị thực tế](#20-khuyến-nghị-thực-tế)
  - [21. Các lỗi thường gặp](#21-các-lỗi-thường-gặp)
  - [22. Tài liệu chính thức nên tham khảo thêm](#22-tài-liệu-chính-thức-nên-tham-khảo-thêm)

## 1. Mục tiêu của tài liệu

Tài liệu này hướng dẫn cách dùng TimescaleDB theo hướng thực hành:

- Hiểu rõ sự khác biệt giữa TimescaleDB và PostgreSQL thông thường
- Biết cách cài đặt và bật extension nếu tự host
- Thiết kế bảng time-series đúng cách
- Tạo `hypertable`, truy vấn bằng `time_bucket`, xây dựng `continuous aggregates`
- Thiết lập `retention`, `columnstore`, `tiering`
- Biết khi nào vẫn dùng ORM bình thường và khi nào nên dùng raw SQL

## 2. TimescaleDB là gì và khác gì PostgreSQL

TimescaleDB là một extension của PostgreSQL dành cho dữ liệu time-series và real-time analytics.

Điều này dẫn đến hai kết luận quan trọng:

- Về kết nối ứng dụng: vẫn dùng giao thức PostgreSQL chuẩn, cùng driver như `psql`, `Npgsql`, `pg`, `pgx`, JDBC
- Về khả năng dữ liệu: có thêm các tính năng đặc thù như `hypertable`, `time_bucket`, `continuous aggregates`, retention policy, columnstore và tiering

Phần lớn câu lệnh SQL cơ bản vẫn giống PostgreSQL:

- `SELECT`, `INSERT`, `UPDATE`, `DELETE`
- transaction
- index
- `JOIN`
- `JSONB`
- extensions khác của PostgreSQL

Phần khác biệt nằm ở cách tổ chức dữ liệu theo thời gian và các API bổ sung của TimescaleDB.

## 3. Khi nào nên dùng TimescaleDB

TimescaleDB phù hợp khi dữ liệu có trục thời gian rõ ràng:

- metrics hệ thống
- audit log
- event log
- IoT/sensor data
- clickstream
- giao dịch thị trường
- lịch sử trạng thái của thiết bị hoặc người dùng

Không nên ép mọi bảng trong hệ thống thành hypertable. Các bảng danh mục, cấu hình, người dùng, vai trò, đối tác, sản phẩm vẫn nên là bảng PostgreSQL thông thường.

Nguyên tắc thực tế:

- Dữ liệu thay đổi theo thời gian và tăng nhanh: cân nhắc dùng hypertable
- Dữ liệu quan hệ tĩnh hoặc tăng chậm: giữ nguyên table PostgreSQL bình thường

## 4. Các khái niệm quan trọng

### 4.1 Hypertable

`Hypertable` là lớp trừu tượng của TimescaleDB cho bảng time-series. Ở phía ứng dụng, bạn vẫn thao tác như một bảng PostgreSQL bình thường, nhưng ở bên dưới TimescaleDB tự chia dữ liệu thành nhiều `chunk`.

### 4.2 Chunk

`Chunk` là các phân vùng dữ liệu theo khoảng thời gian. Ví dụ nếu `chunk_interval` là `1 day`, dữ liệu của mỗi ngày sẽ nằm trong một chunk khác nhau.

Lợi ích:

- insert không dồn vào một vùng dữ liệu khổng lồ
- query theo thời gian chỉ cần đọc những chunk liên quan
- dễ áp dụng retention, nén và tiering trên dữ liệu cũ

### 4.3 Time bucket

`time_bucket` là hàm gom dữ liệu theo cửa sổ thời gian như 1 phút, 5 phút, 1 giờ, 1 ngày.

Ví dụ:

- trung bình CPU theo 5 phút
- số lượng request theo 1 giờ
- doanh thu theo ngày

### 4.4 Continuous aggregate

Là materialized view chuyên dụng của TimescaleDB cho dữ liệu tổng hợp. Thay vì mỗi lần dashboard chạy lại phép `GROUP BY` trên hàng trăm triệu dòng, hệ thống sẽ lưu kết quả tổng hợp và tự refresh theo lịch.

### 4.5 Retention

Policy tự động xóa dữ liệu cũ theo ngưỡng, ví dụ chỉ giữ raw data 90 ngày.

### 4.6 Columnstore

Trên các phiên bản mới, TimescaleDB dùng Hypercore/columnstore để tối ưu lưu trữ và truy vấn phân tích cho các chunk cũ.

### 4.7 Tiering

Dùng để chuyển dữ liệu cũ sang tầng lưu trữ rẻ hơn. Dữ liệu vẫn có thể query nhưng không còn phù hợp để cập nhật thường xuyên.

## 5. Cài đặt và cấu hình self-host

Nếu dùng Timescale Cloud hoặc dịch vụ managed tương đương, phần lớn cấu hình đã được quản lý sẵn. Nếu tự host, cần làm rõ các bước sau.

### 5.1 Bật extension phía PostgreSQL

Trong cấu hình PostgreSQL:

```conf
shared_preload_libraries = 'timescaledb'
```

Sau khi chỉnh cấu hình, restart PostgreSQL.

### 5.2 Tạo extension trong database

Kết nối vào database cần dùng:

```sql
CREATE EXTENSION IF NOT EXISTS timescaledb;
```

Có thể kiểm tra:

```sql
SHOW shared_preload_libraries;

SELECT extname, extversion
FROM pg_extension
WHERE extname = 'timescaledb';
```

### 5.3 Tối ưu cấu hình máy chủ

Nếu self-host, nên dùng `timescaledb-tune` hoặc cấu hình thủ công phù hợp với RAM, CPU, `shared_buffers`, `work_mem`, WAL và connection pool.

Khuyến nghị:

- dùng connection pooler nếu lượng kết nối lớn
- không để ứng dụng mở quá nhiều kết nối trực tiếp tới Postgres
- giám sát disk IOPS, latency, autovacuum và WAL growth

## 6. Kết nối ứng dụng với TimescaleDB

Không cần driver chuyên dụng cho TimescaleDB. Chỉ cần driver PostgreSQL chuẩn.

Ví dụ:

- .NET: `Npgsql`
- Node.js: `pg`
- Go: `pgx`
- Java: PostgreSQL JDBC
- Python: `psycopg`

Điều cần nhớ là:

- CRUD cơ bản: ORM/driver PostgreSQL dùng như cũ
- Tính năng đặc thù của TimescaleDB: thường gọi bằng raw SQL

### 6.1 TimescaleDB Toolkit có bắt buộc không

Không bắt buộc cho kết nối và sử dụng cơ bản.

Toolkit là một Postgres extension bổ sung để có thêm hyperfunctions nâng cao. Chỉ cần cài khi bạn thực sự dùng các hàm mở rộng đó.

Tóm tắt:

- làm việc với TimescaleDB cơ bản: không cần Toolkit
- cần hyperfunctions nâng cao: cài thêm Toolkit
- trên Tiger Cloud: Toolkit thường đã có sẵn
- self-host: cần cài riêng nếu muốn dùng

## 7. Thiết kế schema cho dữ liệu time-series

Ví dụ bảng đo metrics thiết bị:

```sql
CREATE TABLE metrics (
  time        TIMESTAMPTZ      NOT NULL,
  device_id   TEXT             NOT NULL,
  cpu         DOUBLE PRECISION,
  memory      DOUBLE PRECISION,
  temperature DOUBLE PRECISION,
  tags        JSONB
);
```

### 7.1 Quy tắc thiết kế quan trọng

1. Cột thời gian phải rõ ràng và thường là `TIMESTAMPTZ`

2. Cột thời gian phải là `NOT NULL`

3. Nếu dùng `PRIMARY KEY` hoặc `UNIQUE`, khóa phải bao gồm cột partition theo thời gian

Ví dụ hợp lệ:

```sql
ALTER TABLE metrics
ADD CONSTRAINT pk_metrics PRIMARY KEY (time, device_id);
```

Ví dụ không phù hợp:

```sql
ALTER TABLE metrics
ADD CONSTRAINT pk_metrics PRIMARY KEY (device_id);
```

4. Không nên tạo quá nhiều index trên bảng ingest lớn

5. Tách bảng dimension ra riêng nếu dữ liệu metadata không thay đổi theo thời gian

Ví dụ:

- `devices`: bảng quan hệ thường
- `metrics`: hypertable

## 8. Tạo hypertable

### 8.1 Cách hiện đại: tạo hypertable ngay từ `CREATE TABLE`

Docs hiện tại khuyến nghị tạo hypertable trực tiếp bằng `CREATE TABLE ... WITH (...)`.

```sql
CREATE TABLE metrics (
  time        TIMESTAMPTZ      NOT NULL,
  device_id   TEXT             NOT NULL,
  cpu         DOUBLE PRECISION,
  memory      DOUBLE PRECISION,
  temperature DOUBLE PRECISION,
  tags        JSONB
) WITH (
  tsdb.hypertable,
  tsdb.chunk_interval = '1 day',
  tsdb.segmentby = 'device_id',
  tsdb.orderby = 'time DESC'
);
```

Ý nghĩa:

- `tsdb.hypertable`: biến bảng thành hypertable
- `tsdb.chunk_interval = '1 day'`: dữ liệu được chia chunk theo ngày
- `tsdb.segmentby = 'device_id'`: tối ưu phân đoạn trong columnstore
- `tsdb.orderby = 'time DESC'`: tối ưu thứ tự dữ liệu cho truy vấn phân tích

### 8.2 Cách phổ biến trong codebase cũ: `create_hypertable()`

Nhiều hệ thống hiện có vẫn tạo table thường trước rồi chuyển thành hypertable:

```sql
CREATE TABLE metrics (
  time        TIMESTAMPTZ      NOT NULL,
  device_id   TEXT             NOT NULL,
  cpu         DOUBLE PRECISION,
  memory      DOUBLE PRECISION
);

SELECT create_hypertable('metrics', by_range('time'));
```

Hoặc chỉ rõ chunk interval:

```sql
SELECT create_hypertable('metrics', by_range('time', INTERVAL '1 day'));
```

Khuyến nghị thực tế:

- Dự án mới: ưu tiên `CREATE TABLE ... WITH (tsdb.hypertable, ...)`
- Dự án đang dùng migration/ORM mạnh: `create_hypertable()` thường dễ tích hợp hơn

### 8.3 Chọn `chunk_interval`

Đây không có một giá trị đúng cho mọi hệ thống. Chọn theo lưu lượng ghi, khối lượng query và vòng đời dữ liệu.

Gợi ý thực tế:

- ingest rất lớn theo phút/giây: `1 day` hoặc nhỏ hơn
- dữ liệu trung bình: `1 day` đến `7 days`
- dữ liệu phát sinh chậm: `7 days` đến `1 month`

Mục tiêu:

- không tạo quá nhiều chunk nhỏ
- cũng không để chunk quá lớn khiến query và maintenance chậm

## 9. Ghi dữ liệu

Chèn dữ liệu như PostgreSQL thông thường:

```sql
INSERT INTO metrics (time, device_id, cpu, memory, temperature, tags)
VALUES
  (now(), 'device-01', 0.42, 0.73, 31.5, '{"site":"sg","rack":"r1"}'),
  (now(), 'device-02', 0.55, 0.66, 29.8, '{"site":"hn","rack":"r2"}');
```

Khi chèn dữ liệu vào khoảng thời gian mới, TimescaleDB tự động tạo chunk cần thiết.

Nếu ingest theo lô lớn:

- ưu tiên `COPY` thay vì `INSERT` từng dòng
- batch insert theo gói đủ lớn
- hạn chế index không cần thiết trên hypertable ingest nặng

## 10. Truy vấn dữ liệu theo thời gian với time_bucket

Ví dụ lấy CPU trung bình theo 5 phút:

```sql
SELECT
  time_bucket('5 minutes', time) AS bucket,
  device_id,
  avg(cpu) AS avg_cpu
FROM metrics
WHERE time >= now() - INTERVAL '1 day'
GROUP BY bucket, device_id
ORDER BY bucket DESC, device_id;
```

Ví dụ đếm số bản ghi theo giờ:

```sql
SELECT
  time_bucket('1 hour', time) AS bucket,
  count(*) AS total_rows
FROM metrics
WHERE time >= now() - INTERVAL '7 days'
GROUP BY bucket
ORDER BY bucket;
```

Ví dụ lấy giá trị lớn nhất theo ngày cho từng thiết bị:

```sql
SELECT
  time_bucket('1 day', time) AS bucket,
  device_id,
  max(cpu) AS peak_cpu
FROM metrics
WHERE time >= now() - INTERVAL '30 days'
GROUP BY bucket, device_id
ORDER BY bucket DESC;
```

`time_bucket` là điểm khác biệt quan trọng giữa TimescaleDB và PostgreSQL thuần trong các workload time-series.

## 11. Continuous aggregates

### 11.1 Khi nào cần dùng

Nếu dashboard hoặc báo cáo thường xuyên chạy các truy vấn:

- `GROUP BY` theo giờ/ngày/tháng
- `AVG`, `SUM`, `COUNT`, `MIN`, `MAX`
- trên bảng raw data rất lớn

thì nên tạo continuous aggregate.

### 11.2 Tạo continuous aggregate

```sql
CREATE MATERIALIZED VIEW metrics_1h
WITH (timescaledb.continuous) AS
SELECT
  device_id,
  time_bucket('1 hour', time) AS bucket,
  avg(cpu) AS avg_cpu,
  max(cpu) AS max_cpu,
  min(cpu) AS min_cpu,
  count(*) AS total_rows
FROM metrics
GROUP BY device_id, bucket;
```

Sau đó thêm policy refresh:

```sql
SELECT add_continuous_aggregate_policy(
  'metrics_1h',
  start_offset => INTERVAL '30 days',
  end_offset => INTERVAL '1 hour',
  schedule_interval => INTERVAL '15 minutes'
);
```

Ý nghĩa:

- `start_offset`: refresh lại dữ liệu từ 30 ngày trước tới gần hiện tại
- `end_offset`: chừa ra 1 giờ gần nhất để tránh tổng hợp trên vùng dữ liệu còn đang biến động
- `schedule_interval`: cứ 15 phút chạy refresh một lần

### 11.3 Lưu ý quan trọng

1. `GROUP BY` phải chứa `time_bucket(...)` trên cột thời gian của hypertable

2. Không dùng `time_bucket_gapfill` trực tiếp trong continuous aggregate. Nếu cần gap fill, tạo aggregate bằng `time_bucket` rồi gap fill ở lớp query ngoài

3. Với bảng rất lớn, nên dùng `WITH NO DATA`

```sql
CREATE MATERIALIZED VIEW metrics_1d
WITH (timescaledb.continuous) AS
SELECT
  device_id,
  time_bucket('1 day', time) AS bucket,
  avg(cpu) AS avg_cpu
FROM metrics
GROUP BY device_id, bucket
WITH NO DATA;
```

Sau đó refresh thủ công:

```sql
CALL refresh_continuous_aggregate(
  'metrics_1d',
  NULL,
  localtimestamp - INTERVAL '30 days'
);
```

4. Theo docs mới, để tăng hiệu năng invalidate của continuous aggregate, có thể cấu hình:

```sql
SET timescaledb.invalidate_using = 'wal';
```

5. Continuous aggregate dùng hypertable ở phía sau, nên cũng có chunk riêng của nó

## 12. Data retention

Retention policy giúp tự động xóa dữ liệu cũ trên hypertable.

Ví dụ chỉ giữ raw data trong 90 ngày:

```sql
SELECT add_retention_policy('metrics', INTERVAL '90 days');
```

Điều này phù hợp khi:

- raw data rất lớn
- nhu cầu phân tích chi tiết chỉ cần trong thời gian gần
- dữ liệu cũ đã được tổng hợp qua continuous aggregate

### 12.1 Lưu ý với continuous aggregates

Đây là điểm rất dễ cấu hình sai.

Nếu bạn xóa raw data quá sớm nhưng continuous aggregate vẫn refresh trên khoảng thời gian đó, aggregate có thể mất luôn dữ liệu đã tổng hợp.

Ví dụ:

- raw data giữ 30 ngày
- continuous aggregate refresh vùng 7 ngày gần nhất đến 1 ngày gần nhất

Cấu hình này an toàn hơn việc:

- raw data giữ 1 ngày
- continuous aggregate lại refresh dữ liệu từ 7 ngày trước

Nguyên tắc:

- retention của raw hypertable phải dài hơn vùng mà continuous aggregate còn cần refresh

## 13. Columnstore và compression

Trong tài liệu cũ bạn sẽ thấy khái niệm `compression policy`. Với tài liệu hiện tại, API cũ này được thay thế bởi Hypercore/columnstore.

Mục đích:

- giảm chi phí lưu trữ
- tăng hiệu năng các truy vấn phân tích trên dữ liệu cũ

Ví dụ thêm columnstore policy:

```sql
CALL add_columnstore_policy('metrics', after => INTERVAL '30 days');
```

Ý nghĩa:

- các chunk cũ hơn 30 ngày được chuyển sang columnstore

Lưu ý:

- dữ liệu đang nóng, còn update nhiều thì không nên chuyển quá sớm
- dữ liệu càng cũ, càng ít sửa, càng phù hợp với columnstore
- docs mới có cả API cũ và mới, cần ưu tiên cách dùng Hypercore/columnstore thay vì compression API cũ

## 14. Tiering dữ liệu cũ

Tiering phù hợp khi dữ liệu đã rất cũ, hiếm khi truy cập, nhưng vẫn cần khả năng query.

Theo docs hiện tại, trên Tiger Cloud việc enable tiering được thực hiện qua Console trước, sau đó mới thêm SQL policy.

Ví dụ:

```sql
SELECT add_tiering_policy('metrics', INTERVAL '180 days');
```

Điều này có nghĩa:

- chunk cũ hơn 180 ngày sẽ được chuyển sang tầng lưu trữ rẻ hơn

Khi cần query dữ liệu tiered:

```sql
SET timescaledb.enable_tiered_reads = true;
```

Lưu ý vận hành:

- dữ liệu tiered vẫn đọc được
- dữ liệu tiered không phù hợp cho cập nhật thường xuyên
- chỉ tier dữ liệu thật sự lạnh
- nếu self-host, cần kiểm tra rõ edition, deployment model và tính năng hỗ trợ trước khi áp dụng

## 15. Indexing và tối ưu truy vấn

### 15.1 Default indexes

Khi tạo hypertable, TimescaleDB thường tự tạo index mặc định theo cột thời gian.

### 15.2 Nên thêm index nào

Phụ thuộc query thực tế. Ví dụ:

```sql
CREATE INDEX IF NOT EXISTS ix_metrics_device_time_desc
ON metrics (device_id, time DESC);
```

Index này hữu ích cho truy vấn:

- lấy dữ liệu gần nhất của một thiết bị
- lọc theo `device_id` và khoảng thời gian

### 15.3 Các nguyên tắc quan trọng

1. Luôn có điều kiện thời gian trong query nếu có thể

2. Index bừa bãi sẽ làm ingest chậm

3. Với time-series, pattern query thường là:

- theo khoảng thời gian
- theo nguồn dữ liệu như `device_id`, `tenant_id`, `site_id`

4. Nếu query chủ yếu đọc dữ liệu tổng hợp, nên dùng continuous aggregate thay vì cố tối ưu raw query quá mức

## 16. Theo dõi hypertable, chunk và jobs

Một số truy vấn hữu ích:

### 16.1 Liệt kê hypertable

```sql
SELECT *
FROM timescaledb_information.hypertables;
```

### 16.2 Xem chunk

```sql
SELECT *
FROM timescaledb_information.chunks
ORDER BY range_start DESC;
```

### 16.3 Xem jobs nền

```sql
SELECT *
FROM timescaledb_information.jobs;
```

Các jobs này thường bao gồm:

- refresh continuous aggregate
- retention
- columnstore/compression
- tiering

### 16.4 Kiểm tra chunk cũ

```sql
SELECT show_chunks('metrics', older_than => INTERVAL '30 days');
```

## 17. Chuyển đổi bảng PostgreSQL hiện có sang hypertable

Nếu đã có bảng PostgreSQL thường và muốn chuyển sang TimescaleDB:

```sql
CREATE TABLE metrics_legacy (
  time        TIMESTAMPTZ NOT NULL,
  device_id   TEXT        NOT NULL,
  cpu         DOUBLE PRECISION
);

SELECT create_hypertable(
  'metrics_legacy',
  by_range('time'),
  migrate_data => TRUE
);
```

Lưu ý:

- `migrate_data => TRUE` có thể khóa bảng khá lâu
- với bảng lớn, nên chạy trong maintenance window
- foreign key có thể gây hạn chế hoặc dẫn tới deadlock nếu migrate không cẩn thận
- không dùng `create_hypertable()` trên bảng đã partition bằng declarative partitioning của PostgreSQL

Khuyến nghị:

- hệ thống mới: tạo hypertable ngay từ đầu
- hệ thống đang chạy: lên kế hoạch migration rõ ràng, đo thời gian migrate trên môi trường staging trước

## 18. Sử dụng với .NET, Npgsql và EF Core

Repo này đang dùng .NET và PostgreSQL, nên đây là cách tiếp cận thực tế nhất.

### 18.1 Điều gì vẫn dùng EF Core bình thường

- entity mapping
- CRUD
- transaction
- query quan hệ thông thường
- migration tạo bảng cơ bản

### 18.2 Điều gì nên dùng raw SQL

- `CREATE EXTENSION timescaledb`
- `create_hypertable(...)`
- `time_bucket(...)`
- `add_continuous_aggregate_policy(...)`
- `add_retention_policy(...)`
- `add_tiering_policy(...)`
- `add_columnstore_policy(...)`

Lý do:

- ORM không mô hình hóa đầy đủ các khái niệm đặc thù của TimescaleDB
- nhiều tính năng là function/procedure chuyên biệt
- migration raw SQL giúp kiểm soát tốt hơn

### 18.3 Ví dụ migration với EF Core

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb;");

    migrationBuilder.Sql("""
        CREATE TABLE metrics (
            time timestamptz NOT NULL,
            device_id text NOT NULL,
            cpu double precision NULL,
            memory double precision NULL
        );
    """);

    migrationBuilder.Sql("""
        SELECT create_hypertable(
            'metrics',
            by_range('time', INTERVAL '1 day'),
            if_not_exists => TRUE
        );
    """);

    migrationBuilder.Sql("""
        CREATE INDEX IF NOT EXISTS ix_metrics_device_time_desc
        ON metrics (device_id, time DESC);
    """);
}
```

### 18.4 Ví dụ query aggregate bằng raw SQL

```csharp
const string sql = """
    SELECT
        time_bucket('1 hour', time) AS bucket,
        device_id,
        avg(cpu) AS avg_cpu
    FROM metrics
    WHERE time >= now() - interval '7 days'
    GROUP BY bucket, device_id
    ORDER BY bucket DESC, device_id
""";
```

Thực tế triển khai:

- CRUD thường: EF Core
- analytics/time-series query: raw SQL hoặc Dapper

## 19. Ví dụ end-to-end

Ví dụ hoàn chỉnh cho một pipeline tối thiểu:

### 19.1 Tạo hypertable

```sql
CREATE EXTENSION IF NOT EXISTS timescaledb;

CREATE TABLE metrics (
  time        TIMESTAMPTZ      NOT NULL,
  device_id   TEXT             NOT NULL,
  cpu         DOUBLE PRECISION,
  memory      DOUBLE PRECISION,
  temperature DOUBLE PRECISION
) WITH (
  tsdb.hypertable,
  tsdb.chunk_interval = '1 day',
  tsdb.segmentby = 'device_id',
  tsdb.orderby = 'time DESC'
);

CREATE INDEX IF NOT EXISTS ix_metrics_device_time_desc
ON metrics (device_id, time DESC);
```

### 19.2 Ghi dữ liệu

```sql
INSERT INTO metrics (time, device_id, cpu, memory, temperature)
VALUES
  (now() - INTERVAL '10 minutes', 'device-01', 0.32, 0.61, 29.0),
  (now() - INTERVAL '8 minutes',  'device-01', 0.41, 0.64, 29.2),
  (now() - INTERVAL '7 minutes',  'device-02', 0.76, 0.83, 35.1),
  (now() - INTERVAL '5 minutes',  'device-01', 0.28, 0.58, 28.8),
  (now() - INTERVAL '3 minutes',  'device-02', 0.80, 0.85, 35.4);
```

### 19.3 Query theo bucket

```sql
SELECT
  time_bucket('5 minutes', time) AS bucket,
  device_id,
  avg(cpu) AS avg_cpu,
  max(temperature) AS max_temp
FROM metrics
GROUP BY bucket, device_id
ORDER BY bucket DESC, device_id;
```

### 19.4 Tạo continuous aggregate theo giờ

```sql
CREATE MATERIALIZED VIEW metrics_1h
WITH (timescaledb.continuous) AS
SELECT
  device_id,
  time_bucket('1 hour', time) AS bucket,
  avg(cpu) AS avg_cpu,
  max(cpu) AS max_cpu,
  min(cpu) AS min_cpu,
  count(*) AS total_rows
FROM metrics
GROUP BY device_id, bucket;

SELECT add_continuous_aggregate_policy(
  'metrics_1h',
  start_offset => INTERVAL '30 days',
  end_offset => INTERVAL '1 hour',
  schedule_interval => INTERVAL '15 minutes'
);
```

### 19.5 Chỉ giữ raw data 90 ngày

```sql
SELECT add_retention_policy('metrics', INTERVAL '90 days');
```

### 19.6 Chuyển dữ liệu cũ sang columnstore sau 30 ngày

```sql
CALL add_columnstore_policy('metrics', after => INTERVAL '30 days');
```

### 19.7 Tier dữ liệu sau 180 ngày nếu hạ tầng hỗ trợ

```sql
SELECT add_tiering_policy('metrics', INTERVAL '180 days');
```

## 20. Khuyến nghị thực tế

### 20.1 Mô hình lưu trữ phổ biến

Với dữ liệu metrics hoặc logs:

- raw hypertable: giữ 30 đến 90 ngày
- continuous aggregate theo giờ: giữ 6 đến 12 tháng
- continuous aggregate theo ngày: giữ nhiều năm
- columnstore cho dữ liệu cũ
- tiering cho dữ liệu rất lạnh

### 20.2 Query pattern nên ưu tiên

- luôn filter theo khoảng thời gian
- query dashboard đọc continuous aggregate nếu có thể
- chỉ dùng raw hypertable cho dữ liệu gần hiện tại hoặc truy vấn forensic chi tiết

### 20.3 Không nên làm

- biến mọi bảng thành hypertable
- đặt quá nhiều index trên bảng ingest lớn
- xóa raw data quá sớm khi continuous aggregate vẫn còn refresh vào vùng đó
- tier dữ liệu còn bị cập nhật thường xuyên
- cố ép mọi function TimescaleDB đi qua ORM abstraction

## 21. Các lỗi thường gặp

### 21.1 Quên bật extension

Triệu chứng:

- không gọi được `create_hypertable`
- không tạo được bảng với `tsdb.hypertable`

Cách xử lý:

- kiểm tra `shared_preload_libraries`
- restart PostgreSQL
- chạy `CREATE EXTENSION IF NOT EXISTS timescaledb`

### 21.2 Thiết kế khóa chính sai

Triệu chứng:

- lỗi khi tạo `PRIMARY KEY` hoặc `UNIQUE`

Nguyên nhân:

- khóa không chứa cột thời gian partition

### 21.3 Chunk interval không phù hợp

Triệu chứng:

- quá nhiều chunk nhỏ
- maintenance chậm
- query không ổn định

Cách xử lý:

- xem lại lưu lượng dữ liệu thực tế
- điều chỉnh `chunk_interval` phù hợp hơn

### 21.4 Continuous aggregate bị thiếu dữ liệu

Nguyên nhân thường gặp:

- dùng `WITH NO DATA` nhưng chưa refresh
- retention xóa raw data trước khi aggregate refresh xong

### 21.5 Dùng API cũ và mới lẫn lộn

Ví dụ:

- tài liệu cũ dùng compression API cũ
- tài liệu mới dùng Hypercore/columnstore

Khuyến nghị:

- khi đọc docs hoặc blog, luôn đối chiếu version của TimescaleDB đang chạy

## 22. Tài liệu chính thức nên tham khảo thêm

- `CREATE TABLE` cho hypertable: <https://www.tigerdata.com/docs/api/latest/hypertable/create_table>
- `create_hypertable()`: <https://www.tigerdata.com/docs/api/latest/hypertable/create_hypertable>
- `time_bucket()`: <https://www.tigerdata.com/docs/api/latest/hyperfunctions/time_bucket>
- Continuous aggregates: <https://www.tigerdata.com/docs/use-timescale/latest/continuous-aggregates/create-a-continuous-aggregate>
- Data retention: <https://www.tigerdata.com/docs/use-timescale/latest/data-retention/create-a-retention-policy>
- Retention với continuous aggregates: <https://www.tigerdata.com/docs/use-timescale/latest/data-retention/data-retention-with-continuous-aggregates>
- Columnstore policy: <https://www.tigerdata.com/docs/api/latest/hypercore/add_columnstore_policy>
- Tiering: <https://www.tigerdata.com/docs/use-timescale/latest/data-tiering/enabling-data-tiering>
- TimescaleDB Toolkit: <https://www.tigerdata.com/docs/self-hosted/latest/tooling/install-toolkit>

## Ghi chú cuối

Trong docs mới, nhà cung cấp sử dụng thương hiệu Tiger Data cho phần tài liệu và dịch vụ cloud, nhưng extension và cách dùng trong PostgreSQL vẫn là `timescaledb`.
