using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
    [CreateAssetMenu(menuName = "Toolkit/Inventory/Loot Table")]
    public class LootTable : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private LootEntry[] m_items = new LootEntry[] { };

        [SerializeField, Min(0f)]
        private float m_noDropRate;

        #endregion

        #region Properties

        public float totalRates => m_items.Sum(x => x.weight) + m_noDropRate;

        #endregion

        #region Methods

        //public object[] Get(int picks = 1)
        //{

        //}

        #endregion
    }

    [System.Serializable]
    public class DropEntry
    {
        #region Enmerators

        public enum DropType
        {
            Item,
            Currency,
            LootTable,
        }

        public enum AmountType
        {
            Constant,
            Range,
        }

        #endregion

        #region Fields

        [SerializeField]
        protected DropType m_dropType;

        [SerializeField]
        protected ItemType m_item;

        [SerializeField]
        protected CurrencyType m_currency;

        [SerializeField]
        protected LootTable m_lootTable;

        [SerializeField]
        protected AmountType m_amountType;

        [SerializeField, Min(1)]
        protected int m_amount;

        [SerializeField, Min(0)]
        protected int m_minAmount;

        [SerializeField, Min(1)]
        protected int m_maxAmount = 1;

        #endregion
    }

    [System.Serializable]
    public class LootEntry : DropEntry, IWeightedItem<LootEntry>
    {
        #region Fields

        [SerializeField, Min(0f)]
        protected float m_rate;

#if UNITY_EDITOR

        [SerializeField]
        protected float m_percent;

#endif
        #endregion

        #region Properties

        public float weight => m_rate;
        public LootEntry item => this;

        #endregion
    }
}