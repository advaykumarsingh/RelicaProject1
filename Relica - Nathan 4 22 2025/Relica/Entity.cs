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
    public abstract class Entity
    {
        public static KeyboardState oldKey;
        public static KeyboardState curKey;
        public static MouseState curMouse;
        public static MouseState oldMouse;
        public static Random rand = new Random();
        public static GameTime time;
        private Rectangle entityRect;
        private Texture2D entityTxt;
        private double x, y;
        public Entity(Rectangle eRect, Texture2D eTxt)
        {
            this.entityRect = eRect;
            this.entityTxt = eTxt;
            x = eRect.X;
            y = eRect.Y;
        }
        public abstract void update();

        public static void newUpdate()
        {
            curKey = Keyboard.GetState();
            curMouse = Mouse.GetState();
        }
        public static void oldUpdate()
        {
            oldKey = curKey;
            oldMouse = curMouse;
        }
        public double X
        {
            get { return x; }
            set { x = value;
                RectangleSync(); }
        }
        public double Y
        {
            get { return y; }
            set { y = value;
                RectangleSync();}
        }
        public int Width
        {
            get { return entityRect.Width; }
            set { entityRect.Width = value; }
        }
        public int Height
        {
            get { return entityRect.Height; }
            set { entityRect.Height = value; }
        }

        public Rectangle eRect
        {
            get { return entityRect; }
        }
        public Texture2D eTxt
        {
            get { return entityTxt; }
        }
        public Vector2 distBetweenCenter(Entity other, Entity centered)
        {
            return new Vector2((float)(other.eRect.Center.X - centered.eRect.Center.X), (float)(other.eRect.Center.Y - centered.eRect.Center.Y));
        }
        public double distFromPlayer()
        {
            return this.distBetweenCenter(Player.player, this).Length();
        }
        public static bool wasKeyPressed(Keys k)
        {
            return curKey.IsKeyDown(k) && oldKey.IsKeyUp(k);
        }
        public abstract void draw(GameTime gameTime, SpriteBatch spriteBatch);
        public void RectangleSync()
        {
            entityRect.X = (int)x;
            entityRect.Y = (int)y;
        }
        
    }
}
