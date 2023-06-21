using System.Text;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators;

internal class ComponentInfo
{
  public INamedTypeSymbol Component { get; }
  public HashSet<INamedTypeSymbol> Dependencies { get; }

  
  public ComponentInfo(INamedTypeSymbol component, HashSet<INamedTypeSymbol> dependencies)
  {
    Component = component;
    Dependencies = dependencies;
  }
}

[Generator]
public class ServiceInjectionGenerator : ISourceGenerator
{
  private readonly HashSet<INamedTypeSymbol> myComponents;
  private readonly HashSet<INamedTypeSymbol> myNotComponents;

  
  public ServiceInjectionGenerator()
  {
    myComponents = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    myNotComponents = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
  }
  
  
  public void Initialize(GeneratorInitializationContext context)
  {
    myComponents.Clear();
    myNotComponents.Clear();
  }

  public void Execute(GeneratorExecutionContext context)
  {
    var components = GetComponents(context);
    
    
  }

  private IReadOnlyList<ComponentInfo> GetComponents(in GeneratorExecutionContext context)
  {
    var modulesQueue = new Queue<IModuleSymbol>();
    var visited = new HashSet<IModuleSymbol>(SymbolEqualityComparer.Default);
    var components = new List<ComponentInfo>();
    
    modulesQueue.Enqueue(context.Compilation.SourceModule);

    while (modulesQueue.Count != 0)
    {
      var module = modulesQueue.Dequeue();
      visited.Add(module);
      
      components.AddRange(GetComponentTypesFrom(module).Select(ToComponentInfo));

      foreach (var refAsm in module.ReferencedAssemblySymbols)
      {
        foreach (var refAsmModule in refAsm.Modules)
        {
          if (!visited.Contains(refAsmModule))
          {
            modulesQueue.Enqueue(refAsmModule);
          }
        }
      }
    }

    return components;
  }

  private ComponentInfo ToComponentInfo(INamedTypeSymbol symbol)
  {
    if (symbol.Constructors.Length != 1)
    {
      throw new ComponentShouldSpecifyOnlyOneConstructorException(symbol);
    }

    var dependencies = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    var constructor = symbol.Constructors[0];
    foreach (var parameter in constructor.Parameters)
    {
      if (parameter.Type is not INamedTypeSymbol parameterType)
      {
        throw new ComponentParameterIsNotNamedTypeSymbolException(parameter);
      }

      if (CheckIfManualfacComponent(parameterType))
      {
        throw new ParameterIsNotManualfacComponentException(parameter);
      }

      dependencies.Add(parameterType);
    }

    return new ComponentInfo(symbol, dependencies);
  }

  private bool CheckIfManualfacComponent(INamedTypeSymbol symbol)
  {
    if (myNotComponents.Contains(symbol)) return false;
    if (myComponents.Contains(symbol)) return true;

    if (symbol.GetAttributes().Any(IsManualfacAttribute))
    {
      myComponents.Add(symbol);
      return true;
    }

    myNotComponents.Add(symbol);
    return false;
  }

  private IEnumerable<INamedTypeSymbol> GetComponentTypesFrom(IModuleSymbol module)
  {
    return GetAllNamespacesFrom(module.GlobalNamespace)
      .SelectMany(ns => ns.GetTypeMembers())
      .Where(CheckIfManualfacComponent);
  }

  private static bool IsManualfacAttribute(AttributeData attribute)
  {
    var attributeClass = attribute.AttributeClass;
    while (attributeClass is { })
    {
      if (attributeClass.Name == "ManualfacAttribute") return true;
      attributeClass = attributeClass.BaseType;
    }

    return false;
  }

  private static IEnumerable<INamespaceSymbol> GetAllNamespacesFrom(INamespaceSymbol namespaceSymbol)
  {
    yield return namespaceSymbol;
    foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
    {
      foreach (var nextNamespace in GetAllNamespacesFrom(childNamespace))
      {
        yield return nextNamespace;
      }
    }
  }
}

public static class StringBuilderExtensions
{
  public static StringBuilder AppendNewLine(this StringBuilder sb) => sb.Append("\n");
}

public class ManualfacGeneratorException : Exception
{
  
}

public class ComponentShouldSpecifyOnlyOneConstructorException : ManualfacGeneratorException
{
  public override string Message { get; }

  
  public ComponentShouldSpecifyOnlyOneConstructorException(INamedTypeSymbol namedTypeSymbol)
  {
    Message = $"Type {namedTypeSymbol.Name} declared {namedTypeSymbol.Constructors.Length} constructors, when only one is expected";
  }
}

public class ComponentParameterIsNotNamedTypeSymbolException : ManualfacGeneratorException
{
  public override string Message { get; }

  
  public ComponentParameterIsNotNamedTypeSymbolException(IParameterSymbol symbol)
  {
    Message = $"Parameter {symbol.Name} in {symbol.ContainingType.Name} was not of type {nameof(INamedTypeSymbol)}";
  }
}

public class ParameterIsNotManualfacComponentException : ManualfacGeneratorException
{
  public override string Message { get; }
  
  
  public ParameterIsNotManualfacComponentException(IParameterSymbol parameterSymbol)
  {
    Message = $"Parameter {parameterSymbol.Name} in {parameterSymbol.ContainingType.Name} is not a Manualfac component";
  }
}