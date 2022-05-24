using System;
using UnityEngine;

namespace TheOrb.Control
{
    [Serializable]
    struct CursorMapping
    {
        public CursorType type;
        public Texture2D texture;
        public Vector2 hotspot;

    }
}