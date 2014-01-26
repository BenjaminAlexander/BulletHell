using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace NetworkLibrary
{
    public abstract class TCPMessage
    {

        //TODO: this buffer might need to be thread safe
        private const int buffMaxSize = 1024;
        private const int headerSize = 8;
        private byte[] buff = new byte[buffMaxSize];
        private int emptySpot = 0;

        static Type[] gameObjectTypeArray;
        public static void Initialize()
        {
            IEnumerable<Type> types = System.Reflection.Assembly.GetAssembly(typeof(TCPMessage)).GetTypes().Where(t => t.IsSubclassOf(typeof(TCPMessage)));
            types = types.OrderBy(t => t.Name);
            gameObjectTypeArray = types.ToArray();
        }

        protected static int GetTypeID(Type t)
        {
            if (!t.IsSubclassOf(typeof(TCPMessage)))
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

        protected static Type GetType(int id)
        {
            return gameObjectTypeArray[id];
        }

        protected int GetTypeID()
        {
            return GetTypeID(this.GetType());
        }

        public TCPMessage()
        {
            this.Append(this.GetTypeID());
            this.Append(0);
        }

        private void Append(int i)
        {
            Append(BitConverter.GetBytes(i));
        }

        private void Append(byte[] b)
        {
            b.CopyTo(buff, emptySpot);
            emptySpot = emptySpot + b.Length;
        }

        private static TCPMessage ConstructMessage(byte[] b, int lenght)
        {
            if (lenght < 4)
            {
                throw new Exception("Message is to short");
            }
            System.Reflection.ConstructorInfo constructor = TCPMessage.GetType(BitConverter.ToInt32(b, 0)).GetConstructor(Type.EmptyTypes);
            TCPMessage message =  (TCPMessage)constructor.Invoke(new object[0]);
            message.InitializeFromBuffer(b, lenght);
            return message;
        }

        private void SetSize()
        {
            if (emptySpot - headerSize < 0)
            {
                throw new Exception("message too short");
            }
            BitConverter.GetBytes(emptySpot - headerSize).CopyTo(buff, 4);
        }

        public Boolean Send(NetworkStream clientStream, Mutex writeMutex)
        {
            SetSize();
            writeMutex.WaitOne();
            try
            {
                clientStream.Write(buff, 0, emptySpot);
                clientStream.Flush();
            }
            catch
            {
                return false;
            }
            writeMutex.ReleaseMutex();
            return true;
        }

        protected virtual void InitializeFromBuffer(byte[] b, int lenght)
        {
            if (lenght != BitConverter.ToInt32(b, 4) + headerSize)
            {
                throw new Exception("Incorrect message length");
            }
            b.CopyTo(buff, 0);
            emptySpot = BitConverter.ToInt32(buff, 4) + headerSize;
        }

        public static TCPMessage ReadMessage(Client client)
        {
            byte[] readBuff = new byte[buffMaxSize];
            int size = 0;
            size = client.Read(readBuff, 0, headerSize);
            int bytesLeft = BitConverter.ToInt32(readBuff, 4);
            size = size + client.Read(readBuff, size, bytesLeft);
            return ConstructMessage(readBuff, size);
        }
    }
}
