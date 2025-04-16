using System.Text.Json.Serialization;

using Northwind.MinimalAot.Service;

namespace Northwind.Serialization;


[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(List<Product>))]
internal partial class NorthwindJsonSerializerContext : JsonSerializerContext
{

}
