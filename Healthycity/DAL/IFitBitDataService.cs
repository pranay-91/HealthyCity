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
        Task<int> NewFitBitUser(FitBitUser new_user);
        Task<int> RemoveFitBitUserById(string id);
        Task<int> ModifyFitBitUser(FitBitUser user);
        IEnumerable<FitBitUser> GetAllFitBitUsers();
    }
}
