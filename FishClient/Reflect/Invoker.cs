using System.Reflection;

namespace FishClient.Reflect;

public class Invoker
{
    public static T PrivateMethod<T>(object obj, string name, object[] methodParams)
    {
        return (T) getMethod(obj, name, methodParams).Invoke(obj, methodParams);
    }

    private static MethodInfo getMethod(object obj, string name, object[] methodParams)
    {
        Type type = obj.GetType();
        List<MethodInfo> methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        while (type.BaseType != null)
        {
            type = type.BaseType;
            if(type == null) break;
            methods.AddRange(type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
        }
        return methods.Where(m => validateMethod(m, name, methodParams)).First();
    }

    private static bool validateMethod(MethodInfo method, string name, object[] methodParams)
    {
        if (method.Name != name) return false;
        ParameterInfo[] p = method.GetParameters();
        if (methodParams.Length > p.Length) return false;
        bool flag = true;
        for (var i = 0; i < p.Length; i++)
        {
            if (!methodParams[i].GetType().IsAssignableTo(p[i].ParameterType)) flag = false;
        }
        return flag;
    }
}