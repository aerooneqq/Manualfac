using System.Text;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedParameterModel : IGeneratedModel
{
  private readonly string myParameterTypeName;
  private readonly string myParameterName;


  public GeneratedParameterModel(string parameterTypeName, string parameterName)
  {
    myParameterTypeName = parameterTypeName;
    myParameterName = parameterName;
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(myParameterTypeName).AppendSpace().Append(myParameterName);
  }
}