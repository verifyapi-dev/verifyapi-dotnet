﻿using VerifyApi.Entities;

namespace VerifyApi;

public interface IVerifyApiClient
{
    /// <summary>
    /// Gets the base URL for Verify API
    /// </summary>
    /// <value>Base URL for Verify API</value>
    string ApiBaseUrl { get; }

    /// <summary>
    /// Gets the API key used by the client to authenticate requests
    /// </summary>
    /// <value>API key used when sending requests</value>
    string ApiKey { get; }

    /// <summary>
    /// Initiates a new <c>Verification</c> request
    /// </summary>
    /// <param name="destination">Destination number of the request</param>
    /// <param name="channel">Channel to deliver the request</param>
    /// <returns>Verification response</returns>
    Verification Verify(string destination, Channel channel);

    /// <summary>
    /// Initiates a new <c>Verification</c> request asynchronously
    /// </summary>
    /// <param name="destination">Destination number of the request</param>
    /// <param name="channel">Channel to deliver the request</param>
    /// <returns>Verification response</returns>
    Task<Verification> VerifyAsync(string destination, Channel channel);

    /// <summary>
    /// Confirms a <c>Verification</c> request
    /// </summary>
    /// <param name="id">Original request id as generated by <see cref="Verify"/></param>
    /// <param name="code">Code supplied by the end-user</param>
    /// <returns>Confirmation status</returns>
    Verification Confirm(string id, string code);

    /// <summary>
    /// Confirms a <c>Verification</c> request asynchronously
    /// </summary>
    /// <param name="id">Original request id as generated by <see cref="VerifyAsync"/></param>
    /// <param name="code">Code supplied by the end-user</param>
    /// <returns>Confirmation status</returns>
    Task<Verification> ConfirmAsync(string id, string code);
}