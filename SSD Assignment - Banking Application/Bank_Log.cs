using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SSD_Assignment___Banking_Application
{
    public class Bank_Log
    {
        private const string EventSourceName = "SSD Banking Application";
        private const string LogName = "Application Log";

        public enum TransactionType
        {
            Account_Creation,
            Account_Closure,
            Balance_Query,
            Lodgement,
            Withdrawal
        }
        public Bank_Log()
        {
            EnsureEventSourceExists();
        }

        private void EnsureEventSourceExists()
        {
            if (!EventLog.SourceExists(EventSourceName))
            {
                EventLog.CreateEventSource(EventSourceName, LogName);
            }
        }

        public string GetMacAddress()
        {
            var macAddresses = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var nic in macAddresses)
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress().ToString();
                }
            }
            return "Unknown MAC Address";
        }

        public void LogTransaction(string bankTellerName, string accountNumber, string accountHolderName,
        TransactionType transactionType)
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string metaData = $"App Name - {appName}, App Version - {appVersion}";
            string macAddress = GetMacAddress();
            DateTime transactionDateTime = DateTime.Now;

            string logMessage = $"Bank Teller: {bankTellerName}\n" +
                $"Account No: {accountNumber}\n" +
                $"Account Holder Name: {accountHolderName}\n" +
                $"Transaction Type: {transactionType}\n" +
                $"MAC Address: {macAddress}\n" +
                $"Date/Time: {transactionDateTime}\n" +
                $"Software Metadata: {metaData}";

            using (EventLog eventLog = new EventLog(LogName))
            {
                eventLog.Source = EventSourceName;
                eventLog.WriteEntry(logMessage, EventLogEntryType.Information);
            }
        }

    }
}
