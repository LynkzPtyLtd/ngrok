using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Frosting;
using Lynkz.NGrok.Build.Extensions;

namespace Lynkz.NGrok.Build;

[TaskName("CleanProjects")]
public sealed class CleanProjectsTask : FrostingTask<BuildContext>
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
            
            context.Information($"Clean {project}");
            context.DotNetClean(project.ToString());
        }
    }
}