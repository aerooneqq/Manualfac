using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedNamespaceModel(string? namespaceName, Action<StringBuilder, int> namespaceBodyGenerator)
  : IGeneratedModel
{
  public void GenerateInto(StringBuilder sb, int indent)
  {
    OpenCloseStringBuilderOperation? namespaceCookie = null;

    try
    {
      if (namespaceName is { } && !string.IsNullOrWhiteSpace(namespaceName))
      {
        sb.Append("namespace").AppendSpace().Append(namespaceName).AppendSpace().AppendNewLine();
        namespaceCookie = StringBuilderCookies.CurlyBraces(sb, indent);
      }

      namespaceBodyGenerator(sb, namespaceCookie?.Indent ?? indent);
    }
    finally
    {
      namespaceCookie?.Dispose();
    }
  }
}