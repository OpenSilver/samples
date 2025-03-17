using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace OSFSample.Support.UI.Units;

public class ExampleBase : UserControl
{
    public ExampleBase()
    {
        Name = $"OSF_DEMO_{GetHashCode()}";
        Tag = GetAssemblyVersion("OpenSilver.dll");
    }

    private string GetAssemblyVersion(string assemblyName)
    {
        try
        {
            Assembly assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                // 어셈블리의 버전 정보를 가져옵니다.
                AssemblyName assemblyNameInfo = assembly.GetName();
                Version version = assemblyNameInfo.Version;
                return version.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting assembly version: {ex.Message}");
        }

        return "Unknown";
    }
}
