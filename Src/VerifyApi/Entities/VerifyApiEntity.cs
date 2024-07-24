using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using VerifyApi.Infrastructure;

namespace VerifyApi.Entities;

[JsonObject(MemberSerialization.OptIn)]
public abstract class VerifyApiEntity : IVerifyApiEntity
{
    [JsonIgnore]
    public bool IsSuccess => VerifyApiResponse?.StatusCode == HttpStatusCode.OK;

    [JsonIgnore] 
    public VerifyApiResponse VerifyApiResponse { get; set; } = default!;

    /// <summary>Deserializes the JSON to the specified VerifyAPI object type.</summary>
    /// <typeparam name="T">The type of the VerifyAPI object to deserialize to.</typeparam>
    /// <param name="value">The object to deserialize.</param>
    /// <returns>The deserialized VerifyAPI object from the JSON string.</returns>
    public static T FromJson<T>(string value) where T : IVerifyApiEntity
    {
        return (T)JsonConvert.DeserializeObject<T>(value)!;
    }
}

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Generic variant")]
public abstract class VerifyApiEntity<T> : VerifyApiEntity
    where T : VerifyApiEntity<T>
{
    /// <summary>Deserializes the JSON to a VerifyAPI object type.</summary>
    /// <param name="value">The object to deserialize.</param>
    /// <returns>The deserialized VerifyAPI object from the JSON string.</returns>
    public new static T FromJson(string value)
    {
        return VerifyApiEntity.FromJson<T>(value);
    }
}