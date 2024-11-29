using Microsoft.AspNetCore.Http;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface IIHeaderDictionaryExtensions
{
    CommonHttpHeaderFilter GetCommonHttpHeaderFilter(IHeaderDictionary headerDictionary);
}
