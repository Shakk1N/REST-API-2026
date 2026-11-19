using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Exercises;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ExerciseRepository(ApplicationDbContext context)
    : IExerciseRepository,
        IExerciseQueries
{
    public async Task<IReadOnlyList<Exercise>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Exercises.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Option<Exercise>> GetById(ExerciseId id, CancellationToken cancellationToken)
    {
        var entity = await context.Exercises.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken
        );

        return entity ?? Option<Exercise>.None;
    }

    public async Task<Option<Exercise>> GetByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Exercises.FirstOrDefaultAsync(
            x => x.Name == name,
            cancellationToken
        );

        return entity ?? Option<Exercise>.None;
    }

    public async Task<IReadOnlyList<Exercise>> Filter(
        string? name,
        decimal? caloriesPerHourFrom,
        decimal? caloriesPerHourTo,
        bool? requiresEquipment,
        MuscleGroup? muscleGroup,
        DateTime? createdAtFrom,
        DateTime? createdAtTo,
        CancellationToken cancellationToken
    )
    {
        var query = context.Exercises.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{name}%"));
        }

        if (caloriesPerHourFrom.HasValue)
        {
            query = query.Where(x => x.CaloriesPerHour >= caloriesPerHourFrom.Value);
        }

        if (caloriesPerHourTo.HasValue)
        {
            query = query.Where(x => x.CaloriesPerHour <= caloriesPerHourTo.Value);
        }

        if (requiresEquipment.HasValue)
        {
            query = query.Where(x => x.RequiresEquipment == requiresEquipment.Value);
        }

        if (muscleGroup.HasValue)
        {
            query = query.Where(x => x.MuscleGroup == muscleGroup.Value);
        }

        if (createdAtFrom.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(createdAtFrom.Value, DateTimeKind.Utc);
            query = query.Where(x => x.CreatedAt >= fromUtc);
        }

        if (createdAtTo.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(createdAtTo.Value, DateTimeKind.Utc);
            query = query.Where(x => x.CreatedAt <= toUtc);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Exercise> Add(Exercise exercise, CancellationToken cancellationToken)
    {
        await context.Exercises.AddAsync(exercise, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return exercise;
    }

    public async Task<Exercise> Update(Exercise exercise, CancellationToken cancellationToken)
    {
        context.Exercises.Update(exercise);
        await context.SaveChangesAsync(cancellationToken);

        return exercise;
    }
}
