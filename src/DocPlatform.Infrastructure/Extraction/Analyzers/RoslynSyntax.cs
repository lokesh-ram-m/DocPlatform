using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPlatform.Infrastructure.Extraction.Analyzers;

// Shared syntax helpers used by the analyzers.
internal static class RoslynSyntax
{
    public static bool HasAttribute(SyntaxList<AttributeListSyntax> lists, string name) =>
        lists.SelectMany(l => l.Attributes).Any(a => LastSegment(a.Name.ToString()) == name);

    public static string? FirstStringArg(SyntaxList<AttributeListSyntax> lists, string attrName)
    {
        AttributeSyntax? attr = lists.SelectMany(l => l.Attributes)
            .FirstOrDefault(a => LastSegment(a.Name.ToString()) == attrName);
        if (attr?.ArgumentList?.Arguments.FirstOrDefault()?.Expression is LiteralExpressionSyntax lit
            && lit.Token.Value is string s)
            return s;
        return null;
    }

    // Returns the HTTP verb and optional route template from an [Http*("template")] attribute.
    public static (string? Verb, string? Template) HttpVerbAndTemplate(SyntaxList<AttributeListSyntax> lists)
    {
        foreach (AttributeSyntax a in lists.SelectMany(l => l.Attributes))
        {
            string n = LastSegment(a.Name.ToString());
            if (n.StartsWith("Http") && n.Length > 4)
            {
                string? template = null;
                if (a.ArgumentList?.Arguments.FirstOrDefault()?.Expression is LiteralExpressionSyntax lit
                    && lit.Token.Value is string s)
                    template = s;
                return (n[4..].ToUpper(), template);
            }
        }
        return (null, null);
    }

    public static string? FirstStringLiteral(ArgumentListSyntax? args)
    {
        foreach (ArgumentSyntax a in args?.Arguments ?? default)
            if (a.Expression is LiteralExpressionSyntax lit && lit.Token.Value is string s)
                return s;
        return null;
    }

    // "IdentityDbContext<AppUser, Role, string>" -> "IdentityDbContext";  "A.B.DbContext" -> "DbContext"
    public static string NormalizeBase(string typeName)
    {
        int lt = typeName.IndexOf('<');
        if (lt >= 0) typeName = typeName[..lt];
        return LastSegment(typeName);
    }

    public static string LastSegment(string typeName) => typeName.Split('.').Last().Trim();

    public static List<string> BaseTypeNames(BaseListSyntax? baseList) =>
        baseList?.Types.Select(x => NormalizeBase(x.Type.ToString())).ToList() ?? new();
}
