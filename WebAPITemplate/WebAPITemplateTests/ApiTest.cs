using FluentAssertions;
using System.Text.Json;
using WebAPITemplate.Models;

namespace WebAPITemplateTests
{
    public class ApiTest : ApiTestBase
    {
        [Fact]
        public async void GetProducts___DatabaseValid___ResultJSONValid()
        {
            // arrange
            var route = $"{ApplicationBaseUri}/products";

            // act
            var result = SendRequestToDataProviderApi(route);

            // assert
            var content = await result.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<Products>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            products.Should().NotBeNull();
            products?.Items.Count.Should().Be(2);
            foreach (var product in products.Items)
            {
                product.Id.Should().NotBeEmpty();
            }
        }

        [Theory]
        [MemberData(nameof(GetProductRequestDataForTest))]
        public async void GetProduct___DatabaseValidRequestValid___ResultJSONValid(Guid searchKey, string expectedName)
        {            // arrange
            var route = $"{ApplicationBaseUri}/products/{searchKey}";

            // act
            var result = SendRequestToDataProviderApi(route);

            // assert
            var content = await result.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<Product>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            product.Should().NotBeNull();
            product.Name.Should().Be(expectedName);
        }

        public static IEnumerable<object[]> GetProductRequestDataForTest()
        {
            yield return new object[]
                {
                    new Guid("4a358fa4-87ee-4068-b860-09a2303a4090"),
                    "Product1"
                };
            yield return new object[]
            {
                    "14c76585-3c74-43c1-8d3f-43592e0d1c03",
                    "Product2"
            };
        }
    }
}
