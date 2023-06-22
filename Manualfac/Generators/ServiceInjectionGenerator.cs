using System.Text;
using Microsoft.CodeAnalysis;

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
      var assembly = componentInfo.ComponentSymbol.ContainingAssembly;
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
      if (dependency.Namespace is { })
      {
        sb.Append("using ").Append(dependency.Namespace).AppendSemicolon().AppendNewLine(); 
      }
    }

    return sb;
  }
  
  private StringBuilder WriteDependenciesFields(ComponentInfo componentInfo, StringBuilder sb)
  {
    foreach (var dependency in componentInfo.GetOrCreateOrderedListOfDependencies())
    {
      sb = sb.Append("private readonly ").Append(dependency.ShortName).AppendSpace();
      sb = WriteFieldName(dependency, sb);
      
      sb = sb.AppendSemicolon().AppendNewLine();
    }

    return sb;
  }

  private static StringBuilder WriteFieldName(ComponentInfo component, StringBuilder sb)
  {
    return sb.Append("my").Append(component.ShortName);
  }

  private static StringBuilder WriteConstructor(ComponentInfo componentInfo, StringBuilder sb)
  {
    sb.Append("public ").Append(componentInfo.ShortName).AppendOpenBracket();
    
    var index = 0;
    foreach (var dependency in componentInfo.GetOrCreateOrderedListOfDependencies())
    {
      sb.Append(dependency.ShortName).AppendSpace().Append(GetComponentParamName(index++)).AppendComma()
        .AppendSpace();
    }
    
    if (index > 0)
    {
      //remove last space and comma
      sb.Remove(sb.Length - 2, 2);
    }
    
    sb.AppendClosedBracket().AppendNewLine().AppendOpenCurlyBracket().AppendNewLine();

    index = 0;
    foreach (var dependency in componentInfo.GetOrCreateOrderedListOfDependencies())
    {
      WriteFieldName(dependency, sb);
      sb.AppendSpace().AppendEq().Append(GetComponentParamName(index++)).AppendSemicolon().AppendNewLine();
    }

    sb.AppendClosedCurlyBracket();

    return sb;

    static string GetComponentParamName(int index) => $"c{index}";
  }
}