using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.UI.Dragging
{
    public interface IDragSource<T> where T : class
    {
        T GetItem();
        int GetAmount();
        void RemoveItems(int number);

    }
}
