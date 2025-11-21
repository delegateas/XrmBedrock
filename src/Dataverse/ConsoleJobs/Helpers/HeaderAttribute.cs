namespace ConsoleJobs.Helpers;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class HeaderAttribute : Attribute
{
    public string Name { get; internal set; }

    public HeaderAttribute(string name)
    {
        Name = name;
    }
}
