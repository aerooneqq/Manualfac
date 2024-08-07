﻿using Manualfac;
using Manualfac.Analysis;
using Manualfac.Exceptions;
using Manualfac.Exceptions.BeforeAfterRelation;

namespace UnitTests;

[TestFixture]
public class GeneratedFilesTest : TestWithSourceFilesBase<ManualfacGenerator>
{
  [Test] public void SimpleTest() => DoTest();
  [Test] public void SimpleTest2() => DoTest();
  [Test] public void SimpleTest3() => DoTest();
  [Test] public void SimpleTest4() => DoTest();
  [Test] public void SimpleTest5() => DoTest();
  [Test] public void SimpleTest6() => DoTest();
  [Test] public void SimpleTest7() => DoTest();
  [Test] public void SimpleTest8() => DoTest();
  [Test] public void SimpleTestWithoutResolver() => DoTest();
  [Test] public void BeforeAfterTest1() => DoTest();
  [Test] public void BeforeAfterTest2() => DoTest();
  [Test] public void BeforeAfterTest3() => DoTest();
  [Test] public void BeforeAfterTest4() => DoTest();
  [Test] public void CustomComponentAttributeTest() => DoTest();
  [Test] public void NonPartialClassesTest() => DoTest();
  [Test] public void ManualInitialization() => DoTest();

  [Test] public void DuplicatedDepsTest() => DoTestWithDiagnostic(ErrorIds.DuplicatedDependencyId);
  [Test] public void TooManyOverridesTest() => DoTestWithDiagnostic(ErrorIds.MultipleBaseComponentsId);
  [Test] public void NotComponentTest() => DoTestWithDiagnostic(ErrorIds.DependingOnNonComponentSymbolId);
  [Test] public void OverrideNotComponentTest() => DoTestWithDiagnostic(ErrorIds.CanNotOverrideNonComponentSymbolId);
  [Test] public void OverridesMustInheritFromBase() => DoTestWithDiagnostic(ErrorIds.ComponentMustInheritFromDeclaredOverrideId);
  
  [Test] public void CyclicDepsTest() => DoTestWithException<CyclicDependencyException>();
  [Test] public void NoImplementationForInterfaceTest() => DoTestWithException<NoImplementationForInterfaceException>();
  [Test] public void CantResolveImplTest() => DoTestWithException<CantResolveConcreteImplementationException>();

  [Test]
  public void BeforeAfterTestSelfReferenceTest1() => DoTestWithException<SelfReferenceInBeforeAfterRelationException>();
  
  [Test]
  public void BeforeAfterTestSelfReferenceTest2() => DoTestWithException<SelfReferenceInBeforeAfterRelationException>();
  
  [Test] public void BeforeAfterRelationCycleTest1() => DoTestWithException<CyclicDependencyException>();
  [Test] public void BeforeAfterRelationCycleTest2() => DoTestWithException<CyclicDependencyException>();
  [Test] public void BeforeAfterRelationCycleTest3() => DoTestWithException<CyclicDependencyException>();
}