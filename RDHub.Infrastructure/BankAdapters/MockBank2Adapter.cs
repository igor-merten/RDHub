using RDHub.Application.DTOs;
using RDHub.Application.Interfaces;
using RDHub.Infrastructure.BankAdapters.Abstractions;
using System.Text.Json;

namespace RDHub.Infrastructure.BankAdapters;

// adapter que se comunica com o MockServer simulando um banco real
public class MockBank2Adapter : IBankPixAdapter
{
    private readonly HttpClient _http;
    private readonly string _bankId;

    public MockBank2Adapter(HttpClient http, string bankId)
    {
        _http = http;
        _bankId = bankId;
    }

    public string BankId => _bankId;

    public async Task<BankChargeResponse> CreateChargeAsync(
        BankChargeRequest request,
        CancellationToken ct = default)
    {
        var payload = new
        {
            txId = request.TxId,
            amount = request.Amount,
            pixKey = request.PixKey
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await _http.PostAsync("/pix/charge", content, ct);
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
        CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/pix/{txId}", ct);
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