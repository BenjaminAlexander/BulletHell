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

namespace MyGame.Networking
{
    public abstract class TCPMessage
    {
        //TODO: this buffer might need to be thread safe
        private const int BuffMaxSize = 1024;
        private const int TimeStampLocation = 4;
        private const int ClientIDLocation = 12;
        private const int LengthPosition = 16;
        private const int HeaderSize = 20;
        private static Type[] gameObjectTypeArray;
        private readonly byte[] buff = new byte[BuffMaxSize];
        private int size;
        private int readerSpot;
        private int clientID;
 
        protected TCPMessage()
        {
            Append(GetTypeID());                        // Bytes 0-3:  The type of message this is.
            Append(Game1.CurrentGameTime.TotalGameTime.Ticks);    // Bytes 4-7:  The timestamp of the message
            Append(0);    // Bytes 8-11:  ID of the client
            Append(0);                                  // Bytes 12-15:  The length of the message in bytes.

        }

        public long TimeStamp()
        {
            return BitConverter.ToInt64(buff, TimeStampLocation);
        }

        protected TCPMessage(byte[] b, int length)
        {
            if (length != BitConverter.ToInt32(b, LengthPosition) + HeaderSize)
            {
                throw new Exception("Incorrect message length");
            }
            b.CopyTo(buff, 0);
            clientID = BitConverter.ToInt32(buff, ClientIDLocation);
            size = BitConverter.ToInt32(buff, LengthPosition) + HeaderSize;
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
            Buffer.BlockCopy(b, 0, buff, GetSize(), b.Length);
            SetSize(GetSize() + b.Length);
        }

        public void Append(Vector2 v)
        {
            Append(v.X);
            Append(v.Y);
        }

        public void Append<T>(List<GameObjectReference<T>> list) where T : GameObject
        {
            Append(list.Count);
            foreach (GameObjectReference<T> obj in list)
            {
                Append<T>(obj);
            }
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

        public void Append<T>(GameObjectReference<T> obj) where T : GameObject
        {
            Append(obj.ID);
        }

        public void AssertMessageEnd()
        {
            if (readerSpot != size)
            {
                throw new Exception("Message end not reached");
            }
        }

        private static TCPMessage ConstructMessage(byte[] b, int length)
        {
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

            var message = (TCPMessage) constructor.Invoke(constuctorParams);
            return message;
        }

        private int GetSize()
        {
            return size;
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
            if (!t.IsSubclassOf(typeof (TCPMessage)))
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

        public static void Initialize()
        {
            IEnumerable<Type> types =
                Assembly.GetAssembly(typeof (TCPMessage))
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof (TCPMessage)));
            types = types.OrderBy(t => t.Name);
            gameObjectTypeArray = types.ToArray();
        }

        public void PrintBuff()
        {
            foreach (byte b in buff)
            {
                Console.Write(b + " ");
            }
            Console.WriteLine();
        }

        public Boolean ReadBoolean()
        {
            return ReadInt() != 0;
        }

        public float ReadFloat()
        {
            if (readerSpot + 4 > size)
            {
                throw new Exception("Read past end of buffer");
            }
            float rtn = BitConverter.ToSingle(buff, readerSpot);
            readerSpot = readerSpot + 4;
            return rtn;
        }

        public GameObjectReference<T> ReadGameObjectReference<T>() where T : GameObject
        {
            return new GameObjectReference<T>(ReadInt());
        }

        public List<GameObjectReference<T>> ReadGameObjectReferenceList<T>() where T : GameObject
        {
            var rtn = new List<GameObjectReference<T>>();
            int count = ReadInt();
            for (int i = 0; i < count; i++)
            {
                rtn.Add(ReadGameObjectReference<T>());
            }
            return rtn;
        }

        private delegate T ConvertFunction<out T>(byte[] buffer, int readLocation, int readAmount);

        private T Read<T>(ConvertFunction<T> convert, int readAmount)
        {
            if (readerSpot + readAmount > this.size)
            {
                throw new Exception("Read past end of buffer");
            }
            T ret = convert(buff, readerSpot, readAmount);
            readerSpot = readerSpot + readAmount;
            return ret;
        }

        public int ReadInt()
        {
            ConvertFunction<int> del = (buffer, readLocation, readAmount) => BitConverter.ToInt32(buffer, readLocation);
            return Read(del, 4);
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

        public static TCPMessage ReadTCPMessage(Client client)
        {
            var readBuff = new byte[BuffMaxSize];
            int amountRead = client.ReadTCP(readBuff, 0, HeaderSize);
            int bytesLeft = BitConverter.ToInt32(readBuff, LengthPosition);
            amountRead = amountRead + client.ReadTCP(readBuff, amountRead, bytesLeft);
            return ConstructMessage(readBuff, amountRead);
        }

        public static TCPMessage ReadUDPMessage(Client client)
        {
            byte[] readBuff = client.ReadUDP();
            return ConstructMessage(readBuff, readBuff.Length);
        }

        public Vector2 ReadVector2()
        {
            float x = ReadFloat();
            float y = ReadFloat();
            return new Vector2(x, y);
        }

        public void ResetReader()
        {
            readerSpot = HeaderSize;
        }

        public Boolean SendTCP(NetworkStream clientStream, Mutex writeMutex)
        {
            writeMutex.WaitOne();
            try
            {
                clientStream.Write(buff, 0, size);
                clientStream.Flush();
            }
            catch
            {
                return false;
            }
            writeMutex.ReleaseMutex();
            return true;
        }

        public Boolean SendUDP(UdpClient client, TcpClient tcpClient, Mutex writeMutex)
        {
            writeMutex.WaitOne();
            try
            {
                client.Send(buff, size);
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

        private void SetSize(int size)
        {
            if (size < 0)
            {
                throw new Exception("message too short");
            }
            else if (size > BuffMaxSize)
            {
                throw new Exception("Buffer exceded maximum size");
            }
            this.size = size;
            BitConverter.GetBytes(Math.Max(size - HeaderSize, 0)).CopyTo(buff, LengthPosition);
        }
    }
}
