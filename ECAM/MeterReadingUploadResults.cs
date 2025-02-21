namespace ECAM
{
    public class MeterReadingUploadResults
    {
        public int Successful { get; set; }

        public int Failed { get; set; }

        public int Total { get { return Successful + Failed; } }
    }
}
