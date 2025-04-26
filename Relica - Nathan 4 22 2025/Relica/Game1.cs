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
        LevelBuilder lb;
        enemyController mastermind;
        SpriteFont font1;
        Player p;
        PassiveItem pI;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            
            
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
            mastermind = new enemyController(Content);
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            lb = new LevelBuilder(GraphicsDevice, Content, mastermind);
            Player.playerTxt = this.Content.Load<Texture2D>("PlayerSprite");
            p = new Player(Level.xLevelOffset + 400, Level.yLevelOffset + 400);
            Player.player = p;
            Level.enemyController = mastermind;
            Bullet.bulletSheet = this.Content.Load<Texture2D>("Projectiles");
            Treasure_room t = new Treasure_room(this.Content.RootDirectory + "\\Levels\\" + "F1_JUNGLE" + "\\" + "INTROLEVEL" + ".lvl");
            p.level = t;
            Item.room = t;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font1 = this.Content.Load<SpriteFont>("SpriteFont1");
            PassiveItem.passiveSheet = this.Content.Load<Texture2D>("Projectiles");
            Item.room.itemsInRoom.Add(GameItemManager.createPassiveItem(Passives.Gas_Powered_Thrusters.id, (int)p.X, (int)p.Y));

            // TODO: use this.Content to load your game content here
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
            Entity.newUpdate();
            //lb.update();
            Item.room.update();
            Entity.time = gameTime;
            Entity.oldUpdate();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            //lb.draw(gameTime, spriteBatch, font1);
            //spriteBatch.DrawString(font1, "" + graphics.PreferredBackBufferWidth + " " + graphics.PreferredBackBufferHeight, new Vector2(300, 300), Color.White);
            //spriteBatch.DrawString(font1, "" + lb.selTile, new Vector2(Entity.oldMouse.X, Entity.oldMouse.Y + 10), Color.White);
            //spriteBatch.DrawString(font1, "" + Entity.oldMouse.X + " " + Entity.oldMouse.Y, new Vector2(Entity.oldMouse.X, Entity.oldMouse.Y - 20), Color.White);
            Item.room.draw(gameTime, spriteBatch);
            if (pI != null)
                pI.draw(gameTime, spriteBatch);
            //spriteBatch.DrawString(font1, gameTime.ElapsedGameTime.ToString() + "", new Vector2((float)p.X, (float)p.Y + 10f), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
