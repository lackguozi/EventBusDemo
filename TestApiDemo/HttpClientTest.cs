namespace TestApiDemo
{
    public class HttpClientTest : IHttpClientTest
    {
        private readonly HttpClient _httpClient;
        public HttpClientTest(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Test(int a)
        {
            Console.WriteLine(a.ToString() +"--------------------");
        }
    }
}
