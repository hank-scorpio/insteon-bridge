using System.IO;
using System.Reflection;

namespace Moar.Http
{
    public static class Text
    {
        public static string LoadFromFile(string templatePath)
        {
            return File.ReadAllText(templatePath);
        }

        public static string LoadFromResource(string resourceName, Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {

                if (stream == null) return null;

                using (TextReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
