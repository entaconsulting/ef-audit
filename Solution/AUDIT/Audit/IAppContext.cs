using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audit.Audit
{
    public interface IAppContext
    {
        string UserName { get;  }
    }
}
