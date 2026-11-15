using FluentValidation;

namespace Application.Exercises.Commands;

public class CreateExerciseCommandValidator : AbstractValidator<CreateExerciseCommand>
{
    public CreateExerciseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.CaloriesPerHour).InclusiveBetween(0.01m, 10_000m);
        RuleFor(x => x.MuscleGroup).IsInEnum();
    }
}
