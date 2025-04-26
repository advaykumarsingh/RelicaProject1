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
    abstract class Entity
    {
        private Rectangle entityRect;
        private Rectangle oldRect;
        private Texture2D entityTxt;
        private Vector2 origin;
        private int currTxtIdx;
        private Rectangle sourceRect;
        private double x, y;
        private double rotation;
        private int spriteWidth, spriteHeight;
        private int spritePadding, spritesPerLine;

        /// <summary>
        /// Creates a new basic Entity
        /// </summary>
        /// <param name="eRect">    Entity's World Rectangle                        </param>
        /// <param name="eTxt">     Entity's Texture                                </param>
        /// <param name="sWidth">   Width for Single Sprite within SpriteSheet      </param>
        /// <param name="sHeight">  Height for Single Sprite within SpriteSheet     </param>
        /// <param name="sPadding"> Padding between sprites in the SpriteSheet      </param>
        /// <param name="sPerLine"> Sprites per line                                </param>
        public Entity(Rectangle eRect, Texture2D eTxt, int sWidth, int sHeight, int sPadding, int sPerLine)
        {
            entityRect = eRect;
            X = eRect.X;
            Y = eRect.Y;
            origin = new Vector2();
            if (sWidth == -1 && eTxt != null)
            {
                spriteWidth = eTxt.Width;
                spriteHeight = eTxt.Height;
                spritePadding = 0;
                spritesPerLine = 1;
            }
            else
            {
                spriteWidth = sWidth;
                spriteHeight = sHeight;
                spritePadding = sPadding;
                spritesPerLine = sPerLine;
            }
            entityTxt = eTxt;
            sourceRect = new Rectangle(0, 0, spriteWidth, spriteHeight);
            currTxtIdx = 0;
        }
        public double X
        {
            get { return x; }
            set { x = value; entityRect.X = (int)X; }
        }
        public double Y
        {
            get { return y; }
            set { y = value; entityRect.Y = (int)Y; }
        }
        public double Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
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
        public Texture2D Texture
        {
            get { return entityTxt; }
            set { entityTxt = value; }
        }
        /// <summary>
        /// Sets texture ID and source rectangle
        /// </summary>
        public int CurrTextureIDX
        {
            get { return currTxtIdx; }
            set 
            { 
                currTxtIdx = value;
                Rectangle s = SourceRect;
                s.X = spritePadding + (currTxtIdx % spritesPerLine) * (spritePadding * 2 + spriteWidth);
                s.Y = spritePadding + (currTxtIdx / spritesPerLine) * (spritePadding * 2 + spriteHeight);

                origin.X = s.X + s.Center.X;
                origin.Y = s.Y + s.Center.Y;

                sourceRect = s;
            }
        }
        public Rectangle SourceRect
        {
            get { return sourceRect; }
            set { sourceRect = value; }
        }
        public Rectangle Rectangle
        {
            get { return entityRect; }
            set { entityRect = value; }
        }
        public Rectangle OldRectangle
        {
            get { return oldRect; }
            set { oldRect = value; }
        }
        public int SpriteHeight
        {
            get { return spriteHeight; }
            set 
            {
                spriteHeight = value;
                sourceRect.Height = spriteHeight;
            }
        }
        public int SpriteWidth
        {
            get { return spriteWidth; }
            set
            {
                spriteWidth = value;
                sourceRect.Width = spriteWidth;
            }
        }
        public void RectangleSync()
        {
            entityRect.X = (int)x;
            entityRect.Y = (int)y;
        }
        public Vector2 GetIntersectionDepth(Rectangle rectB)
        {
            Rectangle rectA = Rectangle;
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;

            return new Vector2(depthX, depthY);
        }
    }
}

