## Manualfac

Proof-of-concept DI container based on source generators.

Features:
- Specify dependencies and their access modifiers with generic attributes:
  ```csharp
  [Component, DependsOn<PrivateProtected, Class1, Class2, Class3, Class4>]
  public partial class Class5
  {
  }
  ```
  Source generator will generate constructor and fields for dependencies.
- Register components as selected interface:
  ```csharp
  [Component, As<IClass11>]
  public partial class Class1 : IClass1, IClass11
  {
  }
  ```
- Override components in dependant projects:
  ```csharp
  [Component, Overrides<Class2>]
  public partial class Class3 : Class2
  {
  }
  ```
- Query several dependencies implementing same interface:
  ```csharp
  [Component, DependsOn<Internal, IEnumerable<IClass1>>()]
  public class Class3
  {
  }
  ```
- Specify end-projects for which the resolver will be generated:
  ```csharp
  [assembly: GenerateResolver]  
  ```

  ```csharp
  TestConsoleAppInitializer.Initialize();
  TestConsoleAppResolver.ResolveOrThrow<Class1>()
  ```
- Specify naming rules with `Directory.Build.props`
  ```msbuild
  <Project>
    <ItemGroup>
      <CompilerVisibleProperty Include="ManualfacDependencySuffix" />
      <CompilerVisibleProperty Include="ManualfacDependencyPrefix" />
    </ItemGroup>

    <PropertyGroup>
      <ManualfacDependencySuffix>suffix</ManualfacDependencySuffix>
      <ManualfacDependencyPrefix>prefix</ManualfacDependencyPrefix>
    </PropertyGroup>
  </Project>
  ```

Todos:
- Different lifetimes of components?
- Integration with other containers, like ASP NET Core default one, autofac...
- Test it against nuget packages
- Ordering of dependencies in case of `IEnumerable<IComponent>` 
  dependency, e.g. with `Before<T1, T2, ....>` and `After<T1, T2, ...>` attributes
- Feature flags? Zones?
- More configurable naming