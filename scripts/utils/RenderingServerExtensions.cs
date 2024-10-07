namespace hex_grid.scripts.utils;

using Godot;

public static class RenderingServerExtensions
{
    public static void FreeRid(this ref Rid rid)
    {
        if (!rid.IsValid)
        {
            rid = new Rid();
            return;
        }
        RenderingServer.FreeRid(rid);
        rid = new Rid();
    }
}