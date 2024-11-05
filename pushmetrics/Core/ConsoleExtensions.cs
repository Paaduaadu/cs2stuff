public static class Extensions
{
    public static string ReadSecret(string envVariableName) =>
        File.ReadAllText(Environment.GetEnvironmentVariable(envVariableName)!).Trim();
}

