using SQLite;

namespace Pillstacular.Models;

public class Medication
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    public string Nickname { get; set; } = string.Empty;

    public double Strength { get; set; }

    public string Unit { get; set; } = string.Empty;

    public MedicationForm Form { get; set; }

    public double DefaultQuantity { get; set; } = 1;

    public bool IsAsNeeded { get; set; }

    public bool IsActive { get; set; } = true;
}
