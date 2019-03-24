﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization.DataTypes
{
    class SFloat : SGeneric<float>
    {
        public static implicit operator SFloat(float value)
        {
            return new SFloat(value);
        }

        public SFloat() : base()
        {
        }

        public SFloat(float value) : base(value)
        {
        }

        public override int SerializationSize
        {
            get
            {
                return sizeof(float);
            }
        }

        public override void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            this.Value = Utils.ReadFloat(buffer, ref bufferOffset);
        }

        public override void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(this.Value, buffer, ref bufferOffset);
        }
    }
}