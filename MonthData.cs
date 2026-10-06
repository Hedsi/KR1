using System;

namespace FamilyBudgetAnalysis
{
    public class MonthData
    {
        public string Month { get; set; } = "";

        public double MainIncome { get; set; }
        public double AdditionalIncome { get; set; }

        public double Housing { get; set; }
        public double Utilities { get; set; }
        public double Transport { get; set; }
        public double Food { get; set; }
        public double OtherExpenses { get; set; }

        // Загальний дохід
        public double TotalIncome =>
            MainIncome + AdditionalIncome;

        // Загальні витрати
        public double TotalExpenses =>
            Housing +
            Utilities +
            Transport +
            Food +
            OtherExpenses;

        // Залишок після витрат
        public double Remaining =>
            TotalIncome - TotalExpenses;

        // Частка витрат у доході, %
        public double ExpenseShare =>
            TotalIncome > 0
                ? TotalExpenses / TotalIncome * 100
                : 0;

        // Зміна заощаджень
        public double SavingsChange =>
            Remaining;
    }
}