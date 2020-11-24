namespace Mercury.Client
{
    public class MercuryClientSettings
    {
        public MercuryClientSettings()
        {
            Version = 1;
        }

        public string BaseAddress { get; set; }

        public int Version { get; set; }
    }
}