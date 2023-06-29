using System.Collections.Immutable;
using System.Text;
using Manualfac.Generators.Components;
using Microsoft.CodeAnalysis;

namespace Manualfac.Generators.Models;

internal class GeneratedContainerInitializerModel
{
  private readonly ComponentsStorage myStorage;
  private readonly GeneratedClassModel myGeneratedClassModel;


  public GeneratedContainerInitializerModel(ComponentsStorage storage, IAssemblySymbol assemblySymbol)
  {
    myStorage = storage;
    var initializerClassName = $"{assemblySymbol.Name}Initializer";

    myGeneratedClassModel = new GeneratedClassModel(
      initializerClassName,
      ImmutableList<GeneratedConstructorModel>.Empty,
      ImmutableList<GeneratedFieldModel>.Empty,
      new[]
      {
        new GeneratedMethodModel(
          "Initialize", "void", GenerateInitializeMethod, ImmutableList<GeneratedParameterModel>.Empty, 
          AccessModifier.Internal, isStatic: true)
      });
  }


  private void GenerateInitializeMethod(StringBuilder sb, int indent)
  {
    
  }
  
  public void GenerateInto(StringBuilder sb, int indent)
  {
    myGeneratedClassModel.GenerateInto(sb, indent);
  }
}