namespace TechBlog.UnitTests.Fixtures
{
    [CollectionDefinition(nameof(UnitTestsFixtureCollection))]
    public class UnitTestsFixtureCollection : ICollectionFixture<UnitTestsFixture>
    {
    }

    public class UnitTestsFixture
    {
        public HttpContextFixtures HttpContext { get; set; }

        public UnitTestsFixture()
        {
            HttpContext = new HttpContextFixtures();
        }
    }
}
