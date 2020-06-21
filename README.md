# AW.Base
Project containing data logger and serializer

## Logger

Logger can save exception, message and method name. Example:
```C#
using AW.Base.Log;

public void LogTest()
{
    Logger.Log("test");
}
// [21.06 03:17:20]: LogTest() - test
```

## Serializer

Serializer can save and load any data. Example:
```C#
using AW.Base.Serializer;
using AW.Base.Serializer.Common;

[AWSerializable]
public class Test
{
    public DateTime Date { get; set; } = DateTime.Now;
    public double D { get; set; } = 2.09;

    public List<int> LI { get; set; } = new List<int>
    {
        1, 2, 3, 4, 5
    };

    public int[] AI { get; set; } = new int[2] { 1, 2 };

    public List<string> LS { get; set; } = new List<string>
    {
       "222", "asasa", "dwww"
    };

    public Dictionary<string, string> DS { get; set; } = new Dictionary<string, string>
    {
        { "s1", "asas" }, { "s2", "asasas" }, { "s3", "aghghsas" }
    };
}

Test test = new Test();
string data = null;

using (AWSerializer serializer = new AWSerializer())
{
    // Get save data
    data = serializer.Serialize(test);
}

test = null;

using (AWSerializer serializer = new AWSerializer())
{
    // Load from data
    test = serializer.Deserialize<Test>(data);
}
```

SerializerHelper can save data to file as string or byte array. Example:
```C#
using AW.Base.Serializer.Common;

Test test = new Test();
string data = null;

//...

SerializerHelper.SaveText(data, "fileName"); // .SaveByte() to save string as byte array

data = null;
test = null;

data = SerializerHelper.LoadText("fileName"); // .LoadByte() to load string from byte array

//...
```

Serializer can save all reference as one object. Example:
```C#
using AW.Base.Serializer;
using AW.Base.Serializer.Common;

[AWSerializable]
public class Reference : IReference
{
    // Don't use this property 
    public int ReferenceId { get; set; }

    // Any data
}

[AWSerializable]
public class TestReference
{
    // Source
    public Reference F1 { get; set; } = new Reference();

    // Reference to source will be saved after save and load
    [AWReference]
    public List<Reference> References { get; set; } = new List<Reference>();

    public TestReference()
    {
        References.Add(F1);
        References.Add(F1);
        References.Add(F1);
        References.Add(F1);
    }
}
```

# AW.Visual
Project contains some wpf control (used [Material Design](http://materialdesigninxaml.net/) )

## Init
To start in WPF application:
```xaml
// App.xaml
<Application 
    x:Class="AW.VisualTests.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:AW.VisualTests"
    Startup="Application_Startup">
    // Any name [Application_Startup]
...
```
```C#
// App.xaml.cs
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    // Change color theme (dark/light and main color)
    AWWindow.ChangeTheme(true, ColorHelper.TealSet.Color700.ToMediaColor());
}

private void Application_Startup(object sender, StartupEventArgs e)
{
    // Init resource
    AWWindow.Init();
    
    // Create window with any control as content
    Window window = new AWWindow(new Control());

    window.Show();
}
```

## DialogHelper
