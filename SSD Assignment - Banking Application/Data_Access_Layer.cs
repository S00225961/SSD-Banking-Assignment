using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SSD_Assignment___Banking_Application;

namespace Banking_Application
{
    public class Data_Access_Layer
    {
        private Auth auth = new Auth();
        private List<Bank_Account> accounts;
        public static String databaseName = "Banking Database.db";
        private static Data_Access_Layer instance = new Data_Access_Layer();

        private Data_Access_Layer()//Singleton Design Pattern (For Concurrency Control) - Use getInstance() Method Instead.
        {
            accounts = new List<Bank_Account>();
        }

        public static Data_Access_Layer getInstance()
        {
            return instance;
        }

        private SqliteConnection getDatabaseConnection()
        {

            String databaseConnectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = Data_Access_Layer.databaseName,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();
            return new SqliteConnection(databaseConnectionString);

        }

        private void initialiseDatabase()
        {
            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Bank_Accounts (
                    accountNo TEXT PRIMARY KEY,
                    encryptedName BLOB NOT NULL,
                    nameIV BLOB NOT NULL,
                    encryptedAddressLine1 BLOB,
                    encryptedAddressLine2 BLOB,
                    encryptedAddressLine3 BLOB,
                    addressLine1IV BLOB,
                    addressLine2IV BLOB,
                    addressLine3IV BLOB,
                    encryptedTown BLOB NOT NULL,
                    townIV BLOB NOT NULL,
                    balance REAL NOT NULL,
                    accountType INTEGER NOT NULL,
                    overdraftAmount REAL,
                    interestRate REAL
                )";
                command.ExecuteNonQuery();

            }
        }

