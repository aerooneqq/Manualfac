﻿using System.Collections.Immutable;
using System.Text;
using Manualfac.Generators.Components;
using Manualfac.Generators.Components.Dependencies;
using Manualfac.Generators.Util;
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
    foreach (var component in myStorage.AllComponents)
    {
      sb.AppendIndent(indent).Append(component.CreateContainerName()).Append(".Initialize");
      using (var cookie = StringBuilderCookies.DefaultBraces(sb, indent, appendEndIndent: true))
      {
        sb.AppendIndent(cookie.Indent).Append("() => ").AppendNewLine();
        using (var methodCookie = StringBuilderCookies.CurlyBraces(sb, cookie.Indent))
        {
          new GeneratedComponentObjectCreationModel(component, AdjustComponent).GenerateInto(sb, methodCookie.Indent);
        }
      }

      sb.AppendSemicolon().AppendNewLine().AppendNewLine();
    }
  }

  private IConcreteComponent AdjustComponent(IConcreteComponent component)
  {
    var current = component;
    while (myStorage.BaseToOverrides.TryGetValue(current, out var @override))
    {
      current = @override;
    }
    
    return current;
  }

  public void GenerateInto(StringBuilder sb, int indent)
  {
    myGeneratedClassModel.GenerateInto(sb, indent);
  }
}