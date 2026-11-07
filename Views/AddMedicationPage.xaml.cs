using System.Globalization;
using Pillstacular.Models;
using Pillstacular.Services;

namespace Pillstacular.Views;

public partial class AddMedicationPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly Medication? _editingMedication;

    public AddMedicationPage(
        DatabaseService databaseService,
        Medication? medication = null)
    {
        InitializeComponent();

        _databaseService = databaseService;
        _editingMedication = medication;

        if (_editingMedication != null)
        {
            Title = "Edit Medication";

            NameEntry.Text = medication!.Name;
            NicknameEntry.Text = medication.Nickname;
            StrengthEntry.Text = medication.Strength.ToString(
                CultureInfo.CurrentCulture);

            UnitPicker.SelectedItem = medication.Unit;
            FormPicker.SelectedIndex = (int)medication.Form;

            QuantityEntry.Text = medication.DefaultQuantity.ToString(
                CultureInfo.CurrentCulture);

            AsNeededSwitch.IsToggled = !medication.IsAsNeeded;
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Information",
                "Please enter a medication name.",
                "OK");
            return;
        }

        if (!double.TryParse(
            StrengthEntry.Text,
            CultureInfo.CurrentCulture,
            out double strength) || strength <= 0)
        {
            await DisplayAlertAsync(
                "Invalid Dosage",
                "Please enter a valid dosage.",
                "OK");
            return;
        }

        if (!double.TryParse(
            QuantityEntry.Text,
            CultureInfo.CurrentCulture,
            out double quantity) || quantity <= 0)
        {
            await DisplayAlertAsync(
                "Invalid Quantity",
                "Please enter a valid quantity.",
                "OK");
            return;
        }

        if (UnitPicker.SelectedIndex < 0 ||
            FormPicker.SelectedIndex < 0)
        {
            await DisplayAlertAsync(
                "Missing Information",
                "Please select a unit and medication form.",
                "OK");
            return;
        }

        var medication = _editingMedication ?? new Medication();

        medication.Name = NameEntry.Text.Trim();
        medication.Nickname = NicknameEntry.Text?.Trim() ?? string.Empty;
        medication.Strength = strength;
        medication.Unit = UnitPicker.SelectedItem.ToString()!;
        medication.Form = (MedicationForm)FormPicker.SelectedIndex;
        medication.DefaultQuantity = quantity;
        medication.IsAsNeeded = !AsNeededSwitch.IsToggled;

        await _databaseService.SaveMedicationAsync(medication);

        await DisplayAlertAsync(
            "Success",
            _editingMedication == null
                ? "Medication saved successfully."
                : "Medication updated successfully.",
            "OK");

        await Navigation.PopAsync();
    }
}