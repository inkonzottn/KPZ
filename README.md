# OnlineCourses

Консольний застосунок для обліку зарахувань слухачів на онлайн-курси та наповнюваності навчальних груп.

## 1. Відомості про роботу

- Варіант: № 17.
- Виконавець: [Прізвище та ім'я].
- Група: [Номер групи].
- Дисципліна: «Конструювання програмного забезпечення».
- Репозиторій: https://github.com/inkonzottn/KPZ.

## 2. Швидкий старт

Потрібен .NET 8 SDK і Git. Виконайте три команди:

```bash
git clone https://github.com/inkonzottn/KPZ.git
cd KPZ && dotnet build OnlineCourses.slnx -c Release
dotnet run --project src/OnlineCourses.App
```

## 3. Структура рішення

- `src/OnlineCourses.Core` — доменні сутності, контракти, прикладні сервіси, цінові політики та звітність.
- `src/OnlineCourses.App` — консольний інтерфейс і композиційний корінь.
- `tests/OnlineCourses.Tests.Current` — модульні тести логіки.
- `tests/Legacy.Tests` — тести попередніх лабораторних робіт.
- `data/` — умовні демонстраційні дані.
- `docs/` — специфікація, архітектура, посібник, аналіз покриття й інженерні рішення.

## 4. Можливості

- UC-01 — зарахування слухача з перевіркою передумов і вільних місць.
- UC-02 — відрахування слухача.
- UC-03 — переведення до іншої групи того самого курсу.
- UC-04 — розрахунок пакетної вартості.
- UC-05 — пошук зарахувань.
- UC-06 — звіт наповнюваності всіх груп, включно з порожніми.
- UC-07 — завершення навчання.

Детальні сценарії та альтернативні гілки: [docs/spec.md](docs/spec.md). Інструкція користувача: [docs/user-guide.md](docs/user-guide.md).

## 5. Тести й покриття

```bash
dotnet test tests/OnlineCourses.Tests.Current/OnlineCourses.Tests.csproj -c Release
dotnet test tests/Legacy.Tests/OnlineCourses.Legacy.Tests.csproj -c Release
```

Для локальної перевірки, еквівалентної основним quality gates CI:

```bash
dotnet restore OnlineCourses.slnx && dotnet format OnlineCourses.slnx --verify-no-changes --no-restore && dotnet build OnlineCourses.slnx -c Release --no-restore -warnaserror && dotnet test tests/Legacy.Tests/OnlineCourses.Legacy.Tests.csproj -c Release --no-build && dotnet test OnlineCourses.Tests/OnlineCourses.Tests.csproj -c Release --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=70 "/p:ThresholdType=line%2cbranch" "/p:Include=[OnlineCourses.Core]OnlineCourses.Core.Pricing.CourseDiscountPolicy%2c[OnlineCourses.Core]OnlineCourses.Core.Services.RegistrationService%2c[OnlineCourses.Core]OnlineCourses.Core.Domain.Registration%2c[OnlineCourses.Core]OnlineCourses.Core.Domain.RegistrationItem%2c[OnlineCourses.Core]OnlineCourses.Core.Domain.Course%2c[OnlineCourses.Core]OnlineCourses.Core.Domain.CourseGroup%2c[OnlineCourses.Core]OnlineCourses.Core.Domain.Enrollment%2c[OnlineCourses.Core]OnlineCourses.Core.Services.EnrollmentService%2c[OnlineCourses.Core]OnlineCourses.Core.Services.InMemoryEnrollmentRepository%2c[OnlineCourses.Core]OnlineCourses.Core.Pricing.CoursePackagePricingPolicy"
```

Останній успішний прогін на `main` — [CI run 44915999371](https://github.com/inkonzottn/KPZ/actions/runs/44915999371), коміт `93ac1c5`. Він підтвердив 121 основний і 17 Legacy-тестів, 95,86 % line coverage, 100 % branch coverage та 92,85 % method coverage для вибраних класів логіки Core. Деталі, межі вимірювання й класифікація контрольних гілок наведені в [аналізі покриття](docs/coverage-analysis.md). CI вимагає не менше 70 % для рядкового й гілкового покриття. Межа вимірювання задана через `Include` у `.github/workflows/ci.yml`; консоль, форматування й файлове сховище не входять до цього набору. Звіт Cobertura збережено як артефакт `coverage` цього прогону.

## 6. Технології

C# 12, .NET 8, xUnit, Coverlet, Roslyn/.NET analyzers, StyleCop.Analyzers, GitHub Actions та System.Text.Json.

## 7. Конвеєр безперервної інтеграції

Файл `.github/workflows/ci.yml` відновлює залежності, перевіряє форматування, збирає Release із забороною попереджень, запускає основні й Legacy-тести, перевіряє поріг покриття та публікує Cobertura як артефакт. Перевірка запускається для push у `main`, `feature/**`, `work/**`, `ci-demo` і для pull request до `main`.

## 8. Обмеження та відомий технічний борг

Застосунок не має вебінтерфейсу, бази даних або інтеграції з платіжними сервісами. Пакетні ставки 0 %, 5 % і 10 % є припущенням, оскільки методичка не задає точних значень. У репозиторії збережено попередні навчальні модулі лабораторних робіт; вони не є частиною основного сценарію OnlineCourses, але входять до Legacy-проєкту й перевіряються окремим набором тестів. JSON-сховище вже інтегроване; робочий файл створюється із seed-файлу, а резервне копіювання описано в посібнику користувача. Повний перелік боргу та оцінки часу підтримуються в `docs/tech-debt.md` і `docs/self-check.md`.
