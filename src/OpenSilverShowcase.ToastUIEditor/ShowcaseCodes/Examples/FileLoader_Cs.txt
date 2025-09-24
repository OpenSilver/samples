using OpenSilver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenSilverShowcase.ToastUIEditor.Examples;

internal static class FileLoader
{
    public static async Task<bool> TryLoadJavaScriptFile(string url)
    {
        try
        {
            await Interop.LoadJavaScriptFile(url);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public static async Task<bool> TryLoadCssFile(string url)
    {
        try
        {
            await Interop.LoadCssFile(url);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}
