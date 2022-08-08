using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Frosting;
using Lynkz.NGrok.Build.Extensions;

namespace Lynkz.NGrok.Build;

[TaskName("ExecuteTests")]
[IsDependentOn(typeof(BuildProjectsTask))]
public class ExecuteTestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var projects = context.GetFiles("../tests/*.csproj");
        foreach (var project in projects)
        {
            if (project.IsBuildProject(context))
            {
                continue;
            }
            
            context.Information($"Test {project}");
            context.DotNetTest(project.ToString(), new DotNetTestSettings
            {
                Configuration = context.MsBuildConfiguration,
                NoBuild = true,
                NoRestore = true
            });
        }
    }
}