#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using MyGame.IO;
using MyGame.GameStateObjects;
#endregion

namespace MyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            Window.IsBorderless = true;
            graphics.IsFullScreen = true;
            IsMouseVisible = true;
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
            base.Initialize();
            //Window.IsBorderless = true;
            //Window.AllowUserResizing = true;
            
            
            inputManager = new InputManager();
            gameState = new GameStateObjects.GameState(inputManager, new Vector2(2000));
            camera.SetGameState(gameState);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D line = Content.Load<Texture2D>("line");
            camera = new Camera(new Vector2(0), 1, 0, graphics);
            myGraphicsObject = new DrawingUtils.MyGraphicsClass(graphics, spriteBatch, camera);
            DrawingUtils.MyGraphicsClass.LoadContent(Content);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            base.Update(gameTime);
            inputManager.Update();
            camera.Update();
            gameState.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            myGraphicsObject.BeginWorld();
            myGraphicsObject.DrawLine(new Vector2(50), new Vector2(300), Color.Black, 1);
            myGraphicsObject.DrawCircle(new Vector2(50, 100), 46, Color.Purple, .9f);
            myGraphicsObject.DrawSolidRectangle(new Vector2(300), new Vector2(20, 40), new Vector2(10, 20), .3f, Color.Red, .8f);
            
            gameState.Draw(gameTime, myGraphicsObject);
            myGraphicsObject.EndWorld();
            base.Draw(gameTime);
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DrawingUtils.MyGraphicsClass myGraphicsObject;
        Camera camera;
        InputManager inputManager;
        
        GameStateObjects.GameState gameState;
    }
}
