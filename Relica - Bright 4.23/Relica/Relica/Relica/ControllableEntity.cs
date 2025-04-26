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
    class ControllableEntity : Entity
    {
        private KeyboardState oldKey;
        private GamePadState oldPad;
        private Rectangle collidingRectangle;
        private float maxSpeed;
        private float maxDashSpeed;
        private float acceleration;
        private float deceleration;
        private Vector2 input;
        private Vector2 velocity;

        private double dashRefill, dashRefillTime;
        private double dashTimeElapsed;
        private int dashesAvaliable;
        private int maxDashes;
        private Boolean isDashing;
        private Boolean xCollision, yCollision;

        public ControllableEntity(Rectangle eRect, Texture2D eTxt) : base(eRect, eTxt, -1, -1, -1, 1)
        {
            oldKey = Keyboard.GetState();
            oldPad = GamePad.GetState(PlayerIndex.One);
            maxSpeed = 5;
            maxDashSpeed = 20;
            maxDashes = 3;
            acceleration = 0.6f;
            deceleration = 0.3f;
            velocity = new Vector2(0, 0);

            collidingRectangle = new Rectangle(0, 0, Game1.TILE_SIZE, Game1.TILE_SIZE);
            OldRectangle = Rectangle;

            isDashing = false;
            dashesAvaliable = 3;
            dashRefill = 0;
            dashRefillTime = 60 * 3;
            dashTimeElapsed = 0;
        }
        public void move(int timer)
        {
            KeyboardState curKey = Keyboard.GetState();
            GamePadState curPad = GamePad.GetState(PlayerIndex.One);
            xCollision = false;
            yCollision = false;

            input = Vector2.Zero;

            dashRefill--;
            if (dashesAvaliable > 0)
                if (curKey.IsKeyDown(Keys.Space) && !oldKey.IsKeyDown(Keys.Space) && !isDashing)
                {
                    isDashing = true;
                    dashRefill = dashRefillTime;
                    dashTimeElapsed = 10;
                    dashesAvaliable--;
                }

            dashRefill--;
            if (dashRefill < 0)
            {
                dashesAvaliable = maxDashes;
            }

            dashTimeElapsed--;
            if (dashTimeElapsed < 0)
                isDashing = false;

            if (!isDashing)
                MoveRegular(curKey);
            else
                MoveDash();

            

            OldRectangle = Rectangle;
            updateStates(curKey, curPad);
            RectangleSync();
        }

        public void MoveRegular(KeyboardState curKey)
        {
            if (curKey.IsKeyDown(Keys.A))
                input.X -= 1;
            if (curKey.IsKeyDown(Keys.D))
                input.X += 1;
            if (curKey.IsKeyDown(Keys.W))
                input.Y -= 1;
            if (curKey.IsKeyDown(Keys.S))
                input.Y += 1;

            if (input != Vector2.Zero)
                input.Normalize();
            velocity += input * acceleration;

            if (velocity.Length() > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }

            if (input.X == 0)
            {
                if (velocity.Length() > deceleration)
                {
                    Vector2 vel = velocity;
                    vel.Normalize();
                    velocity.X -= vel.X * deceleration;
                }
                else
                    velocity.X = 0;
            }
            if (input.Y == 0)
            {
                if (velocity.Length() > deceleration)
                {
                    Vector2 vel = velocity;
                    vel.Normalize();
                    velocity.Y -= vel.Y * deceleration;
                }
                else
                    velocity.Y = 0;
            }

            MoveWithCollision(true);
        }

        public void MoveDash()
        {
            if (velocity == Vector2.Zero)
            {
                velocity.X = -(float)(Mouse.GetState().X - (X + Width / 2));
                velocity.Y = -(float)(Mouse.GetState().Y - (Y + Height / 2));
            }

            velocity.Normalize();
            velocity *= maxDashSpeed;

            if (MoveWithCollision(true))
            {
                Console.WriteLine("hit");
                isDashing = false;
            }
        }

        public void MoveReboundDash()
        {
            if (velocity == Vector2.Zero)
            {
                velocity.X = -(float)(Mouse.GetState().X - (X + Width / 2));
                velocity.Y = -(float)(Mouse.GetState().Y - (Y + Height / 2));
            }

            velocity.Normalize();
            velocity *= maxDashSpeed;

            if (MoveWithCollision(false))
            {
                if (xCollision)
                    velocity.X *= -1;
                else
                    velocity.Y *= -1;

                dashTimeElapsed /= 2;
            }
        }

        public Boolean MoveWithCollision(Boolean resetVelocity)
        {
            Boolean collided;

            X += velocity.X;
            Y += velocity.Y;

            collided = CheckCollision(resetVelocity);

            return collided;
        }

        public Boolean CheckCollision(Boolean resetVelocity)
        {
            Boolean collided = false;
            int tileSize = Game1.TILE_SIZE;
            int leftTile = (int)Math.Floor((float)(Rectangle.Left-560) / tileSize);
            int topTile = (int)Math.Floor((float)(Rectangle.Top-240) / tileSize);

            for (int r = topTile - 1; r <= topTile + 2; r++)
            {
                for (int c = leftTile - 1; c <= leftTile + 2; c++)
                {
                    collidingRectangle = new Rectangle(c * tileSize + 560, r * tileSize + 240, tileSize, tileSize);
                    if (Game1.InBounds(r, c))
                    {
                        if (Game1.GRID[r, c].Collidable)
                        {
                            collided = ResolveCollision(resetVelocity);
                        }
                    }
                    else
                    {
                        collided = ResolveCollision(resetVelocity);
                    }
                }
            }

            return collided;
        }

        public Boolean ResolveCollision(Boolean resetVelocity)
        {
            Boolean collided = false;
            Vector2 depth = GetIntersectionDepth(collidingRectangle);
            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                if (absDepthY < absDepthX)
                {
                    yCollision = true;
                    Y = Rectangle.Y + depth.Y;
                    if (resetVelocity)
                        velocity.Y = 0;
                }
                else if (absDepthX < absDepthY)
                {
                    xCollision = true;
                    X = Rectangle.X + depth.X;
                    if (resetVelocity)
                        velocity.X = 0;
                }

                collided = true;
            }
            return collided;
        }

        public double FindCornerX(Rectangle rect, Vector2 vel)
        {
            return rect.Center.X + (rect.Center.X * Math.Sign(vel.X));
        }

        public double FindCornerY(Rectangle rect, Vector2 vel)
        {
            return rect.Center.Y + (rect.Center.Y * Math.Sign(vel.Y));
        }

        public void updateStates(KeyboardState curKey, GamePadState curPad)
        {
            oldKey = curKey;
            oldPad = curPad;
        }
        public float MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }
        public Boolean IsDashing
        {
            get { return isDashing; }
        }
        public int DashesAvailable
        {
            get { return dashesAvaliable; }
            set { dashesAvaliable = value; }
        }
        public double DashRefill
        {
            get { return dashRefill; }
            set { dashRefill = value; }
        }
        public double DashRefillTime
        {
            get { return dashRefillTime; }
            set { dashRefillTime = value; }
        }
        public int MaxDashes
        {
            get { return maxDashes; }
            set { maxDashes = value; }
        }
    }
}
