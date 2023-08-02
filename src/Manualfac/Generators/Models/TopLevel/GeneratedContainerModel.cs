using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Components.Dependencies;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models.TopLevel;

internal partial class GeneratedContainerModel : IGeneratedModel
{
  private readonly GeneratedUsingsModel myDependenciesUsingsModel;
  private readonly GeneratedNamespaceModel myGeneratedNamespaceModel;
  private readonly GeneratedUsingsModel myDefaultUsingsModel;


  public GeneratedContainerModel(IComponent component)
  {
    myDependenciesUsingsModel = component.ToDependenciesUsingsModel();

    var generatedClassModel = CreateClassModel(component);

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