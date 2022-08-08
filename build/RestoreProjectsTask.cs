using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Frosting;
using Lynkz.NGrok.Build.Extensions;

namespace Lynkz.NGrok.Build;

[TaskName("RestoreProjects")]
[IsDependentOn(typeof(CleanProjectsTask))]
public sealed class RestoreProjectsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var projects = context.GetFiles("../**/*.csproj");
        foreach (var project in projects)
        {
            if (project.IsBuildProject(context))
            {
                continue;
            }

            context.Information($"Building {project}");
            context.DotNetRestore(project.ToString());
        }
    }
}