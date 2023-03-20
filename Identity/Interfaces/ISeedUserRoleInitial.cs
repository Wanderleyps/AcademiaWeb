using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcademiaWeb.Identity.Interfaces
{
    interface ISeedUserRoleInitial
    {
        void SeedUsers();
        void SeedRoles();

    }
}
