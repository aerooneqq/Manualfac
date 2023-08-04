using System.Text;
using Manualfac.Util;

namespace Manualfac.Models;

internal class GeneratedParameterModel(string parameterTypeName, string parameterName) : IGeneratedModel
{
  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append(parameterTypeName).AppendSpace().Append(parameterName);
  }
}