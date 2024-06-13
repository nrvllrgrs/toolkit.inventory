using System.Collections.Generic;
using System.Linq;
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

        public DropEntry[] Generate()
        {
            List<DropEntry> list = new();
            if (m_picks > 0)
            {
                list.AddRange(m_lootTable.Get(m_picks));
            }

            if (m_guaranteedDrops.Length > 0)
            {
                list.AddRange(m_guaranteedDrops);
            }

            return list.ToArray();
        }

        #endregion
    }
}