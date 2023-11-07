using UnityEngine;

namespace ToolkitEngine.Inventory
{
    public class Item : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private ItemType m_itemType;

        [SerializeField, Min(1)]
        private int m_amount = 1;

        #endregion

        #region Properties

        public ItemType itemType => m_itemType;

        public int amount
        {
            get => m_amount;
            set => m_amount = value;
        }

        #endregion

        #region Dismantle Methods

        public bool Dismantle()
        {
            return Dismantle(transform.position, transform.rotation, null);
        }

        public bool Dismantle(Inventory inventory, out int[] overflows)
        {
            if (m_itemType.Dismantle(inventory, out overflows))
            {
                Destroy(gameObject);
                return true;
            }
            return false;
        }

		public bool Dismantle(Vector3 position, Quaternion rotation, SpawnedAction onSpawnedAction, params object[] args)
        {
            return Dismantle(position, rotation, null, onSpawnedAction, args);
        }

		public bool Dismantle(Vector3 position, Quaternion rotation, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
		{
            if (m_itemType.Dismantle(position, rotation, parent, onSpawnedAction, args))
            {
                Destroy(gameObject);
                return true;
            }
            return false;
        }

        public bool Dismantle(Transform parent, bool instantiateInWorldSpace, SpawnedAction onSpawnedAction, params object[] args)
        {
            if (m_itemType.Dismantle(parent, instantiateInWorldSpace, onSpawnedAction, args))
            {
                Destroy(gameObject);
                return true;
            }
            return false;
        }

        #endregion
    }
}