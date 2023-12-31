using System.Text;
using Manualfac.Components;
using Manualfac.Models.TopLevel;
using Manualfac.Util;

namespace Manualfac.Models;

internal class GeneratedComponentObjectCreationModel : IGeneratedModel
{
  private const string CreatedVarName = "created";

  private readonly IReadOnlyList<string> myDependenciesAccessors;
  private readonly string myComponentFullTypeName;


  public GeneratedComponentObjectCreationModel(
    IComponent component, Func<IComponent, IComponent> componentAdjustFunc)
  {
    myComponentFullTypeName = component.FullName;
    myDependenciesAccessors = component.Dependencies.AllDependenciesSet
      .Select(dep => DependencyAccessorUtil.GenerateDependencyAccessor(dep, componentAdjustFunc))
      .ToList();
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append($"var {CreatedVarName} =").AppendSpace().Append("new").AppendSpace()
      .Append(myComponentFullTypeName);

    using (var bracesCookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true))
    {
      foreach (var dependenciesAccessor in myDependenciesAccessors)
      {
        sb.AppendIndent(bracesCookie.Indent).Append(dependenciesAccessor).AppendComma().AppendNewLine();
      }

      if (myDependenciesAccessors.Count > 0)
      {
        //remove last command and new line
        sb.Remove(sb.Length - 2, 2);
      }
    }

    sb.AppendSemicolon().AppendNewLine().AppendIndent(indent).Append("return ")
      .Append(CreatedVarName).AppendSemicolon();
  }
}