using System.Reflection;

namespace TheOmenDen.CrowsOnTheShelf.Data;

public static class AssemblyValuesForBars
{
    public static string AssemblyProductVersion
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);

            return attributes.Length == 0
                ? String.Empty
                : ((AssemblyFileVersionAttribute)attributes[0]).Version;
        }
    }

    public static string ApplicationDevelopmentCompany
    {
        get
        {
            var attributes = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            return attributes.Length == 0
                ? String.Empty
                : ((AssemblyCompanyAttribute)attributes[0]).Company;
        }
    }
}