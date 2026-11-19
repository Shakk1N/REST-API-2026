using Domain.Exercises;

namespace Tests.Data;

public static class ExercisesTestData
{
    public static Exercise FirstTestExercise() =>
        Exercise.New(
            ExerciseId.New(),
            "Push Up",
            300m,
            false,
            MuscleGroup.Chest
        );

    public static Exercise SecondTestExercise() =>
        Exercise.New(
            ExerciseId.New(),
            "Pull Up",
            400m,
            true,
            MuscleGroup.Back
        );

    public static Exercise ThirdTestExercise() =>
        Exercise.New(
            ExerciseId.New(),
            "Barbell Squat",
            550m,
            true,
            MuscleGroup.Legs
        );

    public static Exercise FourthTestExercise() =>
        Exercise.New(
            ExerciseId.New(),
            "Bicep Curl",
            200m,
            true,
            MuscleGroup.Arms
        );
}
