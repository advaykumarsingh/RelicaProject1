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
    class Player : ControllableEntity
    {
        int health;
        int bulletSpeed;

        MouseState mouse;
        MouseState oldMouse;

        Texture2D bulletText;

        List<Bullet> bullets;

        public Player(Rectangle eRect, Texture2D eTxt) : base(eRect, eTxt)
        {
            health = 100;
            MaxSpeed = 5;
            bulletSpeed = 10;
            oldMouse = Mouse.GetState();

            bullets = new List<Bullet>();
        }

        /// <summary>
        /// Functions - Moving; Update Shooting
        /// </summary>
        public void Update(int timer)
        {
            CurrTextureIDX = 0;
            move(timer);
            CalcMouse();
            foreach (Bullet b in bullets)
            {
                b.Update();
            }
            int i = 0;
            while (i < bullets.Count)
            {
                if (!bullets[i].IsAlive)
                {
                    bullets.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle drawRect = Rectangle;
            drawRect.X = drawRect.Center.X;
            drawRect.Y = drawRect.Center.Y;

            spriteBatch.Draw(Texture, drawRect, null, IsDashing ? Color.White : Color.Red, (float)Rotation-MathHelper.ToRadians(90), Origin, SpriteEffects.None, 1f);
        }

        public void DrawBullets(SpriteBatch spriteBatch)
        {
            foreach (Bullet b in bullets)
            {
                Rectangle r = b.Rectangle;
                r.Width = b.SourceRect.Width;
                r.Height = b.SourceRect.Height;
                Console.WriteLine(b.Origin.X + " " + b.Origin.Y);
                spriteBatch.Draw(b.Texture, r, b.SourceRect, Color.White, (float)b.Rotation - MathHelper.ToRadians(90), b.Origin, SpriteEffects.None, 1f);

                Rectangle rect = b.Rectangle;
                rect.X -= rect.Width / 2;
                rect.Y -= rect.Height / 2;

                //Normal Hitbox
                //spriteBatch.Draw(Texture, rect, Color.Red);
            }
        }

        public void CalcMouse()
        {
            mouse = Mouse.GetState();

            double xCenter = X + Width / 2;
            double yCenter = Y + Height / 2;
            Rotation = Math.Atan2(yCenter - mouse.Y, xCenter - mouse.X);

            if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
            {
                FireNormalBullet(mouse.X, mouse.Y, xCenter, yCenter);
            }
            else if (mouse.RightButton == ButtonState.Pressed && oldMouse.RightButton == ButtonState.Released)
            {
                FireSplitShot(mouse.X, mouse.Y, xCenter, yCenter);
            }

            oldMouse = mouse;
        }

        public void FireNormalBullet(float MouseX, float MouseY, double originX, double originY)
        {

            double hyp = Math.Sqrt(Math.Pow(originX - MouseX, 2) + Math.Pow(originY - MouseY, 2));
            double numUpdates = hyp / bulletSpeed;
            Vector2 vel = new Vector2((float)((MouseX - originX) / numUpdates), (float)((MouseY - originY) / numUpdates));
            double rot = Math.Atan2(originY - MouseY, originX - MouseX);

            bullets.Add(new Bullet(Bullet.Types.Normal, new Rectangle((int)originX, (int)originY, 10, 10), vel, rot, bulletText));
        }

        public void FireSplitShot(float MouseX, float MouseY, double originX, double originY)
        {

            Vector2 mouseVec = new Vector2(MouseX, MouseY);
            Vector2 playerVec = new Vector2((float)originX, (float)originY);
            Vector2 direction = mouseVec - playerVec;
            direction.Normalize();

            float spreadAngle = MathHelper.ToRadians(5); // 5 degrees cone

            Vector2[] directions = new Vector2[3]
            {
                direction,//center
                Vector2.Transform(direction, Matrix.CreateRotationZ(-spreadAngle)), //left
                Vector2.Transform(direction, Matrix.CreateRotationZ(spreadAngle)) //right
            };
            foreach (Vector2 dir in directions)
            {
                Vector2 velocity = dir * bulletSpeed;
                double rot = Math.Atan2(-dir.Y, -dir.X);
                bullets.Add(new Bullet(Bullet.Types.Normal, new Rectangle((int)originX, (int)originY, 10, 10), velocity, rot, bulletText));
            }

        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        public Texture2D BulletText
        {
            get { return bulletText; }
            set { bulletText = value; }
        }
    }
}
