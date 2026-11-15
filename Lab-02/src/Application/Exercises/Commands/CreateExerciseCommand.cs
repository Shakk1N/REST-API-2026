using Application.Common.Interfaces.Repositories;
using Application.Exercises.Exceptions;
using Domain.Exercises;
using LanguageExt;
using MediatR;

namespace Application.Exercises.Commands;

public record CreateExerciseCommand : IRequest<Either<ExerciseException, Exercise>>
{
    public required string Name { get; init; }
    public required decimal CaloriesPerHour { get; init; }
    public required bool RequiresEquipment { get; init; }
    public required MuscleGroup MuscleGroup { get; init; }
}

public class CreateExerciseCommandHandler(IExerciseRepository exerciseRepository)
    : IRequestHandler<CreateExerciseCommand, Either<ExerciseException, Exercise>>
{
    public async Task<Either<ExerciseException, Exercise>> Handle(
        CreateExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        var existing = await exerciseRepository.GetByName(request.Name, cancellationToken);

        return await existing.MatchAsync(
            e => new ExerciseAlreadyExistsException(e.Id),
            () => CreateEntity(request, cancellationToken)
        );
    }

    private async Task<Either<ExerciseException, Exercise>> CreateEntity(
        CreateExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var exercise = Exercise.New(
                ExerciseId.New(),
                request.Name,
                request.CaloriesPerHour,
                request.RequiresEquipment,
                request.MuscleGroup
            );

            return await exerciseRepository.Add(exercise, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ExerciseUnhandledException(ExerciseId.Empty(), exception);
        }
    }
}
