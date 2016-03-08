using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Audit.Audit;

namespace WebApplication1
{
    public class AppContext : IAppContext
    {
        public string UserName
        {
            get { return "UserText"; }
        }
    }
}
