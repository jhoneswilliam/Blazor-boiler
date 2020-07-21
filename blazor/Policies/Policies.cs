using blazor.Enums.Security;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazor.Policies
{
    public static class PoliciesConfig
    {
        public static AuthorizationPolicy Perm1()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .RequireClaim(EnumPermissions.Perm1.ToString(), true.ToString())
                                                   .Build();
        }

        public static AuthorizationPolicy Perm2()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .RequireClaim(EnumPermissions.Perm2.ToString(), true.ToString())
                                                   .Build();
        }
    }
}
