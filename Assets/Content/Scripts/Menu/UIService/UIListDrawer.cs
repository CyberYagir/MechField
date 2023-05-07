using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.Menu.UIService
{
    public class UIListDrawer<T> : UIController<MenuUIService>
    {

        [SerializeField] protected Transform holder;
        [SerializeField] protected UIListItem<T> item;
        
        
        protected T selectedItem;
        private List<UIListItem<T>> items = new List<UIListItem<T>>(10);

        public List<UIListItem<T>> Items => items;

        public virtual void ReInit()
        {

        }

        public virtual void ResetPreview()
        {

        }


        protected void ClearItems()
        {
            foreach (var obj in Items)
            {
                Destroy(obj.gameObject);
            }
            Items.Clear();
        }

        protected void AddItem(UIListItem<T> it)
        {
            Items.Add(it);
        }
    }
}