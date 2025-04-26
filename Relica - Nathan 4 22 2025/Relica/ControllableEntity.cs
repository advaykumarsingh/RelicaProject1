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
    public class ControllableEntity: Entity
    {
        public ControllableEntity (Rectangle eRect,Texture2D eTxt) : base(eRect,eTxt)
        {
        }

        public override void update()
        {
        }
        public void move()
        {
            if (Entity.curKey.IsKeyDown(Keys.A))
                X -= 1;
            if (Entity.curKey.IsKeyDown(Keys.D))
                X += 1;
            if (Entity.curKey.IsKeyDown(Keys.W))
                Y -= 1;
            if (Entity.curKey.IsKeyDown(Keys.S))
                Y += 1;
            RectangleSync();
        }
        public override void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.draw(gameTime, spriteBatch);
        }
    }
}
