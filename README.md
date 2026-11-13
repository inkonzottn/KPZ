# OnlineCourses

![CI](https://github.com/ЛОГІН/РЕПО/actions/workflows/ci.yml/badge.svg)
Навчальний проєкт із дисципліни «Конструювання програмного забезпечення». Варіант № 17: Реєстрація слухачів на онлайн-курси.

## Склад рішення
- `OnlineCourses.Core` — логіка предметної області (бібліотека)
- `OnlineCourses.App` — консольний застосунок (введення, виведення)

## Вимоги
.NET SDK 8.0 або новіший, Git.

## Складання та запуск

```
dotnet build
dotnet run --project OnlineCourses.App
```

## Стандарт кодування
Правила описано у файлі `CODING_STANDARD.md` і закріплено в `.editorconfig`. 
Перевірка: 
```
dotnet format --verify-no-changes
```

## Конвеєр CI
На кожен push і pull request запускаються: перевірка форматування (`dotnet format --verify-no-changes`), складання Release з аналізаторами (попередження як помилки, NuGetAudit), тести та покриття з порогом 60 %.

## Автор
Хромова Анастасія, група 471