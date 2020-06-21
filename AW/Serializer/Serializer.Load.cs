using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AW.Log;
using AW.Serializer.Common;

namespace AW.Serializer
{
    public partial class AWSerializer
    {
        private List<IReference> Sources { get; set; }

        private void AfterDeserialize(object obj)
        {
            Sources = new List<IReference>();

            GetSource(obj);
            SetReference(obj);
        }

        public T Deserialize<T>(string data)
        {
            if (string.IsNullOrEmpty(data))
                return (T)SerializerHelper.GetObject(typeof(T));

            try
            {
                int index = data.IndexOf("&");
                if (index < data.Length && int.TryParse(data.Substring(0, index), out int len))
                {
                    int start = len.ToString().Length + 1;
                    string types = data.Substring(start, len - 1);

                    data = data.Substring(types.Length + start);

                    if (!string.IsNullOrEmpty(types))
                    {
                        TypeTabel = new List<string>();

                        foreach (string t in types.Split('&'))
                            TypeTabel.Add(t);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            T result = (T)DeserializeObject(SerializerHelper.GetObject(typeof(T)), data);
            AfterDeserialize(result);

            return result;
        }

        private void GetSource(object obj, bool isReference = false)
        {
            Type type = obj?.GetType();

            if (type?.GetCustomAttribute<AWSerializableAttribute>() != null)
            {
                if (!isReference && obj is IReference reference)
                    Sources.Add(reference);

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

                            GetSource(keys.Current, isReference);
                            GetSource(values.Current, isReference);

                        }

                        continue;
                    }
                    else if (!(value is string) && value is IEnumerable enumerable)
                    {
                        foreach (object item in enumerable)
                            GetSource(item, isReference);

                        continue;
                    }

                    GetSource(value, isReference);
                }
            }
        }

