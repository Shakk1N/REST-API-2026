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
