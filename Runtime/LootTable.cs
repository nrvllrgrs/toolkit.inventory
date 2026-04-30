using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if USE_COLLECTABLES_TOOLKIT
using ToolkitEngine.Collectables;
#endif

namespace ToolkitEngine.Inventory
{
    [CreateAssetMenu(menuName = "Toolkit/Inventory/Loot Table")]
    public class LootTable : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private List<LootEntry> m_items = new();

        [SerializeField, Min(0f)]
        private float m_noDropRate;

        #endregion

        #region Properties

        public float totalRates => m_items.Sum(x => x.weight) + m_noDropRate;

        #endregion

        #region Methods

        public DropEntry[] Get(int amount = 1, System.Random random = null, bool unique = false)
        {
            if (!unique)
            {
				List<DropEntry> drops = new();

				float cachedTotalRates = totalRates;
                for (int i = 0; i < amount; ++i)
                {
                    if (RandomUtil.Next(0f, cachedTotalRates, random) <= m_noDropRate)
                        continue;

                    var selected = m_items.WeightedRandom(random);
                    switch (selected.dropType)
                    {
                        case DropEntry.DropType.LootTable:
                            drops.AddRange(selected.lootTable.Get(selected.GetAmount(), random));
                            break;

                        default:
                            drops.Add(new DropEntry(selected));
                            break;
                    }
                }
				return drops.ToArray();
			}
            else
            {
				HashSet<int> selectedHashes = new HashSet<int>();
				return GetUniqueDrops(amount, random, selectedHashes);
			}
        }

		private DropEntry[] GetUniqueDrops(int amount, System.Random random, HashSet<int> selectedHashes)
		{
			List<DropEntry> drops = new();
			int attempts = 0;
			int maxAttempts = amount * 10; // Prevent infinite loop

			float cachedTotalRates = totalRates;
			for (int i = 0; i < amount && attempts < maxAttempts; ++attempts)
			{
				if (RandomUtil.Next(0f, cachedTotalRates, random) <= m_noDropRate)
					continue;

				var selected = m_items.WeightedRandom(random);

				switch (selected.dropType)
				{
					case DropEntry.DropType.LootTable:
						var nestedDrops = selected.lootTable.GetUniqueDrops(selected.GetAmount(), random, selectedHashes);
						drops.AddRange(nestedDrops);
						i += nestedDrops.Length;
						break;

					default:
						int hash = selected.GetHashCode();
						if (!selectedHashes.Contains(hash))
						{
							selectedHashes.Add(hash);
							drops.Add(new DropEntry(selected));
							++i;
						}
						break;
				}
			}

			return drops.ToArray();
		}

		public DropEntry[] Get(IEnumerable<LootEntry> excluded, int amount = 1, System.Random random = null, bool unique = false)
		{
			var filteredItems = m_items.Except(excluded).ToList();
			if (filteredItems.Count == 0)
				return Array.Empty<DropEntry>();

			if (!unique)
			{
				List<DropEntry> drops = new();
				float filteredTotalRates = filteredItems.Sum(x => x.weight) + m_noDropRate;
				for (int i = 0; i < amount; ++i)
				{
					if (RandomUtil.Next(0f, filteredTotalRates, random) <= m_noDropRate)
						continue;

					var selected = filteredItems.WeightedRandom(random);
					switch (selected.dropType)
					{
						case DropEntry.DropType.LootTable:
							drops.AddRange(selected.lootTable.Get(excluded, selected.GetAmount(), random));
							break;

						default:
							drops.Add(new DropEntry(selected));
							break;
					}
				}
				return drops.ToArray();
			}
			else
			{
				HashSet<int> selectedHashes = new HashSet<int>();
				return GetUniqueDrops(excluded, filteredItems, amount, random, selectedHashes);
			}
		}

