using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

public class HomeController : Controller
{
    private static readonly HttpClient _httpClient;
    private readonly string _apiRootEndpoint = "http://127.0.0.1/";

    // Static constructor is used to initialize any static data. In this case, HttpClient.
    static HomeController()
    {
        _httpClient = new HttpClient();
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> AuthorizedRequest()
    {
        var httpApiRequest = new HttpRequestMessage(HttpMethod.Get, $"{ _apiRootEndpoint }Api");    // Get from vault.
        var jwtAccessToken = await GetAuthenticationToken();

        httpApiRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtAccessToken);
        
        Send(httpApiRequest);        

        return View("Index", ViewBag.StatusCode);
    }

    private async Task<string> GetAuthenticationToken()
    {
        string jwtAccessToken = String.Empty;
        string uriAuth0 = "https://dev-1bkxbx0p.us.auth0.com/oauth/token";

        var dataToSendToAuth0 = GetDataToSend();            

        var identityProviderHttpResponse = await new HttpClient().PostAsync($"{ uriAuth0 }", dataToSendToAuth0);

        if (identityProviderHttpResponse.IsSuccessStatusCode)
            jwtAccessToken = ExtractAccessToken(identityProviderHttpResponse);

        return jwtAccessToken;
    }

    private FormUrlEncodedContent GetDataToSend() =>
        new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", "I7RKT8cvMlnRkwbfecWiBQJ54f9ISmXY"),
            new KeyValuePair<string, string>("client_secret", "m7NkrngLy7aPv4BCNDL4RP224QSuT-8A7QXt086F4kDpQG9yHUv1PY5R4ekd-0WR"),
            new KeyValuePair<string, string>("audience", _apiRootEndpoint),
        });

    private string ExtractAccessToken(HttpResponseMessage identityProviderHttpResponse)
    {
        var serializedTokenData = identityProviderHttpResponse.Content.ReadAsStringAsync().Result;
        var tokenData = (serializedTokenData != null) ? JsonSerializer.Deserialize<TokenType>(serializedTokenData) : null;

        return tokenData.access_token;
    }

    public IActionResult UnauthorizedRequest()
    {
        var apiClient = new HttpClient();

        var httpApiRequest = new HttpRequestMessage(HttpMethod.Get, $"{ _apiRootEndpoint }Api");   // Armazenar no Vault
        Send(httpApiRequest);

        return View("Index");
    }

    private HttpResponseMessage Send(HttpRequestMessage request)
    {
        HttpResponseMessage response = null;

        try
        {
            response = _httpClient.SendAsync(request).Result;
            ViewBag.StatusCode = response.StatusCode;
        }
        catch (HttpRequestException ex)
        {
            ViewBag.StatusCode = ex.StatusCode;
        }
        catch (AggregateException)
        {
            ViewBag.StatusCode = System.Net.HttpStatusCode.NotFound;
        }

        return response;
    }
}
