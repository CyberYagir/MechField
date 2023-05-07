using System.Collections.Generic;
using Content.Scripts.Weapons.Energy;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Game.PoolService
{
    public interface IPoolService
    {
        Bullet GetItemFromPool(PoolService.BulletType type);
        
        Bullet GetItemFromPool(PoolService.BulletType type, Vector3 pos, Quaternion rot);
    }

    public class PoolService : MonoBehaviour, IPoolService
    {
        public enum BulletType
        {
            EnergyBullet,
            MachinegunBullet
        }

        [System.Serializable]
        public class Pool
        {
            [SerializeField] private Bullet prefab;
            [SerializeField] private BulletType type;
            [SerializeField] private int startCount;

            private Stack<Bullet> stack = new Stack<Bullet>();
            private Transform holder;

            public BulletType Type => type;

            public void Init(PoolService poolService)
            {
                holder = new GameObject(Type.ToString()).transform;
                holder.transform.parent = poolService.transform;

                for (int i = 0; i < startCount; i++)
                {
                    var bullet = AddBullet();
                    bullet.OnBulletEnd += BackBullet;
                }
            }

            public Bullet GetBullet()
            {
                if (stack.Count == 0)
                {
                    AddBullet();
                }
                var item = stack.Pop();
                item.gameObject.SetActive(true);
                return item;
            }

            private void BackBullet(Bullet obj)
            {
                obj.gameObject.SetActive(false);
                stack.Push(obj);
            }

            private Bullet AddBullet()
            {
                var obj = Instantiate(prefab, holder);
                obj.gameObject.SetActive(false);
                stack.Push(obj);
                return obj;
            }
        }


        [SerializeField] private List<Pool> pools;

        private Dictionary<BulletType, Pool> poolsList = new Dictionary<BulletType, Pool>(5);

        [Inject]
        public void Constructor()
        {
            foreach (var pool in pools)
            {
                pool.Init(this);
                poolsList.Add(pool.Type, pool);
            }
        }


        public Bullet GetItemFromPool(BulletType type)
        {
            return poolsList[type].GetBullet();
        }

        public Bullet GetItemFromPool(BulletType type, Vector3 pos, Quaternion rot)
        {
            var bullet = GetItemFromPool(type);
            bullet.transform.position = pos;
            bullet.transform.rotation = rot;

            return bullet;
        }
    }
}
