using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using MyGame;

namespace MyGame.Networking
{
    public abstract class GameMessage
    {
        //TODO: this buffer might need to be thread safe
        private const int BUFF_MAX_SIZE = 1024; // The maximum size for a buffer.
        private const int TIME_STAMP_POSITION = 4;
        public const int LENGTH_POSITION = 12;
        public const int HEADER_SIZE = 16;
        private static bool isInitialized = false;
        private static Type[] messageTypeArray;
        private static Dictionary<int, ConstructorInfo> messageConstructors = new Dictionary<int, ConstructorInfo>();

        private readonly byte[] buff = new byte[BUFF_MAX_SIZE];
        private int readerSpot;

        protected GameMessage(GameTime currentGameTime)
        {
            if(!isInitialized)
            {
                GameMessage.Initialize();
            }

            int typeID = 0;
            for (int i = 0; i < messageTypeArray.Length; i++)
            {
                if (messageTypeArray[i] == this.GetType())
                {
                    typeID = i;
                }
            }

            BitConverter.GetBytes(typeID).CopyTo(buff, 0);                  // Bytes 0-3:  The type of message this is.
            this.TimeStamp = currentGameTime.TotalGameTime.Ticks;           // Bytes 4-7:  The timestamp of the message
            this.Size = HEADER_SIZE;                                        // Bytes 7-11:  The length of the message in bytes.
        }

        public long TimeStamp
        {
            get
            {
                return BitConverter.ToInt64(buff, TIME_STAMP_POSITION);
            }

            private set
            {
                BitConverter.GetBytes(value).CopyTo(buff, TIME_STAMP_POSITION);
            }
        }

        protected GameMessage(byte[] b, int length)
        {
            if (!isInitialized)
            {
                GameMessage.Initialize();
            }

            b.CopyTo(buff, 0);

            if (length != this.Size)
            {
                throw new Exception("Incorrect message length");
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
            if (newSize > BUFF_MAX_SIZE)
            {
                throw new Exception("Buffer exceded maximum size");
            }

            Buffer.BlockCopy(b, 0, buff, this.Size, b.Length);
            this.Size = newSize;
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
        private static GameMessage ConstructMessage(byte[] b, int length)
        {
            if (!isInitialized)
            {
                GameMessage.Initialize();
            }

            if (length < 4)
            {
                throw new Exception("Message is to short");
            }

            ConstructorInfo constructor = messageConstructors[BitConverter.ToInt32(b, 0)];

            var constuctorParams = new object[2];
            constuctorParams[0] = b;
            constuctorParams[1] = length;

            var message = (GameMessage) constructor.Invoke(constuctorParams);
            return message;
        }

        public static GameMessage ConstructMessage(NetworkStream networkStream)
        {
            var readBuff = new byte[GameMessage.BUFF_MAX_SIZE];
            int amountRead = networkStream.Read(readBuff, 0, GameMessage.HEADER_SIZE);
            int bytesLeft = BitConverter.ToInt32(readBuff, GameMessage.LENGTH_POSITION) - HEADER_SIZE;
            amountRead = amountRead + networkStream.Read(readBuff, amountRead, bytesLeft);
            return GameMessage.ConstructMessage(readBuff, amountRead);
        }

        public static GameMessage ConstructMessage(UdpClient udpClient)
        {
            IPEndPoint ep = (IPEndPoint)udpClient.Client.RemoteEndPoint;
            byte[] readBuff = udpClient.Receive(ref ep);
            return GameMessage.ConstructMessage(readBuff, readBuff.Length);
        }

        private int Size
        {
            get
            {
                return BitConverter.ToInt32(buff, LENGTH_POSITION);
            }

            set
            {
                BitConverter.GetBytes(value).CopyTo(buff, LENGTH_POSITION);
            }
        }

        private static void Initialize()
        {
            isInitialized = true;
            IEnumerable<Type> types =
                Assembly.GetAssembly(typeof (GameMessage))
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof (GameMessage)));
            types = types.OrderBy(t => t.Name);
            messageTypeArray = types.ToArray();

            for(int i = 0; i < messageTypeArray.Length; i++)
            {
                var constructorParams = new Type[2];
                constructorParams[0] = typeof(byte[]);
                constructorParams[1] = typeof(int);

                ConstructorInfo constructor = messageTypeArray[i].GetConstructor(constructorParams);
                messageConstructors[i] = constructor;
            }
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

        public void Send(NetworkStream clientStream)
        {
            clientStream.Write(buff, 0, this.Size);
            clientStream.Flush();
        }

        public void Send(UdpClient client)
        {
            client.Send(buff, this.Size);
        }
    }
}
