using Microsoft.AspNetCore.Http;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Extensions;

internal class HeaderDictionaryExtensionsImpl : IIHeaderDictionaryExtensions
{
    public CommonHttpHeaderFilter GetCommonHttpHeaderFilter(IHeaderDictionary headerDictionary)
    {
        var commonHttpHeaderFilter = new CommonHttpHeaderFilter
        {
            AcceptLanguage = new AcceptLanguage
            {
                LanguageCode = headerDictionary
                .Where(item => item.Key.Equals(Constants.ExpectedCustomHttpHeaderParameters.AcceptLanguage, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value).FirstOrDefault()
            },
            Authorization = new Authorization
            {
                Bearer = headerDictionary
                .Where(item => item.Key.Equals(Constants.ExpectedCustomHttpHeaderParameters.Authorization, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value).FirstOrDefault()
            },
            XRoute = headerDictionary
                .Where(item => item.Key.Equals(Constants.ExpectedCustomHttpHeaderParameters.XRoute, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value).FirstOrDefault(),
            XVersion = headerDictionary
                .Where(item => item.Key.Equals(Constants.ExpectedCustomHttpHeaderParameters.XVersion, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value).FirstOrDefault(),
            XRequestId = headerDictionary
                .Where(item => item.Key.Equals(Constants.ExpectedCustomHttpHeaderParameters.XRequestId, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value).FirstOrDefault(),
            Traceparent = headerDictionary
                .Where(item => item.Key.Equals(Constants.ExpectedCustomHttpHeaderParameters.Traceparent, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value).FirstOrDefault()
        };
        return commonHttpHeaderFilter;
    }
}

public static class HeaderDictionaryExtensions
{
    internal static IIHeaderDictionaryExtensions Implementation { private get; set; } = new HeaderDictionaryExtensionsImpl();

    public static CommonHttpHeaderFilter GetCommonHttpHeaderFilter(this IHeaderDictionary headerDictionary)
    {
        return Implementation == null ? new CommonHttpHeaderFilter() : Implementation.GetCommonHttpHeaderFilter(headerDictionary);
    }
}
