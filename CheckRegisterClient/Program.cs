using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLib;
using Utilities;
using System.ServiceModel;

namespace CheckRegisterClient
{
    class Program
    {
        static ICheckRegisterService m_Proxy = null;
        static decimal balance = 0;
        static int hitcount;

        enum MenuChoicesEnum
        {
            Quit = 0,
            AddCredit = 1,
            AddDebit = 2,
            GetAllTrans = 3,
            GetTransByDate = 4,
            GetCreditsByType = 5,
            GetDebitsByType = 6
        }

        static void Main(string[] args)
        {
            try
            {
                MenuChoicesEnum choice = MenuChoicesEnum.Quit;
                m_Proxy = ProxyGen.GetChannel<ICheckRegisterService>();
                m_Proxy.Login();
                do
                {
                    Console.Clear();
                    choice = ConsoleHelpers.ReadEnum<MenuChoicesEnum>("Enter selection: ");
                    switch (choice)
                    {
                        case MenuChoicesEnum.AddCredit:
                            AddCreditEntry();
                            break;
                        case MenuChoicesEnum.AddDebit:
                            AddDebitEntry();
                            break;
                        case MenuChoicesEnum.GetAllTrans:
                            GetAllTrans();
                            break;
                        case MenuChoicesEnum.GetTransByDate:
                            GetAllTransByDate();
                            break;
                        case MenuChoicesEnum.GetCreditsByType:
                            GetCreditsByType();
                            break;
                        case MenuChoicesEnum.GetDebitsByType:
                            GetDebitsByType();
                            break;
                    }
                    Console.WriteLine();
                    Console.WriteLine("Press <ENTER> to continue...");
                    Console.ReadLine();
                } while (choice != MenuChoicesEnum.Quit);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error occurred: {0}", ex.Message);
                Console.ResetColor();
            }
            finally
            {
                if (m_Proxy != null)
                {
                    m_Proxy.Logout();
                }
            }
            Console.Write("Press <ENTER> to quit...");
            Console.ReadLine();
        }

