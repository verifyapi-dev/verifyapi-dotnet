namespace VerifyApi.Infrastructure;

/// <summary>
/// VerifyAPI specific exceptions, please see <see cref="Message"/> for more details
/// </summary>
/// <param name="message">The description of the exception</param>
/// <param name="innerException">The inner exception</param>
public class VerifyApiException(string message, Exception? innerException = null) : Exception(message, innerException);