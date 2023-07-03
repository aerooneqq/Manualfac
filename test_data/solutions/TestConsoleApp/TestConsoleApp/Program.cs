// See https://aka.ms/new-console-template for more information

using ClassLibrary1;
using ClassLibrary2;

TestConsoleAppInitializer.Initialize();

Console.WriteLine(ReferenceEquals(Class1Container.Resolve(), Class1Container.Resolve()));
Console.WriteLine(ReferenceEquals(Class2Container.Resolve(), Class2Container.Resolve()));
Console.WriteLine(ReferenceEquals(Class3Container.Resolve(), Class3Container.Resolve()));
Console.WriteLine(ReferenceEquals(Class4Container.Resolve(), Class4Container.Resolve()));
Console.WriteLine(ReferenceEquals(Class5Container.Resolve(), Class5Container.Resolve()));
Console.WriteLine(ReferenceEquals(Class3Container.Resolve(), Class6Container.Resolve()));

Console.WriteLine(ReferenceEquals(TestConsoleAppResolver.ResolveOrThrow<Class1>(), Class1Container.Resolve()));
Console.WriteLine(ReferenceEquals(TestConsoleAppResolver.ResolveOrThrow<Class2>(), Class2Container.Resolve()));
Console.WriteLine(ReferenceEquals(TestConsoleAppResolver.ResolveOrThrow<Class3>(), Class3Container.Resolve()));
Console.WriteLine(ReferenceEquals(TestConsoleAppResolver.ResolveOrThrow<Class4>(), Class4Container.Resolve()));
Console.WriteLine(ReferenceEquals(TestConsoleAppResolver.ResolveOrThrow<Class5>(), Class5Container.Resolve()));
Console.WriteLine(ReferenceEquals(TestConsoleAppResolver.ResolveOrThrow<Class6>(), Class6Container.Resolve()));

var xd = TestConsoleAppResolver.ResolveComponentsOrThrow<IInterface>().ToList();
Console.WriteLine(ReferenceEquals(xd[0], Class4Container.Resolve()));
Console.WriteLine(ReferenceEquals(xd[1], Class5Container.Resolve()));
Console.WriteLine(ReferenceEquals(xd[2], Class6Container.Resolve()));
Console.WriteLine(ReferenceEquals(xd[3], Class1Container.Resolve()));