        public void loadBankAccounts()
        {
            if (!File.Exists(Data_Access_Layer.databaseName))
                initialiseDatabase();
            else
            {

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Bank_Accounts";
                    SqliteDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        string accountNo = dr.GetString(0);
                        byte[] encryptedName = (byte[])dr[1];
                        byte[] nameIV = (byte[])dr[2];
                        byte[] encryptedAddressLine1 = (byte[])dr[3];
                        byte[] encryptedAddressLine2 = (byte[])dr[4];
                        byte[] encryptedAddressLine3 = (byte[])dr[5];
                        byte[] addressLine1IV = (byte[])dr[6];
                        byte[] addressLine2IV = (byte[])dr[7];
                        byte[] addressLine3IV = (byte[])dr[8];
                        byte[] encryptedTown = (byte[])dr[9];
                        byte[] townIV = (byte[])dr[10];
                        double balance = dr.GetDouble(11);
                        int accountType = dr.GetInt32(12);
                        double? overdraftAmount = dr.IsDBNull(13) ? (double?)null : dr.GetDouble(13);
                        double? interestRate = dr.IsDBNull(14) ? (double?)null : dr.GetDouble(14);

                        Bank_Account ba;
                        if (accountType == 1)
                        {
                            ba = new Current_Account
                            {
                                AccountNo = accountNo,
                                EncryptedName = encryptedName,
                                NameIV = nameIV,
                                EncryptedAddressLine1 = encryptedAddressLine1,
                                EncryptedAddressLine2 = encryptedAddressLine2,
                                EncryptedAddressLine3 = encryptedAddressLine3,
                                AddressLine1IV = addressLine1IV,
                                AddressLine2IV = addressLine2IV,
                                AddressLine3IV = addressLine3IV,
                                EncryptedTown = encryptedTown,
                                TownIV = townIV,
                                Balance = balance,
                                OverdraftAmount = overdraftAmount ?? 0
                            };
                        }
                        else
                        {
                            ba = new Savings_Account
                            {
                                AccountNo = accountNo,
                                EncryptedName = encryptedName,
                                NameIV = nameIV,
                                EncryptedAddressLine1 = encryptedAddressLine1,
                                EncryptedAddressLine2 = encryptedAddressLine2,
                                EncryptedAddressLine3 = encryptedAddressLine3,
                                AddressLine1IV = addressLine1IV,
                                AddressLine2IV = addressLine2IV,
                                AddressLine3IV = addressLine3IV,
                                EncryptedTown = encryptedTown,
                                TownIV = townIV,
                                Balance = balance,
                                InterestRate = interestRate ?? 0
                            };
                        }

                        accounts.Add(ba);

                        //Logging
                        Bank_Log bankLog = new Bank_Log();
                        bankLog.LogTransaction(auth.username, ba.AccountNo, ba.Name, Bank_Log.TransactionType.Balance_Query);

                    }

                }

            }
        }

        public String addBankAccount(Bank_Account ba) 
        {

            accounts.Add(ba);

            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"INSERT INTO Bank_Accounts (accountNo, encryptedName, nameIV, encryptedAddressLine1, encryptedAddressLine2, encryptedAddressLine3, addressLine1IV, addressLine2IV, addressLine3IV, encryptedTown, townIV, balance, accountType, overdraftAmount, interestRate) " +
                    "VALUES (@accountNo, @encryptedName, @nameIV, @encryptedAddressLine1, @encryptedAddressLine2, @encryptedAddressLine3, @addressLine1IV, @addressLine2IV, @addressLine3IV, @encryptedTown, @townIV, @balance, @accountType, @overdraftAmount, @interestRate)";

                command.Parameters.AddWithValue("@accountNo", ba.AccountNo);
                command.Parameters.AddWithValue("@encryptedName", ba.EncryptedName);
                command.Parameters.AddWithValue("@nameIV", ba.NameIV);
                command.Parameters.AddWithValue("@encryptedAddressLine1", ba.EncryptedAddressLine1);
                command.Parameters.AddWithValue("@encryptedAddressLine2", ba.EncryptedAddressLine2);
                command.Parameters.AddWithValue("@encryptedAddressLine3", ba.EncryptedAddressLine3);
                command.Parameters.AddWithValue("@addressLine1IV", ba.AddressLine1IV);
                command.Parameters.AddWithValue("@addressLine2IV", ba.AddressLine2IV);
                command.Parameters.AddWithValue("@addressLine3IV", ba.AddressLine3IV);
                command.Parameters.AddWithValue("@encryptedTown", ba.EncryptedTown);
                command.Parameters.AddWithValue("@townIV", ba.TownIV);
                command.Parameters.AddWithValue("@balance", ba.Balance);
                command.Parameters.AddWithValue("@accountType", ba is Current_Account ? 1 : 2);

                if (ba is Current_Account ca)
                {
                    command.Parameters.AddWithValue("@overdraftAmount", ca.OverdraftAmount);
                    command.Parameters.AddWithValue("@interestRate", DBNull.Value);
                }
                else if (ba is Savings_Account sa)
                {
                    command.Parameters.AddWithValue("@overdraftAmount", DBNull.Value);
                    command.Parameters.AddWithValue("@interestRate", sa.InterestRate);
                }

                command.ExecuteNonQuery();

            }
            //Logging
            Bank_Log bankLog = new Bank_Log();
            bankLog.LogTransaction(auth.username, ba.AccountNo, ba.Name, Bank_Log.TransactionType.Account_Creation);

            return ba.AccountNo;

        }

        public Bank_Account findBankAccountByAccNo(String accNo) 
        { 
        
            foreach(Bank_Account ba in accounts)
            {

                if (ba.AccountNo.Equals(accNo))
                {
                    return ba;
                }

            }

            return null; 
        }

        public bool closeBankAccount(String accNo) 
        {

            Bank_Account toRemove = null;
            
            foreach (Bank_Account ba in accounts)
            {

                if (ba.AccountNo.Equals(accNo))
                {
                    toRemove = ba;
                    break;
                }

            }

            if (toRemove == null)
                return false;
            else
            {
                accounts.Remove(toRemove);

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Bank_Accounts WHERE accountNo = '" + toRemove.AccountNo + "'";
                    command.ExecuteNonQuery();

                }

                //Logging
                Bank_Log bankLog = new Bank_Log();
                bankLog.LogTransaction(auth.username, toRemove.AccountNo, toRemove.Name, Bank_Log.TransactionType.Account_Closure);

                return true;
            }

        }

        public bool lodge(String accNo, double amountToLodge)
        {

            Bank_Account toLodgeTo = null;

            foreach (Bank_Account ba in accounts)
            {

                if (ba.AccountNo.Equals(accNo))
                {
                    ba.lodge(amountToLodge);
                    toLodgeTo = ba;
                    break;
                }

            }

            if (toLodgeTo == null)
                return false;
            else
            {

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = " + toLodgeTo.Balance + " WHERE accountNo = '" + toLodgeTo.AccountNo + "'";
                    command.ExecuteNonQuery();

                }

                //Logging
                Bank_Log bankLog = new Bank_Log();
                bankLog.LogTransaction(auth.username, toLodgeTo.AccountNo, toLodgeTo.Name, Bank_Log.TransactionType.Lodgement);

                return true;
            }

        }

        public bool withdraw(String accNo, double amountToWithdraw)
        {

            Bank_Account toWithdrawFrom = null;
            bool result = false;

            foreach (Bank_Account ba in accounts)
            {

                if (ba.AccountNo.Equals(accNo))
                {
                    result = ba.withdraw(amountToWithdraw);
                    toWithdrawFrom = ba;
                    break;
                }

            }

            if (toWithdrawFrom == null || result == false)
                return false;
            else
            {

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = " + toWithdrawFrom.Balance + " WHERE accountNo = '" + toWithdrawFrom.AccountNo + "'";
                    command.ExecuteNonQuery();

                }

                //Logging
                Bank_Log bankLog = new Bank_Log();
                bankLog.LogTransaction(auth.username, toWithdrawFrom.AccountNo, toWithdrawFrom.Name, Bank_Log.TransactionType.Withdrawal);

                return true;
            }

        }

    }
}
