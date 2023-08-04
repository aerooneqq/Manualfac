using System.Text;
using Manualfac.Components;
using Manualfac.Util;
using Manualfac.Util.Naming;

namespace Manualfac.Models;

internal class GeneratedComponentFileModel : IGeneratedModel
{
  private readonly GeneratedUsingsModel myGeneratedUsingsModel;
  private readonly GeneratedNamespaceModel myGeneratedNamespaceModel;


  public GeneratedComponentFileModel(IComponent component, NamingStyle namingStyle)
  {
    var generatedClassModel = component.ToGeneratedClassModel(namingStyle);
    myGeneratedUsingsModel = component.ToDependenciesUsingsModel();
    myGeneratedNamespaceModel = new GeneratedNamespaceModel(component.Namespace, generatedClassModel.GenerateInto);
  }


  public void GenerateInto(StringBuilder sb, int indent)
  {
    myGeneratedUsingsModel.GenerateInto(sb, indent);
    sb.AppendNewLine();

    myGeneratedNamespaceModel.GenerateInto(sb, indent);
  }
}