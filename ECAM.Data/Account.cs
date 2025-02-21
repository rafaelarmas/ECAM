namespace ECAM.Data
{
    public class Account
    {
        public Account()
        {
            if (MeterReadings == null)
                MeterReadings = new List<int>();
        }

        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public List<int> MeterReadings { get; set; }
    }
}
