namespace Api.Infrastructure;

public static partial class DependencyInjection
{
    public static class DesignTimeDbContextFactory
    {
        public static bool IsDesignTime =>
            AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName?.Contains("Microsoft.EntityFrameworkCore.Design") == true);
    }
}