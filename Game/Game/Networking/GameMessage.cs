using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using MyGame;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.DataStuctures;
namespace MyGame.Networking
{
    public abstract class GameMessage
    {
        //TODO: this buffer might need to be thread safe
        private const int BUFF_MAX_SIZE = 1024; // The maximum size for a buffer.
        private const int TimeStampLocation = 4;
        private const int ClientIDLocation = 12;
        public const int LENGTH_POSITION = 16;
        public const int HEADER_SIZE = 20;
        private static bool isInitialized = false;
        private static Type[] gameObjectTypeArray;

        private readonly byte[] buff = new byte[BUFF_MAX_SIZE];
        private int readerSpot;
        private int clientID;

        protected GameMessage(GameTime currentGameTime)
        {
            if(!isInitialized)
            {
                GameMessage.Initialize();
            }
            Append(GetTypeID());                        // Bytes 0-3:  The type of message this is.
            Append(currentGameTime.TotalGameTime.Ticks);    // Bytes 4-7:  The timestamp of the message
            Append(0);    // Bytes 8-11:  ID of the client
            //TODO: Append automatical fills in the size.  Why do we append 0 as the size?
            Append(0);                                  // Bytes 12-15:  The length of the message in bytes.

        }

        public long TimeStamp()
        {
            return BitConverter.ToInt64(buff, TimeStampLocation);
        }

        protected GameMessage(byte[] b, int length)
        {
            if (!isInitialized)
            {
                GameMessage.Initialize();
            }

            if (length != BitConverter.ToInt32(b, LENGTH_POSITION))
            {
                throw new Exception("Incorrect message length");
            }
            b.CopyTo(buff, 0);
            clientID = BitConverter.ToInt32(buff, ClientIDLocation);
        }

        public int ClientID
        {
            get { return clientID; }
            set
            {
                clientID = value;
                Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buff, ClientIDLocation, 4);
            }
        }

        public void Append(int i)
        {
            Append(BitConverter.GetBytes(i));
        }

        public void Append(long i)
        {
            Append(BitConverter.GetBytes(i));
        }

        public void Append(float i)
        {
            Append(BitConverter.GetBytes(i));
        }

        public void Append(string s)
        {
            Append(s.Length);
            Append(Encoding.Unicode.GetBytes(s.ToCharArray()));
        }

        // Every other append method should boil down to calling one.
        private void Append(byte[] b)
        {
            int newSize = this.Size + b.Length;

            Buffer.BlockCopy(b, 0, buff, this.Size, b.Length);
            

            if (newSize < 0)
            {
                throw new Exception("message too short");
            }
            else if (newSize > BUFF_MAX_SIZE)
            {
                throw new Exception("Buffer exceded maximum size");
            }
            BitConverter.GetBytes(newSize).CopyTo(buff, LENGTH_POSITION);
        }

        public void Append(Vector2 v)
        {
            Append(v.X);
            Append(v.Y);
        }

        public void Append(Boolean b)
        {
            if (b)
            {
                Append(1);
            }
            else
            {
                Append(0);
            }
        }

        public void AssertMessageEnd()
        {
            if (readerSpot != this.Size)
            {
                throw new Exception("Message end not reached");
            }
        }

        //TODO:  Do we really need to use introspection like this?
        //Maybe define a tighter protocol (anticipated the right message), 
        //then use gameObjects one the game gets rolling
        public static GameMessage ConstructMessage(byte[] b, int length)
        {
            if (!isInitialized)
            {
                GameMessage.Initialize();
            }

            if (length < 4)
            {
                throw new Exception("Message is to short");
            }
            var constuctorParamsTypes = new Type[2];
            constuctorParamsTypes[0] = typeof (byte[]);
            constuctorParamsTypes[1] = typeof (int);

            ConstructorInfo constructor =
                GetType(BitConverter.ToInt32(b, 0)).GetConstructor(constuctorParamsTypes);

            var constuctorParams = new object[2];
            constuctorParams[0] = b;
            constuctorParams[1] = length;

            var message = (GameMessage) constructor.Invoke(constuctorParams);
            return message;
        }

