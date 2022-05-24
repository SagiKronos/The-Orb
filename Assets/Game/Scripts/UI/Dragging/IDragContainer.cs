using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.UI.Dragging
{
    public interface IDragContainer<T> : IDragSource<T>, IDragDestination<T> where T : class
    {
    }
}