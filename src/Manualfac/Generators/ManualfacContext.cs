using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Manualfac.Generators;

public readonly struct ManualfacContext(
  SourceProductionContext productionContext, AnalyzerConfigOptionsProvider provider, Compilation compilation)
{
  public SourceProductionContext ProductionContext { get; } = productionContext;
  public AnalyzerConfigOptionsProvider Provider { get; } = provider;
  public Compilation Compilation { get; } = compilation;
}