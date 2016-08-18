#region

using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

#endregion

[assembly: AssemblyTitle("PortAIO")]
[assembly: AssemblyDescription("PortAIO")]
[assembly: AssemblyConfiguration("PortAIO")]
[assembly: AssemblyCompany("PortAIO")]
[assembly: AssemblyProduct("PortAIO")]
[assembly: AssemblyCopyright("Copyright © PortAIO 2016")]
[assembly: AssemblyTrademark("PortAIO")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("fabfb012-089e-4629-9976-509afeaab307")]

[assembly: AssemblyVersion("1.0.0.4")]
[assembly: AssemblyFileVersion("1.0.0.4")]
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]