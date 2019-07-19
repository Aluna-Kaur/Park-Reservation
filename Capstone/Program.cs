using Capstone.DAL;
using Microsoft.Extensions.Configuration;
using Security.BusinessLogic;
using Security.DAO;
using System;
using System.IO;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            TestAuthentication();
        }

        public static void TestAuthentication()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("Project");

            IUserSecurityDAO db = new UserSecurityDAO(connectionString);
            UserManager userMgr = new UserManager(db);
            CampgroundDBCLI cli = new CampgroundDBCLI(userMgr, connectionString);
            IcampgroundDAO cgdb = new CampgroundDAO(connectionString);
            cli.Run();
            
        }

    }
}