		private DropEntry[] GetUniqueDrops(IEnumerable<LootEntry> excluded, List<LootEntry> filteredItems, int amount, System.Random random, HashSet<int> selectedHashes)
		{
			List<DropEntry> drops = new();
			int attempts = 0;
			int maxAttempts = amount * 10;

			float filteredTotalRates = filteredItems.Sum(x => x.weight) + m_noDropRate;
			for (int i = 0; i < amount && attempts < maxAttempts; ++attempts)
			{
				if (RandomUtil.Next(0f, filteredTotalRates, random) <= m_noDropRate)
					continue;

				var selected = filteredItems.WeightedRandom(random);
				switch (selected.dropType)
				{
					case DropEntry.DropType.LootTable:
						var nestedDrops = selected.lootTable.GetUniqueDrops(
                            excluded,
                            selected.lootTable.m_items.Where(x => !excluded.Contains(x)).ToList(),
                            selected.GetAmount(),
                            random,
                            selectedHashes);
						drops.AddRange(nestedDrops);
						i += nestedDrops.Length;
						break;

					default:
						int hash = selected.GetHashCode();
						if (!selectedHashes.Contains(hash))
						{
							selectedHashes.Add(hash);
							drops.Add(new DropEntry(selected));
							++i;
						}
						break;
				}
			}

			return drops.ToArray();
		}

		#endregion

		#region Editor-Only
#if UNITY_EDITOR

		public bool ContainsDrop(LootEntry entry)
        {
            var filteredItems = m_items.Where(x => Equals(x.dropType, entry.dropType));
			switch (entry.dropType)
            {
                case DropEntry.DropType.Item:
                    return filteredItems.Any(x => Equals(x.itemType, entry.itemType));

				case DropEntry.DropType.Currency:
					return filteredItems.Any(x => Equals(x.currencyType, entry.currencyType));

#if USE_COLLECTABLES_TOOLKIT
                case DropEntry.DropType.Collectable:
                    return filteredItems.Any(x => Equals(x.collectableType, entry.collectableType));
#endif
            }

			throw new NotSupportedException();
        }

        public void Add(LootEntry entry)
        {
            m_items.Add(entry);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void Clear()
        {
            m_items.Clear();
			UnityEditor.EditorUtility.SetDirty(this);
		}

#endif
		#endregion
    }

	[Serializable]
    public class DropEntry
    {
        #region Enumerators

        public enum DropType
        {
            Item,
            Currency,
            LootTable,
#if USE_COLLECTABLES_TOOLKIT
            Collectable,
#endif
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

#if USE_COLLECTABLES_TOOLKIT
        [SerializeField]
        protected CollectableType m_collectable;
#endif

        [SerializeField]
        protected AmountType m_amountType;

        [SerializeField, Min(1)]
        protected int m_amount;

        [SerializeField, Min(0)]
        protected int m_minAmount;

        [SerializeField, Min(1)]
        protected int m_maxAmount = 1;

        /// <summary>
        /// Amount from drop
        /// </summary>
        private int? m_dropAmount = null;

        #endregion

        #region Properties

        public DropType dropType
        {
            get => m_dropType;
            set => m_dropType = value;
        }

        public ItemType itemType
        {
            get => m_item;
            set => m_item = value;
        }

        public CurrencyType currencyType
        {
            get => m_currency;
            set => m_currency = value;
        }

        public LootTable lootTable => m_lootTable;

#if USE_COLLECTABLES_TOOLKIT
        public CollectableType collectableType
        {
            get => m_collectable;
            set => m_collectable = value;
		}
#endif

        public AmountType amountType => m_amountType;
		public int amount => m_amount;
        public int minAmount => m_minAmount;
        public int maxAmount => m_maxAmount;

		#endregion

		#region Constructors

        public DropEntry()
        { }

        public DropEntry(DropEntry other)
        {
			m_dropType = other.dropType;
            switch (m_dropType)
            {
                case DropType.Item:
                    m_item = other.m_item;
                    break;

                case DropType.Currency:
                    m_currency = other.m_currency;
                    break;

                case DropType.LootTable:
                    m_lootTable = other.m_lootTable;
                    break;

#if USE_COLLECTABLES_TOOLKIT
				case DropType.Collectable:
                    m_collectable = other.m_collectable;
                    break;
#endif
			}

			m_amountType = other.m_amountType;
            switch (m_amountType)
            {
                case AmountType.Constant:
					m_amount = other.m_amount;
                    break;

                case AmountType.Range:
                    m_minAmount = other.m_minAmount;
                    m_maxAmount = other.m_maxAmount;
                    break;
			}
        }

