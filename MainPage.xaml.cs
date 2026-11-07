namespace Pillstacular;
using Pillstacular.Services;
using Microsoft.Extensions.DependencyInjection;
using Pillstacular.Views;
using Microsoft.Extensions.DependencyInjection;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

	private async void OnAddMedicationClicked(
		object? sender, EventArgs e)
	{
		var page = IPlatformApplication.Current!
			.Services.GetRequiredService<AddMedicationPage>();

		await Navigation.PushAsync(page);
	}

	private async void OnMyMedicationsClicked(
		object? sender, EventArgs e)
	{
		var page = IPlatformApplication.Current!
			.Services.GetRequiredService<MedicationsPage>();

		await Navigation.PushAsync(page);
	}
}
