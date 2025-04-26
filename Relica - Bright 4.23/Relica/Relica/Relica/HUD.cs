using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Relica
{
    class HUD
    {
        public Rectangle frame;
        public Rectangle HUDrect;
        Rectangle dashRect;
        Rectangle dashSource;
        static Rectangle activeIcon = new Rectangle(1064, 112, 80, 80);
        static Rectangle iconDest = new Rectangle(0, 0, 80, 80);
        Rectangle healthBar;
        Color healthBarColor;

        Dictionary<string, Texture2D> textures;

        SpriteBatch Spritebatch;
        Player p1;

        public HUD(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice, SpriteBatch batch, Player player)
        {
            textures = new Dictionary<string, Texture2D>();
            frame = new Rectangle((GraphicsDevice.Viewport.Width / 2) - 440, 0, 880, 1080);
            HUDrect = new Rectangle((GraphicsDevice.Viewport.Width / 2) - 400, 40, 800, 200);
            dashRect = new Rectangle(1064, 0, 88, 16);
            dashSource = new Rectangle(0, 0, 88, 16);
            Spritebatch = batch;
            p1 = player;
            healthBar = new Rectangle(656, 56, player.Health * 4, 32);
            healthBarColor = new Color(222, 36, 36);
            //if (player.itemManager.activeItem != null)
            //{
            //    iconText = player.itemManager.activeItem.Icon;
            //}
        }


        public void UpdateHUD()
        {
            Spritebatch.Draw(textures["frame"], frame, Color.White);
            Spritebatch.Draw(textures["blank"], healthBar, healthBarColor);
            Spritebatch.Draw(textures["HUD"], HUDrect, Color.White);

            for (int i = 0; i < p1.MaxDashes; i++)
            {
                Color c = i < p1.DashesAvailable ? Color.White : Color.Gray;
                dashRect.Y = 50 + i * 24;
                dashRect.Width = 88;
                Spritebatch.Draw(textures["dash"], dashRect, c);
            }
            for (int i = p1.DashesAvailable; i < p1.MaxDashes; i++)
            {
                dashRect.Y = 50 + i * 24;
                dashRect.Width = (int)((p1.DashRefillTime - p1.DashRefill) / p1.DashRefillTime * textures["dash"].Width);
                dashSource.Width = (int)((p1.DashRefillTime-p1.DashRefill) / p1.DashRefillTime * textures["dash"].Width);
                Spritebatch.Draw(textures["dash"], dashRect, dashSource, Color.DarkGray);
            }
            //Spritebatch.Draw(iconText, activeIcon, iconDest, Color.White);
        }

        public void AddTexture(string key, Texture2D texture)
        {
            textures.Add(key, texture);
        }
    }
}
