using RDHub.Application.DTOs;
using RDHub.Application.Interfaces;
using RDHub.Domain.Aggregates;
using RDHub.Infrastructure.BankAdapters.Abstractions;
using System.Net.Http.Json;
using System.Text.Json;

namespace RDHub.Infrastructure.BankAdapters.MockBank;

// adapter que se comunica com o MockServer simulando um banco real
public class MockBankAdapter : IBankPixAdapter
{
    private readonly HttpClient _http;
    private readonly IMockBankTokenProvider _tokenProvider;

    public MockBankAdapter(HttpClient http, IMockBankTokenProvider tokenProvider, string bankId)
    {
        _http = http;
        _tokenProvider = tokenProvider;
        BankId = bankId;
    }

    public string BankId { get; }

    public async Task<BankChargeResponse> CreateCob(
        BankChargeRequest request, 
        Credential credential,
        CancellationToken ct = default)
    {
        var token = await _tokenProvider.GetTokenAsync(_http, credential, ct);

        var payload = new
        {
            txId = request.TxId,
            amount = request.Amount
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/charge/cob");
        httpRequest.Content = JsonContent.Create(payload);

        httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<JsonElement>(raw);

        return new BankChargeResponse(
            TxId: request.TxId,
            Status: result.GetProperty("status").GetString() ?? "Open",
            Emv: result.GetProperty("emv").GetString() ?? string.Empty);
    }

    public async Task<BankChargeResponse> CreateCobV(
        BankChargeRequest request,
        Credential credential,
        CancellationToken ct = default)
    {
        var token = await _tokenProvider.GetTokenAsync(_http, credential, ct);

        var payload = new
        {
            txId = request.TxId,
            amount = request.Amount
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/charge/cobv");
        httpRequest.Content = JsonContent.Create(payload);

        httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<JsonElement>(raw);

        return new BankChargeResponse(
            TxId: request.TxId,
            Status: result.GetProperty("status").GetString() ?? "Open",
            Emv: result.GetProperty("emv").GetString() ?? string.Empty);
    }

    public async Task<BankChargeStatusResponse> GetChargeStatusAsync(
        string txId, 
        Credential credential,
        CancellationToken ct = default)
    {
        string token = await _tokenProvider.GetTokenAsync(_http, credential, ct);

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/pix/{txId}");

        httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<JsonElement>(raw);

        var status = result.GetProperty("status").GetString() ?? "Open";
        decimal? paidAmount = null;
        DateTime? paidAt = null;

        if (status == "Paid")
        {
            paidAmount = result.GetProperty("paidAmount").GetDecimal();
            paidAt = result.GetProperty("paidAt").GetDateTime();
        }

        return new BankChargeStatusResponse(
            TxId: txId,
            Status: status,
            PaidAmount: paidAmount,
            PaidAt: paidAt);
    }
}