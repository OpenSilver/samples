using System.Collections.Generic;
using System.Linq;

namespace OpenSilverShowcase.Support.Local.Helpers
{
    public static class EmbeddedResourceLoaderExtensions
    {
        public static string[] GetShowcaseCodeResources(this EmbeddedResourceLoader loader)
        {
            return loader.GetResourceNames()
                .Where(name => name.Contains("ShowcaseCodes") && name.EndsWith(".txt"))
                .ToArray();
        }

        public static Dictionary<string, string> LoadXamlResources(this EmbeddedResourceLoader loader)
        {
            return loader.LoadResourcesByPattern("*_Xaml.txt");
        }

        public static Dictionary<string, string> LoadCsResources(this EmbeddedResourceLoader loader)
        {
            return loader.LoadResourcesByPattern("*_Cs.txt");
        }

        public static Dictionary<string, string> LoadExampleResources(this EmbeddedResourceLoader loader, string exampleName)
        {
            return loader.LoadResourcesByPattern($"*{exampleName}*");
        }
    }
}
