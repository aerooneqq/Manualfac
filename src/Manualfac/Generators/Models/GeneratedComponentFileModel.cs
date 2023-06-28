using System.Text;
using Manualfac.Generators.Components;

namespace Manualfac.Generators.Models;

internal class GeneratedComponentFileModel
{
  private readonly GeneratedUsingsModel myGeneratedUsingsModel;
  private readonly GeneratedNamespaceModel myGeneratedNamespaceModel;


  public GeneratedComponentFileModel(IConcreteComponent concreteComponent)
  {
    var generatedClassModel = concreteComponent.ToGeneratedClassModel();
    myGeneratedUsingsModel = concreteComponent.ToDependenciesUsingsModel();
    myGeneratedNamespaceModel = new GeneratedNamespaceModel(concreteComponent.Namespace, generatedClassModel.GenerateInto);
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    myGeneratedUsingsModel.GenerateInto(sb, indent);
    sb.AppendNewLine();
    
    myGeneratedNamespaceModel.GenerateInto(sb, indent);
  }
}

internal class GeneratedNamespaceModel
{
  private readonly string? myNamespaceName;
  private readonly Action<StringBuilder, int> myNamespaceBodyGenerator;

  
  public GeneratedNamespaceModel(string? namespaceName, Action<StringBuilder, int> namespaceBodyGenerator)
  {
    myNamespaceName = namespaceName;
    myNamespaceBodyGenerator = namespaceBodyGenerator;
  }

  
  public void GenerateInto(StringBuilder sb, int indent)
  {
    OpenCloseStringBuilderOperation? namespaceCookie = null;

    try
    {
      if (myNamespaceName is { })
      {
        sb.Append("namespace").AppendSpace().Append(myNamespaceName).AppendSpace().AppendNewLine();
        namespaceCookie = StringBuilderCookies.CurlyBraces(sb, indent);
      }

      myNamespaceBodyGenerator(sb, namespaceCookie?.Indent ?? indent);
    }
    finally
    {
      namespaceCookie?.Dispose();
    }
  }
}