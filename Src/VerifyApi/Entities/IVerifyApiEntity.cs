using VerifyApi.Infrastructure;

namespace VerifyApi.Entities;

/// <summary>
/// Interface that identifies all entities returned by VerifyAPI
/// </summary>
public interface IVerifyApiEntity
{
    /// <summary>
    /// The VerifyAPI response
    /// </summary>
    VerifyApiResponse VerifyApiResponse { get; set; }
}