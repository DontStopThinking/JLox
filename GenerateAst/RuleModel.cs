namespace GenerateAst;

public class RuleModel
{
    public string ClassName { get; set; } = string.Empty;

    public List<(string memberType, string memberName)> MemberTypeToNames { get; set; } = new();

    public string ConstructorArguments =>
        string.Join(", ", MemberTypeToNames.Select(m => $"{m.memberType} {m.memberName}"));

    public List<string> MemberNames => MemberTypeToNames
        .Select(m => m.memberName)
        .ToList();
}
