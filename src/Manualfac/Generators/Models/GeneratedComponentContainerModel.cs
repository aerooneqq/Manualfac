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
  private readonly GeneratedUsingsModel myDefaultUsingsModel;


  public GeneratedComponentContainerModel(IComponentInfo component)
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
    myDefaultUsingsModel = new GeneratedUsingsModel(new[] { "System.Threading" });
  }

  
  public void GenerateInto(StringBuilder sb, int indent)
  {
    myDependenciesUsingsModel.GenerateInto(sb, indent);
    myDefaultUsingsModel.GenerateInto(sb, indent);
    
    sb.AppendNewLine();

    myGeneratedNamespaceModel.GenerateInto(sb, indent);
  }
  
  private void GenerateFactoryMethod(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent)
      .Append($"if (Volatile.Read(ref {InstanceFieldName}) is {{ }} {Existing1}) return {Existing1};")
      .AppendNewLine();

    using (var lockCookie = StringBuilderCookies.Lock(sb, SyncFieldName, indent))
    {
      sb.AppendIndent(lockCookie.Indent)
        .Append($"if (Volatile.Read(ref {InstanceFieldName}) is {{ }} {Existing2}) return {Existing2};")
        .AppendNewLine();
      
      sb.AppendIndent(lockCookie.Indent).Append($"var {CreatedVarName} =").AppendSpace().Append("new").AppendSpace()
        .Append(myComponentShortTypeName);

      using (var bracesCookie = StringBuilderCookies.DefaultBraces(sb, lockCookie.Indent, appendEndIndent: true))
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

      sb.AppendSemicolon().AppendNewLine();
      sb.AppendIndent(lockCookie.Indent).Append($"Volatile.Write(ref {InstanceFieldName}, {CreatedVarName});").AppendNewLine();
      sb.AppendIndent(lockCookie.Indent).Append($"return {CreatedVarName};");
    }
  }
}