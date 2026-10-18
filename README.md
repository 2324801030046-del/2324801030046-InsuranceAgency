# Bài thực hành 5 – PricingService và Eureka

Dự án kế thừa solution bảo hiểm BT4, bổ sung microservice định giá và đăng ký dịch vụ với Eureka.

## Công nghệ

- .NET 8 / ASP.NET Core
- Steeltoe Discovery Client và Eureka 3.2.7
- Marten, PostgreSQL, MediatR, FluentValidation
- xUnit, Alba và Testcontainers
- Eureka Server chạy bằng Docker

## Cấu trúc liên quan

- `PricingService.Api/`: command, DTO và converter dùng chung.
- `PricingService/`: ASP.NET Core API, nghiệp vụ định giá, cấu hình Eureka và Marten.
- `PricingService.Test/`: unit test.
- `PricingService.IntegrationTest/`: integration test; cần Docker Engine hoạt động.
- `eureka-server/`: mã build Eureka Server.
- `BaoCao/AnhMinhChung/`: ảnh dashboard Eureka và PricingService.

## Chạy Eureka và PostgreSQL

Tạo file `.env` từ `.env.example`, thay hai giá trị mẫu bằng mật khẩu cục bộ, sau đó từ thư mục gốc repository chạy:

```powershell
docker compose up -d eureka postgres
```

PostgreSQL được khởi tạo với database `lab_netmicro_pricing` và user `lab_user` theo script `postgres/createdatabases.sh`. Trước khi chạy PricingService, cấp chuỗi kết nối bằng biến môi trường. Ví dụ PowerShell (thay mật khẩu bằng giá trị trong `.env`):

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=lab_netmicro_pricing;Username=lab_user;Password=<mật-khẩu-cục-bộ>"
```

Mở [http://localhost:8761](http://localhost:8761) để xem Eureka Dashboard.

## Chạy PricingService

```powershell
dotnet restore InsuranceAgency.sln
dotnet run --project PricingService/PricingService.csproj --launch-profile PricingService
```

Launch profile lắng nghe tại `http://localhost:5040`. Eureka client đăng ký tên `PricingService` tại `http://localhost:8761/eureka`; Swagger ở [http://localhost:5040/swagger](http://localhost:5040/swagger).

## Build và test

```powershell
dotnet build InsuranceAgency.sln
dotnet test PricingService.Test/PricingService.Test.csproj
dotnet test PricingService.IntegrationTest/PricingService.IntegrationTest.csproj
```

Integration test dùng Testcontainers để khởi chạy PostgreSQL riêng, vì vậy cần Docker Engine hoạt động.

Khi chạy API Gateway, cần cấp khóa ký JWT riêng qua biến môi trường `Jwt__SigningKey`; khóa không được lưu trong source code.

## Ghi chú

Steeltoe và Marten phiên bản hiện tại được NuGet cảnh báo lỗ hổng bảo mật. Các phiên bản này đang được giữ theo dependencies của solution; cần đánh giá tương thích trước khi nâng cấp.
