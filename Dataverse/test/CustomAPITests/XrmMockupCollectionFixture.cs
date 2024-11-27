namespace LF.Medlemssystem.DataverseTests
{
    [CollectionDefinition("Xrm Collection")]
    public class XrmMockupCollectionFixture : ICollectionFixture<XrmMockupFixture> // Must be defined in same assembly as test classes.
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}