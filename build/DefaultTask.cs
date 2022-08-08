using Cake.Frosting;

namespace Lynkz.NGrok.Build;

[IsDependentOn(typeof(PackProjectTask))]
public sealed class Default : FrostingTask
{
}