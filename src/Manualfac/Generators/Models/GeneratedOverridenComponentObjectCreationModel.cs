using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedOverridenComponentObjectCreationModel : IGeneratedModel
{
  private readonly string myContainerResolveExpression;

  public GeneratedOverridenComponentObjectCreationModel(IConcreteComponent overrideComponent)
  {
    myContainerResolveExpression = overrideComponent.CreateContainerResolveExpression();
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append("return ").Append(myContainerResolveExpression).AppendSemicolon();
  }
}