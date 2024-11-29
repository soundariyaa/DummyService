using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PieHandlerService.Core.Converters;
using PieHandlerService.Core.Models;
using System.Dynamic;


namespace PieHandlerService.Core.Extensions;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails MakeMeDestructible(this ProblemDetails problemDetails) => problemDetails.MakeMeDestructible(true);

    public static ProblemDetails MakeMeDestructible(this ProblemDetails problemDetails, bool serializeMoreInfoValues) =>
        serializeMoreInfoValues ? MakeMeDestructibleWithSerializedMoreInfoValues(problemDetails) : MakeMeDestructibleAsObject(problemDetails);

    private static ProblemDetails MakeMeDestructibleWithSerializedMoreInfoValues(ProblemDetails problemDetails)
    {
        if (problemDetails?.MoreInfo == null ||
            !problemDetails.MoreInfo.Any())
        {
            return problemDetails.Clone() ?? throw new ArgumentNullException(nameof(problemDetails)); ;
        }

        var destructibleMe = problemDetails.Clone();
        destructibleMe.MoreInfo.Clear();
        foreach (var (key, value) in problemDetails.MoreInfo)
        {
            destructibleMe.MoreInfo.Add(key,
                value is string ? value : JsonConvert.SerializeObject(value, new WriteRawJsonConverter()));
        }
        return destructibleMe;
    }

    private static ProblemDetails MakeMeDestructibleAsObject(ProblemDetails problemDetails)
    {
        if (!TryGetDestructibleMoreInfo(problemDetails.MoreInfo, out var moreInfo))
        {
            return problemDetails.Clone();
        }

        return new ProblemDetails(problemDetails.Clone())
        {
            MoreInfo = moreInfo
        };
    }

    private static bool TryGetDestructibleMoreInfo(Dictionary<string, object> originMoreInfo, out Dictionary<string, object> moreInfo)
    {
        moreInfo = new Dictionary<string, object>();
        try
        {
            if (originMoreInfo == null || !originMoreInfo.Any())
            {
                return false;
            }

            foreach (var (key, value) in originMoreInfo.Where(item => item.Value != null))
            {
                switch (value)
                {
                    case ExpandoObject:
                        moreInfo.Add(key, value);
                        break;
                    case JObject when TryConvertToExpandoObject(value, out var expandoObject):
                        moreInfo.Add(key, expandoObject);
                        break;
                    default:
                        moreInfo.Add(key, value);
                        break;
                }
            }
            return true;
        }
        catch
        {
            moreInfo = new Dictionary<string, object>();
            return false;
        }
    }

    private static bool TryConvertToExpandoObject(object value, out ExpandoObject expandoObject)
    {
        
        try
        {
            dynamic expandedExpando = new ExpandoObject();
            expandedExpando.value = value;
            expandoObject = JsonConvert.DeserializeObject<ExpandoObject>(expandedExpando);
            return true;
        }
        catch
        {
            expandoObject = new ExpandoObject();
            return false;
        }
    }
}
