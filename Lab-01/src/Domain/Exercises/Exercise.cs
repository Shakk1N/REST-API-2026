namespace Domain.Exercises;

public class Exercise
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public decimal CaloriesPerHour { get; private set; }
    public bool RequiresEquipment { get; private set; }
    public MuscleGroup MuscleGroup { get; private set; }
    public DateTime CreatedAt { get; }




    private Exercise(
        Guid id,
        string name,
        decimal caloriesPerHour,
        bool requiresEquipment,
        MuscleGroup muscleGroup,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        CaloriesPerHour = caloriesPerHour;
        RequiresEquipment = requiresEquipment;
        MuscleGroup = muscleGroup;
        CreatedAt = createdAt;
    }



    public static Exercise New(
            Guid id, 
            string name, 
            decimal caloriesPerHour, 
            bool requiresEquipment, 
            MuscleGroup muscleGroup
    )
    {
        return new Exercise(id, name, caloriesPerHour, requiresEquipment, muscleGroup, DateTime.UtcNow);
    }



    public void UpdateDetails(
        string name,
        decimal caloriesPerHour,
        bool requiresEquipment,
        MuscleGroup muscleGroup)
    {
        Name = name;
        CaloriesPerHour = caloriesPerHour;
        RequiresEquipment = requiresEquipment;
        MuscleGroup = muscleGroup;
    }
    
    
    
}