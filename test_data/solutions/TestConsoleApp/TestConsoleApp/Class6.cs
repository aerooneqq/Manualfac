using ClassLibrary1;
using ClassLibrary2;
using ManualfacAttributes;

[Component, Overrides<Class3>, DependsOn<Private, Class4>]
public partial class Class6 : Class3
{
}