namespace LMS.Courses.IntegrationTests;

public static class CoursesIntegrationTestCollection
{
    public const string Name = "courses-application-integration";
}

[CollectionDefinition(CoursesIntegrationTestCollection.Name, DisableParallelization = true)]
public sealed class CoursesIntegrationTestCollectionDefinition
    : ICollectionFixture<CoursesApplicationFixture>
{
}