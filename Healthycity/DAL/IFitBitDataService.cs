using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthycity.DAL
{
    interface IFitBitDataService
    {
        FitBitUser GetFitBitUserByName(string name);
        void NewFitBitUser(FitBitUser new_user);
        void RemoveFitBitUserById(string id);
        void ModifyFitBitUser(FitBitUser user);
        IEnumerable<FitBitUser> GetAllFitBitUsers();
    }
}
