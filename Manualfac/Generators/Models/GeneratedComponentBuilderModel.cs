using System.Collections.Immutable;
using System.Text;

namespace Manualfac.Generators.Models;

internal class GeneratedComponentBuilderModel
{
  private readonly string myComponentShortTypeName;
  private readonly GeneratedUsingsModel myDependenciesUsingsModel;
  private readonly IReadOnlyList<string> myDependenciesAccessors;
  private readonly GeneratedNamespaceModel myGeneratedNamespaceModel;
  

  public GeneratedComponentBuilderModel(ComponentInfo component)
  {
    myDependenciesAccessors = component.Dependencies.Select(dep => $"{dep.CreateContainerName()}.Resolve()").ToList();
    myComponentShortTypeName = component.TypeShortName;
    myDependenciesUsingsModel = component.ToDependenciesUsingsModel();
    
    var generatedClassModel = new GeneratedClassModel(
      component.CreateContainerName(), 
      ImmutableArray<GeneratedFieldModel>.Empty,
      new[] { new GeneratedMethodModel("Resolve", component.TypeShortName, GenerateFactoryMethod, isStatic: true) });
    
    myGeneratedNamespaceModel = new GeneratedNamespaceModel(component.Namespace, generatedClassModel.GenerateInto);
  }

  
  public void GenerateInto(StringBuilder sb, int indent)
  {
    myDependenciesUsingsModel.GenerateInto(sb, indent);
    sb.AppendNewLine();

    myGeneratedNamespaceModel.GenerateInto(sb, indent);
  }
  
  private unsafe void GenerateFactoryMethod(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent).Append("return").AppendSpace().Append("new").AppendSpace()
      .Append(myComponentShortTypeName);

    using (StringBuilderCookies.DefaultBraces(sb, &indent, appendEndIndent: true))
    {
      foreach (var dependenciesAccessor in myDependenciesAccessors)
      {
        sb.AppendIndent(indent).Append(dependenciesAccessor).AppendComma().AppendNewLine();
      }

      if (myDependenciesAccessors.Count > 0)
      {
        //remove last command and new line
        sb.Remove(sb.Length - 2, 2);
      }
    }

    sb.AppendSemicolon();
  }
}