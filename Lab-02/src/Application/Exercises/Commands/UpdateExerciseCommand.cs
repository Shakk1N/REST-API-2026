using Application.Common.Interfaces.Repositories;
using Application.Exercises.Exceptions;
using Domain.Exercises;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Exercises.Commands;

public record UpdateExerciseCommand : IRequest<Either<ExerciseException, Exercise>>
{
    public required Guid ExerciseId { get; init; }
    public required string Name { get; init; }
    public required decimal CaloriesPerHour { get; init; }
    public required bool RequiresEquipment { get; init; }
    public required MuscleGroup MuscleGroup { get; init; }
}

public class UpdateExerciseCommandHandler(IExerciseRepository exerciseRepository)
    : IRequestHandler<UpdateExerciseCommand, Either<ExerciseException, Exercise>>
{
    public async Task<Either<ExerciseException, Exercise>> Handle(
        UpdateExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        var exerciseId = new ExerciseId(request.ExerciseId);
        var exercise = await exerciseRepository.GetById(exerciseId, cancellationToken);

        return await exercise.MatchAsync(
            e =>
                CheckDuplicates(e.Id, request.Name, cancellationToken)
                    .BindAsync(_ => UpdateEntity(e, request, cancellationToken)),
            () => new ExerciseNotFoundException(exerciseId)
        );
    }

    private async Task<Either<ExerciseException, Unit>> CheckDuplicates(
        ExerciseId currentId,
        string name,
        CancellationToken cancellationToken
    )
    {
        var exercise = await exerciseRepository.GetByName(name, cancellationToken);

        return exercise.Match<Either<ExerciseException, Unit>>(
            e => e.Id == currentId ? Unit.Default : new ExerciseAlreadyExistsException(e.Id),
            () => Unit.Default
        );
    }

    private async Task<Either<ExerciseException, Exercise>> UpdateEntity(
        Exercise exercise,
        UpdateExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            exercise.UpdateDetails(
                request.Name,
                request.CaloriesPerHour,
                request.RequiresEquipment,
                request.MuscleGroup
            );

            return await exerciseRepository.Update(exercise, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ExerciseUnhandledException(exercise.Id, exception);
        }
    }
}
