using AW.Serializer.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AW.Serializer
{
    public partial class AWSerializer : IDisposable
    {
        private int Id { get; set; }
        private StringBuilder Builder { get; set; }

        private const char FirstST = '~';
        private const string StringT = "~[";
        private List<string> TypeTabel { get; set; }

        private void BeforeSerialize(object obj)
        {
            Id = 1;
            Builder = new StringBuilder();
            TypeTabel = new List<string>();

            SetId(obj, zero: true);
            SetId(obj);
        }

        public string Serialize(object obj)
        {
            BeforeSerialize(obj);
            SerializeObj(obj);

            string types = "";

            foreach (string t in TypeTabel)
                types += $"&{t}";

            Builder.Insert(0, types.Length.ToString() + types);
            return Builder.ToString();
        }

        private void SetId(object obj, bool isReference = false, bool zero = false)
        {
            Type type = obj?.GetType();

            if (type?.GetCustomAttribute<AWSerializableAttribute>() != null)
            {
                if (obj is IReference reference)
                {
                    reference.ReferenceId = zero ? 0 : Id++;

                    if (isReference)
                        return;
                }

                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.SetMethod == null || property.GetCustomAttribute<AWIgnoreAttribute>() != null)
                        continue;

                    object value = property.GetValue(obj);
                    isReference = property.GetCustomAttribute<AWReferenceAttribute>() != null;

                    if (value is IDictionary dictionary)
                    {
                        IEnumerator keys = dictionary.Keys.GetEnumerator();
                        IEnumerator values = dictionary.Values.GetEnumerator();

                        for (int i = 0; i < dictionary.Count; ++i)
                        {
                            keys.MoveNext();
                            values.MoveNext();

                            SetId(keys.Current, isReference, zero);
                            SetId(values.Current, isReference, zero);
                        }

                        continue;
                    }
                    else if (!(value is string) && value is IEnumerable enumerable)
                    {
                        foreach (object item in enumerable)
                            SetId(item, isReference, zero);

                        continue;
                    }

                    SetId(value, isReference, zero);
                }
            }
        }

        private void SerializeObj(object obj)
        {
            Type type = obj?.GetType();

            if (type?.GetCustomAttribute<AWSerializableAttribute>() != null)
            {
                Builder.Append($"({GetTypeToSave(type)})");

                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.SetMethod == null || property.GetCustomAttribute<AWIgnoreAttribute>() != null)
                        continue;

                    object value = property.GetValue(obj);

                    Builder.Append($"<[{property.Name}]=");
                    SerializeValue(value, property);
                    Builder.Append(">");
                }
            }
            else if (type.IsPrimitive || type.IsEnum || (type.IsValueType && type.IsSerializable))
                Builder.Append($"({GetTypeToSave(type)}){obj}");
        }

        private void SerializeValue(object value, PropertyInfo valueProperty)
        {
            if (value == null)
                return;

            if (value is string str)
            {
                Builder.Append($"{StringT}{str.Length}]){str}");
                return;
            }

            if (value is IDictionary dictionary)
            {
                Builder.Append($"({GetTypeToSave(value)})");

                IEnumerator keys = dictionary.Keys.GetEnumerator();
                IEnumerator values = dictionary.Values.GetEnumerator();

                for (int index = 0; index < dictionary.Count; ++index)
                {
                    keys.MoveNext();
                    values.MoveNext();

                    Builder.Append($"<[{index}]=");
                    SerializeValue(keys.Current, valueProperty);
                    Builder.Append("|");
                    SerializeValue(values.Current, valueProperty);
                    Builder.Append(">");
                }
            }
            else if (value is IEnumerable enumerable)
            {
                Builder.Append($"({GetTypeToSave(value)})");
                int index = 0;

                foreach (object item in enumerable)
                {
                    Builder.Append($"<[{index++}]=");
                    SerializeValue(item, valueProperty);
                    Builder.Append(">");
                }
            }
            else if (valueProperty?.GetCustomAttribute<AWReferenceAttribute>() != null && value is IReference reference)
                Builder.Append($"({GetTypeToSave(reference)})<[{nameof(reference.ReferenceId)}]=({GetTypeToSave(reference.ReferenceId)}){reference.ReferenceId}>");
            else
                SerializeObj(value);
        }

        private int GetTypeToSave(object obj)
            => GetTypeToSave(obj.GetType());

        private int GetTypeToSave(Type t)
        {
            if (!TypeTabel.Contains(t.FullName))
                TypeTabel.Add(t.FullName);

            return TypeTabel.IndexOf(t.FullName);
        }

        public void Dispose()
        {
            Builder?.Clear();
            TypeTabel?.Clear();

            Sources?.Clear();
        }
    }
}
