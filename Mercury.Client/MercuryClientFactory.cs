using System.Net.Http;

namespace Mercury.Client
{
    public class MercuryClientFactory : IMercuryClientFactory
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly MercuryClientSettings settings;

        public MercuryClientFactory(IHttpClientFactory httpClientFactory, MercuryClientSettings settings)
        {
            this.httpClientFactory = httpClientFactory;
            this.settings = settings;
        }

        public IMercuryClient CreateClient()
        {
            return new MercuryClient(httpClientFactory.CreateClient(nameof(IMercuryClient)), settings);
        }
    }
}