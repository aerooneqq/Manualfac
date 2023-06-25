// See https://aka.ms/new-console-template for more information

using ClassLibrary1;
using ClassLibrary2;

Console.WriteLine(ReferenceEquals(Class1Container.Resolve(), Class1Container.Resolve()));
Console.WriteLine(ReferenceEquals(Class2Container.Resolve(), Class2Container.Resolve()));
Console.WriteLine(ReferenceEquals(Class3Container.Resolve(), Class3Container.Resolve()));
Console.WriteLine(ReferenceEquals(Class4Container.Resolve(), Class4Container.Resolve()));
Console.WriteLine(ReferenceEquals(Class5Container.Resolve(), Class5Container.Resolve()));

Console.WriteLine("Hello, World!");