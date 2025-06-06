using System.Collections.Generic;

public class ApplyToDictionary<T>
{
    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();
        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            dict[field.Name] = field.GetValue(this);
        }
        return dict;
    }
}