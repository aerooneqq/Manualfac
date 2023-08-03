using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedOverridenComponentObjectCreationModel(IComponent overrideComponent) : IGeneratedModel
{
  private readonly string myContainerResolveExpression = overrideComponent.CreateContainerResolveExpression();


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append("return ").Append(myContainerResolveExpression).AppendSemicolon();
  }
}