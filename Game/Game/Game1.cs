#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using MyGame.IO;
using MyGame.GameStateObjects;
using MyGame.DrawingUtils;
using MyGame.Networking;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;

#endregion

namespace MyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private GameTime currentGameTime = new GameTime();
        private GraphicsDeviceManager graphics;
        private MyGraphicsClass myGraphicsObject;
        private Camera camera;
        private InputManager inputManager;
        private BackGround backGround;
        private Vector2 worldSize;

        public static void AsserIsServer()
        {
            //TODO: remove this
        }
        public static void AssertIsNotServer()
        {
            //TODO: remove this
        }

        
        public GameTime CurrentGameTime
        {
            get { return currentGameTime; }
            private set { currentGameTime = value; }
        }

        public InputManager InputManager
        {
            get { return inputManager; }
        }

        public Camera Camera
        {
            get { return camera; }
        }

        public Vector2 WorldSize
        {
            get { return worldSize; }
        }

        public MyGraphicsClass GraphicsObject
        {
            get { return myGraphicsObject; }
        }

        public Game1(Vector2 worldSize)
            : base()
        {
            inputManager = new InputManager();

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            this.Window.AllowUserResizing = false;
            this.InactiveSleepTime = new TimeSpan(0);
            this.IsFixedTimeStep = false;
            IsMouseVisible = true;
            graphics.ApplyChanges();

            this.worldSize = worldSize;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            TextureLoader.Initialize(Content);

            backGround = new BackGround(worldSize);
            StaticGameObjectCollection.Initialize(worldSize);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Textures.LoadContent(Content);

            camera = new Camera(new Vector2(0), 1f, 0, graphics);

            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            myGraphicsObject = new DrawingUtils.MyGraphicsClass(graphics, spriteBatch, camera);
            DrawingUtils.MyGraphicsClass.LoadContent(Content);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.currentGameTime = gameTime;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);

            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            
            if (this.IsActive)
            {
                inputManager.Update();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Wheat);

            myGraphicsObject.BeginWorld();
            backGround.Draw(gameTime, myGraphicsObject);
            myGraphicsObject.End();

        }
    }
}
