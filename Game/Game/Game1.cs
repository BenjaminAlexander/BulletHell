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
        private static Boolean isServer;
        private static int playerID;
        public static ThreadSafeQueue<GameMessage> outgoingQueue;
        public static ThreadSafeQueue<GameMessage> incomingQueue;
        private static GameTime currentGameTime = new GameTime();
        private GraphicsDeviceManager graphics;
        private MyGraphicsClass myGraphicsObject;
        private Camera camera;
        private InputManager inputManager;
        private ServerLogic serverLogic = null;
        private ClientLogic clientLogic = null;
        private BackGround backGround;
        private Vector2 worldSize;
        public static int PlayerID
        {
            get { return playerID; }
        }
        public static Boolean IsServer
        {
            get { return isServer; }
        }
        public static void AsserIsServer()
        {
            if (!isServer)
            {
                throw new Exception("AsserIsServer Failed");
            }
        }
        public static void AssertIsNotServer()
        {
            if (isServer)
            {
                throw new Exception("AssertIsNotServer Failed");
            }
        }

        public static GameTime CurrentGameTime
        {
            get { return currentGameTime; }
            private set { currentGameTime = value; }
        }

        public Game1(ThreadSafeQueue<GameMessage> outgoingQue, ThreadSafeQueue<GameMessage> inCommingQue, int playerID, Vector2 worldSize)
            : base()
        {
            Game1.playerID = playerID;
            Game1.isServer = playerID == 0;
            Game1.outgoingQueue = outgoingQue;
            Game1.incomingQueue = inCommingQue;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            /*
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;*/
            graphics.IsFullScreen = false;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            this.Window.AllowUserResizing = true;
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

            inputManager = new InputManager(graphics);

            if (isServer)
            {
                serverLogic = new ServerLogic(worldSize, inputManager);
            }
            else
            {
                clientLogic = new ClientLogic(inputManager, camera);
            }
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
            Game1.CurrentGameTime = gameTime;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);

            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;


            Queue<GameMessage> messageQueue = incomingQueue.DequeueAll();
            while (messageQueue.Count > 0)
            {
                GameMessage m = messageQueue.Dequeue();
                if (m is GameUpdate)
                {
                    ((GameUpdate)m).Apply(this);
                }
            }
            
            if (this.IsActive)
            {
                inputManager.Update();
            }

            if (isServer)
            {
                serverLogic.Update(secondsElapsed);
            }
            else
            {
                clientLogic.Update(secondsElapsed);
            }

            StaticGameObjectCollection.Collection.Update(gameTime);
            camera.Update(secondsElapsed);
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
            StaticGameObjectCollection.Collection.Draw(gameTime, myGraphicsObject);
            myGraphicsObject.Begin(Matrix.Identity);

            Ship focus;
            if (Game1.IsServer)
            {
                focus = PlayerControllers.StaticControllerFocus.GetFocus(1);
            }
            else
            {
                focus = PlayerControllers.StaticControllerFocus.GetFocus(Game1.PlayerID);
            }
            if (focus != null)
            {
                myGraphicsObject.DrawDebugFont("Health: "+focus.Health.ToString(), new Vector2(0), 1);
                myGraphicsObject.DrawDebugFont("Kills: " + focus.Kills().ToString(), new Vector2(0, 30), 1);
            }
            myGraphicsObject.End();
        }
    }
}
