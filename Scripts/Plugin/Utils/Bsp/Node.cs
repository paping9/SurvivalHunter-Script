using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Bsp
{
    public class Node
    {
        public Node leftNode;
        public Node rightNode;
        public Node parNode;
        public RectInt nodeRect;
        public RectInt roomRect;
        public Vector2Int center
        {
            get
            {
                return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
            }
        }
        public Node(RectInt rect)
        {
            this.nodeRect = rect;
        }
    }
}

