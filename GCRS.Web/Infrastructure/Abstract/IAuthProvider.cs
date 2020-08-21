using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCRS.Web.Infrastructure.Abstract
{
    public interface IAuthProvider
    {
        bool Authentificate(string username, string pasword, bool remember);

        void LogOff();
    }
}
