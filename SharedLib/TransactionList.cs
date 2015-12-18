using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    [CollectionDataContract]
    public class TransactionList : List<Transaction>
    {
        #region Constructors
        public TransactionList()
        {

        }

        public TransactionList(IEnumerable<Transaction> source) : base(source)
        {

        }
        #endregion Constructors
    }
}
