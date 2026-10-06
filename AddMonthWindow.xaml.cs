using System.Globalization;
using System.Windows;

namespace FamilyBudgetAnalysis
{
    public partial class AddMonthWindow : Window
    {
        public MonthData? Result { get; private set; }

        public AddMonthWindow()
        {
            InitializeComponent();
        }

        private bool TryGetValue(
            string text,
            string fieldName,
            out double value)
        {
            value = 0;

            text = text.Trim().Replace(',', '.');

            if (!double.TryParse(
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value))
            {
                MessageBox.Show(
                    $"Поле «{fieldName}» повинно містити число.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (value < 0)
            {
                MessageBox.Show(
                    $"Поле «{fieldName}» не може бути від'ємним.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string month = MonthTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(month))
            {
                MessageBox.Show(
                    "Введіть назву місяця.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!TryGetValue(
                    MainIncomeTextBox.Text,
                    "Основний дохід",
                    out double mainIncome))
                return;

            if (!TryGetValue(
                    AdditionalIncomeTextBox.Text,
                    "Додатковий дохід",
                    out double additionalIncome))
                return;

            if (!TryGetValue(
                    HousingTextBox.Text,
                    "Житло",
                    out double housing))
                return;

            if (!TryGetValue(
                    UtilitiesTextBox.Text,
                    "Комунальні",
                    out double utilities))
                return;

            if (!TryGetValue(
                    TransportTextBox.Text,
                    "Транспорт",
                    out double transport))
                return;

            if (!TryGetValue(
                    FoodTextBox.Text,
                    "Харчування",
                    out double food))
                return;

            if (!TryGetValue(
                    OtherExpensesTextBox.Text,
                    "Інші витрати",
                    out double otherExpenses))
                return;

            Result = new MonthData
            {
                Month = month,

                MainIncome = mainIncome,
                AdditionalIncome = additionalIncome,

                Housing = housing,
                Utilities = utilities,
                Transport = transport,
                Food = food,
                OtherExpenses = otherExpenses
            };

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}