using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MockDataSystem
{
    public class CustomAttributes
    {
        private interface IMockAttribute
        {
            object GenerateValue(Type targetType = null);
        }

        public class BaseAttribute : Attribute, IMockAttribute
        {
            public virtual object GenerateValue(Type targetType = null)
            {
                throw new NotImplementedException();
            }
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class IntValueAttribute : BaseAttribute
        {
            private readonly int _value;
            public IntValueAttribute(int value) => _value = value;
            public override object GenerateValue(Type targetType = null) => _value;
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class IntRandomAttribute : BaseAttribute
        {
            private readonly int _min;
            private readonly int _max;

            public IntRandomAttribute(int min, int max)
            {
                _min = min;
                _max = max;
            }

            public override object GenerateValue(Type targetType = null) => UnityEngine.Random.Range(_min, _max + 1);
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class StringValueAttribute : BaseAttribute
        {
            private readonly string _value;
            public StringValueAttribute(string value) => _value = value;
            public override object GenerateValue(Type targetType = null) => _value;
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class FloatValueAttribute : BaseAttribute
        {
            private readonly float _value;
            public FloatValueAttribute(float value) => _value = value;
            public override object GenerateValue(Type targetType = null) => _value;
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class FloatRandomAttribute : BaseAttribute
        {
            private readonly float _min;
            private readonly float _max;

            public FloatRandomAttribute(float min, float max)
            {
                _min = min;
                _max = max;
            }

            public override object GenerateValue(Type targetType = null) => UnityEngine.Random.Range(_min, _max);
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class BoolValueAttribute : BaseAttribute
        {
            private readonly bool _value;
            public BoolValueAttribute(bool value) => _value = value;
            public override object GenerateValue(Type targetType = null) => _value;
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class BoolRandomAttribute : BaseAttribute
        {
            public override object GenerateValue(Type targetType = null) => UnityEngine.Random.value > 0.5f;
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class ListValueAttribute : BaseAttribute
        {
            private readonly object[] _values;
            public ListValueAttribute(params object[] values) => _values = values;

            public override object GenerateValue(Type targetType = null)
            {
                if (targetType == null || !targetType.IsGenericType ||
                    targetType.GetGenericTypeDefinition() != typeof(List<>))
                {
                    throw new ArgumentException("ListValueAttribute requires a List<T> type.");
                }

                var elementType = targetType.GetGenericArguments()[0];
                var list = Activator.CreateInstance(targetType);
                var addMethod = targetType.GetMethod("Add");

                foreach (var value in _values)
                {
                    if (elementType.IsInstanceOfType(value))
                    {
                        addMethod.Invoke(list, new[] { value });
                    }
                    else
                    {
                        Debug.LogWarning($"Value {value} is not compatible with list element type {elementType.Name}");
                    }
                }

                return list;
            }
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class ListRandomAttribute : BaseAttribute
        {
            private readonly int _minCount;
            private readonly int _maxCount;
            private readonly Type _attributeType;
            private readonly object[] _attributeParams;

            public ListRandomAttribute(int minCount, int maxCount, Type attributeType, params object[] attributeParams)
            {
                _minCount = minCount;
                _maxCount = maxCount;
                _attributeType = attributeType;
                _attributeParams = attributeParams;
            }

            public override object GenerateValue(Type targetType = null)
            {
                if (targetType == null || !targetType.IsGenericType ||
                    targetType.GetGenericTypeDefinition() != typeof(List<>))
                {
                    throw new ArgumentException("ListRandomAttribute requires a List<T> type.");
                }

                if (!_attributeType.GetInterfaces().Contains(typeof(IMockAttribute)))
                {
                    throw new ArgumentException($"Type {_attributeType.Name} must implement IMockAttribute.");
                }

                Type elementType = targetType.GetGenericArguments()[0];
                var list = Activator.CreateInstance(targetType);
                var addMethod = targetType.GetMethod("Add");

                int count = UnityEngine.Random.Range(_minCount, _maxCount + 1);
                var attributeInstance = (IMockAttribute)Activator.CreateInstance(_attributeType, _attributeParams);

                for (int i = 0; i < count; i++)
                {
                    object value = attributeInstance.GenerateValue(elementType);
                    if (elementType.IsInstanceOfType(value))
                    {
                        addMethod.Invoke(list, new[] { value });
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Generated value {value} is not compatible with list element type {elementType.Name}");
                    }
                }

                return list;
            }
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
        public class NestedMockAttribute : BaseAttribute
        {
            public override object GenerateValue(Type targetType = null)
            {
                if (targetType == null || targetType.IsPrimitive || targetType == typeof(string))
                {
                    throw new ArgumentException("NestedMockAttribute requires a class or struct type.");
                }

                // MockGenerator tarafından işlenecek
                return null;
            }
        }
    }
}