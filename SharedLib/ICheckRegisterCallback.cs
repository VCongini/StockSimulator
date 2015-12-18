using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    public interface ICheckRegisterCallback
    {
        [OperationContract(IsOneWay = true)]
        void RegisterUpdated(TransactionList tx);
    }
}
