using System.Text;
using Manualfac.Components;
using Manualfac.Util;

namespace Manualfac.Models;

internal class GeneratedOverridenComponentObjectCreationModel(IComponent overrideComponent) : IGeneratedModel
{
  private readonly string myContainerResolveExpression = overrideComponent.CreateContainerResolveExpression();


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append("return ").Append(myContainerResolveExpression).AppendSemicolon();
  }
}