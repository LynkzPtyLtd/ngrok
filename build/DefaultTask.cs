using Cake.Frosting;

namespace Lynkz.NGrok.Build;

[IsDependentOn(typeof(PublishArtifactsTask))]
public sealed class Default : FrostingTask
{
}