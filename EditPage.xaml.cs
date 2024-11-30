using System.Collections.ObjectModel;
using System.Text.Json;

namespace Lab3;

public partial class EditPage : ContentPage
{
    private CPU CPU { get; set; }

    public EditPage(CPU cpu)
    {
        InitializeComponent();

        CPU = cpu;

        BindingContext = CPU;
    }

    /* Допоміжний метод, який перезаписує наш json файл */
    private void RewriteJson(ObservableCollection<CPU> data)
    {
        string updatedJson = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        File.WriteAllText(MainPage.FilePath, updatedJson);
    }

    /* Дія кнопки "Зберегти" */
    private async void SaveButtonHandler(object sender, EventArgs e)
    {
        try
        {
            if (Application.Current.MainPage is NavigationPage navigationPage &&
                navigationPage.RootPage is MainPage mainPage)
            {
                var CPUInCollection = mainPage.CPUCollection.FirstOrDefault(b => b.Model == CPU.Model);

                if (new[] { ModelEntry.Text, CoresEntry.Text, BaseClockEntry.Text, BoostClockEntry.Text, DescriptionEntry.Text }
                    .All(string.IsNullOrWhiteSpace))
                {
                    bool confirmDelete = await DisplayAlert("Підтвердження", "Усі поля порожні. Видалити цей процесор?", "Так", "Ні");
                    if (confirmDelete)
                    {
                        mainPage.CPUCollection.Remove(CPUInCollection);

                        RewriteJson(mainPage.CPUCollection);

                        await DisplayAlert("Успіх", "Процесор видалено!", "OK");
                        await Navigation.PopAsync();
                        return;
                    }
                    else
                    {
                        await Navigation.PopAsync();
                    }
                }

                CPUInCollection.Model = ModelEntry.Text?.Trim() ?? string.Empty;
                CPUInCollection.BaseClock = BaseClockEntry.Text?.Trim() ?? string.Empty;
                CPUInCollection.BoostClock = BoostClockEntry.Text?.Trim() ?? string.Empty;
                CPUInCollection.Description = DescriptionEntry.Text?.Trim() ?? string.Empty;
                if (int.TryParse(CoresEntry.Text?.Trim(), out int cores) && cores > 0)
                {
                    CPUInCollection.Cores = cores;

                    RewriteJson(mainPage.CPUCollection);

                    await DisplayAlert("Успіх", "Процесор успішно збережено!", "OK");

                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Помилка", "Будь ласка, введіть коректне число для кількості ядер.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Помилка", "Головну сторінку програми не знайдено.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося зберегти Процесор через несподівану помилку:\n{ex.Message}", "OK");
        }
    }
}