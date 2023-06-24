using System.Text;

namespace Manualfac.Generators.Models;

internal class GeneratedComponentFileModel
{
  private readonly GeneratedClassModel myGeneratedClassModel;
  private readonly GeneratedUsingsModel myGeneratedUsingsModel;
  private readonly string? myNamespace;

  
  public GeneratedComponentFileModel(ComponentInfo component)
  {
    myNamespace = component.Namespace;
    myGeneratedClassModel = component.ToGeneratedClassModel();
    myGeneratedUsingsModel = component.ToDependenciesUsingsModel();
  }


  public unsafe void GenerateInto(StringBuilder sb, int indent)
  {
    myGeneratedUsingsModel.GenerateInto(sb, indent);
    sb.AppendNewLine();
    
    OpenCloseStringBuilderOperation? namespaceCookie = null;

    try
    {
      if (myNamespace is { })
      {
        sb.Append("namespace").AppendSpace().Append(myNamespace).AppendSpace().AppendNewLine();
        namespaceCookie = StringBuilderCookies.CurlyBraces(sb, &indent);
      }
      
      myGeneratedClassModel.GenerateInto(sb, indent);
    }
    finally
    {
      namespaceCookie?.Dispose();
    }
  }
}