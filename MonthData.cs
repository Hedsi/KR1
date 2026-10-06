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


        public double TotalIncome =>
            MainIncome + AdditionalIncome;

 
        public double TotalExpenses =>
            Housing +
            Utilities +
            Transport +
            Food +
            OtherExpenses;


        public double Remaining =>
            TotalIncome - TotalExpenses;

   
        public double ExpenseShare =>
            TotalIncome > 0
                ? TotalExpenses / TotalIncome * 100
                : 0;

  
        public double SavingsChange =>
            Remaining;
    }
}