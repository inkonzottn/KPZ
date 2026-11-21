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
dotnet run --project OnlineCourses.App
```

## 3. Структура рішення

- `OnlineCourses.Core` — доменні сутності, контракти, прикладні сервіси, цінові політики та звітність.
- `OnlineCourses.App` — консольний інтерфейс і композиційний корінь.
- `OnlineCourses.Tests` — модульні тести логіки.
- `tests/Legacy.Tests` — тести попередніх лабораторних робіт.
- `data/` — умовні демонстраційні дані.
- `docs/` — специфікація, архітектура, посібник і інженерні рішення.

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
dotnet test OnlineCourses.Tests/OnlineCourses.Tests.csproj -c Release
dotnet test tests/Legacy.Tests/OnlineCourses.Legacy.Tests.csproj -c Release
```

Для локальної перевірки, еквівалентної основним quality gates CI:

```bash
dotnet restore OnlineCourses.slnx && dotnet format OnlineCourses.slnx --verify-no-changes --no-restore && dotnet build OnlineCourses.slnx -c Release --no-restore -warnaserror && dotnet test tests/Legacy.Tests/OnlineCourses.Legacy.Tests.csproj -c Release --no-build && dotnet test OnlineCourses.Tests/OnlineCourses.Tests.csproj -c Release --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=70 "/p:ThresholdType=line%2cbranch"
```

CI перевіряє 70 % для рядкового та гілкового покриття вибраних класів логіки. Межа вимірювання задана через Include у `.github/workflows/ci.yml`; консоль, форматування й файлове сховище не входять до цього набору. Фактичні line та branch coverage потрібно брати з артефакту конкретного успішного запуску, а не з попереднього звіту.

## 6. Технології

C# 12, .NET 8, xUnit, Coverlet, Roslyn/.NET analyzers, StyleCop.Analyzers, GitHub Actions та System.Text.Json.

## 7. Конвеєр безперервної інтеграції

Файл `.github/workflows/ci.yml` відновлює залежності, перевіряє форматування, збирає Release із забороною попереджень, запускає основні й Legacy-тести, перевіряє поріг покриття та публікує Cobertura як артефакт. Перевірка запускається для push у `main`, `feature/**`, `work/**`, `ci-demo` і для pull request до `main`.

## 8. Обмеження та відомий технічний борг

Застосунок не має вебінтерфейсу, бази даних або інтеграції з платіжними сервісами. Пакетні ставки 0 %, 5 % і 10 % є припущенням, оскільки методичка не задає точних значень. Поточний репозиторій має попередні навчальні класи, які не використовуються новим сценарієм OnlineCourses. Після завершення інтеграції файлового сховища потрібно перевірити резервне копіювання й поведінку при пошкодженому JSON. Повний перелік боргу та оцінки часу підтримуються в `docs/tech-debt.md` і `docs/self-check.md`.
