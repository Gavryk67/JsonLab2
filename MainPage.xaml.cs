using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace Lab3
{
    public partial class MainPage : ContentPage
    {
        public static string FilePath { get; set; }
        public ObservableCollection<CPU> CPUCollection { get; set; } 

        public MainPage()
        {
            InitializeComponent();
            CPUList.HeightRequest = App.WindowHeight * 0.6667;
            CPUCollection = new ObservableCollection<CPU>();
            BindingContext = this;
        }

        /* Метод, що викликається на початку життєвого циклу програми */
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (FilePath != null)
            {
                UpdateCPUList();
            }
        }

        /* Оновлює процесори в CPUCollection  */
        private void UpdateCPUList()
        {
            if (string.IsNullOrEmpty(FilePath)) return;

            try
            {
                string jsonContent = File.ReadAllText(FilePath);

                var CPUs = JsonSerializer.Deserialize<List<CPU>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (CPUs != null)
                {
                    CPUCollection.Clear();
                    foreach (var cpu in CPUs)
                    {
                        if (cpu.Cores > 0)
                            CPUCollection.Add(cpu);
                        else throw new Exception("Negative value for cores");
                    }
                }
            }
            catch (Exception)
            {
                DisplayAlert("Помилка", "Помилка під час обробки файлу.", "OK");
            }
        }

        /* Дія кнопки "Обрати файл" */
        private async void OpenFileHandler(object sender, EventArgs e)
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".json" } }
            });

            try
            {
                var result = await FilePicker.PickAsync(new PickOptions { FileTypes = customFileType });
                if (result != null)
                {
                    FilePath = result.FullPath;
                    UpdateCPUList();
                    FilePathLabel.Text = $"Обрано: {FilePath}";
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Помилка", "Не вірний файл!", "OK");
            }
        }

        /* Дія кнопки "і" */
        private async void InfoButtonHandler(object sender, EventArgs e)
        {
            string studentInfo = "Виконав Василиг Даниїл, студент другого курсу групи К-26, 2024" +
                                 "\n\nПрограма для роботи з json файлами, яка забезпечує відкривання, редагування, видаляння, додавання даних," +
                                 " а також виконання пошуку за параметрами." +
                                 "\n\nТема json файлу: процесори";
            await DisplayAlert("Про лабораторну", studentInfo, "ОК");
        }

        /* Дія кнопки "+" */
        private async void AddCPUHandler(object sender, EventArgs e)
        {
            if (FilePath == null)
                await DisplayAlert("Помилка", "Неможливо додати новий процесор у ще не обраний json файл!", "OK");
            else
            {
                await Navigation.PushAsync(new AddCPUPage());
            }
        }

        /* Дія кнопки "-" */
        private async void DeleteCPUHandler(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is CPU cpu)
            {

                bool isConfirmed = await DisplayAlert(
                    "Підтвердження видалення",
                    $"Ви дійсно хочете видалити процесор: {cpu.Model}?",
                    "Так",
                    "Ні");
                if (isConfirmed)
                {
                    CPUCollection.Remove(cpu);
                    SaveToFile();
                }
            }
        }

        /* Зберігання змін*/
        private async void SaveToFile()
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(CPUCollection);
                File.WriteAllText(FilePath, jsonContent);
            }
            catch (Exception)
            {
                await DisplayAlert("Помилка", "Не вдалося зберегти дані до файлу.", "OK");
            }
        }

        /* Дія кнопки "Пошук"*/
        private async void SearchHandler(object sender, EventArgs e)
        {
            if (CPUCollection == null || !CPUCollection.Any())
            {
                await DisplayAlert("Попередження", "Список процесорів порожній.", "OK");
                return;
            }

            string ModelFilter = ModelEntry.Text?.Trim().ToLower() ?? string.Empty;
            string BaseClockFilter = BaseClockEntry.Text?.Trim().ToLower() ?? string.Empty;
            string BoostClockFilter = BoostClockEntry.Text?.Trim().ToLower() ?? string.Empty;
            string DescriptionFilter = DescriptionEntry.Text?.Trim().ToLower() ?? string.Empty;

            int? CoresFilter = null;
            int cores = 0;
            if (int.TryParse(CoresEntry.Text?.Trim(), out cores) && cores <= 0)
            {
                await DisplayAlert("Помилка", "Будь ласка, введіть коректне число для кількості ядер.", "OK");
                return;
            }
            else if (!string.IsNullOrEmpty(CoresEntry.Text?.Trim()))
            { 
                CoresFilter = cores;
            }

            var filteredCPUs = CPUCollection.Where(cpu =>
                (string.IsNullOrEmpty(ModelFilter) || cpu.Model.ToLower().Contains(ModelFilter)) &&
                (!CoresFilter.HasValue || cpu.Cores == CoresFilter) &&
                (string.IsNullOrEmpty(BaseClockFilter) || cpu.BaseClock.ToLower().Contains(BaseClockFilter)) &&
                (string.IsNullOrEmpty(BoostClockFilter) || cpu.BoostClock.ToLower().Contains(BoostClockFilter)) &&
                (string.IsNullOrEmpty(DescriptionFilter) || cpu.Description.ToLower().Contains(DescriptionFilter))
            ).ToList();

            if (filteredCPUs.Any())
            {
                CPUCollectionView.ItemsSource = new ObservableCollection<CPU>(filteredCPUs);
            }
            else
            {
                CPUCollectionView.ItemsSource = new ObservableCollection<CPU>();
            }
        }


        /* Дія кнопки "Очистити" */
        private void ClearFiltersHander(object sender, EventArgs e)
        {
            ModelEntry.Text = string.Empty;
            CoresEntry.Text = string.Empty;
            BaseClockEntry.Text = string.Empty;
            BoostClockEntry.Text = string.Empty;
            DescriptionEntry.Text = string.Empty;
        }

        /* Дія кнопки "Детальніше" */
        private async void ViewDescriptionHandler(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is CPU cpu)
            {
                await DisplayAlert("Опис процесора", cpu.Description, "OK");
            }
        }

        /* Дія кнопки "Редагувати"*/
        private async void EditCPUHandler(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is CPU cpu)
            {
                await Navigation.PushAsync(new EditPage(cpu));
            }
        }
    }

}
