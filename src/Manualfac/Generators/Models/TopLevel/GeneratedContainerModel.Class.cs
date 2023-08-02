using System.Collections.Immutable;
using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Models.Fields;
using Manualfac.Generators.Models.Methods;
using Manualfac.Generators.Util;

namespace Manualfac.Generators.Models.TopLevel;

internal partial class GeneratedContainerModel
{
  private const string DefaultInitializeMethodName = "DefaultInitialize";

  private const string InstanceFieldName = "ourInstance";
  private const string SyncFieldName = "ourSync";
  private const string InitializationFuncFieldName = "ourF";
  private const string InitializeFuncParamName = "f";

  private const string CreatedVarName = "created";
  private const string Existing1 = "existing1";
  private const string Existing2 = "exiting2";

  private const string Void = "void";


  private static GeneratedClassModel CreateClassModel(IComponent component)
  {
    var constructors = ImmutableArray<GeneratedConstructorModel>.Empty;

    return new GeneratedClassModel(
      component.CreateContainerName(), constructors, CreateFields(component), CreateMethods(component));
  }

  private static IReadOnlyList<GeneratedFieldModel> CreateFields(IComponent component) => new[]
  {
    FieldFactory.PrivateStatic(component.FullName, InstanceFieldName),
    FieldFactory.PrivateStaticInitialized("object", SyncFieldName, "new object()"),
    FieldFactory.PrivateStaticInitialized($"Func<{component.FullName}>", InitializationFuncFieldName,
      DefaultInitializeMethodName)
  };

  private static IReadOnlyList<GeneratedMethodModel> CreateMethods(IComponent component)
  {
    var initializationModel = new GeneratedComponentObjectCreationModel(component, static c => c);
    var initializeMethodParams = new[]
    {
      new GeneratedParameterModel($"Func<{component.FullName}>", InitializeFuncParamName)
    };

    return new[]
    {
      MethodFactory.PublicStatic(Constants.ResolveMethod, component.FullName, GenerateFactoryMethod),
      MethodFactory.PublicStaticParameters(Constants.InitializeMethod, Void, GenerateInitializeMethod,
        initializeMethodParams),
      MethodFactory.PrivateStatic(DefaultInitializeMethodName, component.FullName, initializationModel.GenerateInto)
    };
  }

  private static void GenerateFactoryMethod(StringBuilder sb, int indent)
  {
    sb.AppendIndent(indent)
      .Append($"if (Volatile.Read(ref {InstanceFieldName}) is {{ }} {Existing1}) return {Existing1};")
      .AppendNewLine();

    using var lockCookie = StringBuilderCookies.Lock(sb, SyncFieldName, indent);

    sb.AppendIndent(lockCookie.Indent)
      .Append($"if (Volatile.Read(ref {InstanceFieldName}) is {{ }} {Existing2}) return {Existing2};")
      .AppendNewLine();

    sb.AppendIndent(lockCookie.Indent).Append($"var {CreatedVarName} = ").Append(InitializationFuncFieldName)
      .Append("()").AppendSemicolon().AppendNewLine();

    sb.AppendIndent(lockCookie.Indent).Append($"Volatile.Write(ref {InstanceFieldName}, {CreatedVarName});")
      .AppendNewLine();

    sb.AppendIndent(lockCookie.Indent).Append($"return {CreatedVarName};");
  }

  private static void GenerateInitializeMethod(StringBuilder sb, int indent)
  {
    using var lockCookie = StringBuilderCookies.Lock(sb, SyncFieldName, indent);
    sb.AppendIndent(lockCookie.Indent).Append($"{InitializationFuncFieldName}").AppendEq()
      .Append(InitializeFuncParamName).AppendSemicolon();
  }
}