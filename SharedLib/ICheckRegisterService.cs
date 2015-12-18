using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace SharedLib
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ICheckRegisterService
    {

        #region Methods

        [OperationContract]
        Debit CreateDebitObject(DateTime date, string description, decimal amount, DebitTypeEnum debitType, int checkNo, decimal fee);

        [OperationContract]
        void AddDebit(Debit debit);

        [OperationContract]
        Credit CreateCreditObject(DateTime date, string description, decimal amount, CreditTypeEnum creditType);

        [OperationContract]
        void AddCredit(Credit credit);

        [OperationContract]
        List<Transaction> GetAllTransactions();

        [OperationContract]
        List<Transaction> GetAllTransactionsByDateRange(DateTime start, DateTime end);

        [OperationContract]
        List<Transaction> GetCreditsByType(CreditTypeEnum creditType);

        [OperationContract]
        List<Transaction> GetDebitsByType(DebitTypeEnum debitType);

        [OperationContract(IsInitiating = true)]
        void Login();

        [OperationContract(IsTerminating = true)]
        void Logout(string sessionID = null);

        #endregion Methods
    }
}
