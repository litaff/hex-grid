namespace hex_grid.scripts.utils;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text.Json;

internal class ModuleInitializer
{
    /// <summary>
    /// https://github.com/godotengine/godot/issues/78513#issuecomment-1625004361
    /// </summary>
    [ModuleInitializer]
    public static void Initialize()
    {
        var assemblyLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
        if (assemblyLoadContext == null) return;
        assemblyLoadContext.Unloading += _ =>
        {
            var assembly = typeof(JsonSerializerOptions).Assembly;
            var updateHandlerType = assembly.GetType("System.Text.Json.JsonSerializerOptionsUpdateHandler");
            var clearCacheMethod = updateHandlerType?.GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public);
            clearCacheMethod?.Invoke(null, [null]);
        };
    }
}