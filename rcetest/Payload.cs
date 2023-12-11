using System.Text.Json.Serialization;

namespace rcetest;

public partial class Payload
{
    [JsonPropertyName("manifest")]
    public string Manifest { get; set; } = "ethpm/3";
    
    [JsonPropertyName("sources")]
    public FileContents Sources { get; set; } = new FileContents();
}

public partial class FileContents
{
    [JsonPropertyName("contracts/BlindAuction.vy")]
    public BlindAuction ContractName { get; set; } = new BlindAuction();
}

public partial class BlindAuction
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = VyperCode.BlindAuction;
}