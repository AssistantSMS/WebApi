using System.Collections.Generic;

namespace ScrapMechanic.Domain.Constants
{
    public static class ApiAccess
    {
        public static string PublicBasic = "public-basic";
        public static string Public = "public-advanced";
        public static string Auth = "authenticated";
        public static string All = "all";

        public static List<string> PublicBasicControllers = new List<string>
        {
            "About",
            "Steam",
            "Version"
        };
    }
}
