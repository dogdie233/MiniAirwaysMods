using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GoodlifeShortcut;

public static class FastPropertyHelper<TClass, TValue> where TClass : class
{
    private static Dictionary<string, Func<TClass, TValue>> getters = new();
    private static Dictionary<string, Action<TClass, TValue>> setters = new();

    public static TValue Get(string propertyName, TClass instance)
    {
        if (!getters.TryGetValue(propertyName, out var getter))
        {
            var property = typeof(TClass).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            
            var instanceParameterExpression = Expression.Parameter(typeof(TClass), "instance");
            var propertyExpression = Expression.Property(property.SetMethod.IsStatic ? null : instanceParameterExpression, property);
            var lambda = Expression.Lambda<Func<TClass, TValue>>(propertyExpression, [instanceParameterExpression]);
            getter = lambda.Compile();
            getters.Add(propertyName, getter);
        }
        return getter(instance);
    }

    public static void Set(string propertyName, TClass instance, TValue value)
    {
        if (!setters.TryGetValue(propertyName, out var setter))
        {
            var property = typeof(TClass).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (property.SetMethod == null)
                throw new Exception($"Property {propertyName} in type {typeof(TClass).Name} have no setter");

            var instanceParameterExpression = Expression.Parameter(typeof(TClass), "instance");
            var valueParameterExpression = Expression.Parameter(typeof(TValue), "value");
            var propertyExpression = Expression.Property(property.SetMethod.IsStatic ? null : instanceParameterExpression, property);
            var assignExpression = Expression.Assign(propertyExpression, valueParameterExpression);
            var lambda = Expression.Lambda<Action<TClass, TValue>>(assignExpression, [instanceParameterExpression, valueParameterExpression]);
            setter = lambda.Compile();
            setters.Add(propertyName, setter);
        }
        setter(instance, value);
    }
}