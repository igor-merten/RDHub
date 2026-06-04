namespace RDHub.API.Contracts.Credentials;

public sealed record UpdateCredentialRequest(
    string ClientSecret,
    string Certificate,
    string CertificatePassword);