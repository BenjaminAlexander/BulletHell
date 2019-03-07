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
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        protected GraphicsDeviceManager graphics;
        private MyGraphicsClass myGraphicsObject;
        private Camera camera;
        private InputManager inputManager;
        private BackGround backGround;
        private GameObjectCollection gameObjectCollection;
        private Vector2 worldSize;

        public InputManager InputManager
        {
            get { return inputManager; }
        }

        public Camera Camera
        {
            get { return camera; }
        }

        public MyGraphicsClass GraphicsObject
        {
            get { return myGraphicsObject; }
        }

        public GameObjectCollection GameObjectCollection
        {
            get { return gameObjectCollection; }
        }

        public Vector2 WorldSize
        {
            get
            {
                return worldSize;
            }
        }

        public Game1()
            : base()
        {
            this.inputManager = new InputManager();

            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.HardwareModeSwitch = false;
            this.graphics.IsFullScreen = false;
            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.PreferredBackBufferHeight = 1080;
            this.Window.IsBorderless = false;
            this.Window.AllowUserResizing = false;
            this.InactiveSleepTime = new TimeSpan(0);
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.graphics.ApplyChanges();
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
            this.camera = new Camera(new Vector2(0), 1f, 0, this.graphics);

            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            myGraphicsObject = new DrawingUtils.MyGraphicsClass(this.graphics, spriteBatch, this.camera);

            backGround = new BackGround(worldSize);
            gameObjectCollection = new GameObjectCollection(worldSize);

            this.graphics.PreferredBackBufferWidth = this.graphics.GraphicsDevice.DisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = this.graphics.GraphicsDevice.DisplayMode.Height;
            this.graphics.ApplyChanges();
        }

        protected void SetWorldSize(Vector2 worldSize)
        {
            this.worldSize = worldSize;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of the content.
        /// </summary>
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
            MyGraphicsClass.LoadContent(Content);
            TextureLoader.LoadContent(Content);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);

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

            this.GameObjectCollection.Draw(gameTime, this.GraphicsObject);
        }
    }
}
