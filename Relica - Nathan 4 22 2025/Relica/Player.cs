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
    public class Player : ControllableEntity
    {
        public List<PassiveItem> passiveItems;
        public static int size = 1;
        public ActiveItem[] actives;
        public static Player player;
        public static Texture2D playerTxt;
        public int[] stats;
        public float rotation;
        public static Vector2 spriteOrigin;
        public enum stat
        {
            Hp,
            Dmg,
            MaxMoveSpeed,
            BulletSpeed,
            MaxDashes,
            DashCooldown,
            fireCooldown,
            Size,
            
        }

        public Level level;

        public List<Bullet> playerBullets;

        public Player(int x,int y) : base(new Rectangle(x,y,size*32,size*32), playerTxt)
        {
            stats = new int[8];
            stats[(int)stat.Hp] = 50;
            stats[(int)stat.MaxMoveSpeed] = 5;
            stats[(int)stat.BulletSpeed] = 10;
            playerBullets = new List<Bullet>();
            actives = new ActiveItem[2];
            passiveItems = new List<PassiveItem>();
            rotation = 0f;
            spriteOrigin = new Vector2(playerTxt.Width / 2, playerTxt.Height / 2);
            //intitialize rest of previous variables
        }

        /// <summary>
        /// Functions - Moving; Update Shooting
        /// </summary>
        public override void update()
        {
            move();
            CalcMouse();
            foreach (Bullet b in playerBullets)
            {
                b.update();
            }

            //check to see if description for room item should be displayed
            RectangleSync();

        
        }

        public void CalcMouse()
        {

            if (curMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
            {
                FireNormalBullet(curMouse.X, curMouse.Y, eRect.Center.X, eRect.Center.Y);
            }
            rotation = -(float)Math.Atan2(eRect.Center.Y - curMouse.X, eRect.Center.X - curMouse.Y);
            if (curMouse.RightButton == ButtonState.Pressed && oldMouse.RightButton == ButtonState.Released)
            {
                actives[0].ActiveUse();
            }
            foreach (ActiveItem a in actives)
            {
                if (a != null && !a.active && a.chargeType == ActiveItem.TypeCooldown.TimedCooldown && a.curCharge < a.MaxCharge)
                {
                    a.curCharge++;
                }
            }
        }

        public new void move()
        {
            if (Entity.curKey.IsKeyDown(Keys.A))
                X -= stats[(int)stat.MaxMoveSpeed];
            if (Entity.curKey.IsKeyDown(Keys.D))
                X += stats[(int)stat.MaxMoveSpeed];
            if (Entity.curKey.IsKeyDown(Keys.W))
                Y -= stats[(int)stat.MaxMoveSpeed];
            if (Entity.curKey.IsKeyDown(Keys.S))
                Y += stats[(int)stat.MaxMoveSpeed];
            RectangleSync();
        }

        public void FireNormalBullet(float MouseX, float MouseY, double originX, double originY)
        {

            double hyp = Math.Sqrt(Math.Pow(originX - MouseX, 2) + Math.Pow(originY - MouseY, 2));
            double numUpdates = hyp / stats[(int)stat.BulletSpeed];
            Vector2 vel = new Vector2((float)((MouseX - originX) / numUpdates), (float)((MouseY - originY) / numUpdates));

            foreach (PassiveItem p in passiveItems)
            {
                p.onFireUse();
            }
            playerBullets.Add(new Bullet(new Rectangle((int)originX, (int)originY, 5, 5), vel, rotation,true,stats[(int)stat.Dmg]));
        }

        internal void roomClear()
        {
            foreach (ActiveItem a in actives)
            {
                if (a.chargeType == ActiveItem.TypeCooldown.RoomCooldown && a.curCharge < a.MaxCharge)
                    a.curCharge++;
            }
            foreach (PassiveItem p in passiveItems)
            {
                p.onRoomClear();
            }
        }

        public override void draw(GameTime t,SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(eTxt, eRect,null, Color.White,rotation,spriteOrigin,SpriteEffects.None,0);
            foreach (Bullet b in playerBullets)
                b.draw(t, spriteBatch);

        }
    }
}