        public static GameMessage ConstructMessage(NetworkStream networkStream)
        {
            var readBuff = new byte[GameMessage.BUFF_MAX_SIZE];
            int amountRead = GameMessage.ReadTCP(networkStream, readBuff, 0, GameMessage.HEADER_SIZE);
            int bytesLeft = BitConverter.ToInt32(readBuff, GameMessage.LENGTH_POSITION) - HEADER_SIZE;
            amountRead = amountRead + GameMessage.ReadTCP(networkStream, readBuff, amountRead, bytesLeft);
            return GameMessage.ConstructMessage(readBuff, amountRead);
        }

        private int Size
        {
            get
            {
                return BitConverter.ToInt32(buff, LENGTH_POSITION);
            }
        }

        protected static Type GetType(int id)
        {
            try
            {
                return gameObjectTypeArray[id];
            }
            catch
            {
                Console.WriteLine(id);
                throw new Exception("bad ID");
            }
        }

        protected static int GetTypeID(Type t)
        {
            if (!t.IsSubclassOf(typeof (GameMessage)))
            {
                throw new Exception("Not a type of TCPMessage");
            }

            for (int i = 0; i < gameObjectTypeArray.Length; i++)
            {
                if (gameObjectTypeArray[i] == t)
                {
                    return i;
                }
            }
            throw new Exception("Unknown type of TCPMessage");
        }

        protected int GetTypeID()
        {
            return GetTypeID(GetType());
        }

        private static void Initialize()
        {
            isInitialized = true;
            IEnumerable<Type> types =
                Assembly.GetAssembly(typeof (GameMessage))
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof (GameMessage)));
            types = types.OrderBy(t => t.Name);
            gameObjectTypeArray = types.ToArray();
        }

        public Boolean ReadBoolean()
        {
            return ReadInt() != 0;
        }

        public float ReadFloat()
        {
            if (readerSpot + 4 > this.Size)
            {
                throw new Exception("Read past end of buffer");
            }
            float rtn = BitConverter.ToSingle(buff, readerSpot);
            readerSpot = readerSpot + 4;
            return rtn;
        }

        public int ReadInt()
        {
            if (readerSpot + 4 > this.Size)
            {
                throw new Exception("Read past end of buffer");
            }
            int ret = BitConverter.ToInt32(buff, readerSpot);
            readerSpot = readerSpot + 4;
            return ret;
        }

        public string ReadString()
        {
            int length = ReadInt();

            var chars = new char[length];
            Buffer.BlockCopy(buff, readerSpot, chars, 0, length*sizeof (char));
            var str = new string(chars);

            readerSpot = readerSpot + length*sizeof (char);

            if (readerSpot > length)
            {
                throw new Exception("Read past end of buffer");
            }

            return str;
        }

        public Vector2 ReadVector2()
        {
            float x = ReadFloat();
            float y = ReadFloat();
            return new Vector2(x, y);
        }

        public void ResetReader()
        {
            readerSpot = HEADER_SIZE;
        }

        public Boolean SendTCP(NetworkStream clientStream, Mutex writeMutex)
        {
            writeMutex.WaitOne();
            try
            {
                clientStream.Write(buff, 0, this.Size);
                clientStream.Flush();
            }
            catch
            {
                return false;
            }
            writeMutex.ReleaseMutex();
            return true;
        }

        public Boolean SendUDP(UdpClient client, Mutex writeMutex)
        {
            writeMutex.WaitOne();
            try
            {
                client.Send(buff, this.Size);
                //clientStream.Write(buff, 0, emptySpot);
                //clientStream.Flush();
            }
            catch (Exception e)
            {
                throw e;
            }
            writeMutex.ReleaseMutex();
            return true;
        }

        public static int ReadTCP(NetworkStream clientStream, byte[] buffer, int offset, int size)
        {
            //blocks until a client sends a message
            int totalBytesRead = 0;

            // This will block until the whole message is read, or until something goes wrong.
            while (totalBytesRead != size)
            {
                if (clientStream.CanRead)
                {
                    totalBytesRead += clientStream.Read(buffer, offset + totalBytesRead, size - totalBytesRead);
                }
                else
                {
                    // If we cannot read, throwing an exception.
                    throw new UdpTcpPair.ClientNotConnectedException();
                }
            }
            return totalBytesRead;
        }
    }
}
