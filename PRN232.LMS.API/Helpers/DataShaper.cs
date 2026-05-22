using System.Dynamic;
using System.Reflection;

namespace PRN232.LMS.API.Helpers;

public static class DataShaper
{
    public static ExpandoObject ShapeData<T>(this T source, string? fields)
    {
        var dataShapedObject = new ExpandoObject();
        var propertyInfoList = new List<PropertyInfo>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            propertyInfoList.AddRange(propertyInfos);
        }
        else
        {
            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    continue;
                }

                propertyInfoList.Add(propertyInfo);
            }
        }

        foreach (var propertyInfo in propertyInfoList)
        {
            var propertyValue = propertyInfo.GetValue(source);
            ((IDictionary<string, object?>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
        }

        return dataShapedObject;
    }

    public static IEnumerable<ExpandoObject> ShapeData<T>(this IEnumerable<T> entities, string? fields)
    {
        var shapedData = new List<ExpandoObject>();
        var propertyInfoList = new List<PropertyInfo>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            propertyInfoList.AddRange(propertyInfos);
        }
        else
        {
            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    continue;
                }

                propertyInfoList.Add(propertyInfo);
            }
        }

        foreach (var entity in entities)
        {
            var dataShapedObject = new ExpandoObject();

            foreach (var propertyInfo in propertyInfoList)
            {
                var propertyValue = propertyInfo.GetValue(entity);
                ((IDictionary<string, object?>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }

            shapedData.Add(dataShapedObject);
        }

        return shapedData;
    }
}
