using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace FamilyBudgetAnalysis
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<MonthData> months =
            new ObservableCollection<MonthData>();

        private string numberFormat = "N2";

        public MainWindow()
        {
            InitializeComponent();

            BudgetDataGrid.ItemsSource = months;
        }

        private void AddMonth_Click(object sender, RoutedEventArgs e)
        {
            AddMonthWindow window = new AddMonthWindow
            {
                Owner = this
            };

            if (window.ShowDialog() == true &&
                window.Result != null)
            {
                months.Add(window.Result);

                ClearResults();
            }
        }


        private void DeleteMonth_Click(object sender, RoutedEventArgs e)
        {
            if (BudgetDataGrid.SelectedItem is MonthData selected)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Видалити місяць «{selected.Month}»?",
                    "Підтвердження",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    months.Remove(selected);
                    ClearResults();
                }
            }
            else
            {
                MessageBox.Show(
                    "Спочатку виберіть місяць у таблиці.",
                    "Інформація",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            if (months.Count == 0)
                return;

            MessageBoxResult result = MessageBox.Show(
                "Очистити всі введені дані?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                months.Clear();

                InitialSavingsTextBox.Text = "0";

                ClearResults();
            }
        }


        private bool TryGetInitialSavings(out double value)
        {
            value = 0;

            string text = InitialSavingsTextBox.Text
                .Trim()
                .Replace(',', '.');

            if (!double.TryParse(
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value))
            {
                MessageBox.Show(
                    "Початкові заощадження повинні бути числом.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (value < 0)
            {
                MessageBox.Show(
                    "Початкові заощадження не можуть бути від'ємними.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }


        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (months.Count == 0)
            {
                MessageBox.Show(
                    "Немає даних для аналізу. Додайте хоча б один місяць.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!TryGetInitialSavings(out double initialSavings))
                return;

            double totalIncome = months.Sum(x => x.TotalIncome);

            double totalExpenses = months.Sum(x => x.TotalExpenses);

            double totalRemaining = months.Sum(x => x.Remaining);

            double finalSavings =
                initialSavings + totalRemaining;

            double averageIncome =
                totalIncome / months.Count;

            double averageExpenses =
                totalExpenses / months.Count;

            MonthData maxExpensesMonth =
                months.OrderByDescending(x => x.TotalExpenses)
                      .First();

            MonthData minRemainingMonth =
                months.OrderBy(x => x.Remaining)
                      .First();

            double housingTotal =
                months.Sum(x => x.Housing);

            double utilitiesTotal =
                months.Sum(x => x.Utilities);

            double transportTotal =
                months.Sum(x => x.Transport);

            double foodTotal =
                months.Sum(x => x.Food);

            double otherTotal =
                months.Sum(x => x.OtherExpenses);

            double maxExpenseValue =
                new[]
                {
                    housingTotal,
                    utilitiesTotal,
                    transportTotal,
                    foodTotal,
                    otherTotal
                }.Max();

            string largestExpense;

            if (maxExpenseValue == housingTotal)
                largestExpense = "Житло";
            else if (maxExpenseValue == utilitiesTotal)
                largestExpense = "Комунальні витрати";
            else if (maxExpenseValue == transportTotal)
                largestExpense = "Транспорт";
            else if (maxExpenseValue == foodTotal)
                largestExpense = "Харчування";
            else
                largestExpense = "Інші витрати";


            double forecastMonthlySavings =
                averageIncome - averageExpenses;

            double forecastSavings =
                finalSavings + forecastMonthlySavings;


            TotalSavingsText.Text =
                $"Заощадження на кінець періоду: " +
                $"{finalSavings.ToString(numberFormat)} грн";

            AverageIncomeText.Text =
                $"Середній місячний дохід: " +
                $"{averageIncome.ToString(numberFormat)} грн";

            AverageExpensesText.Text =
                $"Середні місячні витрати: " +
                $"{averageExpenses.ToString(numberFormat)} грн";

            MaxExpensesMonthText.Text =
                $"Місяць із найбільшими витратами: " +
                $"{maxExpensesMonth.Month} " +
                $"({maxExpensesMonth.TotalExpenses.ToString(numberFormat)} грн)";

            MinRemainingMonthText.Text =
                $"Місяць із найменшим залишком: " +
                $"{minRemainingMonth.Month} " +
                $"({minRemainingMonth.Remaining.ToString(numberFormat)} грн)";

            LargestExpenseText.Text =
                $"Найбільша стаття витрат: " +
                $"{largestExpense} " +
                $"({maxExpenseValue.ToString(numberFormat)} грн)";

            ForecastText.Text =
                $"Прогноз заощаджень на наступний місяць: " +
                $"{forecastSavings.ToString(numberFormat)} грн";

            BudgetDataGrid.Items.Refresh();
        }


        private void ClearResults_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
        }

        private void ClearResults()
        {
            TotalSavingsText.Text =
                "Заощадження на кінець періоду: —";

            AverageIncomeText.Text =
                "Середній місячний дохід: —";

            AverageExpensesText.Text =
                "Середні місячні витрати: —";

            MaxExpensesMonthText.Text =
                "Місяць із найбільшими витратами: —";

            MinRemainingMonthText.Text =
                "Місяць із найменшим залишком: —";

            LargestExpenseText.Text =
                "Найбільша стаття витрат: —";

            ForecastText.Text =
                "Прогноз заощаджень на наступний місяць: —";
        }

        private void OpenData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Відкрити дані",
                Filter = "Текстові файли (*.txt;*.csv)|*.txt;*.csv|Всі файли (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                string[] lines =
                    File.ReadAllLines(dialog.FileName, Encoding.UTF8);

                if (lines.Length <= 1)
                {
                    MessageBox.Show(
                        "Файл не містить даних.",
                        "Помилка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                months.Clear();

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] parts = lines[i].Split(';');

                    if (parts.Length < 8)
                        continue;

                    if (!TryParseFileNumber(parts[1], out double mainIncome))
                        continue;

                    if (!TryParseFileNumber(parts[2], out double additionalIncome))
                        continue;

                    if (!TryParseFileNumber(parts[3], out double housing))
                        continue;

                    if (!TryParseFileNumber(parts[4], out double utilities))
                        continue;

                    if (!TryParseFileNumber(parts[5], out double transport))
                        continue;

                    if (!TryParseFileNumber(parts[6], out double food))
                        continue;

                    if (!TryParseFileNumber(parts[7], out double other))
                        continue;

                    months.Add(new MonthData
                    {
                        Month = parts[0],

                        MainIncome = mainIncome,
                        AdditionalIncome = additionalIncome,

                        Housing = housing,
                        Utilities = utilities,
                        Transport = transport,
                        Food = food,
                        OtherExpenses = other
                    });
                }

                ClearResults();

                MessageBox.Show(
                    $"Завантажено записів: {months.Count}",
                    "Успішно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Помилка при відкритті файлу:\n{ex.Message}",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool TryParseFileNumber(
            string text,
            out double value)
        {
            text = text.Trim().Replace(',', '.');

            return double.TryParse(
                text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out value);
        }


        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {
            if (months.Count == 0)
            {
                MessageBox.Show(
                    "Немає даних для збереження.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!TryGetInitialSavings(out double initialSavings))
                return;

            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Зберегти звіт",
                Filter = "Текстовий файл (*.txt)|*.txt|Всі файли (*.*)|*.*",
                FileName = "Звіт_сімейного_бюджету.txt"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                double totalIncome =
                    months.Sum(x => x.TotalIncome);

                double totalExpenses =
                    months.Sum(x => x.TotalExpenses);

                double totalRemaining =
                    months.Sum(x => x.Remaining);

                double finalSavings =
                    initialSavings + totalRemaining;

                double averageIncome =
                    totalIncome / months.Count;

                double averageExpenses =
                    totalExpenses / months.Count;

                MonthData maxExpensesMonth =
                    months.OrderByDescending(
                        x => x.TotalExpenses).First();

                MonthData minRemainingMonth =
                    months.OrderBy(
                        x => x.Remaining).First();

                double housing =
                    months.Sum(x => x.Housing);

                double utilities =
                    months.Sum(x => x.Utilities);

                double transport =
                    months.Sum(x => x.Transport);

                double food =
                    months.Sum(x => x.Food);

                double other =
                    months.Sum(x => x.OtherExpenses);

                double maxExpense =
                    new[]
                    {
                        housing,
                        utilities,
                        transport,
                        food,
                        other
                    }.Max();

                string largestExpense;

                if (maxExpense == housing)
                    largestExpense = "Житло";
                else if (maxExpense == utilities)
                    largestExpense = "Комунальні витрати";
                else if (maxExpense == transport)
                    largestExpense = "Транспорт";
                else if (maxExpense == food)
                    largestExpense = "Харчування";
                else
                    largestExpense = "Інші витрати";

                double forecastSavings =
                    finalSavings +
                    averageIncome -
                    averageExpenses;

                StringBuilder report =
                    new StringBuilder();

                report.AppendLine(
                    "АНАЛІЗ СІМЕЙНОГО БЮДЖЕТУ");

                report.AppendLine(
                    "========================================");

                report.AppendLine();

                report.AppendLine(
                    $"Початкові заощадження: " +
                    $"{initialSavings.ToString(numberFormat)} грн");

                report.AppendLine();

                report.AppendLine("ДЕТАЛЬНІ ДАНІ");

                report.AppendLine(
                    "----------------------------------------");

                foreach (MonthData month in months)
                {
                    report.AppendLine(
                        $"Місяць: {month.Month}");

                    report.AppendLine(
                        $"  Загальний дохід: " +
                        $"{month.TotalIncome.ToString(numberFormat)} грн");

                    report.AppendLine(
                        $"  Загальні витрати: " +
                        $"{month.TotalExpenses.ToString(numberFormat)} грн");

                    report.AppendLine(
                        $"  Залишок: " +
                        $"{month.Remaining.ToString(numberFormat)} грн");

                    report.AppendLine(
                        $"  Частка витрат: " +
                        $"{month.ExpenseShare.ToString(numberFormat)} %");

                    report.AppendLine(
                        $"  Зміна заощаджень: " +
                        $"{month.SavingsChange.ToString(numberFormat)} грн");

                    report.AppendLine();
                }

                report.AppendLine(
                    "ПІДСУМКОВІ ПОКАЗНИКИ");

                report.AppendLine(
                    "----------------------------------------");

                report.AppendLine(
                    $"Заощадження на кінець періоду: " +
                    $"{finalSavings.ToString(numberFormat)} грн");

                report.AppendLine(
                    $"Середній місячний дохід: " +
                    $"{averageIncome.ToString(numberFormat)} грн");

                report.AppendLine(
                    $"Середні місячні витрати: " +
                    $"{averageExpenses.ToString(numberFormat)} грн");

                report.AppendLine(
                    $"Місяць із найбільшими витратами: " +
                    $"{maxExpensesMonth.Month}");

                report.AppendLine(
                    $"Місяць із найменшим залишком: " +
                    $"{minRemainingMonth.Month}");

                report.AppendLine(
                    $"Найбільша стаття витрат: " +
                    $"{largestExpense}");

                report.AppendLine();

                report.AppendLine(
                    "ПРОГНОЗ");

                report.AppendLine(
                    "----------------------------------------");

                report.AppendLine(
                    $"Прогнозовані заощадження на " +
                    $"наступний місяць: " +
                    $"{forecastSavings.ToString(numberFormat)} грн");

                File.WriteAllText(
                    dialog.FileName,
                    report.ToString(),
                    Encoding.UTF8);

                MessageBox.Show(
                    "Звіт успішно збережено.",
                    "Успішно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Помилка при збереженні:\n{ex.Message}",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ==========================================
        // ВИХІД
        // ==========================================

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // ==========================================
        // ФОРМАТ ЧИСЕЛ
        // ==========================================

        private void Format2_Click(object sender, RoutedEventArgs e)
        {
            numberFormat = "N2";

            BudgetDataGrid.Items.Refresh();

            MessageBox.Show(
                "Формат чисел: 2 знаки після коми.",
                "Налаштування",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Format0_Click(object sender, RoutedEventArgs e)
        {
            numberFormat = "N0";

            BudgetDataGrid.Items.Refresh();

            MessageBox.Show(
                "Формат чисел: без знаків після коми.",
                "Налаштування",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}