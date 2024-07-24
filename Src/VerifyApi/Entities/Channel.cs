using Newtonsoft.Json;

namespace VerifyApi.Entities;

/// <summary>
/// Channel delivery method
/// </summary>
public enum Channel
{
    /// <summary>
    /// Voice delivery channel
    /// </summary>
    /// <value>Voice delivery channel</value>
    [JsonProperty("voice")]
    Voice,
    /// <summary>
    /// SMS delivery channel
    /// </summary>
    /// <value>SMS delivery channel</value>
    [JsonProperty("sms")]
    Sms
}