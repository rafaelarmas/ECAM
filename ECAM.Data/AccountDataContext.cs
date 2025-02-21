using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text;

namespace ECAM.Data
{
    public class AccountDataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public AccountDataContext(IConfiguration configuration)
        {
            Configuration = configuration;
            Database.OpenConnection();
            Database.EnsureCreated();

            if (Accounts == null || Accounts.Count() == 0)
                Seed();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
        }
        public override void Dispose()
        {
            Database.CloseConnection();
            base.Dispose();
        }

        public void Seed()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ECAM.Data.Resource.Test_Accounts.csv");

            if (stream == null)
                return;

            var streamReader = new StreamReader(stream);

            string? line;
            
            while((line = streamReader.ReadLine()) != null)
            {
                var values = line.Split(',');

                // TODO: Add better validation.
                int accountId;
                bool isValidAccount = int.TryParse(values[0], out accountId);

                if (!isValidAccount)
                    continue;

                var account = new Account();
                account.Id = accountId;
                account.FirstName = values[1];
                account.LastName = values[2];
                Accounts.Add(account);
            }
            SaveChanges();
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<MeterReading> MeterReadings { get; set; }
    }
}
