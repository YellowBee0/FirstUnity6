using YBFramework.Common;
using YBFramework.Component;

namespace YBFramework.Test
{
    public sealed class BuffTest : Entity
    {
        public BuffAsset Buff1;

        public BuffAsset Buff2;

        public BuffAsset Buff3;

        private void Awake()
        {
            AddComponent(new BuffManager());
            PropertyManager propertyManager = new();
            Property heal = ObjectPool.Allocate<Property>();
            Property attack = ObjectPool.Allocate<Property>();
            heal.Init(100, 0, 100, false, false, false);
            attack.Init(100, 0, 100, false, false, false);
            propertyManager.AddProperty("heal", heal);
            propertyManager.AddProperty("attack", attack);
            AddComponent(propertyManager);
        }

        public void AddBuff1()
        {
            GetCustomComponent<BuffManager>().AddBuff(this, Buff1);
        }

        public void AddBuff2()
        {
            GetCustomComponent<BuffManager>().AddBuff(this, Buff2);
        }

        public void AddBuff3()
        {
            GetCustomComponent<BuffManager>().AddBuff(this, Buff3);
        }

        public void RemoveBuff1()
        {
            GetCustomComponent<BuffManager>().RemoveBuff(BuffType.BUFF);
        }

        public void RemoveBuff2()
        {
            GetCustomComponent<BuffManager>().RemoveBuff(BuffType.Neutral);
        }

        public void RemoveBuff3()
        {
            GetCustomComponent<BuffManager>().RemoveBuff(BuffType.DeBUFF);
        }
    }
}