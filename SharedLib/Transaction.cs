using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    [KnownType(typeof(Credit))]
    [KnownType(typeof(Debit))]
    [DataContract]
    public class Transaction
    {

        #region Properties

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        #endregion Properties

        #region Constructors

        public Transaction()
        {

        }

        public Transaction(DateTime date, string description, decimal amount)
        {
            Date = date;
            Description = description;
            Amount = amount;
        }

        #endregion Constructors

        #region Methods
        #endregion Methods
    }
}
