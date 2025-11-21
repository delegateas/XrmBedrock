using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

namespace DataverseLogic;

internal static class ExecutionContextExtensions
{
    private static TE? GetImage<TE>(this IPluginExecutionContext context, ImageType imageType, string name)
        where TE : Entity
    {
        EntityImageCollection? collection = null;
        if (imageType == ImageType.PreImage)
        {
            collection = context.PreEntityImages;
        }
        else if (imageType == ImageType.PostImage)
        {
            collection = context.PostEntityImages;
        }

        Entity entity;
        if (collection != null && collection.TryGetValue(name, out entity))
        {
            return entity.ToEntity<TE>();
        }
        else
        {
            return null;
        }
    }

    internal static TE? GetPreImage<TE>(this IPluginExecutionContext context, string name = "PreImage")
        where TE : Entity
    {
        return context.GetImage<TE>(ImageType.PreImage, name);
    }

    internal static TE GetRequiredPreImage<TE>(this IPluginExecutionContext context, string name = "PreImage")
        where TE : Entity
    {
        return context.GetPreImage<TE>(name) ?? throw new InvalidPluginExecutionException("PreImage is required");
    }

    internal static TE? GetPostImage<TE>(this IPluginExecutionContext context, string name = "PostImage")
        where TE : Entity
    {
        return context.GetImage<TE>(ImageType.PostImage, name);
    }

    internal static TE GetRequiredPostImage<TE>(this IPluginExecutionContext context, string name = "PostImage")
        where TE : Entity
    {
        return context.GetPostImage<TE>(name) ?? throw new InvalidPluginExecutionException("PostImage is required");
    }

    internal static TE GetTarget<TE>(this IPluginExecutionContext context)
        where TE : Entity
    {
        if (!context.InputParameters.TryGetValue("Target", out Entity target))
            throw new InvalidOperationException("Message had no target. Make sure the request is of type Create or Update");

        return target.ToEntity<TE>();
    }

    internal static TE GetTargetMergedWithPreImage<TE>(this IPluginExecutionContext context)
        where TE : Entity, new()
    {
        var merged = new TE();
        var preImage = context.GetPreImage<TE>();
        if (preImage != null)
        {
            foreach (var attr in preImage.Attributes)
            {
                merged[attr.Key] = attr.Value;
            }

            merged.Id = preImage.Id;
        }

        var target = context.GetTarget<TE>();
        if (target != null)
        {
            foreach (var attr in target.Attributes)
            {
                merged[attr.Key] = attr.Value;
            }

            merged.Id = target.Id;
        }

        return merged;
    }

    internal static bool IsOperationUpdate(this IPluginExecutionContext context) =>
        context.MessageName?.Equals("update", StringComparison.OrdinalIgnoreCase) ?? false;

    internal static TE GetPostImageDefaultTarget<TE>(this IPluginExecutionContext context)
        where TE : Entity => context.GetPostImage<TE>() ?? context.GetTarget<TE>();

    internal static T GetRequest<T>(this IPluginExecutionContext context)
    {
        var requestString = context.InputParameters["Payload"] as string ?? throw new InvalidPluginExecutionException($"Payload parameter required");

        return JsonConvert.DeserializeObject<T>(requestString) ?? throw new InvalidPluginExecutionException($"Invalid request format {requestString}");
    }
}
