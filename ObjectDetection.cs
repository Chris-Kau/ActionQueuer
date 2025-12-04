using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using StardewValley.Objects;
using StardewValley.Tools;

namespace ActionQueuer;

public class ObjectDetection
{
    public static bool hasClickedBreakable(GameLocation location, Vector2 tile)
    {
        return IsBreakable(location, tile);
    }
    private static bool IsBreakable(GameLocation location, Vector2 tile)
    {
        Console.WriteLine($"Required Tool: {ToolDetection.GetRequiredTool(location, tile)?.GetType()}");
        // Small objects (stones, ore nodes, weeds, twigs, etc.)
        if (location.objects.TryGetValue(tile, out StardewValley.Object obj))
        {
            if (obj.IsBreakableStone())
            {
                Console.WriteLine("Small Objects");
                return true;
            }
            
            if (obj.IsTwig() || obj.IsWeeds())
            {
                Console.WriteLine("Twig / Weeds");
                return true;
            }

            if (obj.isForage())
            {
                Console.WriteLine("Forageable");
                return true;
            }
        }

        // Trees (and stumps)
        if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature))
        {
            if (feature is Tree tree)
            {
                Console.WriteLine("Tree");
                return true;
                
            }
        }

        // Large breakable things (stumps, logs, boulders)
        foreach (var clump in location.resourceClumps)
        {
            if (clump.getBoundingBox().Contains((int)tile.X * 64, (int)tile.Y * 64))
            {
                Console.WriteLine("Large Breakable");
                return true;
            }
        }

        return false;
    }
}