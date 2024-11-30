using Lab3;
using Microsoft.Maui.Controls.Compatibility;
using System.Text.Json;

namespace Lab3;

public partial class AddCPUPage : ContentPage
{
    public AddCPUPage()
    {
        InitializeComponent();
    }

    /* Дія кнопки "Зберегти" */
    private async void SaveButtonHandler(object sender, EventArgs e)
    {
        // Перевірка на створення "пустого процесора"
        if (new[] { ModelEntry.Text, CoresEntry.Text, BaseClockEntry.Text, BoostClockEntry.Text, DescriptionEntry.Text }
            .All(string.IsNullOrWhiteSpace))
        {
            await DisplayAlert("Попередження!", "Не можливо додати процесор без даних.\nНеобхідно заповнити принаймні одне поле", "ОК");
            return;
        }


        try
        {
            // Створення нового процесора

            var newCPU = new CPU
            {
                Model = ModelEntry.Text?.Trim(),
                Cores = int.Parse(CoresEntry.Text?.Trim()),
                BaseClock = BaseClockEntry.Text?.Trim(),
                BoostClock = BoostClockEntry.Text?.Trim(),
                Description = DescriptionEntry.Text?.Trim()
            };

            if (newCPU.Cores <= 0)
            {
                throw new Exception("Wrong input");
            }

            // Завантаження існуючих процесорів з файлу
            var filePath = Path.Combine(FileSystem.AppDataDirectory, MainPage.FilePath);
            List<CPU> CPUList;

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                CPUList = JsonSerializer.Deserialize<List<CPU>>(json);
            }
            else CPUList = new List<CPU>();

            // Додавання нового процесора до списку
            CPUList.Add(newCPU);

            // Збереження оновленого списку у файл
            var updatedJson = JsonSerializer.Serialize(CPUList, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            await File.WriteAllTextAsync(filePath, updatedJson);

            // Повернення до головної сторінки
            await Navigation.PopAsync();
        }
        catch (Exception)
        {
            await DisplayAlert("Помилка", "Неправильне значення кількості ядер.", "OK");
        }
    }
}