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
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }
        
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            Farmer player = Game1.player;

            if (Movement.IsAutoWalking(player))
            {
                player.controller = null;
                player.Halt();
            }
            
            if (e.Button == SButton.MouseLeft && Helper.Input.IsDown(SButton.LeftControl))
            {
                Helper.Input.Suppress(SButton.MouseLeft); // Stop user from using tool
                GameLocation location = player.currentLocation;
                Vector2 clicked = e.Cursor.Tile;
                if(ObjectDetection.hasClickedBreakable(location, clicked))
                    Movement.MovePlayer(location, player, clicked.ToPoint());
            }

        }
    }
}