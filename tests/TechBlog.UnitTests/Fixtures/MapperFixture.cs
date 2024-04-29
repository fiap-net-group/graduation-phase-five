using Mapster;
using System.Reflection;
using TechBlog.Application.Email.Send.Boundaries;

namespace TechBlog.UnitTests.Fixtures
{
    public static class MapperFixture
    {
        public static void AddMapper(Assembly assembly = null)
        {
            TypeAdapterConfig.GlobalSettings.Scan(assembly ?? Assembly.GetExecutingAssembly());
        }
    }
}
