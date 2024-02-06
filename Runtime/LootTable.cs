using System.Collections.Generic;
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

        public DropEntry[] Get(int amount = 1)
        {
            List<DropEntry> drops = new();

            float cachedTotalRates = totalRates;
			for (int i = 0; i < amount; ++i)
            {
                if (Random.Range(0f, cachedTotalRates) <= m_noDropRate)
                    continue;

                drops.Add(m_items.WeightedRandom());
            }

            return drops.ToArray();
        }

        #endregion
    }

    [System.Serializable]
    public class DropEntry
    {
        #region Enumerators

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

        #region Properties

        public DropType dropType => m_dropType;
        public ItemType itemType => m_item;
        public CurrencyType currencyType => m_currency;
        public LootTable lootTable => m_lootTable;
        public int amount => m_amount;
        public int minAmount => m_minAmount;
        public int maxAmount => m_maxAmount;

		#endregion

		#region Methods

		public int GetAmount()
		{
			switch (m_amountType)
			{
				case AmountType.Constant:
					return m_amount;

				case AmountType.Range:
					return Random.Range(m_minAmount, m_maxAmount);
			}
			return 0;
		}

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