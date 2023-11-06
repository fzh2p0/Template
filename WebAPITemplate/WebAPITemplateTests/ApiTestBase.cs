using Microsoft.Extensions.DependencyInjection;
using WebAPITemplate.AppData;
using WebAPITemplate.Models;

namespace WebAPITemplateTests
{
    /// <summary>
    /// This class is a base for all functional tests and some of them are writing into the context.
    /// If this tests would run parallel, some test cases would fail, because of wrong data.
    /// </summary>
    [Collection("Non-Parallel")]
    public class ApiTestBase
    {
        protected readonly string ApplicationBaseUri;
        protected readonly HttpClient HttpClient;

        public ApiTestBase()
        {
            var applicationBaseUri = new Uri($"https://functional.test.{new Random().Next().ToString()}");
            ApplicationBaseUri = applicationBaseUri.OriginalString;

            var serverFactory = new ServerFactory(applicationBaseUri);

            var dbContext = serverFactory.Services.GetRequiredService<ProductContext>();
            FillProducts(dbContext);

            HttpClient = serverFactory.CreateClient();

            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Functional.Test");
        }

        internal static HttpRequestMessage GetHttpRequestMessage(string route)
        {
            var requestMessage = new HttpRequestMessage();
            requestMessage.Method = HttpMethod.Get;
            requestMessage.RequestUri = new Uri(route);
            return requestMessage;
        }

        protected HttpResponseMessage SendRequestToDataProviderApi(string route)
        {
            var requestMessage = GetHttpRequestMessage(route);

            var result = HttpClient
            .SendAsync(requestMessage)
                         .Result;
            return result;
        }

        private static void FillProducts(ProductContext apiContext)
        {
            apiContext.Product.Add(new Product()
            {
                Id = new Guid("4a358fa4-87ee-4068-b860-09a2303a4090"),
                Name = "Product1",
                Description = "Description1",
                Price = new Decimal(10.0),
                DeliveryPrice = new Decimal(1.5)
            });

            apiContext.Product.Add(new Product()
            {
                Id = new Guid("14c76585-3c74-43c1-8d3f-43592e0d1c03"),
                Name = "Product2",
                Description = "Description2",
                Price = new Decimal(20.0),
                DeliveryPrice = new Decimal(2.5)
            });

            apiContext.SaveChanges();
        }
    }
}