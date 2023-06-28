using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedUsingsModel
{
  private readonly IReadOnlyList<string> myUsings;

  
  public GeneratedUsingsModel(IReadOnlyList<string> usings)
  {
    myUsings = usings;
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    foreach (var usingToAdd in myUsings)
    {
      sb.Append("using ").Append(usingToAdd).AppendSemicolon().AppendNewLine();
    }
  }
}