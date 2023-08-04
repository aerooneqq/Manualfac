using System.Text;
using Manualfac.Util;

namespace Manualfac.Models;

internal class GeneratedUsingsModel(IReadOnlyList<string> usings) : IGeneratedModel
{
  public void GenerateInto(StringBuilder sb, int indent)
  {
    foreach (var usingToAdd in usings)
    {
      sb.Append("using ").Append(usingToAdd).AppendSemicolon().AppendNewLine();
    }
  }
}