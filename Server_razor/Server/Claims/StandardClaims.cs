using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Claims
{
    public class StandardClaims : IClaim
    {
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string View = "View";
        public const string Delete = "Delete";

        public static string[] All = new[]
        {
            Add, Edit, View, Delete
        };
    }
}
