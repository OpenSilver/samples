using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace OpenSilverShowcase.Support.UI.Units;

public class ExampleBase : UserControl
{
    public ExampleBase()
    {
        Name = $"OSF_DEMO_{GetHashCode()}";
        Tag = GetUserControlAssemblyVersion(); // UserControl의 어셈블리 버전 가져오기
    }

    private string GetUserControlAssemblyVersion()
    {
        try
        {
            // UserControl 클래스의 어셈블리 정보를 가져옴
            Assembly assembly = typeof(UserControl).Assembly;
            AssemblyName assemblyNameInfo = assembly.GetName();
            Version version = assemblyNameInfo.Version;
            return version.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error getting assembly version: {ex.Message}");
            return "Unknown";
        }
    }
}