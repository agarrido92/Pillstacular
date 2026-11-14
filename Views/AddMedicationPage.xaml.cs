
using System.Globalization;
using Pillstacular.Models;
using Pillstacular.Services;

namespace Pillstacular.Views;

public partial class AddMedicationPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly Medication? _editingMedication;

    private readonly List<TimePicker> _reminderTimePickers = new();

    private bool _loadingSchedule;

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

        ScheduleSection.IsVisible = AsNeededSwitch.IsToggled;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_editingMedication == null || _loadingSchedule)
            return;

        _loadingSchedule = true;

        try
        {
            var schedules = await _databaseService
                .GetSchedulesAsync(_editingMedication.Id);

            if (schedules.Count == 0)
                return;

            string[] days = schedules[0].DaysOfWeek
                .Split(',', StringSplitOptions.RemoveEmptyEntries);

            SundayCheck.IsChecked = days.Contains("Sunday");
            MondayCheck.IsChecked = days.Contains("Monday");
            TuesdayCheck.IsChecked = days.Contains("Tuesday");
            WednesdayCheck.IsChecked = days.Contains("Wednesday");
            ThursdayCheck.IsChecked = days.Contains("Thursday");
            FridayCheck.IsChecked = days.Contains("Friday");
            SaturdayCheck.IsChecked = days.Contains("Saturday");

            ReminderTimesContainer.Children.Clear();
            _reminderTimePickers.Clear();

            foreach (var schedule in schedules)
                AddReminderTime(schedule.ScheduledTime);
        }
        finally
        {
            _loadingSchedule = false;
        }
    }

    private void OnScheduleToggled(
        object? sender,
        ToggledEventArgs e)
    {
        ScheduleSection.IsVisible = e.Value;
    }

    private void OnAddReminderTimeClicked(
        object? sender,
        EventArgs e)
    {
        AddReminderTime(new TimeSpan(8, 0, 0));
    }

    private void AddReminderTime(TimeSpan time)
    {
        var picker = new TimePicker
        {
            Time = time,
            WidthRequest = 280,
            HorizontalOptions = LayoutOptions.Start
        };

        var removeButton = new Button
        {
            Text = "Remove"
        };

        var row = new HorizontalStackLayout
        {
            Spacing = 12
        };

        row.Children.Add(picker);
        row.Children.Add(removeButton);

        removeButton.Clicked += (_, _) =>
        {
            ReminderTimesContainer.Children.Remove(row);
            _reminderTimePickers.Remove(picker);
        };

        _reminderTimePickers.Add(picker);
        ReminderTimesContainer.Children.Add(row);
    }

    private string GetSelectedDays()
    {
        var days = new List<string>();

        if (SundayCheck.IsChecked) days.Add("Sunday");
        if (MondayCheck.IsChecked) days.Add("Monday");
        if (TuesdayCheck.IsChecked) days.Add("Tuesday");
        if (WednesdayCheck.IsChecked) days.Add("Wednesday");
        if (ThursdayCheck.IsChecked) days.Add("Thursday");
        if (FridayCheck.IsChecked) days.Add("Friday");
        if (SaturdayCheck.IsChecked) days.Add("Saturday");

        return string.Join(",", days);
    }

    private async void OnSaveClicked(
        object? sender,
        EventArgs e)
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

        string selectedDays = string.Empty;

        if (AsNeededSwitch.IsToggled)
        {
            selectedDays = GetSelectedDays();

            if (string.IsNullOrEmpty(selectedDays))
            {
                await DisplayAlertAsync(
                    "Missing Schedule",
                    "Please select at least one reminder day.",
                    "OK");
                return;
            }

            if (_reminderTimePickers.Count == 0)
            {
                await DisplayAlertAsync(
                    "Missing Schedule",
                    "Please add at least one reminder time.",
                    "OK");
                return;
            }
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

        // Replace old schedules with the current selection.
        await _databaseService.DeleteSchedulesAsync(medication.Id);

        if (AsNeededSwitch.IsToggled)
        {
            foreach (var time in _reminderTimePickers
                .Select(p => p.Time ?? TimeSpan.Zero)
                .Distinct())
            {
                var schedule = new MedicationSchedule
                {
                    MedicationId = medication.Id,
                    ScheduledTime = time,
                    Quantity = quantity,
                    DaysOfWeek = selectedDays
                };

                await _databaseService.SaveScheduleAsync(schedule);
            }
        }

        await DisplayAlertAsync(
            "Success",
            _editingMedication == null
                ? "Medication saved successfully."
                : "Medication updated successfully.",
            "OK");

        await Navigation.PopAsync();
    }
}
