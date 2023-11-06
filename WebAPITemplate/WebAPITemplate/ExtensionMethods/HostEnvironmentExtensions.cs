namespace WebAPITemplate.ExtensionMethods
{
    internal static class HostEnvironmentExtensions
    {
        internal static bool IsIsolated(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.IsEnvironment("Isolated");
        }
    }
}