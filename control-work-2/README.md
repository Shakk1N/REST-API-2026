# Контрольна робота №2 — Фільтрація даних та інтеграційне тестування Web API на ASP.NET Core

## Тема: Спортзал (Сутність: `Exercise`)

### Поля сутності `Exercise`:

- `Id` (`ExerciseId`) — типізований ідентифікатор (GUID)
- `Name` (`string`) — назва вправи (унікальне поле)
- `CaloriesPerHour` (`decimal`) — кількість спалюваних калорій за годину (число)
- `RequiresEquipment` (`bool`) — чи потрібне обладнання (логічне значення)
- `MuscleGroup` (`MuscleGroup`) — група м'язів (`Chest`, `Back`, `Legs`, `Arms`, `Shoulders`) (перелік / enum)
- `CreatedAt` (`DateTime`) — дата й час створення запису (UTC)

---

Test summary: total: 23, failed: 0, succeeded: 23, skipped: 0, duration: 11.0s
Build succeeded in 12.5s
