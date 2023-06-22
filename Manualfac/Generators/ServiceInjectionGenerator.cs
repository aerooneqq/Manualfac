using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Manualfac.Generators;

[Generator]
public class ServiceInjectionGenerator : ISourceGenerator
{
  public void Initialize(GeneratorInitializationContext context)
  {
  }

  public void Execute(GeneratorExecutionContext context)
  {
    var storage = new ComponentInfoStorage();
    storage.FillComponents(context);
    GenerateDependenciesPart(storage.Components, context);
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
    var sb = WriteUsings(componentInfo, new StringBuilder()).AppendNewLine();
    
    if (componentInfo.Namespace is { })
    {
      sb.Append("namespace").AppendSpace().Append(componentInfo.Namespace).AppendSpace()
        .AppendNewLine().AppendOpenCurlyBracket().AppendNewLine();
    }
    
    sb.Append("public partial class ").Append(componentInfo.ShortName)
      .AppendNewLine().AppendOpenCurlyBracket().AppendNewLine();

    WriteDependenciesFields(componentInfo, sb);
    sb.AppendNewLine();
    WriteConstructor(componentInfo, sb);
    
    sb.AppendNewLine().AppendClosedCurlyBracket();

    if (componentInfo.Namespace is { })
    {
      sb.AppendNewLine().AppendClosedCurlyBracket(); 
    }
    
    context.AddSource($"{componentInfo.ShortName}.g", sb.ToString());
  }

  private StringBuilder WriteUsings(ComponentInfo componentInfo, StringBuilder sb)
  {
    foreach (var dependency in componentInfo.Dependencies)
    {
      sb.Append("using ").Append(dependency.ContainingNamespace.Name).AppendSemicolon()
        .AppendNewLine();
    }

    return sb;
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