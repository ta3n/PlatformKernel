# PlatformKernel.TimescaleDbDemo

Sample project này dùng để kiểm thử tài liệu TimescaleDB trong repo bằng implementation thật.

Mục tiêu:

- bootstrap một database TimescaleDB theo đúng tài liệu
- ghi dữ liệu bằng EF Core
- query analytics bằng Dapper
- tạo `hypertable`, `time_bucket`, `continuous aggregate`, retention policy, columnstore policy
- kiểm tra migration từ bảng PostgreSQL thường sang hypertable
- probe capability của tiering trong môi trường local

## Chạy bằng Docker Compose

```bash
docker compose \
  --env-file src/timescaledb-demo/docker/ccu-10k.env.example \
  -f src/timescaledb-demo/docker/docker-compose.yml \
  up -d
```

Endpoint local mặc định:

- qua PgBouncer: `localhost:56432`
- direct vào TimescaleDB: `localhost:55432`
- database: `timescale_demo`
- user: `postgres`
- password: `postgres`

Sample app mặc định kết nối qua PgBouncer trên port `56432`. Port `55432` giữ lại để debug/troubleshoot trực tiếp vào TimescaleDB khi cần.

PgBouncer trong sample được cấu hình `AUTH_TYPE=scram-sha-256` để tương thích với PostgreSQL 17/TimescaleDB image, vốn lưu password dạng
`SCRAM-SHA-256`. Nếu đang chạy stack cũ và thấy lỗi `server login failed: wrong password type`, recreate lại riêng container `pgbouncer` sau
khi cập nhật config:

```bash
docker compose \
  --env-file src/timescaledb-demo/docker/ccu-10k.env.example \
  -f src/timescaledb-demo/docker/docker-compose.yml \
  up -d --force-recreate pgbouncer
```

Thiết lập DBeaver nên dùng:

- host: `localhost`
- port: `56432`
- database: `timescale_demo`
- user: `postgres`
- password: `postgres`

PgBouncer cũng publish thêm alias database `postgres` để các client thích default DB này vẫn vào được, nhưng data demo nằm trong
`timescale_demo`.

Profile trong sample này đã được nâng theo hướng `10k+ CCU` với giả định:

- ứng dụng dùng `PgBouncer` ở `transaction pooling`
- phần lớn CCU là user đang giữ kết nối ứng dụng, không phải `10,000` query SQL chạy đồng thời
- transaction ngắn, không phụ thuộc session state của PostgreSQL

Các giá trị tune nằm
trong [ccu-10k.env.example](/Users/tanguyen/Workspaces/Psnl/Github/PlatformKernel/src/timescaledb-demo/docker/ccu-10k.env.example). Trước
khi dùng production thật, cần chỉnh lại theo CPU, RAM, disk IOPS và kết quả load test thực tế.

## Chạy bằng .NET Aspire

```bash
dotnet run --project src/timescaledb-demo/PlatformKernel.TimescaleDbDemo.AppHost
```

AppHost sẽ dựng:

- `timescaledb`: server TimescaleDB direct
- `pgbouncer`: pooler đứng trước TimescaleDB
- `timescaledb-demo`: resource chạy sample console app, được inject `TIMESCALE_CONNECTION_STRING` trỏ tới PgBouncer

`timescaledb-demo` được đánh dấu `explicit start`, nên AppHost chỉ dựng hạ tầng trước. Khi cần chạy sample end-to-end, start resource đó từ
Aspire dashboard.

AppHost đọc cùng nhóm env var với Docker Compose như `POSTGRES_*`, `TIMESCALE_TUNE_*`, `TIMESCALE_SHM_SIZE`, `PGBOUNCER_*`, nên có thể dùng
chung một profile tune khi chạy local hoặc CI.

## Chạy sample

```bash
TIMESCALE_DB_PASSWORD=postgres \
dotnet run --project src/timescaledb-demo/PlatformKernel.TimescaleDbDemo -- demo
```

Các lệnh hỗ trợ:

- `bootstrap`: tạo extension, hypertable, continuous aggregate và policies
- `seed`: nạp dữ liệu mẫu bằng EF Core
- `summary`: đọc snapshot hiện tại từ `timescaledb_information` và các query demo
- `legacy`: chạy luồng migrate từ PostgreSQL table thường sang hypertable
- `tiering`: probe khả năng dùng `add_tiering_policy`
- `demo`: chạy trọn bộ

Nếu muốn đổi connection string:

```bash
TIMESCALE_CONNECTION_STRING="Host=localhost;Port=56432;Database=timescale_demo;Username=postgres;Password=postgres" \
dotnet run --project src/timescaledb-demo/PlatformKernel.TimescaleDbDemo -- demo
```

## Chạy integration tests

```bash
dotnet test src/timescaledb-demo/PlatformKernel.TimescaleDbDemo.Tests
```

Test dùng `Testcontainers` và tự khởi tạo một container TimescaleDB riêng, không phụ thuộc vào Docker Compose.
