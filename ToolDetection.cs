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

public class ToolDetection
{

    public static Tool? GetRequiredTool(GameLocation location, Vector2 tile)
    {
        // Large breakables: stumps, logs, boulders, meteorites, etc.
        var clumpTool = GetToolForResourceClump(location, tile);
        if (clumpTool != null)
            return clumpTool;

        // Small objects: stones, ore, weeds, twigs, artifact spots, etc.
        if (location.objects.TryGetValue(tile, out StardewValley.Object obj))
        {
            var objTool = GetToolForObject(obj);
            if (objTool != null)
                return objTool;
        }

        // Terrain features: trees, fruit trees, small bushes
        if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature))
        {
            var tfTool = GetToolForTerrainFeature(feature);
            if (tfTool != null)
                return tfTool;
        }

        return null;
    }
    
    private static Tool? GetToolForObject(StardewValley.Object obj)
    {
        // Stones / ore / geode nodes, etc
        if (obj.IsBreakableStone())
            return new Pickaxe();

        // Weeds (fiber)
        if (obj.IsWeeds())
            return new MeleeWeapon();

        // Twigs
        if (obj.Name == "Twig")
            return new Axe();

        return null;
    }
    
    private static Tool? GetToolForTerrainFeature(TerrainFeature feature)
    {
        switch (feature)
        {
            case Tree t:
                return new Axe();
            /*case FruitTree:
                return new Axe();*/
            default:
                return null;
        }

    }
    
    private static Tool? GetToolForResourceClump(GameLocation location, Vector2 tile)
    {
        int px = (int)tile.X * 64;
        int py = (int)tile.Y * 64;

        foreach (var clump in location.resourceClumps)
        {
            if (!clump.getBoundingBox().Contains(px, py))
                continue;

            switch (clump.parentSheetIndex.Value)
            {
                case ResourceClump.stumpIndex:      // hardwood stump
                case ResourceClump.hollowLogIndex:  // hardwood log
                    return new Axe();

                case ResourceClump.boulderIndex:    // big stone boulder
                case ResourceClump.meteoriteIndex:  // purple meteor
                    return new Pickaxe();

                // Giant crops & other clumps:
                default:
                    return new Axe(); 
            }
        }

        return null;
    }
    
}