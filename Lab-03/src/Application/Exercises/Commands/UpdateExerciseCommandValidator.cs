using FluentValidation;

namespace Application.Exercises.Commands;

public class UpdateExerciseCommandValidator : AbstractValidator<UpdateExerciseCommand>
{
    public UpdateExerciseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.CaloriesPerHour).InclusiveBetween(1m, 5000m);
        RuleFor(x => x.MuscleGroup).IsInEnum();
    }
}
