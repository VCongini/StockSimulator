using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLib;
using System.ServiceModel;
using System.Collections.Concurrent;


namespace CheckRegisterServiceLib
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class CheckRegisterService : ICheckRegisterService
    {

        // Logon methods and attributes
        private ConcurrentDictionary<string, ClientContainer> m_Clients;

        public CheckRegisterService()
        {
            m_Clients = new ConcurrentDictionary<string, ClientContainer>();
        }

        /// <summary>
        /// Method for client login.
        /// </summary>
        public void Login()
        {
            string sessionID = OperationContext.Current.SessionId;
            if (!m_Clients.ContainsKey(sessionID))
            {
                Console.WriteLine("Client '{0} logged in.", sessionID);
                //m_Clients.TryAdd(sessionID, new ClientContainer(client, false));
            }
            else
            {
                var msg = string.Format("A client with the token '{0}' has already logged in!", sessionID);
                Console.WriteLine(msg);
                throw new FaultException(msg);
            }
        }

        /// <summary>
        /// Method for logging out client.
        /// </summary>
        /// <param name="sessionID"></param>
        public void Logout(string sessionID = null)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionID))
                {
                    sessionID = OperationContext.Current.SessionId;
                }
                if (m_Clients.ContainsKey(sessionID))
                {
                    Console.WriteLine("Client '{0}' logged off.", sessionID);
                    ClientContainer removedItem;
                    if (!m_Clients.TryRemove(sessionID, out removedItem))
                    {
                        var msg = string.Format("Unable to remove client '{0}'", sessionID);
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("An error occured removing client '{0}' : {1}", sessionID, ex.Message);

                Console.WriteLine(msg);
                throw new FaultException(msg);
            }
        }

        /// <summary>
        /// Method to add a debit object to system.
        /// </summary>
        /// <param name="debit"></param>
        public void AddDebit(Debit debit)
        {
            var data = DataStore.LoadData();
            
            try
            {
                data.Add(debit);
                DataStore.SaveData();
            }
            catch(Exception ex)
            {
                Console.WriteLine("The following error occured: {0}", ex.Message);
                throw new FaultException(ex.Message);
            }
        }

        /// <summary>
        /// Method to add credit object to system.
        /// </summary>
        /// <param name="credit"></param>
        public void AddCredit(Credit credit)
        {
            var data = DataStore.LoadData();

            try
            {
                data.Add(credit);
                DataStore.SaveData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("The following error occured: {0}", ex.Message);
                throw new FaultException(ex.Message);
            }
        }

        /// <summary>
        /// Method to list all transactions.
        /// </summary>
        /// <returns></returns>
        public List<Transaction> GetAllTransactions()
        {
            var data = DataStore.LoadData();
            var query = from transaction in data
                        let debit = transaction as Debit
                        let credit = transaction as Credit
                        where credit != null || debit != null
                        orderby transaction.Date, transaction.Amount, transaction.Description ascending
                        select transaction;
            return query.ToList();
        }

        /// <summary>
        /// Method to list all transactions in a date range.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Transaction> GetAllTransactionsByDateRange(DateTime start, DateTime end)
        {
            var data = DataStore.LoadData();
            var query = from transaction in data
                        let debit = transaction as Debit
                        let credit = transaction as Credit
                        where transaction.Date >= start && transaction.Date <= end
                        orderby transaction.Date, transaction.Amount, transaction.Description ascending
                        select transaction;
            return query.ToList();
        }

        /// <summary>
        /// Method to get all credit transactions by type.
        /// </summary>
        /// <param name="creditType"></param>
        /// <returns></returns>
        public List<Transaction> GetCreditsByType(CreditTypeEnum creditType)
        {
            var data = DataStore.LoadData();
            var query = from transaction in data
                        let credit = transaction as Credit
                        where credit != null && credit.CreditType == creditType
                        orderby transaction.Date, transaction.Amount, transaction.Description ascending
                        select transaction;
            return query.ToList();
        }

        /// <summary>
        /// Method to get all debits by type.
        /// </summary>
        /// <param name="debitType"></param>
        /// <returns></returns>
        public List<Transaction> GetDebitsByType(DebitTypeEnum debitType)
        {
            var data = DataStore.LoadData();
            var query = from transaction in data
                        let debit = transaction as Debit
                        where debit != null && debit.DebitType == debitType
                        orderby transaction.Date, transaction.Amount, transaction.Description ascending
                        select transaction;
            return query.ToList();
        }

        /// <summary>
        /// Method for creating a debit object.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="description"></param>
        /// <param name="amount"></param>
        /// <param name="debitType"></param>
        /// <param name="checkNo"></param>
        /// <param name="fee"></param>
        /// <returns></returns>
        public Debit CreateDebitObject(DateTime date, string description, decimal amount, DebitTypeEnum debitType, int checkNo, decimal fee)
        {
            Debit newDebit = null;
            try
            {
                newDebit = new Debit() { Date = date, Description = description, Amount = amount, DebitType = debitType, CheckNo = checkNo, Fee = fee };
            }
            catch(Exception ex)
            {
                Console.WriteLine("The following error occured: {0}", ex.Message);
                throw new FaultException(ex.Message);
            }
            return newDebit;
        }

        /// <summary>
        /// Method for creating a credit object.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="description"></param>
        /// <param name="amount"></param>
        /// <param name="creditType"></param>
        /// <returns></returns>
        public Credit CreateCreditObject(DateTime date, string description, decimal amount, CreditTypeEnum creditType)
        {
            Credit newCredit = null;
            try
            {
                newCredit = new Credit() { Date = date, Description = description, Amount = amount, CreditType = creditType };
            }
            catch(Exception ex)
            {
                Console.WriteLine("The following error occured: {0}", ex.Message);
                throw new FaultException(ex.Message);
            }
            return newCredit;
        }
    }
}
