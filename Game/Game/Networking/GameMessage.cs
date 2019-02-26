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
using MyGame.Engine.Networking;

namespace MyGame.Networking
{
    public abstract class GameMessage
    {
        //TODO: this buffer might need to be thread safe
        private const int BUFF_MAX_SIZE = 1024;
        private const int TYPE_POSITION = 0;
        private const int TIME_STAMP_POSITION = 4;
        public const int LENGTH_POSITION = 12;
        public const int HEADER_SIZE = 16;

        private static Type[] messageTypeArray;

        private readonly byte[] buff = new byte[BUFF_MAX_SIZE];
        private int readerSpot;

        protected internal GameMessage(GameTime currentGameTime)
        {
            GameMessage.Initialize();
            int typeID = 0;
            for (int i = 0; i < messageTypeArray.Length; i++)
            {
                if (messageTypeArray[i] == this.GetType())
                {
                    typeID = i;
                }
            }

            this.Type = typeID;
            this.TimeStamp = currentGameTime.TotalGameTime.Ticks;
            this.Size = HEADER_SIZE;
        }

        protected internal GameMessage(byte[] buffer)
        {
            GameMessage.Initialize();
            this.buff = buffer;

            this.ResetReader();
            this.AssertExactBufferSize();
            this.AssertMessageType();
        }

        protected internal GameMessage(UdpClient udpClient)
        {
            GameMessage.Initialize();
            IPEndPoint ep = (IPEndPoint)udpClient.Client.RemoteEndPoint;
            this.buff = udpClient.Receive(ref ep);

            this.ResetReader();
            this.AssertExactBufferSize();
            this.AssertMessageType();
        }

        protected internal GameMessage(NetworkStream networkStream)
        {
            GameMessage.Initialize();
            byte[] headerBuffer = new byte[GameMessage.HEADER_SIZE];
            networkStream.Read(headerBuffer, 0, GameMessage.HEADER_SIZE);
            int size = BitConverter.ToInt32(headerBuffer, GameMessage.LENGTH_POSITION);
            int bytesLeft = size - HEADER_SIZE;

            this.buff = new byte[size];
            headerBuffer.CopyTo(this.buff, 0);

            networkStream.Read(this.buff, GameMessage.HEADER_SIZE, bytesLeft);

            this.ResetReader();
            this.AssertExactBufferSize();
            this.AssertMessageType();
        }

        private int Type
        {
            get
            {
                return BitConverter.ToInt32(buff, TYPE_POSITION);
            }

            set
            {
                BitConverter.GetBytes(value).CopyTo(buff, TYPE_POSITION);
            }
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

        public int Size
        {
            get
            {
                return BitConverter.ToInt32(buff, LENGTH_POSITION);
            }

            private set
            {
                BitConverter.GetBytes(value).CopyTo(buff, LENGTH_POSITION);
            }
        }

        protected internal byte[] MessageBuffer
        {
            get
            {
                return buff;
            }
        }

        internal static void Initialize()
        {
            if (messageTypeArray == null)
            {
                IEnumerable<Type> types =
                    Assembly.GetAssembly(typeof(GameMessage))
                        .GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(GameMessage)));
                types = types.OrderBy(t => t.Name);
                messageTypeArray = types.ToArray();
            }
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

        public void AssertMessageEnd()
        {
            if (readerSpot != this.Size)
            {
                throw new Exception("Message end not reached");
            }
        }

        public void AssertMessageType()
        {
            if (this.GetType() != messageTypeArray[this.Type])
            {
                throw new Exception("Incorrect message type");
            }
        }

        public void AssertExactBufferSize()
        {
            if (this.buff.Length != this.Size)
            {
                throw new Exception("Incorrect message length");
            }
        }

        public abstract void Send(UdpTcpPair pair);
    }
}