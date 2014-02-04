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

#endregion

namespace MyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private static Boolean isServer;
        public static Boolean IsServer
        {
            get { return isServer; }
        }

        PacketWriter packetWriter = new PacketWriter();
        PacketReader packetReader = new PacketReader();

        public static ThreadSafeQueue<TCPMessage> outgoingQue;
        public static ThreadSafeQueue<TCPMessage> inCommingQue;
        private Boolean input = false;

        private static GameTime currentGameTime = new GameTime();

        public static GameTime GetGameTime()
        {
            return currentGameTime;
        }

        private static void SetGameTime(GameTime time)
        {
            currentGameTime = time;
        }

        public Game1(ThreadSafeQueue<TCPMessage> outgoingQue, ThreadSafeQueue<TCPMessage> inCommingQue, Boolean isServer)
            : base()
        {
            Game1.isServer = isServer;
            Game1.outgoingQue = outgoingQue;
            Game1.inCommingQue = inCommingQue;


            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Window.IsBorderless = false;
            graphics.IsFullScreen = false;
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

            Vector2 worldSize = new Vector2(20000);
            backGround = new BackGround(worldSize);
            StaticGameObjectCollection.Initialize(worldSize);

            inputManager = new InputManager();
            MyGame.PlayerControllers.GunnerController.Initialize(myGraphicsObject, inputManager, camera);
            gameState = new GameStateObjects.GameState(worldSize, camera);
            camera.SetGameState(gameState);

            if (isServer)
            {
                serverLogic = new ServerLogic(worldSize, inputManager);
            }

            

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
            camera = new Camera(new Vector2(0), .6f, 0, graphics);
            myGraphicsObject = new DrawingUtils.MyGraphicsClass(graphics, spriteBatch, camera);
            DrawingUtils.MyGraphicsClass.LoadContent(Content);
            Textures.LoadContent(Content);

            
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
            SetGameTime(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            base.Update(gameTime);
            
            if (!isServer)
            {
                input = inCommingQue.IsEmpty;
                while (!inCommingQue.IsEmpty)
                {
                    StaticGameObjectCollection.Collection.ApplyMessage(inCommingQue.Dequeue());
                }
            }
            inputManager.Update();
            
            if (isServer)
            {
                serverLogic.Update(gameTime);
            }

            gameState.Update(gameTime);
            camera.Update();
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Wheat);
            // TODO: Add your drawing code here
            myGraphicsObject.BeginWorld();
            backGround.Draw(gameTime, myGraphicsObject);
            myGraphicsObject.End(); 
            gameState.Draw(gameTime, myGraphicsObject);
            myGraphicsObject.Begin(Matrix.Identity);
            myGraphicsObject.DrawDebugFont(gameTime.IsRunningSlowly.ToString(), new Vector2(0), 1);
            myGraphicsObject.DrawDebugFont(input.ToString(), new Vector2(0, 30), 1);
            myGraphicsObject.End();

        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DrawingUtils.MyGraphicsClass myGraphicsObject;
        Camera camera;
        InputManager inputManager;
        private ServerLogic serverLogic = null;
        BackGround backGround;
        GameStateObjects.GameState gameState;
    }
}
