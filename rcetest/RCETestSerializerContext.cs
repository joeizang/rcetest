using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace rcetest;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Serialization  
    )]
[JsonSerializable(typeof(Payload))]
[JsonSerializable(typeof(FileContents))]
internal partial class RCETestSerializerContext : JsonSerializerContext
{
}