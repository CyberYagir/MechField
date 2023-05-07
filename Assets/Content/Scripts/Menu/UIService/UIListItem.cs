using System;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Menu.UIService
{
    public class UIListItem<T> : MonoBehaviour
    {
        [SerializeField] protected Image selection;
        [SerializeField] protected Button button;

        protected T data;
        protected MenuUIService menuUIService;

        public T Data => data;


        public virtual void Init(T data, MenuUIService menuUIService, Action<T, UIListItem<T>> OnClick)
        {
            this.data = data;
            this.menuUIService = menuUIService;
            
            
            BindButtonAction(OnClick);
        }

        protected void BindButtonAction(Action<T, UIListItem<T>> OnClick)
        {
            button.onClick.AddListener(delegate
            {
                OnClick?.Invoke(Data, this);
            });
        }
        
        public virtual void UpdateSelection()
        {
            
        }


    }
}