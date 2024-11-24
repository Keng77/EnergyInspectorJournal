namespace InspectorJournal.Infrastructure;

//Преобразование словаря в объект
public static class Transformations
{
    public static T DictionaryToObject<T>(IDictionary<string, string> dict) where T : new()
    {
        var t = new T();
        var properties = t.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                continue;

            var item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

            // Find which property type (int, string, double? etc) the CURRENT property is...
            var tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

            // Fix nullables...
            var newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

            // ...and change the type
            var newA = Convert.ChangeType(item.Value, newT);
            t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
        }

        return t;
    }
}