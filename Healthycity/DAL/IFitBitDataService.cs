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
        async void NewFitBitUser(FitBitUser new_user);
        void RemoveFitBitUser(FitBitUser user);
    }
}
