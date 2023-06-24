using System.Collections.Immutable;
using System.Text;

namespace Manualfac.Generators.Models;

internal class GeneratedComponentContainerModel
{
  private readonly string myComponentShortTypeName;
  private readonly GeneratedUsingsModel myDependenciesUsingsModel;
  private readonly IReadOnlyList<string> myDependenciesAccessors;
  private readonly GeneratedNamespaceModel myGeneratedNamespaceModel;
  

  public GeneratedComponentContainerModel(ComponentInfo component)
  {
    myDependenciesAccessors = component.Dependencies.Select(dep => $"{dep.CreateContainerName()}.Resolve()").ToList();
    myComponentShortTypeName = component.TypeShortName;
    myDependenciesUsingsModel = component.ToDependenciesUsingsModel();

    var generatedClassModel = new GeneratedClassModel(
      component.CreateContainerName(),
      ImmutableArray<GeneratedConstructorModel>.Empty,
      new[]
      {
        new GeneratedFieldModel(component.TypeShortName, "ourInstance", AccessModifier.Private, false, true),
        new GeneratedFieldModel("object", "ourSync", AccessModifier.Private, false, true, "new object()")
      },
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
    sb.AppendIndent(indent).Append("if (Volatile.Read(ref ourInstance) is { } existing1) return existing1;")
      .AppendNewLine();

    using (StringBuilderCookies.Lock(sb, "ourSync", &indent))
    {
      sb.AppendIndent(indent).Append("if (Volatile.Read(ref ourInstance) is { } existing2) return existing2;")
        .AppendNewLine();
      
      sb.AppendIndent(indent).Append("var created =").AppendSpace().Append("new").AppendSpace()
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

      sb.AppendSemicolon().AppendNewLine();
      sb.AppendIndent(indent).Append("Volatile.Write(ref ourInstance, created);").AppendNewLine();
      sb.AppendIndent(indent).Append("return created;");
    }
  }
}