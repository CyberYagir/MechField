using UnityEngine;

namespace Content.Scripts.Game.Maps
{
    public class MapPart<T> : MonoBehaviour
    {
        private T data;

        public T Data => data;

        public virtual void Init(T data)
        {
            this.data = data;
        }
    }
}