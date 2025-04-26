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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player p1;
        Cursor cursor;
        Texture2D box, dashed;
        public static Texture2D BOX;
        int timer;
        HUD hud;
        Texture2D FrameText;
        Texture2D HUDbgText;
        Texture2D blank;
        Texture2D wall;

        public static Tile[,] GRID;
        public static int TILE_SIZE = 32;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GRID = new Tile[25, 25];
            for (int r = 0; r < GRID.GetLength(0); r++)
            {
                for (int c = 0; c < GRID.GetLength(1); c++)
                {
                    GRID[r, c] = new Tile(5);
                }
            }

            GRID[7, 11] = new Tile(24);
            GRID[7, 12] = new Tile(24);
            GRID[7, 13] = new Tile(24);
            GRID[7, 14] = new Tile(24);
            GRID[7, 15] = new Tile(24);
            GRID[7, 16] = new Tile(24);
            GRID[8, 16] = new Tile(24);
            GRID[9, 16] = new Tile(24);
            GRID[13, 16] = new Tile(18);
            GRID[14, 16] = new Tile(18);
            GRID[15, 16] = new Tile(18);
            GRID[14, 17] = new Tile(18);
            GRID[15, 17] = new Tile(18);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            BOX = this.Content.Load<Texture2D>("box");
            wall = this.Content.Load<Texture2D>("Wall");
            box = this.Content.Load<Texture2D>("box");
            dashed = this.Content.Load<Texture2D>("dashed");
            p1 = new Player(new Rectangle(580, 260, 56, 40), this.Content.Load<Texture2D>("PlayerSprite"));
            p1.BulletText = this.Content.Load<Texture2D>("Projectiles");
            cursor = new Cursor(new Rectangle(0, 0, TILE_SIZE, TILE_SIZE), this.Content.Load<Texture2D>("Cursor sprite"));
            blank = this.Content.Load<Texture2D>("box");

            hud = new HUD(graphics, GraphicsDevice, spriteBatch, p1);
            hud.AddTexture("frame", this.Content.Load<Texture2D>("Frame"));
            hud.AddTexture("HUD", this.Content.Load<Texture2D>("HUD"));
            hud.AddTexture("blank", blank);
            hud.AddTexture("dash", this.Content.Load<Texture2D>("DashForHUD"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            timer++;
            cursor.Update(timer);
            p1.Update(timer);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            for (int r = 0; r < GRID.GetLength(0); r++)
            {
                for (int c = 0; c < GRID.GetLength(1); c++)
                {
                    if (!GRID[r, c].InTopLayer)
                    {
                        if (GRID[r, c].Collidable)
                            spriteBatch.Draw(wall, new Rectangle(560 + TILE_SIZE * c, 240 + TILE_SIZE * r, TILE_SIZE, TILE_SIZE), Color.White);
                        else
                            spriteBatch.Draw(dashed, new Rectangle(560 + TILE_SIZE * c, 240 + TILE_SIZE * r, TILE_SIZE, TILE_SIZE), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(dashed, new Rectangle(560 + TILE_SIZE * c, 240 + TILE_SIZE * r, TILE_SIZE, TILE_SIZE), Color.Green);
                    }
                }
            }

            p1.Draw(spriteBatch);
            p1.DrawBullets(spriteBatch);
            spriteBatch.Draw(cursor.Texture, cursor.Rectangle, cursor.SourceRect, Color.White);

            for (int r = 0; r < GRID.GetLength(0); r++)
            {
                for (int c = 0; c < GRID.GetLength(1); c++)
                {
                    if (GRID[r, c].InTopLayer)
                    {
                        float distToPlayer = (float)Math.Sqrt(Math.Pow(p1.X - (560 + TILE_SIZE * c + TILE_SIZE / 2), 2) + Math.Pow(p1.Y - (240 + TILE_SIZE * r + TILE_SIZE / 2), 2));
                        float fullyTransparentDist = TILE_SIZE * 2;
                        float beginTransparencyDist = TILE_SIZE * 3;
                        spriteBatch.Draw(dashed, new Rectangle(560 + TILE_SIZE * c, 240 + TILE_SIZE * r, TILE_SIZE, TILE_SIZE), 
                            Color.Green * ((distToPlayer - fullyTransparentDist) / beginTransparencyDist));
                    }
                }
            }

            hud.UpdateHUD();
            //spriteBatch.Draw(cursor.Texture, cursor.Rectangle, cursor.SourceRect, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public static Boolean InBounds(int r, int c)
        {
            return r >= 0 && r < GRID.GetLength(0) && c >= 0 && c < GRID.GetLength(1);
        }
    }
}
