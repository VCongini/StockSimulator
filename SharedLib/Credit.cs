using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    [DataContract]
    public class Credit : Transaction
    {
        #region Properties
        [DataMember]
        public CreditTypeEnum CreditType { get; set; }
        #endregion Properties

        #region Constructors
        public Credit()
        {

        }
        public Credit(DateTime date, string description, decimal amount, CreditTypeEnum creditType) : base(date, description, amount)
        {
            CreditType = creditType;
        }
        #endregion Constructors

        #region Methods
        #endregion Methods
    }
}
