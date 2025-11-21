// polyfill because we use new languageversion and old .NET
// https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined
namespace System.Runtime.CompilerServices;

#pragma warning disable S2094 // Classes should not be empty
internal static class IsExternalInit
#pragma warning restore S2094 // Classes should not be empty
{
}
