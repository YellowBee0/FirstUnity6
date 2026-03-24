using UnityEngine;
using UnityEngine.InputSystem;
using YBFramework.Component;

namespace YBFramework.Test
{
    public class Test : MonoBehaviour
    {
        public Entity Entity;

        public BuffAsset BuffAsset1;

        public BuffAsset BuffAsset2;

        public BuffAsset BuffAsset3;

        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                Entity.GetCustomComponent<BuffManager>().AddBuff(Entity, BuffAsset1);
            }
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                Entity.GetCustomComponent<BuffManager>().AddBuff(Entity, BuffAsset2);
            }
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Entity.GetCustomComponent<BuffManager>().AddBuff(Entity, BuffAsset3);
            }
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                Entity.GetCustomComponent<BuffManager>().RemoveBuff(BuffType.BUFF);
            }
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                Entity.GetCustomComponent<BuffManager>().RemoveBuff(BuffType.Neutral);
            }
            if (Keyboard.current.yKey.wasPressedThisFrame)
            {
                Entity.GetCustomComponent<BuffManager>().RemoveBuff(BuffType.DeBUFF);
            }
        }
    }
}