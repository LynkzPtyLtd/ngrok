using System.Reflection;
using Cake.Common;
using Cake.Core;
using Cake.Frosting;

namespace Lynkz.NGrok.Build;

public class BuildContext : FrostingContext
{
    public string MsBuildConfiguration { get; set; }
    public bool Delay { get; set; }
    public string AssemblyVersion { get; set; }
    public string Version { get; set; }
    public string ExecutingAssemblyName { get; set; }


    public BuildContext(ICakeContext context)
        : base(context)
    {
        ExecutingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        Delay = context.Arguments.HasArgument("Delay");
        MsBuildConfiguration = context.Argument("Configuration", "Debug");
        AssemblyVersion = context.Argument("AssemblyVersion", "1.0.0.0");
        Version = context.Argument("Version", "1.0.0-alpha1");
    }
}