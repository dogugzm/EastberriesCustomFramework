using System;
using System.Collections.Concurrent;
using System.Reflection;
using UnityEngine;

namespace MockDataSystem
{
    public interface IMockGenerator
    {
        public T Generate<T>(int recursionDepth = 0) where T : new();
    }

    public class MockGenerator : IMockGenerator
    {
        private readonly ConcurrentDictionary<Type, (FieldInfo[], PropertyInfo[])> _typeCache = new();
        private const int MaxRecursionDepth = 5;

        public T Generate<T>(int recursionDepth = 0) where T : new()
        {
            if (recursionDepth > MaxRecursionDepth)
            {
                Debug.LogWarning($"Max recursion depth reached for type {typeof(T).Name}. Returning default.");
                return default;
            }

            try
            {
                object instance = new T(); // object ref for structs
                Type type = typeof(T);

                var (fields, properties) = _typeCache.GetOrAdd(type, t => (
                    t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                    t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ));

                foreach (var field in fields)
                {
                    if (TryGenerateMockValue(field, recursionDepth, out var value))
                    {
                        field.SetValue(instance, value);
                    }
                }

                foreach (var property in properties)
                {
                    if (property.CanWrite && TryGenerateMockValue(property, recursionDepth, out var value))
                    {
                        property.SetValue(instance, value);
                    }
                }

                return (T)instance;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Mock generation failed for type {typeof(T).Name}: {ex.Message}");
                throw;
            }
        }

        private bool TryGenerateMockValue(MemberInfo memberInfo, int recursionDepth, out object value)
        {
            value = null;
            var mockAttr = memberInfo.GetCustomAttribute<CustomAttributes.BaseAttribute>();
            if (mockAttr == null) return false;

            Type memberType = memberInfo is FieldInfo fi ? fi.FieldType : ((PropertyInfo)memberInfo).PropertyType;
            object generatedValue = mockAttr.GenerateValue(memberType);

            if (mockAttr is CustomAttributes.NestedMockAttribute && generatedValue == null)
            {
                if (memberType.IsClass ||
                    (memberType.IsValueType && !memberType.IsPrimitive && memberType != typeof(string)))
                {
                    var generateMethod = typeof(MockGenerator).GetMethod(nameof(Generate), new[] { typeof(int) })
                        ?.MakeGenericMethod(memberType);
                    try
                    {
                        value = generateMethod.Invoke(this, new object[] { recursionDepth + 1 });

                        if (value != null || memberType.IsValueType)
                        {
                            bool isFilled = false;
                            var fields = memberType.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                              BindingFlags.Instance);
                            foreach (var field in fields)
                            {
                                var fieldValue = field.GetValue(value);
                                if (fieldValue != null && !fieldValue.Equals(field.FieldType.IsValueType
                                        ? Activator.CreateInstance(field.FieldType)
                                        : null))
                                {
                                    isFilled = true;
                                    break;
                                }
                            }

                            if (!isFilled)
                            {
                                Debug.LogWarning(
                                    $"NestedMockAttribute on {memberInfo.Name} produced an empty struct check fields.");
                                value = Activator.CreateInstance(memberType); // new instance
                                return false;
                            }

                            return true;
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"Failed to generate mock for {memberInfo.Name} of type {memberType.Name}.");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(
                            $"Recursive mock generation failed for {memberInfo.Name} of type {memberType.Name}: {ex.Message}");
                        return false;
                    }
                }
                else
                {
                    Debug.LogWarning($"NestedMockAttribute on {memberInfo.Name} requires a class or struct type.");
                    return false;
                }
            }

            if (generatedValue != null && memberType.IsAssignableFrom(generatedValue.GetType()))
            {
                value = generatedValue;
                return true;
            }

            if (memberType.IsValueType && generatedValue == null)
            {
                value = Activator.CreateInstance(memberType);
                Debug.LogWarning(
                    $"Generated null for value type {memberType.Name} on {memberInfo.Name}. Using default instance.");
                return true;
            }

            Debug.LogWarning(
                $"Type mismatch for {memberInfo.Name}: Expected {memberType.Name}, got {generatedValue?.GetType().Name}");
            return false;
        }
    }
}