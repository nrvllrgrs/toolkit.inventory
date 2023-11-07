using UnityEngine;

namespace ToolkitEngine.Inventory
{
    public class Loot : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private LootTable m_lootTable;

        [SerializeField, Min(0)]
        private int m_picks;

        [Space]

        [SerializeField]
        private DropEntry[] m_guaranteedDrops;

        #endregion

        #region Methods

        //public object[] Generate()
        //{

        //}

        #endregion
    }
}