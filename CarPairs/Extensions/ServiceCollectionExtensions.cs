using CarPairs.Web.Services.Interfaces;
using CarPairs.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace CarPairs.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, IConfiguration config)
    {
        var baseUrl = config["ApiSettings:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");

        var baseUri = new Uri(baseUrl);

        services.AddHttpClient<IPartApiService, PartApiService>(client =>
        {
            client.BaseAddress = baseUri;
        });

        services.AddHttpClient<ILookupApiService, LookupApiService>(client =>
        {
            client.BaseAddress = baseUri;
        });

        services.AddHttpClient<IManufacturerApiService, ManufacturerApiService>(client =>
        {
            client.BaseAddress = baseUri;
        });

        return services;
    }
}