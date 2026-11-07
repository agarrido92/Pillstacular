using Pillstacular.Models;
using Pillstacular.Services;

namespace Pillstacular.Views;

public partial class MedicationsPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public MedicationsPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMedicationsAsync();
    }

    private async Task LoadMedicationsAsync()
    {
        var medications = await _databaseService.GetMedicationsAsync();

        MedicationsList.ItemsSource = medications
            .Where(m => m.IsActive)
            .ToList();
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not Medication medication)
        {
            return;
        }

        await Navigation.PushAsync(
            new AddMedicationPage(_databaseService, medication));
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not Medication medication)
        {
            return;
        }

        bool confirmed = await DisplayAlertAsync(
            "Delete Medication",
            $"Remove {medication.Name} from your active medications? Its history will be preserved.",
            "Delete",
            "Cancel");

        if (!confirmed)
        {
            return;
        }

        await _databaseService.DeleteMedicationAsync(medication);

        await LoadMedicationsAsync();
    }
}