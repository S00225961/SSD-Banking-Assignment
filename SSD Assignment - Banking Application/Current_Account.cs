using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking_Application
{
    public class Current_Account: Bank_Account
    {

        public double OverdraftAmount;

        public Current_Account(): base() { }
        
        public Current_Account(String name, String addressLine1, String addressLine2, String addressLine3, String town, double balance, double overdraftAmount) : base(name, addressLine1, addressLine2, addressLine3, town, balance)
        {
            this.OverdraftAmount = overdraftAmount;
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

        public override double getAvailableFunds()
        {
            return (base.Balance + OverdraftAmount);
        }

        public override String ToString()
        {

            return base.ToString() +
                "Account Type: Current Account\n" +
                "Overdraft Amount: " + OverdraftAmount + "\n";

        }

    }
}
