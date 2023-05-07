using GenerateAst;
using GenerateAst.Templates;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: GenerateAst <output directory>");
    Environment.Exit(64);
}

string outputDir = args[0];

// Each string is the name of the class to generate followed by ':' and the list of fields, separated by commas.
// Each field has a type and a name.
List<string> rules = new()
{
    "Binary     : Expr left, Token op, Expr right",
    "Grouping   : Expr expression",
    "Literal    : object value",
    "Unary      : Token op, Expr right"
};

List<RuleModel> ruleModels = GenerateRuleModels(rules);

string outputFilePath = Path.Combine(outputDir, "Expr.generated.cs");

Console.WriteLine($"---------- Generating file ----------");
try
{
    Console.WriteLine(outputFilePath);
    ExprTemplate exprTemplate = new() { ContainerClassName = "Expr", RuleModels = ruleModels };
    string outputContent = exprTemplate.TransformText();

    File.WriteAllText(outputFilePath, outputContent);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"---------- Error generating file ----------");
    Console.Error.WriteLine(ex.Message);
}
finally
{
    Console.WriteLine("---------- Generating finished ----------");
}

static List<RuleModel> GenerateRuleModels(IReadOnlyCollection<string> rules)
{
    List<RuleModel> result = new(rules.Count);

    foreach (string rule in rules)
    {
        RuleModel ruleModel = new();

        string className = rule.Split(':')[0].Trim();
        ruleModel.ClassName = className;

        string allMembers = rule.Split(':')[1].Trim();
        string[] members = allMembers.Split(", ");
        foreach (string member in members)
        {
            string memberType = member.Split(' ')[0];
            string memberName = member.Split(' ')[1];
            ruleModel.MemberTypeToNames.Add((memberType, memberName));
        }

        result.Add(ruleModel);
    }

    return result;
}
