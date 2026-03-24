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
docker compose -f src/timescaledb-demo/docker/docker-compose.yml up -d
```

Database local mặc định:

- host: `localhost`
- port: `55432`
- database: `timescale_demo`
- user: `postgres`
- password: `postgres`

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
TIMESCALE_CONNECTION_STRING="Host=localhost;Port=55432;Database=timescale_demo;Username=postgres;Password=postgres" \
dotnet run --project src/timescaledb-demo/PlatformKernel.TimescaleDbDemo -- demo
```

## Chạy integration tests

```bash
dotnet test src/timescaledb-demo/PlatformKernel.TimescaleDbDemo.Tests
```

Test dùng `Testcontainers` và tự khởi tạo một container TimescaleDB riêng, không phụ thuộc vào Docker Compose.
