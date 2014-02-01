﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
namespace MyGame.Networking
{
    public abstract class TCPMessage
    {

        //TODO: this buffer might need to be thread safe
        private const int buffMaxSize = 1024;
        private const int headerSize = 8;
        private byte[] buff = new byte[buffMaxSize];
        private int emptySpot = 0;
        private int readerSpot = 0;

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

        protected int GetTypeID()
        {
            return GetTypeID(this.GetType());
        }

        public TCPMessage()
        {
            this.Append(this.GetTypeID());
            this.Append(0);
        }

        public void Append(int i)
        {
            Append(BitConverter.GetBytes(i));
        }

        public void Append(float i)
        {
            Append(BitConverter.GetBytes(i));
        }

        public void Append(string s)
        {
            this.Append(s.Length);
            char[] sArray = s.ToCharArray();
            System.Buffer.BlockCopy(sArray, 0, buff, emptySpot, sArray.Length * sizeof(char));
            emptySpot = emptySpot + sArray.Length * sizeof(char);
            if (emptySpot > buffMaxSize)
            {
                throw new Exception("Buffer exceded maximum size");
            }
        }

        private void Append(byte[] b)
        {
            //b.CopyTo(buff, emptySpot);
            Buffer.BlockCopy(b, 0, buff, emptySpot, b.Length);
            emptySpot = emptySpot + b.Length;
            if (emptySpot > buffMaxSize)
            {
                throw new Exception("Buffer exceded maximum size");
            }
        }

        private static TCPMessage ConstructMessage(byte[] b, int lenght)
        {
            if (lenght < 4)
            {
                throw new Exception("Message is to short");
            }
            Type[] constuctorParamsTypes = new Type[2];
            constuctorParamsTypes[0] = typeof(byte[]);
            constuctorParamsTypes[1] = typeof(int);

            System.Reflection.ConstructorInfo constructor = TCPMessage.GetType(BitConverter.ToInt32(b, 0)).GetConstructor(constuctorParamsTypes);

            object[] constuctorParams = new object[2];
            constuctorParams[0] = b;
            constuctorParams[1] = lenght;

            TCPMessage message = (TCPMessage)constructor.Invoke(constuctorParams);
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

        public Boolean SendTCP(NetworkStream clientStream, Mutex writeMutex)
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

        public Boolean SendUDP(UdpClient client, TcpClient tcpClient, Mutex writeMutex)
        {
            SetSize();
            writeMutex.WaitOne();
            try
            {
                client.Send(buff, emptySpot);
                //clientStream.Write(buff, 0, emptySpot);
                //clientStream.Flush();
            }
            catch (Exception e)
            {
                return false;
            }
            writeMutex.ReleaseMutex();
            return true;
        }

        public TCPMessage(byte[] b, int lenght)
        {
            if (lenght != BitConverter.ToInt32(b, 4) + headerSize)
            {
                throw new Exception("Incorrect message length");
            }
            b.CopyTo(buff, 0);
            emptySpot = BitConverter.ToInt32(buff, 4) + headerSize;
        }

        public static TCPMessage ReadTCPMessage(Client client)
        {
            byte[] readBuff = new byte[buffMaxSize];
            int size = 0;
            size = client.ReadTCP(readBuff, 0, headerSize);
            int bytesLeft = BitConverter.ToInt32(readBuff, 4);
            size = size + client.ReadTCP(readBuff, size, bytesLeft);
            return ConstructMessage(readBuff, size);
        }

        public static TCPMessage ReadUDPMessage(Client client)
        {
            byte[] readBuff = client.ReadUDP();
            return ConstructMessage(readBuff, readBuff.Length);
        }

        public void ResetReader()
        {
            readerSpot = headerSize;
        }

        public int ReadInt()
        {
            if (readerSpot + 4 > emptySpot)
            {
                throw new Exception("Read past end of buffer");
            }
            int rtn = BitConverter.ToInt32(buff, readerSpot);
            readerSpot = readerSpot + 4;
            return rtn;
        }

        public float ReadFloat()
        {
            if (readerSpot + 4 > emptySpot)
            {
                throw new Exception("Read past end of buffer");
            }
            float rtn = BitConverter.ToSingle(buff, readerSpot);
            readerSpot = readerSpot + 4;
            return rtn;
        }

        public string ReadString()
        {
            int length = this.ReadInt();

            char[] chars = new char[length];
            System.Buffer.BlockCopy(buff, readerSpot, chars, 0, length * sizeof(char));
            string str = new string(chars);

            readerSpot = readerSpot + length * sizeof(char);

            if (readerSpot > emptySpot)
            {
                throw new Exception("Read past end of buffer");
            }

            return str;
        }

        public void PrintBuff()
        {
            foreach (byte b in buff)
            {
                Console.Write(b.ToString()+ " ");
            }
            Console.WriteLine();
        }

        public void AssertMessageEnd()
        {
            if(readerSpot != emptySpot)
            {
                throw new Exception("Message end not reached");
            }
        }

        public void Append(Vector2 v)
        {
            this.Append(v.X);
            this.Append(v.Y);
        }

        public Vector2 ReadVector2()
        {
            float x = this.ReadFloat();
            float y = this.ReadFloat();
            return new Vector2(x, y);
        }

        public void Append(List<GameObject> members)
        {
            this.Append(members.Count);
            foreach(MemberPhysicalObject member in members)
            {
                this.Append(member);
            }
        }

        public List<GameObject> ReadGameObjectList()
        {
            List<GameObject> rtn = new List<GameObject>();
            int count = this.ReadInt();
            for (int i = 0; i < count; i++)
            {
                rtn.Add((GameObject)ReadGameObject());
            }
            return rtn;
        }

        public void Append(Boolean b)
        {
            if (b)
            {
                this.Append(1);
            }
            else
            {
                this.Append(0);
            }
        }

        public Boolean ReadBoolean()
        {
            return this.ReadInt() != 0;
        }

        public void Append(GameObject obj)
        {
            if (obj == null)
            {
                this.Append((int)0);
            }
            else
            {
                this.Append(obj.ID);
            }
        }

        public GameObject ReadGameObject()
        {
            return GameObject.Collection.Get(this.ReadInt());
        }


    }
}
