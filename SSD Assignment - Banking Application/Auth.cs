using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD_Assignment___Banking_Application
{
    public class Auth
    {
        private string domainName = "ITSLIGO.LAN";
        string groupName = "Bank Teller";
        public string username;
        private string password;
        public bool BankTellerSignIn()
        {
            Console.WriteLine("Bank Teller Login");

            // Prompt for username
            Console.Write("Enter your username: ");
            username = Console.ReadLine();

            // Prompt for password
            Console.Write("Enter your password: ");
            password = Console.ReadLine();
            return Authenticate();
        }
        public bool Authenticate()
        {

            //Verfiy Validity Of User Credentials
            PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, domainName);
            bool validCreds = domainContext.ValidateCredentials(username, password);

            //Verify Group Membership Of User Account
            UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, username);
            bool isGroupMember = false;

            if (userPrincipal != null)
                isGroupMember = userPrincipal.IsMemberOf(domainContext, IdentityType.SamAccountName, groupName);

            //Output
            if (validCreds && isGroupMember)
            {
                Console.WriteLine("User Is Authorized To Perform Access Control Protected Action");
                return true;
            } else {
                Console.WriteLine("User Is Not Authorized To Perform This Action.");
                if (validCreds == false)
                    Console.WriteLine("Invalid User Credentials Provided.");
                if (isGroupMember == false)
                    Console.WriteLine("User Is Not A Member Of The Authorized User Group.");
                return false;
            }
        }
        
    }
}
