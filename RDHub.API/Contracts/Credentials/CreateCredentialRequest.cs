namespace RDHub.API.Contracts.Credentials;

public sealed record CreateCredentialRequest(
    string ClientId,
    string ClientSecret,
    string Certificate,
    string CertificatePassword);