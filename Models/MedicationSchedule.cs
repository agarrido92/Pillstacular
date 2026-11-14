using SQLite;

namespace Pillstacular.Models;

public class MedicationSchedule
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int MedicationId { get; set; }

    public TimeSpan ScheduledTime { get; set; }

    public double Quantity { get; set; } = 1;

    public string DaysOfWeek { get; set; } = string.Empty;
}
