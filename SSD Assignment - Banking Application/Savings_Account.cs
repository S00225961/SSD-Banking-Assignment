using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking_Application
{
    public class Savings_Account: Bank_Account
    {

        public double InterestRate { get; set; }

        public Savings_Account() : base() { }
        
        public Savings_Account(String name, String addressLine1, String addressLine2, String addressLine3, String town, double balance, double interestRate) : base(name, addressLine1, addressLine2, addressLine3, town, balance)
        {
            this.InterestRate = interestRate;
        }
        public override double getAvailableFunds()
        {
            return base.Balance;
        }

        public override bool withdraw(double amountToWithdraw)
        {
            double avFunds = getAvailableFunds();

            if (avFunds >= amountToWithdraw)
            {
                Balance -= amountToWithdraw;
                return true;
            }

            else
                return false;
        }

        public override String ToString()
        {

            return base.ToString() + 
                "Account Type: Savings Account\n" +
                "Interest Rate: " + InterestRate + "\n";

        }


    }
}
