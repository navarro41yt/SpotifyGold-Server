namespace SpotifyGoldServer.Utils;

public class StringEnum<T> where T : class
{
    public static string[] Values()
    {
        var reflection = typeof(T).GetFields();

        var values = new List<string>();
        foreach (var field in reflection) {
            if (field.IsStatic && field.IsPublic) {
                var value = field.GetValue(null);
                if (value is string strValue) {
                    values.Add(strValue);
                }
            }
        }

        return values.ToArray();
    }
}
