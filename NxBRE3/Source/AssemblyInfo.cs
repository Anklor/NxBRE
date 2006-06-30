using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly:AssemblyTitle("NxBRE")]
[assembly:AssemblyDescription(".NET Business Rules Engine")]
[assembly:AssemblyConfiguration("")]
[assembly:AssemblyCompany("AgilePartner S.A.")]
[assembly:AssemblyProduct("NxBRE")]
[assembly:AssemblyCopyright("Copyright (C) 2003-2006 David Dossot et al.")]
[assembly:AssemblyTrademark("NxBRE is distributed under the GNU LESSER GENERAL PUBLIC LICENSE.")]
[assembly:AssemblyCulture("")]
[assembly:AssemblyVersion("3.0.0.*")]
[assembly:AssemblyDelaySign(false)]
[assembly:AssemblyKeyFile("")]
[assembly:CLSCompliant(true)]

//TODO: all ToArray() and non generic collections must be replaced with generic collections
//TODO: ensure no internal class or method in non-core package - split classes like Fact in Fact & Core/FactUtils
