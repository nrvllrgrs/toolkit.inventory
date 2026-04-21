using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
    public class Loot : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private int m_seed = 0;

        [SerializeField]
        private LootTable m_lootTable;

        [SerializeField]
        private DropEntry.AmountType m_amountType;

        [SerializeField, Min(0)]
        private int m_picks;

        [SerializeField, MinMax(0, int.MaxValue)]
        private Vector2Int m_pickRange;

        [SerializeField]
        private List<DropEntry> m_guaranteedDrops = new();

        private System.Random m_random = null;

		#endregion

		#region Properties

        public int totalPicks => m_picks + m_guaranteedDrops.Count;

		#endregion

		#region Methods

		private void Awake()
		{
			if (m_seed != 0)
            {
                SetSeed(m_seed);
            }
		}

        public void SetSeed(int seed)
        {
            m_seed = seed;
            m_random = RandomUtil.GetRandom(seed);
        }

        private int GetPicks()
        {
			int picks = 0;
			switch (m_amountType)
			{
				case DropEntry.AmountType.Constant:
					picks = m_picks;
					break;

				case DropEntry.AmountType.Range:
					picks = RandomUtil.Next(m_pickRange.x, m_pickRange.y + 1, m_random);
					break;
			}

            return picks;
		}

        public DropEntry[] Generate() => Generate(false);

		public DropEntry[] Generate(bool unique)
        {
            int picks = GetPicks();

			HashSet<DropEntry> drops = new();
            if (picks > 0)
            {
                if (m_lootTable == null)
                {
                    Debug.LogError($"LootTable is undefined on {gameObject.name}!");
                }
                else
                {
                    AddDrops(drops, m_lootTable.Get(picks, m_random, unique));
                }
            }

            if (m_guaranteedDrops.Count > 0)
            {
                AddDrops(drops, m_guaranteedDrops);
            }

            return drops.ToArray();
        }

        public DropEntry[] Generate(IEnumerable<LootEntry> excluded, bool unique = false)
        {
			int picks = GetPicks();

			HashSet<DropEntry> drops = new();
			if (picks > 0)
			{
                if (m_lootTable == null)
                {
                    Debug.LogError($"LootTable is undefined on {gameObject.name}!");
                }
                else
                {
					AddDrops(drops, m_lootTable.Get(excluded, picks, m_random, unique));
				}
			}

			if (m_guaranteedDrops.Count > 0)
			{
				AddDrops(drops, m_guaranteedDrops);
			}

			return drops.ToArray();
		}

        public void SetLootTable(LootTable lootTable) => m_lootTable = lootTable;

        public void AddGuaranteedDrops(IEnumerable<DropEntry> drops)
        {
            foreach (var drop in drops)
            {
                if (drop == null)
                    continue;

                switch (drop.amountType)
                {
                    case DropEntry.AmountType.Constant:
                        m_guaranteedDrops.Add(drop);
                        break;

                    case DropEntry.AmountType.Range:
						// Make copy of drop so its amount is not fixedA
						m_guaranteedDrops.Add(new DropEntry(drop));
                        break;
				}
            }
        }

		public void ClearGuaranteedDrops() => m_guaranteedDrops.Clear();

		#endregion

		#region Static Methods

		public static void AddDrop(IList<DropEntry> list, DropEntry drop)
        {
            AddDrops(list, new[] { drop });
        }

		public static void AddDrops(IList<DropEntry> list, IEnumerable<DropEntry> collection)
        {
            var set = list.ToHashSet();
            AddDrops(set, collection);
            list = set.ToList();
        }

		public static void AddDrop(HashSet<DropEntry> set, DropEntry drop)
        {
            AddDrops(set, new[] { drop });
        }

		public static void AddDrops(HashSet<DropEntry> set, IEnumerable<DropEntry> collection)
        {
			foreach (var drop in collection)
			{
				if (!set.TryGetValue(drop, out var stored))
				{
					set.Add(new DropEntry(drop));
				}
				else
				{
					stored.Combine(drop);
				}
			}
		}

		#endregion
	}
}