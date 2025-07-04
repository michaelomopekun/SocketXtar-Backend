namespace Infrastructure.Helpers;

public class EnvLoader
{
    public static void Load(string path = "../.env")
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);

        if (!File.Exists(fullPath)) return;

        foreach (var line in File.ReadAllLines(fullPath))
        {
            var variable = line.Split("=", 2);

            if (variable.Length != 2) continue;

            Environment.SetEnvironmentVariable(variable[0], variable[1]);
        }
    }
}
