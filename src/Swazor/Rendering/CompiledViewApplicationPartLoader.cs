namespace Swazor.Rendering;

using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

internal static class CompiledViewApplicationPartLoader
{
    internal static void AddCompiledViewParts(ApplicationPartManager manager)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
            {
                continue;
            }

            var factory = ApplicationPartFactory.GetApplicationPartFactory(assembly);

            if (factory is DefaultApplicationPartFactory)
            {
                continue;
            }

            foreach (var part in factory.GetApplicationParts(assembly))
            {
                if (!IsAlreadyRegistered(manager, part))
                {
                    manager.ApplicationParts.Add(part);
                }
            }
        }
    }

    private static bool IsAlreadyRegistered(ApplicationPartManager manager, ApplicationPart part)
        => manager
            .ApplicationParts
            .Any(existing => existing.GetType() == part.GetType() && existing.Name == part.Name);
}