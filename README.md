# CarPairs

## Екип

| Име | Факултетен номер |
|-----|-----------------|
| Стилиян Илиев | 2501322045 |
| Мартин Мечков | 2501322044 |

## Описание

**CarPairs** е уеб приложение за управление на автомобилни части, производители и категории. Системата е изградена с мулти-тенант архитектура — всяка организация работи в изолирана среда със собствени данни. Администраторите имат достъп до всички организации и могат да управляват потребители, организации и цялостната система чрез административен панел.

Проектът се състои от три слоя:
- **CarPairs.Web** — MVC уеб приложение (потребителски интерфейс)
- **CarPairs.API** — REST API (бизнес логика и автентикация с JWT)
- **CarPairs.Core** — библиотека с данни, модели и услуги (Entity Framework Core + SQL Server)

## Технологии

- .NET 8.0 (ASP.NET Core MVC + Web API)
- Entity Framework Core 8 + SQL Server
- ASP.NET Core Identity
- JWT автентикация (API) / Cookie автентикация (Web)
- Bootstrap 5

## Предварителни изисквания

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (локален или отдалечен)

## Инсталация и стартиране

### 1. Клониране на хранилището

```bash
git clone https://github.com/StiliyanIliev27/CarPairs.git
cd CarPairs
```

### 2. Настройка на базата данни

В `CarPairs.API/appsettings.json` задайте connection string към вашия SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=ВАШИЯТ_СЪРВЪР;Database=CarPairsDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;"
}
```

### 3. Прилагане на миграциите

```bash
dotnet ef database update -p CarPairs.Core -s CarPairs.API
```

### 4. Стартиране на API сървъра

```bash
cd CarPairs.API
dotnet run
```

API-то стартира на `https://localhost:7029`.

### 5. Стартиране на уеб приложението

В нов терминал:

```bash
cd CarPairs
dotnet run
```

Уеб приложението стартира на `https://localhost:7044`.