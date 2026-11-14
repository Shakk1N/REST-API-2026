using FluentValidation;
namespace Application.Exercises.Commands;
public class CreateExerciseCommandValidator :
AbstractValidator<CreateExerciseCommand>
{
public CreateExerciseCommandValidator()
{
RuleFor(x => x.Title).NotEmpty().MinimumLength(3).MaximumLength(255);
RuleFor(x => x.Price).InclusiveBetween(0.01m, 1_000_000m);
}
}