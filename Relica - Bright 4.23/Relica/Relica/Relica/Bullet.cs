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
    class Bullet : Entity
    {
        Vector2 velocity;
        Boolean isAlive;

        public enum Types
        {
            Normal,
        }

        public Bullet(Types type, Rectangle eRect, Vector2 vel, double rot, Texture2D bulletText) : base(eRect, bulletText, -1, -1, -1, 1)
        {
            velocity = vel;
            Rotation = rot;
            isAlive = true;
            if (type == Types.Normal)
            {
                SpriteWidth = 8;
                SpriteHeight = 20;
                CurrTextureIDX = 0;
            }
        }

        public void Update()
        {
            X += velocity.X;
            Y += velocity.Y;

            if (ToDestroy())
                isAlive = false;
        }

        public Boolean ToDestroy()
        {
            Boolean destroys = false;
            //TODO - Collisions with enemies
            destroys = CheckWallCollision();
            return destroys;
        }

        public Boolean CheckWallCollision()
        {
            Boolean collided = false;
            int tileSize = Game1.TILE_SIZE;

            Rectangle rect = Rectangle;
            rect.X -= rect.Width / 2;
            rect.Y -= rect.Height / 2;

            //int c = (int)Math.Floor((float)(rect.Left - 560) / tileSize);
            //int cRight = (int)Math.Floor((float)(rect.Right - 560) / tileSize);
            //int r = (int)Math.Floor((float)(rect.Top - 240) / tileSize);
            //int rDown = (int)Math.Floor((float)(rect.Bottom - 240) / tileSize);
            //Rectangle collidingRectangle = new Rectangle(c * tileSize + 560, r * tileSize + 240, tileSize, tileSize);

            //if (r >= 0 && r < Game1.GRID.GetLength(0) && c >= 0 && c < Game1.GRID.GetLength(1) && rDown < Game1.GRID.GetLength(0) && cRight < Game1.GRID.GetLength(1))
            //{
            //    if (Game1.GRID[r, c] && rect.Intersects(collidingRectangle))
            //    {
            //        collided = true;
            //    }
            //}
            //else
            //{
            //    collided = true;
            //}

            int leftTile = (int)Math.Floor((float)(Rectangle.Left - 560) / tileSize);
            int topTile = (int)Math.Floor((float)(Rectangle.Top - 240) / tileSize);

            for (int r = topTile - 1; r <= topTile + 1; r++)
            {
                for (int c = leftTile - 1; c <= leftTile + 1; c++)
                {
                    Rectangle collidingRectangle = new Rectangle(c * tileSize + 560, r * tileSize + 240, tileSize, tileSize);
                    if (Game1.InBounds(r, c))
                    {
                        if (Game1.GRID[r, c].Collidable && rect.Intersects(collidingRectangle))
                        {
                            collided = true;
                        }
                    }
                    else if (rect.Intersects(collidingRectangle))
                    {
                        collided = true;
                    }
                }
            }

            return collided;
        }

        public Boolean IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }
    }
}
