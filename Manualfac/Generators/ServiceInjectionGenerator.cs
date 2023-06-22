using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Manualfac.Generators;

internal class ComponentInfo
{
  private IReadOnlyList<INamedTypeSymbol>? myOrderedDependencies;

  public INamedTypeSymbol Component { get; }
  public HashSet<INamedTypeSymbol> Dependencies { get; }


  public string ShortName => Component.Name;
  
  // ReSharper disable once ReturnTypeCanBeNotNullable
  public string? Namespace => Component.ContainingNamespace.Name;

  
  public ComponentInfo(INamedTypeSymbol component, HashSet<INamedTypeSymbol> dependencies)
  {
    Component = component;
    Dependencies = dependencies;
  }


  public IReadOnlyList<INamedTypeSymbol> GetOrCreateOrderedListOfDependencies()
  {
    if (myOrderedDependencies is null)
    {
      myOrderedDependencies = Dependencies.OrderBy(dep => dep.Name).ToList();
    }

    return myOrderedDependencies;
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
    GenerateDependenciesPart(components, context);
  }

  private void GenerateDependenciesPart(IReadOnlyList<ComponentInfo> components, GeneratorExecutionContext context)
  {
    foreach (var componentInfo in components)
    {
      var assembly = componentInfo.Component.ContainingAssembly;
      if (SymbolEqualityComparer.Default.Equals(assembly, context.Compilation.Assembly))
      {
        GenerateDependenciesPart(componentInfo, context);
      }
    }
  }

  private void GenerateDependenciesPart(ComponentInfo componentInfo, GeneratorExecutionContext context)
  {
    var sb = new StringBuilder();

    if (componentInfo.Namespace is { })
    {
      sb.Append("namespace").AppendSpace().Append(componentInfo.Namespace).AppendSpace()
        .AppendNewLine().AppendOpenCurlyBracket();
    }
    
    sb.Append("public partial class ").Append(componentInfo.ShortName)
      .AppendNewLine().AppendOpenCurlyBracket().AppendNewLine();

    sb = WriteDependenciesFields(componentInfo, sb);
    sb.AppendNewLine();
    sb = WriteConstructor(componentInfo, sb);
    
    sb.AppendNewLine().AppendClosedCurlyBracket();

    if (componentInfo.Namespace is { })
    {
      sb.AppendNewLine().AppendClosedCurlyBracket(); 
    }
    
    context.AddSource($"{componentInfo.ShortName}.g", sb.ToString());
  }

  private StringBuilder WriteDependenciesFields(ComponentInfo componentInfo, StringBuilder sb)
  {
    foreach (var dependency in componentInfo.GetOrCreateOrderedListOfDependencies())
    {
      sb = sb.Append("private readonly ").Append(dependency.Name).AppendSpace();
      sb = WriteFieldName(dependency, sb);
      
      sb = sb.AppendSemicolon().AppendNewLine();
    }

    return sb;
  }

  private static StringBuilder WriteFieldName(INamedTypeSymbol component, StringBuilder sb)
  {
    return sb.Append("my").Append(component.Name);
  }

  private static StringBuilder WriteConstructor(ComponentInfo componentInfo, StringBuilder sb)
  {
    sb = sb.Append("public ").Append(componentInfo.ShortName).AppendOpenBracket();
    
    var index = 0;
    foreach (var dependency in componentInfo.GetOrCreateOrderedListOfDependencies())
    {
      sb = sb.Append(dependency.Name).AppendSpace().Append(GetComponentParamName(index++)).AppendComma()
        .AppendSpace();
    }
    
    if (index > 0)
    {
      //remove last space and comma
      sb = sb.Remove(sb.Length - 2, 2);
    }
    
    sb.AppendClosedBracket().AppendNewLine().AppendOpenCurlyBracket().AppendNewLine();

    index = 0;
    foreach (var dependency in componentInfo.GetOrCreateOrderedListOfDependencies())
    {
      sb = WriteFieldName(dependency, sb);
      sb.AppendSpace().AppendEq().Append(GetComponentParamName(index++)).AppendSemicolon().AppendNewLine();
    }

    sb.AppendClosedCurlyBracket();

    return sb;

    static string GetComponentParamName(int index) => $"c{index}";
  }

  private IReadOnlyList<ComponentInfo> GetComponents(GeneratorExecutionContext context)
  {
    var modulesQueue = new Queue<IModuleSymbol>();
    var visited = new HashSet<IModuleSymbol>(SymbolEqualityComparer.Default);
    var components = new List<ComponentInfo>();
    
    modulesQueue.Enqueue(context.Compilation.SourceModule);

    while (modulesQueue.Count != 0)
    {
      var module = modulesQueue.Dequeue();
      visited.Add(module);
      
      components.AddRange(GetComponentTypesFrom(module).Select(type => ToComponentInfo(type, context)));

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

  private static ComponentInfo ToComponentInfo(INamedTypeSymbol symbol, GeneratorExecutionContext context)
  {
    var dependencies = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    var compilation = context.Compilation;

    var dependenciesSymbols = symbol.GetAttributes()
      .Where(attr => attr.AttributeClass?.Name == "DependsOnAttribute")
      .Select(attr => attr.ApplicationSyntaxReference?.GetSyntax())
      .OfType<AttributeSyntax>()
      .Select(attributeSyntax => attributeSyntax.DescendantNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault())
      .Where(typeArgs => typeArgs is { })
      .SelectMany(typeArgsSyntax => typeArgsSyntax!.Arguments)
      .Select(genericArg => compilation.GetSemanticModel(genericArg.SyntaxTree).GetTypeInfo(genericArg).Type)
      .OfType<INamedTypeSymbol>();

    foreach (var typeSymbol in dependenciesSymbols)
    {
      dependencies.Add(typeSymbol);
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
  public static StringBuilder AppendNewLine(this StringBuilder sb) => sb.Append('\n');
  public static StringBuilder AppendSemicolon(this StringBuilder sb) => sb.Append(';');
  public static StringBuilder AppendSpace(this StringBuilder sb) => sb.Append(' ');
  public static StringBuilder AppendComma(this StringBuilder sb) => sb.Append(',');
  public static StringBuilder AppendOpenBracket(this StringBuilder sb) => sb.Append('(');
  public static StringBuilder AppendClosedBracket(this StringBuilder sb) => sb.Append(')');
  public static StringBuilder AppendOpenCurlyBracket(this StringBuilder sb) => sb.Append('{');
  public static StringBuilder AppendClosedCurlyBracket(this StringBuilder sb) => sb.Append('}');
  public static StringBuilder AppendEq(this StringBuilder sb) => sb.Append('=');

  public static StringBuilder AppendTab(this StringBuilder sb) => sb.AppendSpace().AppendSpace();
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