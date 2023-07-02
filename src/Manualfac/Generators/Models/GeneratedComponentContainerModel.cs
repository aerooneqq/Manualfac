using System.Collections.Immutable;
using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Components.Dependencies;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models;

internal class GeneratedComponentContainerModel : IGeneratedModel
{
  private const string DefaultInitializeMethodName = "DefaultInitialize";
  
  private const string InstanceFieldName = "ourInstance";
  private const string SyncFieldName = "ourSync";
  private const string InitializationFuncFieldName = "ourF";
  private const string InitializeFuncParamName = "f";

  private const string CreatedVarName = "created";
  private const string Existing1 = "existing1";
  private const string Existing2 = "exiting2";

  private const string Void = "void";
  
  private readonly GeneratedUsingsModel myDependenciesUsingsModel;
  private readonly GeneratedNamespaceModel myGeneratedNamespaceModel;
  private readonly GeneratedUsingsModel myDefaultUsingsModel;
  private readonly GeneratedComponentObjectCreationModel myComponentCreationModel;


  public GeneratedComponentContainerModel(IComponent component)
  {
    myDependenciesUsingsModel = component.ToDependenciesUsingsModel();
    myComponentCreationModel = new GeneratedComponentObjectCreationModel(component, static c => c);

    var generatedClassModel = new GeneratedClassModel(
      component.CreateContainerName(),
      ImmutableArray<GeneratedConstructorModel>.Empty,
      new[]
      {
        new GeneratedFieldModel(component.FullName, InstanceFieldName, AccessModifier.Private, false, true),
        new GeneratedFieldModel("object", SyncFieldName, AccessModifier.Private, false, true, "new object()"),
        new GeneratedFieldModel($"Func<{component.FullName}>", InitializationFuncFieldName, AccessModifier.Private, 
          false, true, defaultValue: DefaultInitializeMethodName)
      },
      new[]
      {
        new GeneratedMethodModel(
          Constants.ResolveMethod, component.FullName, GenerateFactoryMethod, 
          ImmutableList<GeneratedParameterModel>.Empty, isStatic: true),
        
        new GeneratedMethodModel(Constants.InitializeMethod, Void, GenerateInitializeMethod, new []
        {
          new GeneratedParameterModel($"Func<{component.FullName}>", InitializeFuncParamName)
        }, isStatic: true),
        
        new GeneratedMethodModel(
          DefaultInitializeMethodName, component.FullName, GenerateDefaultInitializeMethod, 
          ImmutableList<GeneratedParameterModel>.Empty, AccessModifier.Private, isStatic: true)
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

    using var lockCookie = StringBuilderCookies.Lock(sb, SyncFieldName, indent);
    
    sb.AppendIndent(lockCookie.Indent)
      .Append($"if (Volatile.Read(ref {InstanceFieldName}) is {{ }} {Existing2}) return {Existing2};")
      .AppendNewLine();

    sb.AppendIndent(lockCookie.Indent).Append($"var {CreatedVarName} = ").Append(InitializationFuncFieldName)
      .Append("()").AppendSemicolon().AppendNewLine();
    
    sb.AppendIndent(lockCookie.Indent).Append($"Volatile.Write(ref {InstanceFieldName}, {CreatedVarName});").AppendNewLine();
    sb.AppendIndent(lockCookie.Indent).Append($"return {CreatedVarName};");
  }

  private void GenerateInitializeMethod(StringBuilder sb, int indent)
  {
    using var lockCookie = StringBuilderCookies.Lock(sb, SyncFieldName, indent);
    sb.AppendIndent(lockCookie.Indent).Append($"{InitializationFuncFieldName}").AppendEq()
      .Append(InitializeFuncParamName).AppendSemicolon();
  }

  private void GenerateDefaultInitializeMethod(StringBuilder sb, int indent) => 
    myComponentCreationModel.GenerateInto(sb, indent);
}

internal static class DependencyAccessorUtil
{
  public static string GenerateDependencyAccessor(
    IComponentDependency dependency, Func<IComponent, IComponent> adjustComponentFunc)
  {
    switch (dependency)
    {
      case ConcreteComponentDependency or NonCollectionInterfaceDependency:
        var dep = adjustComponentFunc(dependency.ResolveUnderlyingConcreteComponents().First());
        return $"{dep.CreateContainerResolveExpression()}";
      case CollectionDependency collectionDependency:
        var impls = collectionDependency.ResolveUnderlyingConcreteComponents().Select(adjustComponentFunc);
        var name = collectionDependency.CollectionItemInterface.GetFullName();
        return $"new {name}[] {{{string.Join(",", impls.Select(impl => impl.CreateContainerResolveExpression()))}}}";
      default:
        throw new ArgumentOutOfRangeException(dependency.GetType().Name);
    }
  }
}