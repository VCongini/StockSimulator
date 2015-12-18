using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    public class Debit : Transaction
    {

        #region Properties

        [DataMember]
        public DebitTypeEnum DebitType { get; set; }

        [DataMember]
        public int CheckNo { get; set; }

        [DataMember]
        public decimal Fee { get; set; }
        
        #endregion Properties

        #region Constructors
        public Debit()
        {

        }
        public Debit(DateTime date, string description, decimal amount, DebitTypeEnum debitType, int checkNo, decimal fee) : base(date, description, amount)
        {
            DebitType = debitType;
            CheckNo = checkNo;
            Fee = fee;
        }
        #endregion Constructors

        #region Methods
        #endregion Methods
    }
}
