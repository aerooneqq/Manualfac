using Manualfac.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace UnitTests;

[TestFixture]
public class GeneratedFilesTest : TestWithSourceFilesBase<ServiceInjectionGenerator>
{
  [Test] public void SimpleTest() => DoTest();
  [Test] public void SimpleTest2() => DoTest();
  [Test] public void SimpleTest3() => DoTest();
}