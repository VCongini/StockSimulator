using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLib;

namespace CheckRegisterServiceLib
{
    public class ClientContainer
    {
        #region Properties
        public bool IsActive { get; set; }
        #endregion Properties

        #region Constructors
        public ClientContainer()
        {

        }
        public ClientContainer(bool isActive)
        {
            IsActive = isActive;
        }
        #endregion Constructors
    }
}
