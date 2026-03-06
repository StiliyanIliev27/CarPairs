using CarPairs.Web.Services.Interfaces;
using CarPairs.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http.Headers;

namespace CarPairs.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, IConfiguration config)
    {
        var baseUrl = config["ApiSettings:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");

        var baseUri = new Uri(baseUrl);

        services.AddHttpClient<IAccountApiService, AccountApiService>(client =>
        {
            client.BaseAddress = baseUri;
        }).AddHttpMessageHandler<JwtTokenHandler>();

        services.AddHttpClient<IPartApiService, PartApiService>(client =>
        {
            client.BaseAddress = baseUri;
        }).AddHttpMessageHandler<JwtTokenHandler>();

        services.AddHttpClient<ILookupApiService, LookupApiService>(client =>
        {
            client.BaseAddress = baseUri;
        }).AddHttpMessageHandler<JwtTokenHandler>();

        services.AddHttpClient<IManufacturerApiService, ManufacturerApiService>(client =>
        {
            client.BaseAddress = baseUri;
        }).AddHttpMessageHandler<JwtTokenHandler>();

        // Add the JWT token handler
        services.AddTransient<JwtTokenHandler>();

        return services;
    }
}

public class JwtTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}