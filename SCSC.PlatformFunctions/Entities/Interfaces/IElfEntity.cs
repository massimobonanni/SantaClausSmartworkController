using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions.Entities.Interfaces
{
    public interface IElfEntity
    {
        void PackagingStarted(PackagingOperationModel package);
    }
}
