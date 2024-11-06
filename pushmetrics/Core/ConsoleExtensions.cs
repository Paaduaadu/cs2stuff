namespace pushmetrics.Core
{
    public static class ConsoleExtensions
    {
        public static string ReadSecret(string envVariableName) =>
            File.ReadAllText(Environment.GetEnvironmentVariable(envVariableName)!).Trim();
    }
}