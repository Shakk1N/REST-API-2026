# Контрольна робота №1 — Web API на ASP.NET Core з багатошаровою архітектурою

## Тема: Спортзал (Сутність: `Exercise`)

## Мета роботи

Самостійно створити Web API для власної предметної області, застосувавши підходи, розглянуті на заняттях: поділ рішення на шари (Domain, Application, Infrastructure, Api), роботу з базою даних через EF Core, CQRS-підхід із MediatR, валідацію вхідних даних за допомогою FluentValidation, функціональну обробку помилок за допомогою LanguageExt та типізовані ідентифікатори.

---

## Технології

- **Платформа:** .NET 10, ASP.NET Core Web API (контролери)
- **СУБД:** PostgreSQL
- **ORM:** Entity Framework Core 10 + Npgsql
- **CQRS / Mediator:** MediatR 14
- **Валідація:** FluentValidation 12
- **Функціональна обробка помилок:** LanguageExt 4 (`Option<T>`, `Either<L, R>`)
- **Документація API:** Swagger (Swashbuckle)

---

## Архітектура рішення

Рішення складається з чотирьох проєктів із залежностями, спрямованими всередину:

```
Api → Infrastructure → Application → Domain
```

| Проєкт           | Відповідальність                                                                 |
| ---------------- | -------------------------------------------------------------------------------- |
| `Domain`         | Сутність `Exercise`, типізований `ExerciseId`, перелік `MuscleGroup`             |
| `Application`    | Команди MediatR, валідатори, інтерфейси репозиторію та запитів, ієрархія помилок |
| `Infrastructure` | EF Core, PostgreSQL, репозиторій, конфігурація, міграції                         |
| `Api`            | Контролер, DTO, обробка помилок → HTTP-коди, Swagger, CORS                       |

---

## Структура проєкту

```
control-work-1/
└── src/
    ├── Domain/
    │   └── Exercises/
    │       ├── Exercise.cs          # Сутність (інкапсульована, фабричний метод)
    │       ├── ExerciseId.cs        # Типізований ідентифікатор (record + Guid)
    │       └── MuscleGroup.cs       # Enum: Chest, Back, Legs, Arms, Shoulders
    ├── Application/
    │   ├── Common/
    │   │   ├── Behaviours/
    │   │   │   └── ValidationBehaviour.cs   # MediatR pipeline — запуск валідаторів
    │   │   └── Interfaces/
    │   │       ├── Queries/
    │   │       │   └── IExerciseQueries.cs  # GetAll, GetById → Option<T>
    │   │       └── Repositories/
    │   │           └── IExerciseRepository.cs # GetById, GetByName, Add, Update
    │   └── Exercises/
    │       ├── Commands/
    │       │   ├── CreateExerciseCommand.cs          # Команда + обробник
    │       │   ├── CreateExerciseCommandValidator.cs  # Валідатор FluentValidation
    │       │   ├── UpdateExerciseCommand.cs          # Команда + обробник
    │       │   └── UpdateExerciseCommandValidator.cs  # Валідатор FluentValidation
    │       └── Exceptions/
    │           └── ExerciseExceptions.cs  # AlreadyExists, NotFound, Unhandled
    ├── Infrastructure/
    │   ├── Migrations/              # EF Core міграції
    │   └── Persistence/
    │       ├── ApplicationDbContext.cs
    │       ├── ApplicationDbContextInitialiser.cs   # Автозастосування міграцій
    │       ├── Configurations/
    │       │   └── ExerciseConfiguration.cs         # IEntityTypeConfiguration
    │       ├── Converters/
    │       │   └── DateTimeUtcConverter.cs          # UTC-конвертер для дат
    │       ├── Repositories/
    │       │   └── ExerciseRepository.cs            # IExerciseRepository + IExerciseQueries
    │       └── ConfigurePersistenceServices.cs
    └── Api/
        ├── Controllers/
        │   └── ExercisesController.cs   # GET/POST/PUT ендпоїнти
        ├── Dtos/
        │   └── ExerciseDtos.cs          # ExerciseDto, CreateExerciseDto, UpdateExerciseDto
        └── Modules/
            ├── Errors/
            │   ├── ExerciseErrorFactory.cs      # Помилки → HTTP 409/404/500
            │   └── ValidationExceptionFilter.cs # FluentValidation → HTTP 400
            ├── DbModule.cs      # Розширення для InitialiseDatabaseAsync
            └── SetupModule.cs   # CORS, контролери, фільтри
```