		public DropEntry(ItemType item, int amount = 0)
		{
			m_dropType = DropType.Item;
			m_item = item;
			m_amountType = AmountType.Constant;
			m_amount = amount;
		}

		public DropEntry(ItemType item, int min, int max)
		{
			m_dropType = DropType.Item;
			m_item = item;
			m_amountType = AmountType.Range;
			m_minAmount = min;
			m_maxAmount = max;
		}

		public DropEntry(CurrencyType currency, int amount = 0)
		{
			m_dropType = DropType.Currency;
			m_currency = currency;
			m_amountType = AmountType.Constant;
			m_amount = amount;
		}

		public DropEntry(CurrencyType currency, int min, int max)
		{
			m_dropType = DropType.Currency;
			m_currency = currency;
			m_amountType = AmountType.Range;
			m_minAmount = min;
			m_maxAmount = max;
		}

		public DropEntry(LootTable lootTable, int amount = 0)
		{
			m_dropType = DropType.LootTable;
			m_lootTable = lootTable;
			m_amountType = AmountType.Constant;
			m_amount = amount;
		}

		public DropEntry(LootTable lootTable, int min, int max)
		{
			m_dropType = DropType.LootTable;
			m_lootTable = lootTable;
			m_amountType = AmountType.Range;
			m_minAmount = min;
			m_maxAmount = max;
		}

		#endregion

		#region Methods

		public int GetAmount()
		{
            if (!m_dropAmount.HasValue)
            {
                switch (m_amountType)
                {
                    case AmountType.Constant:
                        m_dropAmount = m_amount;
                        break;

                    case AmountType.Range:
                        m_dropAmount = UnityEngine.Random.Range(m_minAmount, m_maxAmount);
                        break;

                    default:
                        m_dropAmount = 0;
                        break;
                }
            }
			return m_dropAmount.Value;
		}

        public void Combine(DropEntry entry)
        {
            m_dropAmount = GetAmount() + entry.GetAmount();
        }

		public override int GetHashCode()
        {
            switch (m_dropType)
            {
                case DropType.Item:
                    return HashCode.Combine(m_dropType, m_item);

                case DropType.Currency:
                    return HashCode.Combine(m_dropType, m_currency);

                case DropType.LootTable:
                    return HashCode.Combine(m_dropType, m_lootTable);

#if USE_COLLECTABLES_TOOLKIT
                case DropType.Collectable:
                    return HashCode.Combine(m_dropType, m_collectable);
#endif
			}
			return 0;
        }

        public override bool Equals(object obj)
		{
			if (obj is DropEntry other)
            {
                if (m_dropType != other.dropType)
                    return false;

                switch (m_dropType)
                {
                    case DropType.Item:
                        return m_item == other.itemType;

                    case DropType.Currency:
                        return m_currency == other.currencyType;

#if USE_COLLECTABLES_TOOLKIT
					case DropType.Collectable:
						return m_collectable = other.collectableType;
#endif
				}
			}
            return false;
		}

		#endregion

		#region Editor-Only
#if UNITY_EDITOR
		public void SetAmount(int amount)
		{
			m_amountType = AmountType.Constant;
			m_amount = amount;
		}

		public void SetAmount(int minAmount, int maxAmount)
		{
			m_amountType = AmountType.Range;
			m_minAmount = minAmount;
			m_maxAmount = maxAmount;
		}

#endif
		#endregion
	}

    [Serializable]
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

        public float weight
        {
            get => m_rate;
#if UNITY_EDITOR
            set => m_rate = value;
#endif
        }

        public LootEntry item => this;

        #endregion

        #region Constructors

        public LootEntry()
        { }

        public LootEntry(ItemType item, int amount = 1)
            : base(item, amount) { }

        public LootEntry(ItemType item, int min, int max)
            : base(item, min, max) { }

        public LootEntry(CurrencyType currency, int amount = 1)
            : base(currency, amount) { }

        public LootEntry(CurrencyType currency, int min, int max)
			: base(currency, min, max) { }

		public LootEntry(LootTable lootTable, int amount = 1)
			: base(lootTable, amount) { }

		public LootEntry(LootTable lootTable, int min, int max)
			: base (lootTable, min, max) { }

		#endregion
	}
}