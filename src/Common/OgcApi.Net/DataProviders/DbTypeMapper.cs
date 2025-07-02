namespace OgcApi.Net.DataProviders;

public static class DbTypeMapper
{
    public static string MapDbTypeToSimpleType(string dbTypeName)
    {
        if (string.IsNullOrEmpty(dbTypeName)) return "unknown";

        var typeName = dbTypeName.ToLowerInvariant();

        if (IsStringType(typeName)) return "string";
        if (IsNumberType(typeName)) return "number";
        if (IsBooleanType(typeName)) return "boolean";
        if (IsGeometryType(typeName)) return "geometry";

        return "unknown";
    }

    private static bool IsStringType(string typeName) =>
        typeName.Contains("char") ||
        typeName.Contains("text") ||
        typeName.Contains("uuid") ||
        typeName.Contains("xml") ||
        typeName.Contains("json") ||
        typeName.Contains("date") ||
        typeName.Contains("time") ||
        typeName.Contains("interval");

    private static bool IsNumberType(string typeName) =>
        typeName.Contains("int") ||
        typeName.Contains("bigint") ||
        typeName.Contains("smallint") ||
        typeName.Contains("tinyint") ||
        typeName.Contains("decimal") ||
        typeName.Contains("numeric") ||
        typeName.Contains("money") ||
        typeName.Contains("real") ||
        typeName.Contains("float") ||
        typeName.Contains("double");

    private static bool IsBooleanType(string typeName) =>
        typeName.Contains("bit") ||
        typeName.Contains("bool");

    private static bool IsGeometryType(string typeName) =>
            typeName.Contains("geometry") ||
            typeName.Contains("geography") ||
            typeName.Contains("point") ||
            typeName.Contains("polygon") ||
            typeName.Contains("linestring");
}