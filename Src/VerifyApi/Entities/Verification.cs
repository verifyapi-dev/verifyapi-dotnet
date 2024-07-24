using Newtonsoft.Json;

namespace VerifyApi.Entities;

public class Verification : VerifyApiEntity<Verification>
{
    /// <summary>
    /// Unique request identifier
    /// </summary>
    /// <value>Unique request identifier</value>
    [JsonProperty("id")]
    public string? Id { get; set; }
        
    /// <summary>
    /// Verification destination number
    /// </summary>
    /// <value>Verification destination number</value>
    [JsonProperty("destination")]
    public string? Destination { get; set; }

    /// <summary>
    /// Verification delivery channel
    /// </summary>
    /// <value>Verification delivery channel</value>
    [JsonProperty("channel")]
    public Channel? Channel { get; set; }
}