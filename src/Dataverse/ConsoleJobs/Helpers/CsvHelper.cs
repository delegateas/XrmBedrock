using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Text;

namespace ConsoleJobs.Helpers;

internal static class CsvHelper
{
    public static void WriteToCsv<T>(string folderPath, string fileName, IEnumerable<T> items, bool appendToFileIfExists = false)
        where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentException("File path is empty", nameof(folderPath));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is empty", nameof(fileName));

        var filePath = Path.Combine(folderPath, fileName);

        FileInfo fi = new FileInfo(filePath);
        Directory.CreateDirectory(fi.DirectoryName);

        using StreamWriter writer = new StreamWriter(filePath, appendToFileIfExists, Encoding.UTF8);

        if (!appendToFileIfExists)
        {
            writer.WriteLine(GetCsvHeader<T>());
        }

        foreach (var item in items)
        {
            writer.WriteLine(RowToCsv(item));
        }
    }

    public static List<T> ReadFromCsv<T>(string filePath, bool hasHeader = true)
        where T : class, new()
    {
        var returnList = new List<T>();

        using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
        {
            var res = reader.ReadLine();
            if (hasHeader && res != null)
            {
                res = reader.ReadLine();
            }

            while (res != null)
            {
                returnList.Add(RowFromCsv<T>(res));
                res = reader.ReadLine();
            }
        }

        return returnList;
    }

    public static string GetCsvHeader<T>()
        where T : class, new()
    {
        var properties = typeof(T).GetProperties();
        var strings = new string[properties.Length];
        for (int i = 0; i < properties.Length; ++i)
        {
            if (properties[i].GetCustomAttributes(typeof(HeaderAttribute), true).FirstOrDefault() is HeaderAttribute headerNameAttribute)
            {
                strings[i] = headerNameAttribute.Name;
            }
            else
            {
                strings[i] = properties[i].Name;
            }
        }

        return string.Join(ConfigurationManager.AppSettings["CsvSeparator"], strings);
    }

    public static string RowToCsv<T>(T item)
        where T : class, new()
    {
        var properties = typeof(T).GetProperties();
        var strings = new string[properties.Length];
        for (int i = 0; i < properties.Length; ++i)
        {
            strings[i] = properties[i].GetValue(item)?.ToString() ?? string.Empty;
        }

        return string.Join(ConfigurationManager.AppSettings["CsvSeparator"], strings);
    }

    public static T RowFromCsv<T>(string item)
        where T : class, new()
    {
        var attributeStrings = item.Split(ConfigurationManager.AppSettings["CsvSeparator"].ToArray());
        var properties = typeof(T).GetProperties();

        if (attributeStrings.Length != properties.Length)
            throw new InvalidDataException("Number of columns in csv does not match number of properties in class");

        T returnval = new T();
        for (int i = 0; i < attributeStrings.Length; ++i)
        {
            var property = properties[i];
            object value = FromString(property.PropertyType, attributeStrings[i]);
            property.SetValue(returnval, value);
        }

        return returnval;
    }

    private static object FromString(Type type, string s)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Int32:
                if (type.IsEnum)
                    return Enum.Parse(type, s);

                return int.Parse(s, CultureInfo.InvariantCulture);
            case TypeCode.Int64:
                return long.Parse(s, CultureInfo.InvariantCulture);
            case TypeCode.String:
                return s;
            case TypeCode.Boolean:
                return bool.Parse(s);
            case TypeCode.DateTime:
                return DateTime.Parse(s, CultureInfo.InvariantCulture);
            case TypeCode.Double:
                return double.Parse(s, CultureInfo.InvariantCulture);
            case TypeCode.Decimal:
                return decimal.Parse(s, CultureInfo.InvariantCulture);
            case TypeCode.Object:
                if (Nullable.GetUnderlyingType(type) != null && !string.IsNullOrEmpty(s))
                    return FromString(Nullable.GetUnderlyingType(type), s);

                if (Guid.TryParse(s, out Guid res))
                    return res;

                return TypeDescriptor.GetConverter(type).ConvertFromString(null, CultureInfo.InvariantCulture, s);

            default:
                return TypeDescriptor.GetConverter(type).ConvertFromString(null, CultureInfo.InvariantCulture, s);
        }
    }
}
