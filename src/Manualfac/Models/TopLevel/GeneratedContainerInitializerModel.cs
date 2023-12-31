﻿using System.Collections.Immutable;
using System.Text;
using Manualfac.Components;
using Manualfac.Models.Fields;
using Manualfac.Models.Methods;
using Manualfac.Util;
using Microsoft.CodeAnalysis;

namespace Manualfac.Models.TopLevel;

internal class GeneratedContainerInitializerModel : IGeneratedModel
{
  private readonly ComponentsStorage myStorage;
  private readonly GeneratedClassModel myGeneratedClassModel;


  public GeneratedContainerInitializerModel(ComponentsStorage storage, IAssemblySymbol assemblySymbol)
  {
    myStorage = storage;
    myGeneratedClassModel = new GeneratedClassModel(
      $"{assemblySymbol.Name}Initializer",
      ImmutableList<GeneratedConstructorModel>.Empty,
      ImmutableList<GeneratedFieldModel>.Empty,
      new[]
      {
        MethodFactory.InternalStatic("Initialize", "void", GenerateInitializeMethod)
      });
  }


  private void GenerateInitializeMethod(StringBuilder sb, int indent)
  {
    foreach (var component in myStorage.AllComponents)
    {
      sb.AppendIndent(indent).Append(component.CreateContainerFullName()).Append($".{Constants.InitializeMethod}");
      using (var cookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true))
      {
        sb.AppendIndent(cookie.Indent).Append("() => ").AppendNewLine();
        using (var methodCookie = StringBuilderCookies.CurlyBraces(sb, cookie.Indent))
        {
          var adjustedComponent = myStorage.AdjustComponent(component);
          IGeneratedModel model = ReferenceEquals(adjustedComponent, component) switch
          {
            true => new GeneratedComponentObjectCreationModel(component, myStorage.AdjustComponent),
            false => new GeneratedOverridenComponentObjectCreationModel(adjustedComponent)
          };

          model.GenerateInto(sb, methodCookie.Indent);
        }
      }

      sb.AppendSemicolon().AppendNewLine().AppendNewLine();
    }
  }

  public void GenerateInto(StringBuilder sb, int indent)
  {
    myGeneratedClassModel.GenerateInto(sb, indent);
  }
}