        /// <summary>
        /// Gets all debit transactions by type.
        /// </summary>
        private static void GetDebitsByType()
        {
            DebitTypeEnum debType = ConsoleHelpers.ReadEnum<DebitTypeEnum>("Enter type of debit entry to search on: ");
            List<Transaction> newList = null;
            try
            {
                newList = m_Proxy.GetDebitsByType(debType);
               
                Console.WriteLine("{0,7} {1,10} {2,-20} {3,10:N2} {4,8} {5,10} {6,12:N2}",
                                    "Check #", "Date", "Description", "Debit", "Fee", "Credit", "Balance");
                Console.WriteLine(new string('=', Console.WindowWidth));  //seperation of data

                foreach (Debit deb in newList)
                {
                    Console.WriteLine("{0,7} {1:MM/dd/yyyy} {2,-20} {3,10:N2} {4,8} {5,10} {6,12:N2}", 
                        deb.CheckNo, deb.Date, deb.Description, deb.Amount, deb.Fee, string.Empty, string.Empty);

                    hitcount++;
                }
                Console.WriteLine();
                Console.WriteLine("{0} results returned.", hitcount);
                hitcount = 0;
                newList = null;
            }
            catch(FaultException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;                             //  Throw error if fault occurs.
                Console.WriteLine("Error occured: {0}", ex.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Get all credit transactions by type.
        /// </summary>
        private static void GetCreditsByType()
        {
            CreditTypeEnum credType = ConsoleHelpers.ReadEnum<CreditTypeEnum>("Enter type of credit entry to search on: ");
            List<Transaction> newList = null;
            try
            {
                // Get the credits
                newList = m_Proxy.GetCreditsByType(credType);

                // Write out the header
                Console.WriteLine("{0,7} {1,10} {2,-20} {3,10:N2} {4,8} {5,10} {6,12:N2}",
                                    "Check #", "Date", "Description", "Debit", "Fee", "Credit", "Balance");
                Console.WriteLine(new string('=', Console.WindowWidth));  //seperation of data
                
                foreach (Credit cred in newList)
                {
                    Console.WriteLine("{0,7} {1:MM/dd/yyyy} {2,-20} {3,10:N2} {4,8} {5,10} {6,12:N2}",
                            string.Empty, cred.Date, cred.Description, string.Empty, string.Empty, cred.Amount, string.Empty);

                    hitcount++;
                }
                Console.WriteLine();
                Console.WriteLine("{0} results returned.", hitcount);
                hitcount = 0;
                newList = null;
            }
            catch(FaultException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;                             //  Throw error if fault occurs.
                Console.WriteLine("Error occured: {0}", ex.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Get all transactions between the start and end date.
        /// </summary>
        private static void GetAllTransByDate()
        {
            DateTime startDate = ConsoleHelpers.ReadDate("Enter a start date to search on: ", DateTime.MinValue, DateTime.MaxValue);
            DateTime endDate = ConsoleHelpers.ReadDate("Enter a start date to search on: ", DateTime.MinValue, DateTime.MaxValue);

            List<Transaction> newList = null;
            try
            {
                // Get all the transactions by date
                newList = m_Proxy.GetAllTransactionsByDateRange(startDate, endDate);
                
                // Write out the header
                Console.WriteLine("{0,7} {1,10} {2,-20} {3,10:N2} {4,8} {5,10} {6,12:N2}",
                "Check #", "Date", "Description", "Debit", "Fee", "Credit", "Balance");
                Console.WriteLine(new string('=', Console.WindowWidth));  //seperation of data
                    
                foreach (var transaction in newList)
                {
                    if (transaction is Credit)
                    {
                        Credit morphTran = transaction as Credit;
                        balance += morphTran.Amount;
                        Console.WriteLine(GetCreditString(morphTran));
                        hitcount++;
                    }
                    else
                    {
                        Debit morphTran = transaction as Debit;
                        balance -= (morphTran.Amount + morphTran.Fee);
                        Console.WriteLine(GetDebitString(morphTran));
                        hitcount++;
                    }
                }
                Console.WriteLine();
                Console.WriteLine("{0} results returned.", hitcount);
                hitcount = 0;
                newList = null;
            }
            catch (FaultException)
            {
                Console.ForegroundColor = ConsoleColor.Red;                             //  Throw error if fault occurs.
                Console.WriteLine("No entries could be found...");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Get all transactions and display the balance for each.
        /// </summary>
        private static void GetAllTrans()
        {
            
            List<Transaction> newList = null;
            try
            {
                // Get all transactions
                newList = m_Proxy.GetAllTransactions();
                
                // Write out the header
                Console.WriteLine("{0,7} {1,10} {2,-20} {3,10:N2} {4,8} {5,10} {6,12:N2}", 
                                    "Check #", "Date", "Description", "Debit", "Fee", "Credit", "Balance");
                Console.WriteLine(new string('=', Console.WindowWidth));  //seperation of data
                foreach (var transaction in newList)
                {
                    if(transaction is Credit)
                    {
                        Credit morphTran = transaction as Credit;
                        balance += morphTran.Amount;
                        Console.WriteLine(GetCreditString(morphTran));
                        hitcount++;
                    }
                    else
                    {
                        Debit morphTran = transaction as Debit;
                        balance -= (morphTran.Amount + morphTran.Fee);
                        Console.WriteLine(GetDebitString(morphTran));
                        hitcount++;
                    }
                }
                Console.WriteLine();
                Console.WriteLine("{0} results returned.", hitcount);
                hitcount = 0;
                newList = null;
                balance = 0;
            }
            catch (FaultException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;                             //  Throw error if fault occurs.
                Console.WriteLine("Error occured: {0}", ex.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Add a debit entry to the system.
        /// </summary>
        private static void AddDebitEntry()
        {
            try
            {
                // Initialize the check number
                int checkNo = 0;

                decimal amount = ConsoleHelpers.ReadDecimal("Enter a debit amount: ");
                string description = ConsoleHelpers.ReadString("Enter a debit description:  ");
                DebitTypeEnum type = ConsoleHelpers.ReadEnum<DebitTypeEnum>("Enter type of debit entry: ");
                
                if (type == DebitTypeEnum.Check)
                {
                    checkNo = ConsoleHelpers.ReadInt("Enter a check number: ");
                }
                decimal fee = ConsoleHelpers.ReadDecimal("Enter a debit fee amount:  ");

                Debit newDebit = m_Proxy.CreateDebitObject(DateTime.Now, description, amount, type, checkNo, fee);
                m_Proxy.AddDebit(newDebit);
            }
            catch(FaultException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;                             //  Throw error if fault occurs.
                Console.WriteLine("Error occured: {0}", ex.Message);
                Console.ResetColor();
            }

        }

        /// <summary>
        /// Add a credit entry to the system.
        /// </summary>
        private static void AddCreditEntry()
        {
            string description = ConsoleHelpers.ReadString("Enter a credit description:  ");
            decimal amount = ConsoleHelpers.ReadDecimal("Enter a credit amount: ");
            CreditTypeEnum credType = ConsoleHelpers.ReadEnum<CreditTypeEnum>("Enter type of credit entry: ");
            
            try
            {
                Credit newCredit = m_Proxy.CreateCreditObject(DateTime.Now, description, amount, credType);
                m_Proxy.AddCredit(newCredit);
            }
            catch(FaultException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;                             //  Throw error if fault occurs.
                Console.WriteLine("Error occured: {0}", ex.Message);
                Console.ResetColor();
            }

        }

        //TO DO: Think about putting these together for one method.

        /// <summary>
        /// Create a string for credit objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetCreditString(Credit obj)
        {

            return string.Format("{0,7} {1:MM/dd/yyyy} {2,-20} {3,10:N2} {4,8} {5,10} {6,12:N2}", 
               string.Empty, obj.Date, obj.Description, string.Empty, string.Empty, obj.Amount, balance);
        }

        /// <summary>
        /// Create a string for debit objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetDebitString(Debit obj)
        {
            return string.Format("{0,7} {1:MM/dd/yyyy} {2,-20} {3,10:N2} {4,8} {5,10} {6,12:N2}", 
                obj.CheckNo, obj.Date, obj.Description, obj.Amount, obj.Fee, string.Empty, balance);
        }
    }
}

