using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Pathfinding;
namespace ActionQueuer;

public class Movement
{
    public static void MovePlayer(GameLocation location, Farmer player, Point clicked)
    {
        // choose best reachable neighbor
        Point? best = GetBestReachableNeighbor(location, player, clicked);
        if (best == null)
        {
            Console.WriteLine("No reachable neighbor around clicked tile.");
            return;
        }
        var pfc = new PathFindController(player, location, best.Value, player.FacingDirection);
        if (pfc?.pathToEndPoint == null)
        {
            Console.WriteLine($"No path to tile {best}.");
            return;
        }
        player.controller = pfc;
    }
    private static Point? GetBestReachableNeighbor(GameLocation location, Farmer player, Point clicked)
    {
        // 4 cardinal neighbors
        Point[] offsets =
        {
            new Point(0, -1), // up
            new Point(1, 0),  // right
            new Point(0, 1),  // down
            new Point(-1, 0), // left
        };

        var results = offsets
            .Select(offset => clicked + offset)
            .Select(tile =>
            {
                // check if the tile is on the map or if it's blocked
                Vector2 tileVec = new Vector2(tile.X, tile.Y);
                if (!location.isTileOnMap(tileVec) || location.IsTileBlockedBy(tileVec, CollisionMask.All, CollisionMask.All, true))
                    return (tile: (Point?)null, pathLen: int.MaxValue);

                // use built-in pathfinding to check if a path exists to the tile
                var test = new PathFindController(player, location, tile, 0);
                if (test?.pathToEndPoint == null)
                    return (tile: (Point?)null, pathLen: int.MaxValue);

                int len = test.pathToEndPoint.Count;
                return (tile: (Point?)tile, pathLen: len);
            })
            .Where(r => r.tile != null)
            .ToList();

        if (!results.Any())
            return null; // no reachable neighbor

        return results
            .OrderBy(r => r.pathLen)
            .First().tile;
    }
    
    public static bool IsAutoWalking(Farmer player)
    {
        return player.controller != null;
    }
}