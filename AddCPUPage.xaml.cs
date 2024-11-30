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

    /* ĳ� ������ "��������" */
    private async void SaveButtonHandler(object sender, EventArgs e)
    {
        // �������� �� ��������� "������� ���������"
        if (new[] { ModelEntry.Text, CoresEntry.Text, BaseClockEntry.Text, BoostClockEntry.Text, DescriptionEntry.Text }
            .All(string.IsNullOrWhiteSpace))
        {
            await DisplayAlert("������������!", "�� ������� ������ �������� ��� �����.\n��������� ��������� �������� ���� ����", "��");
            return;
        }


        try
        {
            // ��������� ������ ���������

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

            // ������������ �������� ��������� � �����
            var filePath = Path.Combine(FileSystem.AppDataDirectory, MainPage.FilePath);
            List<CPU> CPUList;

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                CPUList = JsonSerializer.Deserialize<List<CPU>>(json);
            }
            else CPUList = new List<CPU>();

            // ��������� ������ ��������� �� ������
            CPUList.Add(newCPU);

            // ���������� ���������� ������ � ����
            var updatedJson = JsonSerializer.Serialize(CPUList, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            await File.WriteAllTextAsync(filePath, updatedJson);

            // ���������� �� ������� �������
            await Navigation.PopAsync();
        }
        catch (Exception)
        {
            await DisplayAlert("�������", "����������� �������� ������� ����.", "OK");
        }
    }
}