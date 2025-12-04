using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Pathfinding;

namespace ActionQueuer
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
         ** Public methods
         *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }
        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (!(e.Held.Contains(SButton.LeftControl) && e.Pressed.Contains(SButton.MouseLeft)))
                return;
            Farmer player = Game1.player;
            GameLocation location = player.currentLocation;
            Point clicked = e.Cursor.Tile.ToPoint();
            Vector2 clickedVec = e.Cursor.Tile;
            Point target;

            if (!location.IsTileBlockedBy(clickedVec, CollisionMask.All, CollisionMask.All, true))
            {
                target = clicked;
            }
            else
            {
                // choose best reachable neighbor
                Point? best = GetBestReachableNeighbor(location, player, clicked);
                if (best == null)
                {
                    this.Monitor.Log("No reachable neighbor around clicked tile.", LogLevel.Debug);
                    return;
                }
                target = best.Value;
            }
            var pfc = new PathFindController(player, location, target, player.FacingDirection);
            if (pfc?.pathToEndPoint == null)
            {
                Console.WriteLine($"No path to tile {target}.");
                return;
            }
            player.controller = pfc;
            /*if (location.objects.ContainsKey(e.Cursor.Tile))
            {
                Console.WriteLine(location.Objects[e.Cursor.Tile].Name);
            }*/
        }

        private Point? GetBestReachableNeighbor(GameLocation location, Farmer player, Point clicked)
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

        
    }
}