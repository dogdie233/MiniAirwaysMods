﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GiveMeAnUpgrade;

public static class FastMemberHelper<TClass, TValue> where TClass : class
{
    private static Dictionary<string, Func<TClass, TValue>> getters = new();
    private static Dictionary<string, Action<TClass, TValue>> setters = new();

    private static readonly BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

    public static TValue Get(string memberName, TClass instance)
    {
        if (!getters.TryGetValue(memberName, out var getter))
        {
            (var member, var isStatic) = FindMember(memberName);
            if (member == null)
                throw new Exception($"Couldn't find member {memberName} for type {typeof(TClass).Name}");

            var instanceParameterExpression = Expression.Parameter(typeof(TClass), "instance");
            var memberExpression = Expression.MakeMemberAccess(isStatic ? null : instanceParameterExpression, member);
            var lambda = Expression.Lambda<Func<TClass, TValue>>(memberExpression, [instanceParameterExpression]);
            getter = lambda.Compile();
            getters.Add(memberName, getter);
        }
        return getter(instance);
    }

    public static void Set(string memberName, TClass instance, TValue value)
    {
        if (!setters.TryGetValue(memberName, out var setter))
        {
            (var member, var isStatic) = FindMember(memberName);
            if (member == null)
                throw new Exception($"Couldn't find member {memberName} for type {typeof(TClass).Name}");
            if (member is PropertyInfo { GetMethod: null })
                throw new Exception($"Property {memberName} in type {typeof(TClass).Name} have no setter");

            var instanceParameterExpression = Expression.Parameter(typeof(TClass), "instance");
            var valueParameterExpression = Expression.Parameter(typeof(TValue), "value");
            var memberExpression = Expression.MakeMemberAccess(isStatic ? null : instanceParameterExpression, member);
            var assignExpression = Expression.Assign(memberExpression, valueParameterExpression);
            var lambda = Expression.Lambda<Action<TClass, TValue>>(assignExpression, [instanceParameterExpression, valueParameterExpression]);
            setter = lambda.Compile();
            setters.Add(memberName, setter);
        }
        setter(instance, value);
    }

    private static (MemberInfo member, bool isStatic) FindMember(string memberName)
    {
        if (typeof(TClass).GetProperty(memberName, bindingAttr) is { } property)
            return (property, property.GetMethod.IsStatic);

        if (typeof(TClass).GetField(memberName, bindingAttr) is { } field)
            return (field, field.IsStatic);

        return (null, false);
    }
}