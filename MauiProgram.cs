using Microsoft.Extensions.Logging;
using Pillstacular.Services;
using Pillstacular.Views;

namespace Pillstacular;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddSingleton<DatabaseService>();
		builder.Services.AddTransient<AddMedicationPage>();
		builder.Services.AddTransient<MedicationsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
