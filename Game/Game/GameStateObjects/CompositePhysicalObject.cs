﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;

namespace MyGame.GameStateObjects
{

    public abstract class CompositePhysicalObject : PhysicalObject
    {
        private Vector2 position = new Vector2(0);
        private float direction = 0;
        private Leaf leaf;
        private QuadTree tree;

        public CompositePhysicalObject(Vector2 position, float direction) : base()
        {
            if (GameState.Tree == null)
            {
                throw new NoQuadTree();
            }
            tree = GameState.Tree;

            leaf = tree.Add(this);
            this.Position = position;
            this.direction = direction;
        }

        public void SetLeaf(Leaf leaf)
        {
            this.leaf = leaf;
        }

        private class NoQuadTree : Exception
        { }


        public override Vector2 WorldPosition()
        {
            return Position;
        }

        public override float WorldDirection()
        {
            return Direction;
        }

        public virtual Vector2 Position
        {
            protected set {
                if (tree != null )
                {
                    if (!GameState.GetWorldRectangle().Contains(value))
                    {
                        MoveOutsideWorld(this.Position, value);
                    }
                    else
                    {
                        if (leaf != null)
                        {
                            position = value;
                            leaf.Move(this);
                        }

                    }
                }
                else
                {
                    position = value;
                }
            }
            get { return position; }
        }

        protected abstract void MoveOutsideWorld(Vector2 position, Vector2 movePosition);

        public float Direction
        {
            protected set { direction = value; }
            get { return direction; }
        }

        public override PhysicalObject Root()
        {
            return this;
        }

        public virtual Boolean IsFlyingGameObject
        {
            get { return false; }
        }

        public override Boolean IsCompositePhysicalObject
        {
            get { return true; }
        }
    }
}

