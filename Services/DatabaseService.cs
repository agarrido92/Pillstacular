using SQLite;
using Pillstacular.Models;

namespace Pillstacular.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        string databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "pillstacular.db3");

        _database = new SQLiteAsyncConnection(databasePath);

        await _database.CreateTableAsync<Medication>();
        await _database.CreateTableAsync<MedicationSchedule>();
        await _database.CreateTableAsync<MedicationEntry>();

        var columns = await _database.GetTableInfoAsync(
            "MedicationSchedule");

        if (!columns.Any(c => c.Name == "DaysOfWeek"))
        {
            await _database.ExecuteAsync(
                "ALTER TABLE MedicationSchedule " +
                "ADD COLUMN DaysOfWeek TEXT NOT NULL DEFAULT ''");
        }
    }

    // Medication operations

    public async Task<List<Medication>> GetMedicationsAsync()
    {
        await InitAsync();

        return await _database!
            .Table<Medication>()
            .ToListAsync();
    }

    public async Task<int> SaveMedicationAsync(
        Medication medication)
    {
        await InitAsync();

        if (medication.Id == 0)
            return await _database!.InsertAsync(medication);

        return await _database!.UpdateAsync(medication);
    }

    public async Task<int> DeleteMedicationAsync(
        Medication medication)
    {
        await InitAsync();

        medication.IsActive = false;

        return await _database!.UpdateAsync(medication);
    }

    // Medication schedule operations

    public async Task<List<MedicationSchedule>>
        GetSchedulesAsync(int medicationId)
    {
        await InitAsync();

        return await _database!
            .Table<MedicationSchedule>()
            .Where(s => s.MedicationId == medicationId)
            .ToListAsync();
    }

    public async Task<int> SaveScheduleAsync(
        MedicationSchedule schedule)
    {
        await InitAsync();

        if (schedule.Id == 0)
            return await _database!.InsertAsync(schedule);

        return await _database!.UpdateAsync(schedule);
    }

    public async Task<int> DeleteSchedulesAsync(
        int medicationId)
    {
        await InitAsync();

        return await _database!.ExecuteAsync(
            "DELETE FROM MedicationSchedule WHERE MedicationId = ?",
            medicationId);
    }

    // Medication entry operations

    public async Task<List<MedicationEntry>>
        GetEntriesAsync(int medicationId)
    {
        await InitAsync();

        return await _database!
            .Table<MedicationEntry>()
            .Where(e => e.MedicationId == medicationId)
            .ToListAsync();
    }

    public async Task<int> SaveEntryAsync(
        MedicationEntry entry)
    {
        await InitAsync();

        if (entry.Id == 0)
            return await _database!.InsertAsync(entry);

        return await _database!.UpdateAsync(entry);
    }
}
