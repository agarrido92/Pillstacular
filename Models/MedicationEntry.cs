using SQLite;

namespace Pillstacular.Models;

public class MedicationEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int MedicationId { get; set; }

    public DateTime TakenAt { get; set; }

    public double QuantityTaken { get; set; }
}
