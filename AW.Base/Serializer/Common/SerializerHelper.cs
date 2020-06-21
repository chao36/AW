using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using AW.Base.Log;

namespace AW.Base.Serializer.Common
{
    public static class SerializerHelper
    {
        static SerializerHelper()
        {
            Assembly mainAsm = Assembly.GetEntryAssembly();

            foreach (AssemblyName refAsmName in mainAsm.GetReferencedAssemblies())
            {
                try
                {
                    Assembly.Load(refAsmName);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, refAsmName.FullName, "SerializerHelper.Load");
                }
            }

            string folder = new FileInfo(mainAsm.Location).Directory.FullName;
            foreach (string path in Directory.GetFiles(folder).Where(p => p.EndsWith(".dll")))
            {
                try
                {
                    Assembly.LoadFrom(path);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, $"[path: {path}]", "SerializerHelper.Load");
                }
            }
        }

        public static void SaveText(string data, string path = "__data")
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);

                using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(data);
                    stream.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static string LoadText(string path = "__data")
        {
            try
            {
                if (File.Exists(path))
                    return File.ReadAllText(path);

                return default;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return default;
        }

        public static void SaveByte(string stringBit, string path = "__data")
        {
            try
            {
                byte[] data = stringBit
                    .Split('-')
                    .Select(b => byte.Parse(b, NumberStyles.HexNumber))
                    .ToArray();

                if (data?.Length == 0)
                    return;

                if (File.Exists(path))
                    File.Delete(path);

                using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static string LoadByte(string path = "__data")
        {
            try
            {
                if (!File.Exists(path))
                    return default;

                byte[] data;
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                }

                if (data?.Length == 0)
                    return default;

                return BitConverter.ToString(data);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return default;
        }

        public static object GetObject(Type type, object[] @params = null)
        {
            if (type.IsArray)
                return Activator.CreateInstance(type, 0);

            ConstructorInfo[] constructors = type.GetConstructors();

            if (constructors.Length == 0 || @params == null)
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            else
                foreach (ConstructorInfo constructor in constructors)
                {
                    if (IsParameters(constructor.GetParameters(), @params))
                    {
                        try
                        {
                            return constructor.Invoke(@params);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }

            Logger.Log($"Null control [type: {type.Name}]");

            return null;
        }

        private static bool IsParameters(ParameterInfo[] parameters, object[] @params)
        {
            if (parameters.Length == @params.Length)
            {
                for (int i = 0; i < parameters.Length; ++i)
                {
                    Type parameterType = parameters[i].ParameterType;

                    if (@params[i] != null)
                    {
                        Type paramType = @params[i].GetType();

                        if (parameterType != paramType)
                            if (parameterType.IsInterface)
                            {
                                if (!paramType.GetInterfaces().Contains(parameterType))
                                    return false;
                            }
                            else
                                return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static Type GetType(string typeName)
        {
            Type type = Type.GetType(typeName);

            if (type != null)
                return type;

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }

            throw new ArgumentNullException(typeName);
        }
    }
}
