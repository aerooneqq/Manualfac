using System.Collections.Immutable;
using System.Text;

namespace Manualfac.Generators.Models;

internal class GeneratedComponentContainerModel
{
  private const string ResolveMethodName = "Resolve";
  private const string InstanceFieldName = "ourInstance";
  private const string SyncFieldName = "ourSync";
  private const string CreatedVarName = "created";
  private const string Existing1 = "existing1";
  private const string Existing2 = "exiting2";
  
  private readonly string myComponentShortTypeName;
  private readonly GeneratedUsingsModel myDependenciesUsingsModel;
  private readonly IReadOnlyList<string> myDependenciesAccessors;
  private readonly GeneratedNamespaceModel myGeneratedNamespaceModel;
  

  public GeneratedComponentContainerModel(ComponentInfo component)
  {
    myComponentShortTypeName = component.TypeShortName;
    myDependenciesUsingsModel = component.ToDependenciesUsingsModel();
    myDependenciesAccessors = component.Dependencies
      .Select(dep => $"{dep.CreateContainerName()}.{ResolveMethodName}()")
      .ToList();
    
    var generatedClassModel = new GeneratedClassModel(
      component.CreateContainerName(),
      ImmutableArray<GeneratedConstructorModel>.Empty,
      new[]
      {
        new GeneratedFieldModel(component.TypeShortName, InstanceFieldName, AccessModifier.Private, false, true),
        new GeneratedFieldModel("object", SyncFieldName, AccessModifier.Private, false, true, "new object()")
      },
      new[]
      {
        new GeneratedMethodModel(ResolveMethodName, component.TypeShortName, GenerateFactoryMethod, isStatic: true)
      });
    
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
    sb.AppendIndent(indent)
      .Append($"if (Volatile.Read(ref {InstanceFieldName}) is {{ }} {Existing1}) return {Existing1};")
      .AppendNewLine();

    using (StringBuilderCookies.Lock(sb, SyncFieldName, &indent))
    {
      sb.AppendIndent(indent)
        .Append($"if (Volatile.Read(ref {InstanceFieldName}) is {{ }} {Existing2}) return {Existing2};")
        .AppendNewLine();
      
      sb.AppendIndent(indent).Append($"var {CreatedVarName} =").AppendSpace().Append("new").AppendSpace()
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
      sb.AppendIndent(indent).Append($"Volatile.Write(ref {InstanceFieldName}, {CreatedVarName});").AppendNewLine();
      sb.AppendIndent(indent).Append($"return {CreatedVarName};");
    }
  }
}