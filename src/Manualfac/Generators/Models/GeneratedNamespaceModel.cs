using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedNamespaceModel : IGeneratedModel
{
  private readonly string? myNamespaceName;
  private readonly Action<StringBuilder, int> myNamespaceBodyGenerator;

  
  public GeneratedNamespaceModel(string? namespaceName, Action<StringBuilder, int> namespaceBodyGenerator)
  {
    myNamespaceName = namespaceName;
    myNamespaceBodyGenerator = namespaceBodyGenerator;
  }

  
  public void GenerateInto(StringBuilder sb, int indent)
  {
    OpenCloseStringBuilderOperation? namespaceCookie = null;

    try
    {
      if (myNamespaceName is { } && !string.IsNullOrWhiteSpace(myNamespaceName))
      {
        sb.Append("namespace").AppendSpace().Append(myNamespaceName).AppendSpace().AppendNewLine();
        namespaceCookie = StringBuilderCookies.CurlyBraces(sb, indent);
      }

      myNamespaceBodyGenerator(sb, namespaceCookie?.Indent ?? indent);
    }
    finally
    {
      namespaceCookie?.Dispose();
    }
  }
}