        private void SetReference(object obj)
        {
            Type type = obj?.GetType();

            if (type?.GetCustomAttribute<AWSerializableAttribute>() != null)
            {
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.SetMethod == null || property.GetCustomAttribute<AWIgnoreAttribute>() != null)
                        continue;

                    object value = property.GetValue(obj);
                    bool isReference = property.GetCustomAttribute<AWReferenceAttribute>() != null;

                    if (value is string)
                        continue;

                    if (value is IDictionary dictionary)
                    {
                        IDictionary rDictionary = (IDictionary)SerializerHelper.GetObject(value.GetType());

                        IEnumerator keys = dictionary.Keys.GetEnumerator();
                        IEnumerator values = dictionary.Values.GetEnumerator();

                        for (int i = 0; i < dictionary.Count; ++i)
                        {
                            keys.MoveNext();
                            values.MoveNext();

                            object key = keys.Current;
                            object item = values.Current;

                            if (isReference)
                            {
                                object sourceKey = key;
                                object sourceItem = item;

                                if (sourceKey is IReference referenceKey)
                                    sourceKey = Sources.First(s => s.ReferenceId == referenceKey.ReferenceId);
                                if (sourceItem is IReference referenceItem)
                                    sourceItem = Sources.First(s => s.ReferenceId == referenceItem.ReferenceId);

                                rDictionary.Add(sourceKey, sourceItem);

                                continue;
                            }

                            SetReference(key);
                            SetReference(item);

                            rDictionary.Add(key, item);
                        }

                        property.SetValue(obj, rDictionary);
                    }
                    else if (value is IEnumerable enumerable)
                    {
                        Type itemType = value.GetType().IsArray ? enumerable.GetType().GetElementType() : enumerable.GetType().GenericTypeArguments[0];
                        List<object> items = new List<object>();

                        foreach (object item in enumerable)
                        {
                            if (isReference)
                            {
                                object sourceItem = item;

                                if (sourceItem is IReference referenceItem)
                                    sourceItem = Sources.First(s => s.ReferenceId == referenceItem.ReferenceId);

                                items.Add(sourceItem);

                                continue;
                            }

                            SetReference(item);

                            items.Add(item);
                        }

                        Type enumerableType = typeof(Enumerable);
                        MethodInfo castMethod = enumerableType.GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(itemType);

                        object castedItems = castMethod.Invoke(null, new[] { items });

                        if (value.GetType().IsArray)
                        {
                            MethodInfo toArrayMethod = enumerableType.GetMethod(nameof(Enumerable.ToArray)).MakeGenericMethod(itemType);
                            castedItems = toArrayMethod.Invoke(null, new[] { castedItems });
                        }
                        else
                        {
                            MethodInfo toListMethod = enumerableType.GetMethod(nameof(Enumerable.ToList)).MakeGenericMethod(itemType);
                            castedItems = toListMethod.Invoke(null, new[] { castedItems });
                        }

                        property.SetValue(obj, castedItems);

                        continue;
                    }

                    if (isReference && value is IReference referenceValue)
                    {
                        object sourceValue = Sources.FirstOrDefault(s => s.ReferenceId == referenceValue.ReferenceId && s.GetType() == referenceValue.GetType());

                        if (sourceValue != null)
                            property.SetValue(obj, sourceValue);

                        continue;
                    }

                    SetReference(value);
                }
            }
        }

        private object DeserializeObject(object obj, string data)
        {
            Type type = obj?.GetType();

            if (type?.GetCustomAttribute<AWSerializableAttribute>() != null)
            {
                foreach (KeyValuePair<string, string> param in Parse(data))
                {
                    PropertyInfo property = type.GetProperty(param.Key);

                    if (property != null)
                        property.SetValue(obj, DeserializeValue(param.Value));
                }

                return obj;
            }

            try
            {
                return Convert.ChangeType(data, type);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return null;
        }

        private object DeserializeValue(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            Type type = null;

            if (data.StartsWith(StringT))
            {
                data = data.Remove(0, StringT.Length);

                string lenString = data.Substring(0, data.IndexOf(']'));
                int len = int.Parse(lenString);

                data = data.Substring(2 + lenString.Length, len);
                type = typeof(string);
            }
            else
            {
                data = data.Remove(0, 1);
                type = GetSaveType(data.Substring(0, data.IndexOf(')')));
                data = data.Remove(0, data.IndexOf(')') + 1);
            }

            if (type == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(data))
                return Convert.ChangeType(null, type) ?? SerializerHelper.GetObject(type);

            if (type == typeof(string))
                return data;

            object value = SerializerHelper.GetObject(type);

            if (value is IDictionary dictionary)
            {
                Type keyType = dictionary.GetType().GenericTypeArguments[0];
                Type valueType = dictionary.GetType().GenericTypeArguments[1];

                foreach (KeyValuePair<string, string> param in Parse(data))
                {
                    string item = param.Value;
                    string k, v;

                    if (item.StartsWith(StringT))
                    {
                        string lenString = item.Substring(StringT.Length, item.IndexOf(']') - StringT.Length);
                        int len = int.Parse(lenString);

                        k = item.Substring(0, len + $"{StringT}{len}])".Length);
                        v = item.Substring(k.Length + 1);
                    }
                    else
                    {
                        k = item.Substring(0, item.IndexOf('|'));
                        v = item.Substring(k.Length + 1);
                    }

                    dictionary.Add(DeserializeValue(k), DeserializeValue(v));
                }

                return dictionary;
            }
            else if (value is IEnumerable enumerable)
            {
                Type itemType = value.GetType().IsArray ? enumerable.GetType().GetElementType() : enumerable.GetType().GenericTypeArguments[0];
                List<object> items = new List<object>();

                foreach (KeyValuePair<string, string> param in Parse(data))
                    items.Add(DeserializeValue(param.Value));

                Type enumerableType = typeof(Enumerable);

                MethodInfo castMethod = enumerableType.GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(itemType);
                object castedItems = castMethod.Invoke(null, new[] { items });

                if (value.GetType().IsArray)
                {
                    MethodInfo toArrayMethod = enumerableType.GetMethod(nameof(Enumerable.ToArray)).MakeGenericMethod(itemType);
                    return toArrayMethod.Invoke(null, new[] { castedItems });
                }
                else
                {
                    MethodInfo toListMethod = enumerableType.GetMethod(nameof(Enumerable.ToList)).MakeGenericMethod(itemType);
                    return toListMethod.Invoke(null, new[] { castedItems });
                }
            }

            if (type.IsEnum)
                return Enum.Parse(value.GetType(), data);

            if (value is Guid)
                return new Guid(data);

            return DeserializeObject(value, data);
        }

        private Dictionary<string, string> Parse(string data)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(data))
                return result;

            if (data[0] == '(')
                data = data.Remove(0, data.IndexOf(')') + 1);

            while (data.Length > 0)
            {
                string key, value;

                data = data.Remove(0, 2);
                key = data.Substring(0, data.IndexOf(']'));
                data = data.Remove(0, key.Length + 2);

                int start = 1;
                int end = 0;
                int index = 0;

                for (int i = 0; i < data.Length; ++i)
                {
                    char c = data[i];

                    if (c == FirstST && data.Substring(i).StartsWith(StringT))
                    {
                        i += StringT.Length;

                        string str = data.Substring(i);
                        string lenString = str.Substring(0, str.IndexOf(']'));

                        i += 2 + lenString.Length + int.Parse(lenString);

                        c = data[i];
                    }

                    if (c == '<')
                        start++;
                    else if (c == '>')
                        end++;

                    if (start == end)
                    {
                        index = i;
                        break;
                    }
                }

                value = data.Substring(0, index);
                data = data.Remove(0, value.Length);

                result.Add(key, value);

                if (!string.IsNullOrEmpty(data) && data[0] == '>')
                    data = data.Remove(0, 1);
            }

            return result;
        }

        public Type GetSaveType(string name)
        {
            if (int.TryParse(name, out int index) && index < TypeTabel?.Count)
                name = TypeTabel[index];

            return SerializerHelper.GetType(name);
        }
    }
}
