using System.Text;
using System.Text.Json;
using Liberty.GmoPaymentGateway.Exceptions;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Models.Responses;
using Liberty.GmoPaymentGateway.Options;
using Liberty.GmoPaymentGateway.Utils;
using Microsoft.Extensions.Options;

namespace Liberty.GmoPaymentGateway.Services.Implementations;

public class GmoPaymentGatewayService(
    IOptions<GmoPaymentOptions> gmoPaymentOptions
) : IGmoPaymentGatewayService
{
    public async Task<OnLinePaymentSearchTradeResponse> SearchTradeAsync(
        SearchTradeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var gmoPayment = gmoPaymentOptions.Value;
        var url = $"{gmoPayment.PaymentHost}/{EndpointConstants.SearchTradeEndpoint}";

        // Dictionaryにまとめる
        var requestData = new Dictionary<string, string?>
        {
            { ParameterConstants.ShopId, gmoPayment.ShopId },
            { ParameterConstants.ShopPass, gmoPayment.ShopPassword },
            { ParameterConstants.OrderId, request.OrderId }
        };

        //requestDataをPOST送信可能な形式に変換
        using var postContent = new FormUrlEncodedContent(requestData);
        // 通信処理
        using var httpClient = new HttpClient();
        //送受信実行
        using var response = httpClient.PostAsync(
                url,
                postContent,
                cancellationToken
            )
            .Result;
        await using var responseContentStream = response.Content.ReadAsStreamAsync(cancellationToken).Result;
        using var streamReader = new StreamReader(
            responseContentStream,
            Encoding.UTF8
        );
        //テキスト形式で受信結果を取得
        var contentResult = await streamReader.ReadToEndAsync(cancellationToken);

        var result = HelperUtil.Convert<OnLinePaymentSearchTradeResponse>(contentResult);

        if (result.HasError)
        {
            throw new PaymentSearchTradeException(request.OrderId);
        }

        return result;
    }

    public async Task<OnLinePaymentCancelResponse> CancelAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.IsChangeAmount)
        {
            return await ChangeAmountAsync<CancelOrderRequest, OnLinePaymentCancelResponse>(request, cancellationToken);
        }

        var gmoPayment = gmoPaymentOptions.Value;
        var url = $"{gmoPayment.PaymentHost}/{EndpointConstants.CancelEndpoint}";

        // Dictionaryにまとめる
        var requestData = new Dictionary<string, string?>
        {
            { ParameterConstants.ShopId, gmoPayment.ShopId },
            { ParameterConstants.ShopPass, gmoPayment.ShopPassword },
            { ParameterConstants.AccessId, request.AccessId },
            { ParameterConstants.AccessPass, request.AccessPass },
            { ParameterConstants.JobCd, "CANCEL" },
            { ParameterConstants.Amount, request.Amount },
            { ParameterConstants.Tax, request.Tax }
        };

        //requestDataをPOST送信可能な形式に変換
        using var postContent = new FormUrlEncodedContent(requestData);
        // 通信処理
        using var httpClient = new HttpClient();
        //送受信実行
        using var response = httpClient.PostAsync(
                url,
                postContent,
                cancellationToken
            )
            .Result;
        await using var responseContentStream = response.Content.ReadAsStreamAsync(cancellationToken).Result;
        using var streamReader = new StreamReader(
            responseContentStream,
            Encoding.UTF8
        );
        //テキスト形式で受信結果を取得
        var contentResult = await streamReader.ReadToEndAsync(cancellationToken);

        var result = HelperUtil.Convert<OnLinePaymentCancelResponse>(contentResult);

        if (result.HasError)
        {
            throw new PaymentCancelException(request.AccessId);
        }

        return result;
    }

    public async Task<EntryTranResponse> EntryTranAsync(
        PaymentEntryTranRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var gmoPayment = gmoPaymentOptions.Value;
        var url = $"{gmoPayment.PaymentHost}/{EndpointConstants.EntryTranEndpoint}";

        var requestData = new Dictionary<string, string?>
        {
            { ParameterConstants.ShopId, gmoPayment.ShopId },
            { ParameterConstants.ShopPass, gmoPayment.ShopPassword },
            { ParameterConstants.OrderId, request.OrderId },
            { ParameterConstants.JobCd, gmoPayment.JobCd },
            { ParameterConstants.Amount, request.Amount.ToString("F0") },
            { ParameterConstants.Tax, request.Tax.ToString("F0") }
        };

        //requestDataをPOST送信可能な形式に変換
        using var postContent = new FormUrlEncodedContent(requestData);
        // 通信処理
        using var httpClient = new HttpClient();
        //送受信実行
        using var response = httpClient.PostAsync(
                url,
                postContent,
                cancellationToken
            )
            .Result;
        await using var responseContentStream = response.Content.ReadAsStreamAsync(cancellationToken).Result;
        using var streamReader = new StreamReader(
            responseContentStream,
            Encoding.UTF8
        );
        //テキスト形式で受信結果を取得
        var contentResult = await streamReader.ReadToEndAsync(cancellationToken);

        var result = HelperUtil.Convert<EntryTranResponse>(contentResult);

        if (result.HasError)
        {
            throw new PaymentTransactionException(request.OrderId ?? string.Empty);
        }

        return result;
    }

    public async Task<ExecTranResponse> ExecTranAsync(
        PaymentExecTranRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var gmoPayment = gmoPaymentOptions.Value;
        var url = $"{gmoPayment.PaymentHost}/{EndpointConstants.ExecTranEndpoint}";

        var requestData = new Dictionary<string, string?>
        {
            { ParameterConstants.AccessId, request.AccessId },
            { ParameterConstants.AccessPass, request.AccessPass },
            { ParameterConstants.OrderId, request.OrderId },
            { ParameterConstants.Method, "1" },
            { ParameterConstants.CardNo, request.CardNumber },
            { ParameterConstants.Expire, request.ExpiryDate },
            { ParameterConstants.SecurityCode, request.CCV }
        };

        //requestDataをPOST送信可能な形式に変換
        using var postContent = new FormUrlEncodedContent(requestData);
        // 通信処理
        using var httpClient = new HttpClient();
        //送受信実行
        using var response = httpClient.PostAsync(
                url,
                postContent,
                cancellationToken
            )
            .Result;
        await using var responseContentStream = response.Content.ReadAsStreamAsync(cancellationToken).Result;
        using var streamReader = new StreamReader(
            responseContentStream,
            Encoding.UTF8
        );
        //テキスト形式で受信結果を取得
        var contentResult = await streamReader.ReadToEndAsync(cancellationToken);

        var result = HelperUtil.Convert<ExecTranResponse>(contentResult);

        if (result.HasError)
        {
            throw new PaymentTransactionException(request.AccessId ?? string.Empty);
        }

        return result;
    }

    public async Task<string> GetPaymentUrlAsync(
        PaymentGetUrlRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var gmoPayment = gmoPaymentOptions.Value;
        var payMethods = gmoPayment.GetPayMethods();

        var requestBody = new
        {
            configid = gmoPayment.ConfigId,
            geturlparam = new
            {
                ShopID = gmoPayment.ShopId,
                ShopPass = gmoPayment.ShopPassword
            },
            transaction = new
            {
                OrderID = request.OrderId,
                request.Amount,
                request.Tax,
                PayMethods = payMethods,
                gmoPayment.RetUrl,
                gmoPayment.CompleteUrl,
                gmoPayment.CancelUrl
            },
            credit = new
            {
                gmoPayment.JobCd,
                gmoPayment.Tds2Type,
                gmoPayment.TdFlag
            },
            displaysetting = new { Lang = request.LanguageCode ?? "ja" }
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);

        using var httpClient = new HttpClient();
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync(gmoPayment.UrlPayment, content, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode || responseBody.Contains("LinkUrl"))
            {
                var result = JsonSerializer.Deserialize<GetUrlPaymentResponse>(responseBody);
                return result?.LinkUrl ?? string.Empty;
            }

            var resultError = JsonSerializer.Deserialize<List<ErrorGetUrlPaymentResponse>>(responseBody);
            var errInfo = resultError?.FirstOrDefault()?.ErrInfo ?? string.Empty;

            throw new PaymentGetUrlException(request.OrderId ?? string.Empty, errInfo);
        }
        catch (Exception ex)
        {
            throw new PaymentGetUrlException(request.OrderId ?? string.Empty, ex.Message);
        }
    }

    public async Task<OnlinePaymentChangeResponse> ChangeOrderAsync(
        ChangeOrderRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return await ChangeAmountAsync<ChangeOrderRequest, OnlinePaymentChangeResponse>(request, cancellationToken);
    }

    private async Task<U> ChangeAmountAsync<T, U>(
        T request,
        CancellationToken cancellationToken = default
    ) where T : ChangeTranBaseRequest
        where U : class, new()
    {
        var gmoPayment = gmoPaymentOptions.Value;
        var url = $"{gmoPayment.PaymentHost}/{EndpointConstants.ChangeAmountEndpoint}";

        // Dictionaryにまとめる
        var requestData = new Dictionary<string, string?>
        {
            { ParameterConstants.ShopId, gmoPayment.ShopId },
            { ParameterConstants.ShopPass, gmoPayment.ShopPassword },
            { ParameterConstants.AccessId, request.AccessId },
            { ParameterConstants.AccessPass, request.AccessPass },
            { ParameterConstants.JobCd, gmoPayment.JobCd },
            { ParameterConstants.Amount, request.Amount },
            { ParameterConstants.Tax, request.Tax }
        };

        //requestDataをPOST送信可能な形式に変換
        using var postContent = new FormUrlEncodedContent(requestData);
        // 通信処理
        using var httpClient = new HttpClient();
        //送受信実行
        using var response = httpClient.PostAsync(
                url,
                postContent,
                cancellationToken
            )
            .Result;
        await using var responseContentStream = response.Content.ReadAsStreamAsync(cancellationToken).Result;
        using var streamReader = new StreamReader(
            responseContentStream,
            Encoding.UTF8
        );
        //テキスト形式で受信結果を取得
        var contentResult = await streamReader.ReadToEndAsync(cancellationToken);

        var result = HelperUtil.Convert<U>(contentResult);

        return result switch
        {
            OnLinePaymentCancelResponse { HasError: true }
                => throw new PaymentChangeAmountException(request.AccessId),

            OnlinePaymentChangeResponse { HasError: true } when typeof(T) == typeof(ChangeOrderRequest)
                => throw new OnlinePaymentChangeOrderException(request.OrderId),

            _ => result
        };
    }
}
