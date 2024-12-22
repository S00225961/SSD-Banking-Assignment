using SSD_Assignment___Banking_Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking_Application
{
    public abstract class Bank_Account
    {

        public string AccountNo { get; set; }
        public byte[] EncryptedName { get; set; }
        public byte[] NameIV { get; set; }
        public byte[] EncryptedAddressLine1 { get; set; }
        public byte[] EncryptedAddressLine2 { get; set; }
        public byte[] EncryptedAddressLine3 { get; set; }

        public byte[] AddressLine1IV { get; set; }
        public byte[] AddressLine2IV { get; set; }
        public byte[] AddressLine3IV { get; set; }

        public byte[] EncryptedTown { get; set; }
        public byte[] TownIV { get; set; }
        public double Balance { get; set; }

        protected Bank_Account() { }

        public string Name
        {
            get => AES_Encryption.Decrypt(EncryptedName, NameIV);
            set
            {
                (EncryptedName, NameIV) = AES_Encryption.Encrypt(value);
            }
        }

        public string AddressLine1
        {
            get => AES_Encryption.Decrypt(EncryptedAddressLine1, AddressLine1IV);
            set
            {
                (EncryptedAddressLine1, AddressLine1IV) = AES_Encryption.Encrypt(value);
            }
        }

        public string AddressLine2
        {
            get => AES_Encryption.Decrypt(EncryptedAddressLine2, AddressLine2IV);
            set
            {
                (EncryptedAddressLine2, AddressLine2IV) = AES_Encryption.Encrypt(value);
            }
        }

        public string AddressLine3
        {
            get => AES_Encryption.Decrypt(EncryptedAddressLine3, AddressLine3IV);
            set
            {
                (EncryptedAddressLine3, AddressLine3IV) = AES_Encryption.Encrypt(value);
            }
        }

        public string Town
        {
            get => AES_Encryption.Decrypt(EncryptedTown, TownIV);
            set
            {
                (EncryptedTown, TownIV) = AES_Encryption.Encrypt(value);
            }
        }

        public Bank_Account(string name, string addressLine1, string addressLine2, string addressLine3, string town, double balance)
        {
            AccountNo = Guid.NewGuid().ToString();
            Name = name;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            AddressLine3 = addressLine3;
            Town = town;
            Balance = balance;
        }

        public void lodge(double amountIn)
        {

            Balance += amountIn;

        }

        public abstract bool withdraw(double amountToWithdraw);

        public abstract double getAvailableFunds();

        public override String ToString()
        {

            return "\nAccount No: " + AccountNo + "\n" +
            "Name: " + Name + "\n" +
            "Address Line 1: " + AddressLine1 + "\n" +
            "Address Line 2: " + AddressLine2 + "\n" +
            "Address Line 3: " + AddressLine3 + "\n" +
            "Town: " + Town + "\n" +
            "Balance: " + Balance + "\n";

        }

    }
}
