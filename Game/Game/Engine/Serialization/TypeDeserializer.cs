﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class TypeDeserializer<BaseType> : Deserializer<BaseType>
    {
        private TypeFactory<BaseType> factory;
        private Deserializer<BaseType> deserializer;

        public TypeDeserializer(TypeFactory<BaseType> factory, Deserializer<BaseType> deserializer)
        {
            this.factory = factory;
            this.deserializer = deserializer;
        }

        public void Deserialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            this.Deserialize(obj, typeId, buffer, ref bufferOffset);
        }

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            BaseType obj = this.factory.Construct(typeId);
            this.Deserialize(obj, typeId, buffer, ref bufferOffset);
            return obj;
        }

        private void Deserialize(BaseType obj, int typeId, byte[] buffer, ref int bufferOffset)
        {
            if (this.factory.GetTypeID(obj) == typeId)
            {
                deserializer.Deserialize(obj, buffer, ref bufferOffset);
            }
            else
            {
                throw new Exception("Serialized type does not match object type");
            }
        }
    }
}