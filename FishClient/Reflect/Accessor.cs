using System.Reflection;

namespace FishClient.Reflect;

public class Accessor
{
    public static T GetPrivateField<T>(object obj, string name)
    {
        return (T)getField(obj, name).GetValue(obj);
    }

    public static void SetPrivateField(object obj, string name, object value)
    {
        getField(obj, name).SetValue(obj, value);
    }

    public static T GetPrivateProperty<T>(object obj, string name)
    {
        return (T)getPropertyBackingField(obj, name).GetValue(obj);
    }

    public static void SetPrivateProperty(object obj, string name, object value)
    {
        getPropertyBackingField(obj, name).SetValue(obj, value);
    }

    private static FieldInfo getField(object obj, string name)
    {
        Type type = obj.GetType();
        List<FieldInfo> fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        while (type.BaseType != null)
        {
            type = type.BaseType;
            if(type == null) break;
            fields.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
            
        }
        return fields.Where(o => o.Name == name).First();
    }
    
    private static FieldInfo getPropertyBackingField(object obj, string name)
    {
        Type dec = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(o => o.Name == name).First().DeclaringType;
        List<FieldInfo> fields = dec.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
        while (dec.BaseType != null)
        {
            dec = dec.BaseType;
            if(dec == null) break;
            fields.AddRange(dec.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        }
        return fields.Where(o => o.Name == $"<{name}>k__BackingField").First();
    }
}