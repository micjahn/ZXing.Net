using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyProduct("ZXing.Windows.Compatibility")]
[assembly: AssemblyCompany("ZXing.Net Development")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCopyright("Copyright © 2018")]
[assembly: AssemblyDescription("ZXing.Net Bindings to Windows Compatibility Pack")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1EDB7852-C08A-477C-A0F4-688798044273")]

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatformAttribute("windows")]
#endif