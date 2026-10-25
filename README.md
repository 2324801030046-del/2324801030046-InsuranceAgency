# Bài thực hành 5 và 6 – Insurance Agency

Dự án kế thừa solution bảo hiểm BT4, bổ sung PricingService và Eureka ở Bài 5, sau đó cấu hình Agent Portal API Gateway dùng Ocelot ở Bài 6.

## Công nghệ

- .NET 8 / ASP.NET Core
- Steeltoe Discovery Client và Eureka 3.2.7
- Marten, PostgreSQL, MediatR, FluentValidation
- xUnit, Alba và Testcontainers
- Eureka Server chạy bằng Docker
- Ocelot 23.3.3, Ocelot Provider Eureka và Ocelot Cache Manager cho Agent Portal API Gateway

## Bài thực hành 6 – API Gateway

`AgentPortalApiGateway/` dùng Ocelot và Eureka Service Discovery. Gateway chạy tại `http://localhost:8099`; tuyến `GET /Products/{code}` chuyển tiếp tới `/api/Products/{code}` trên service Eureka có tên `ProductService`. Các tuyến Product hiện yêu cầu JWT hợp lệ có claim `userType=SALESMAN`.

Khởi động Eureka:

```powershell
docker compose up -d eureka
```

Chạy Gateway trong PowerShell. Tạo khóa chỉ dùng trên máy cá nhân; không đưa khóa này vào Git:

```powershell
$env:Jwt__SigningKey = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
dotnet run --project AgentPortalApiGateway/AgentPortalApiGateway.csproj --launch-profile AgentPortalApiGateway
```

Kiểm tra trang Eureka tại [http://localhost:8761](http://localhost:8761). Để kiểm thử tuyến `/Products/{code}` thực tế, cần ProductService đang chạy, đã đăng ký tên `ProductService` trong Eureka, cùng JWT có claim `userType=SALESMAN`. Workspace hiện chỉ có project `ProductService.Api` dạng thư viện dùng chung; chưa có ứng dụng ProductService chạy và đăng ký với Eureka để xác nhận chuyển tiếp đầu cuối.

## AuthService

AuthService chạy tại `http://localhost:6060` và cung cấp `POST /api/User` để đăng nhập, `GET /api/User` để lấy thông tin đại lý. Tài khoản mẫu trong bộ nhớ gồm `jimmy.solid` / `secret`, `danny.solid` / `secret` và `admin` / `admin`; chỉ dùng cho bài thực hành.

## Chat Service

Chat Service chạy tại `http://localhost:4099`, đăng ký với Eureka dưới tên `ChatService`, phát SignalR Hub tại `/agentsChat` và nhận sự kiện `PolicyCreated` qua RabbitMQ. Endpoint gửi thông báo là `POST /api/Notification`.

## Dashboard Service

Dashboard Service chạy tại `http://localhost:5035`, đăng ký với Eureka dưới tên `DashboardService`, đọc dữ liệu từ Elasticsearch và nhận sự kiện `PolicyCreated` qua RabbitMQ. Các endpoint POST nằm dưới `/api/Dashboard`: `agents-sales`, `total-sales` và `sales-trends`. Mã mẫu xóa rồi tạo lại index `policy_lab_stats` khi khởi động.

## Chạy các service bài 6

Sau khi tạo `.env` như phần PostgreSQL bên dưới, chạy các dependency dùng chung:

```powershell
docker compose up -d eureka rabbitmq elasticsearch
```

AuthService, Chat Service và API Gateway cần cùng một `Jwt__SigningKey`. Tạo khóa trong PowerShell và đặt cùng giá trị trong mỗi terminal chạy service; không lưu khóa vào source code:

```powershell
$env:Jwt__SigningKey = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
```

Chạy mỗi ứng dụng trong một terminal riêng:

```powershell
dotnet run --project AuthService/AuthService.csproj --launch-profile AuthService
dotnet run --project ChatService/ChatService.csproj --launch-profile ChatService
dotnet run --project DashboardService/DashboardService.csproj --launch-profile DashboardService
```

Swagger bật trong Development. Chat cần RabbitMQ; Dashboard cần RabbitMQ, Elasticsearch và Eureka. Workspace chưa có Policy Service đang chạy để phát sự kiện `PolicyCreated`, nên luồng xử lý sự kiện chưa được kiểm thử đầu cuối.

`BlazorWasmClient` chưa nằm trong `InsuranceAgency.sln`; build riêng hiện còn lỗi vì các contract Policy, Policy Search và Product mà UI tham chiếu vẫn là project placeholder trong workspace. Các query contract Dashboard của mẫu đã được bổ sung.

## Cấu trúc liên quan

- `PricingService.Api/`: command, DTO và converter dùng chung.
- `PricingService/`: ASP.NET Core API, nghiệp vụ định giá, cấu hình Eureka và Marten.
- `PricingService.Test/`: unit test.
- `PricingService.IntegrationTest/`: integration test; cần Docker Engine hoạt động.
- `eureka-server/`: mã build Eureka Server.
- `AgentPortalApiGateway/`: API Gateway Ocelot cho các backend được đăng ký trong Eureka.
- `AuthService/`, `ChatService/`, `DashboardService/`: các API services được bổ sung theo source mẫu.
- `ChatService.Api/`, `DashboardService.Api/`: contracts dùng chung giữa services và client.
- `DashboardService.Test/`: unit/integration tests của Dashboard Service.

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
dotnet test DashboardService.Test/DashboardService.Test.csproj
dotnet test PricingService.Test/PricingService.Test.csproj
dotnet test PricingService.IntegrationTest/PricingService.IntegrationTest.csproj
```

Integration test dùng Testcontainers để khởi chạy Elasticsearch hoặc PostgreSQL, vì vậy cần Docker Engine hoạt động.

## Ghi chú

Steeltoe và Marten phiên bản hiện tại được NuGet cảnh báo lỗ hổng bảo mật. Các phiên bản này đang được giữ theo dependencies của solution; cần đánh giá tương thích trước khi nâng cấp.
