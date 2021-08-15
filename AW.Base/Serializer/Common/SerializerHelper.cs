using AW.Log;

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AW
{
    public interface IReference
    {
        int ReferenceId { get; set; }
    }


    public static class SerializerHelper
    {
        internal static ILogger Logger { get; }


        static SerializerHelper()
        {
            Logger = new Logger("Serializer", "serializer.log");

            var mainAsm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly() ?? Assembly.GetCallingAssembly();
            foreach (var refAsmName in mainAsm.GetReferencedAssemblies())
            {
                try
                {
                    Assembly.Load(refAsmName);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, $"Assembly: {refAsmName.FullName}");
                }
            }

            var folder = new FileInfo(mainAsm.Location).Directory.FullName;
            foreach (var path in Directory.GetFiles(folder).Where(p => p.EndsWith(".dll")))
            {
                try
                {
                    Assembly.LoadFrom(path);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, $"File: {path}");
                }
            }
        }


        public static void SaveText(string data, string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);

                using (var stream = new FileStream(path, FileMode.CreateNew))
                {
                    var info = new UTF8Encoding(true).GetBytes(data);
                    stream.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        public static string LoadText(string path)
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


        public static void SaveByte(string stringBit, string path)
        {
            try
            {
                var data = stringBit
                    .Split('-')
                    .Select(b => byte.Parse(b, NumberStyles.HexNumber))
                    .ToArray();

                if (data?.Length == 0)
                    return;

                if (File.Exists(path))
                    File.Delete(path);

                using (var stream = new FileStream(path, FileMode.CreateNew))
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        public static string LoadByte(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return default;

                byte[] data;
                using (var fs = new FileStream(path, FileMode.Open))
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


        internal static object GetObject(Type type, object[] @params = null)
        {
            if (type.IsArray)
                return Activator.CreateInstance(type, 0);

            var constructors = type.GetConstructors();

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
                foreach (var constructor in constructors)
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
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameterType = parameters[i].ParameterType;

                    if (@params[i] != null)
                    {
                        var paramType = @params[i].GetType();

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


        internal static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);

            if (type != null)
                return type;

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }

            throw new ArgumentNullException(typeName);
        }
    }
}
