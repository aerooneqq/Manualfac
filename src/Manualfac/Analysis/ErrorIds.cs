namespace Manualfac.Analysis;

public static class ErrorIds
{
  public const string ErrorPrefix = "MFAC";
  
  private const uint DependingOnNonComponentSymbol = 1;
  private const uint DuplicatedDependency = 2;
  private const uint MultipleBaseComponents = 3;
  private const uint BaseComponentIsNotClass = 4;
  private const uint CanNotOverrideNonComponentSymbol = 5;
  private const uint ComponentMustInheritFromDeclaredOverride = 6;
  
  public static string DependingOnNonComponentSymbolId { get; } = CreateErrorId(DependingOnNonComponentSymbol);
  public static string DuplicatedDependencyId { get; } = CreateErrorId(DuplicatedDependency);
  public static string MultipleBaseComponentsId { get; } = CreateErrorId(MultipleBaseComponents);
  public static string BaseComponentIsNotClassId { get; } = CreateErrorId(BaseComponentIsNotClass);
  public static string CanNotOverrideNonComponentSymbolId { get; } = CreateErrorId(CanNotOverrideNonComponentSymbol);
  public static string ComponentMustInheritFromDeclaredOverrideId { get; } = CreateErrorId(ComponentMustInheritFromDeclaredOverride);


  private static string CreateErrorId(uint id) => $"{ErrorPrefix}{id.ToString().PadLeft(5, '0')}";
}