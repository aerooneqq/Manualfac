using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Manualfac.Generators;

public readonly struct ManualfacContext
{
  public SourceProductionContext ProductionContext { get; }
  public AnalyzerConfigOptionsProvider Provider { get; }
  public Compilation Compilation { get; }

  
  public ManualfacContext(
    SourceProductionContext productionContext, 
    AnalyzerConfigOptionsProvider provider, 
    Compilation compilation)
  {
    ProductionContext = productionContext;
    Provider = provider;
    Compilation = compilation;
  }
}