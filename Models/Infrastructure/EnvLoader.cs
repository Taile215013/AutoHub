using System;
using System.IO;

namespace AutoHub.Infrastructure
{
    public static class EnvLoader
    {
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                {
                    continue;
                }

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
