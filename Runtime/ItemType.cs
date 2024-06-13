using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToolkitEngine.Inventory
{
    [CreateAssetMenu(menuName = "Toolkit/Inventory/Item")]
    public class ItemType : ScriptableObject
    {
        #region Enumerators

        public enum DismantleMode
        {
            None,
            Ingredients,
            Scrap
        }

        #endregion

        #region Fields

        [SerializeField]
        private string m_id = Guid.NewGuid().ToString();

        [SerializeField]
        private ItemType m_parent;

        [SerializeField]
        private Spawner m_spawner;

        [SerializeField]
        private string m_name;

        [SerializeField, TextArea]
        private string m_description;

        [SerializeField]
        private Sprite m_icon;

        [SerializeField]
        private Color m_color = Color.white;

        [SerializeField, Min(0f)]
        private float m_weight;

        [SerializeField, Min(1)]
        private int m_maxStack = 1;

        [SerializeField]
        private Price m_price;

        [SerializeField]
        private bool m_sellable = true;

        [SerializeField, Range(0f, 1f)]
        private float m_sellFactor = 1f;

        //[Header("Equipment")]

        //[SerializeField]
        //private bool m_equippable;

        // TODO: Define slot to equip to

        // TODO: Define equipment conditions
        // (e.g. player must have minimum skill/attribute or level)

        //[Header("Usable")]

        // TODO: Add support for using item in bag (without needing to create gameObject instance)

        // TODO: Define crafting conditions
        // (e.g. player must know a recipe or must have a minimum skill or level)

        [SerializeField]
        private Ingredient[] m_ingredients;

        [SerializeField]
        private DismantleMode m_dismantleMode;

        [SerializeField]
        private Scrap[] m_scraps;

        #endregion

        #region Properties

        public string id => m_id;
        public ItemType parent => m_parent;
        public new string name { get => m_name; set => m_name = value; }
        public string description => m_description;
        public Sprite icon => m_icon;
        public Color color => m_color;
        public float weight => m_weight;
        public int maxStack => m_maxStack;
        public Price buyPrice => m_price;

        /// <summary>
        /// Indicates whether item can be sold
        /// </summary>
        public bool sellable => m_sellable;
        public float sellFactor => m_sellable ? m_sellFactor : 0f;

        public Price sellPrice
        {
            get
            {
                if (!m_sellable)
                    return null;

                if (m_sellFactor == 1f)
                    return m_price;

                return new Price()
                {
                    currency = buyPrice.currency,
                    amount = Mathf.RoundToInt(buyPrice.amount * m_sellFactor)
                };
            }
        }

        public Ingredient[] ingredients => m_ingredients;
        public bool craftable => m_ingredients.Length > 0;
        public bool dismantlable => m_dismantleMode != DismantleMode.None;

        #endregion

        #region Methods

        public void Instantiate()
        {
            Instantiate(onSpawnedAction: null);
        }

        public void Instantiate(SpawnedAction onSpawnedAction, params object[] args)
        {
            m_spawner.Instantiate(onSpawnedAction, args);
        }

		public void Instantiate(Vector3 position, Quaternion rotation)
        {
            Instantiate(position, rotation, onSpawnedAction: null);
        }

		public void Instantiate(Vector3 position, Quaternion rotation, SpawnedAction onSpawnedAction, params object[] args)
		{
			m_spawner.Instantiate(position, rotation, null, onSpawnedAction, args);
		}

        public void Instantiate(Vector3 position, Quaternion rotation, Transform parent)
        {
            Instantiate(position, rotation, parent, onSpawnedAction: null);
        }

		public void Instantiate(Vector3 position, Quaternion rotation, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
        {
            m_spawner.Instantiate(position, rotation, parent, onSpawnedAction, args);
        }

        public void Instantiate(Transform parent)
        {
            Instantiate(parent, onSpawnedAction: null);
        }

		public bool Instantiate(Transform parent, SpawnedAction onSpawnedAction, params object[] args)
        {
            return m_spawner.Instantiate(parent, onSpawnedAction, args);
        }

		public void Instantiate(Transform parent, bool instantiateInWorldSpace)
        {
            Instantiate(parent, instantiateInWorldSpace, onSpawnedAction: null);
        }

		public void Instantiate(Transform parent, bool instantiateInWorldSpace, SpawnedAction onSpawnedAction, params object[] args)
        {
            m_spawner.Instantiate(parent, instantiateInWorldSpace, onSpawnedAction, args);
        }

        /// <summary>
        /// Determines whether item is a subtype of specified ItemType
        /// </summary>
        /// <param name="itemType">ItemType to compare with current type</param>
        /// <returns>Returns true if current type is descendant of itemType; otherwise false</returns>
        public bool IsSubtypeOf(ItemType itemType)
        {
            if (itemType == null)
                return false;

            var type = this;
            while (type != null)
            {
                if (type == itemType)
                    return true;

                type = type.m_parent;
            }

            return false;
        }

        /// <summary>
        /// Determines whether item is a supertype of specified ItemType
        /// </summary>
        /// <param name="itemType">ItemType to compare with current type</param>
        /// <returns>Returns true if current type is ancestor of itemType; otherwise false</returns>
        public bool IsSupertypeOf(ItemType itemType)
        {
            if (itemType == null)
                return false;

            return itemType.IsSubtypeOf(this);
        }

		public override bool Equals(object other)
		{
			if (other == null)
				return false;

			if (other is ItemType otherItemType)
				return m_id == otherItemType.id;

			return false;
		}

		public override int GetHashCode()
		{
			return m_id.GetHashCode();
		}

		#endregion

		#region Buy Methods

		public bool CanBuy(Inventory inventory)
		{
		    var currencySlot = inventory.currencies.FirstOrDefault(x => Equals(x.slotType, buyPrice.currency));
			if (currencySlot == null)
				return false;

			if (currencySlot.amount < buyPrice.amount)
				return false;

			return true;
		}

		public bool Buy(Inventory inventory)
        {
			if (!CanBuy(inventory))
			{
				return false;
			}

			RemoveCurrencies(inventory);
			return true;
		}

		/// <summary>
		/// Buy item type, adding it to inventory and removing the necessary currencies
		/// </summary>
		/// <param name="itemType"></param>
		public bool Buy(Inventory inventory, out int overflow)
		{
            if (!Buy(inventory))
            {
                overflow = 0;
                return false;
            }

			inventory.AddItem(this, 1, out overflow);
			return true;
		}

		public bool Buy(Inventory inventory, SpawnedAction onSpawnedAction, params object[] args)
		{
			if (!CanBuy(inventory))
				return false;

			RemoveCurrencies(inventory);
			Instantiate(onSpawnedAction, args);
			return true;
		}

		public bool Buy(Inventory inventory, Vector3 position, Quaternion rotation, SpawnedAction onSpawnedAction, params object[] args)
		{
			return Buy(inventory, position, rotation, null, onSpawnedAction, args);
		}

		public bool Buy(Inventory inventory, Vector3 position, Quaternion rotation, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
		{
			if (!CanBuy(inventory))
				return false;

			RemoveCurrencies(inventory);
			Instantiate(position, rotation, parent, onSpawnedAction, args);
			return true;
		}

		public bool Buy(Inventory inventory, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
		{
			return Buy(inventory, parent, onSpawnedAction, args);
		}

		public bool Buy(Inventory inventory, Transform parent, bool instantiateInWorldSpace, SpawnedAction onSpawnedAction, params object[] args)
		{
			if (!CanBuy(inventory))
				return false;

			RemoveCurrencies(inventory);
			Instantiate(parent, instantiateInWorldSpace, onSpawnedAction, args);
			return true;
		}

		private void RemoveCurrencies(Inventory inventory)
		{
			var currencySlot = inventory.currencies.FirstOrDefault(x => Equals(x.slotType, buyPrice.currency));
			if (currencySlot == null)
				return;

			if (currencySlot.amount < buyPrice.amount)
				return;

			currencySlot.RemoveFromStack(buyPrice.amount);
		}

		#endregion

		#region Sell Methods

        public bool Sell(Inventory inventory)
        {
            if (!m_sellable)
                return false;

			var p = sellPrice;
			inventory.AddCurrency(p.currency, p.amount);
            return true;
        }

		#endregion

		#region Craft Methods

		public bool CanCraft(Inventory inventory)
        {
            if (inventory == null)
                return false;

            return m_ingredients.All(x => inventory.GetItemTotal(x.item) >= x.amount);
        }

        public bool CanCraft(IEnumerable<Item> items)
        {
            if (items == null)
                return false;

            var itemCounts = new Dictionary<ItemType, int>();
            foreach (var g in items.GroupBy(x => x.itemType))
            {
                itemCounts.Add(g.Key, g.Sum(x => x.amount));
            }

            return m_ingredients.All(x => itemCounts.TryGetValue(x.item, out int value) && value >= x.amount);
        }

        public bool Craft(Inventory inventory, out int overflow)
        {
            if (!CanCraft(inventory))
            {
                overflow = 0;
                return false;
            }

            RemoveIngredients(inventory);
            inventory.AddItem(this, 1, out overflow);
            return true;
        }

        public bool Craft(Inventory inventory, Vector3 position, Quaternion rotation, SpawnedAction onSpawnedAction, params object[] args)
        {
            return Craft(inventory, position, rotation, null, onSpawnedAction, args);
        }

        public bool Craft(Inventory inventory, Vector3 position, Quaternion rotation, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
        {
            if (!CanCraft(inventory))
                return false;

            RemoveIngredients(inventory);
            Instantiate(position, rotation, parent, onSpawnedAction, args);
            return true;
        }

		public bool Craft(Inventory inventory, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
        {
            return Craft(inventory, parent, false, onSpawnedAction, args);
        }

		public bool Craft(Inventory inventory, Transform parent, bool instantiateInWorldSpace, SpawnedAction onSpawnedAction, params object[] args)
        {
            if (!CanCraft(inventory))
                return false;

            RemoveIngredients(inventory);
            Instantiate(parent, instantiateInWorldSpace, onSpawnedAction, args);
            return true;
        }

        public bool Craft(IEnumerable<Item> items, Inventory inventory, out int overflow)
        {
            if (!CanCraft(items))
            {
                overflow = 0;
                return false;
            }

            RemoveIngredients(items);
            inventory.AddItem(this, 1, out overflow);
            return true;
        }

        public bool Craft(IEnumerable<Item> items, Vector3 position, Quaternion rotation, SpawnedAction onSpawnedAction, params object[] args)
        {
            return Craft(items, position, rotation, null, onSpawnedAction, args);
        }

        public bool Craft(IEnumerable<Item> items, Vector3 position, Quaternion rotation, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
        {
            if (!CanCraft(items))
                return false;

            RemoveIngredients(items);
            Instantiate(position, rotation, parent, onSpawnedAction, args);
            return true;
        }

        public bool Craft(IEnumerable<Item> items, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
        {
            return Craft(items, parent, false, onSpawnedAction, args);
        }


        public bool Craft(IEnumerable<Item> items, Transform parent, bool instantiateInWorldSpace, SpawnedAction onSpawnedAction, params object[] args)
        {
            if (!CanCraft(items))
                return false;

            RemoveIngredients(items);
            Instantiate(parent, instantiateInWorldSpace, onSpawnedAction, args);
            return true;
        }

        private void RemoveIngredients(Inventory inventory)
        {
            foreach (var ingredient in m_ingredients)
            {
                inventory.TryRemoveItem(ingredient.item, ingredient.amount);
            }
        }

        private void RemoveIngredients(IEnumerable<Item> items)
        {
            var orderedItems = new Dictionary<ItemType, List<Item>>();
            foreach (var g in items.GroupBy(x => x.itemType))
            {
                orderedItems.Add(g.Key, g.OrderByDescending(x => x.amount).ToList());
            }

            foreach (var ingredient in m_ingredients)
            {
                // Get ordered items and remove ingredients from least to greatest
                var ingredientItems = orderedItems[ingredient.item];
                int index = ingredientItems.Count - 1;

                for (int i = 0; i < ingredient.amount; ++i)
                {
                    --ingredientItems[index].amount;

                    // If item has no value, destroy
                    if (ingredientItems[index].amount == 0)
                    {
                        Destroy(ingredientItems[index].gameObject);
                        ingredientItems.RemoveAt(index);
                        --index;
                    }
                }
            }
        }

        #endregion

        #region Dismantle Methods

        public bool Dismantle(Inventory inventory, out int[] overflows)
        {
            if (!dismantlable)
            {
                overflows = null;
                return false;
            }

            var reclaimIngredients = GetReclaimIngredients();
            if (reclaimIngredients == null)
            {
                overflows = null;
                return false;
            }

            List<int> overflowList = new();
            foreach (var reclaim in reclaimIngredients)
            {
                inventory.AddItem(reclaim.item, reclaim.amount, out int overflow);
                overflowList.Add(overflow);
            }

            overflows = overflowList.ToArray();
            return true;
        }

		public bool Dismantle(Vector3 position, Quaternion rotation, SpawnedAction onSpawnedAction, params object[] args)
		{
            return Dismantle(position, rotation, null, onSpawnedAction, args);
		}

		public bool Dismantle(Vector3 position, Quaternion rotation, Transform parent, SpawnedAction onSpawnedAction, params object[] args)
        {
			if (!dismantlable)
				return false;

			var reclaimIngredients = GetReclaimIngredients();
			if (reclaimIngredients == null)
				return false;

			foreach (var reclaim in reclaimIngredients)
			{
                reclaim.item.Instantiate(position, rotation, parent, onSpawnedAction, args);
			}
			return true;
		}

		public bool Dismantle(Transform parent, SpawnedAction onSpawnedAction, params object[] args)
		{
            return Dismantle(parent, false, onSpawnedAction, args);
		}

		public bool Dismantle(Transform parent, bool instantiateInWorldSpace, SpawnedAction onSpawnedAction, params object[] args)
        {
			if (!dismantlable)
				return false;

			var reclaimIngredients = GetReclaimIngredients();
			if (reclaimIngredients == null)
				return false;

			foreach (var reclaim in reclaimIngredients)
			{
                reclaim.item.Instantiate(parent, instantiateInWorldSpace, onSpawnedAction, args);
			}
			return true;
		}

        private Ingredient[] GetReclaimIngredients()
        {
            List<Ingredient> reclaimedIngredients = null;
            switch (m_dismantleMode)
            {
                case DismantleMode.Ingredients:
                    reclaimedIngredients = new List<Ingredient>(m_ingredients);
                    break;

                case DismantleMode.Scrap:
                    foreach (var scrap in m_scraps)
                    {
                        var reclaim = new Ingredient()
                        {
                            item = scrap.item
                        };

                        if (scrap.reclaimPercent < 1f)
                        {
                            for (int i = 0; i < scrap.amount; ++i)
                            {
                                if (Random.Range(0f, 1f) <= scrap.reclaimPercent)
                                {
                                    ++reclaim.amount;
                                }
                            }
                        }
                        else
                        {
                            reclaim.amount = scrap.amount;
                        }

                        reclaimedIngredients.Add(reclaim);
                    }
                    break;
            }
            return reclaimedIngredients.ToArray();
        }

        #endregion

        #region Structures

        [Serializable]
        public class Price
        {
            public CurrencyType currency;

            [Min(0)]
            public int amount;
        }

        [Serializable]
        public class Ingredient
        {
            public ItemType item;

            [Min(1)]
            public int amount = 1;
        }

        [Serializable]
        public class Scrap
        {
            public ItemType item;

            [Min(1)]
            public int amount = 1;

            [Range(0f, 1f), Tooltip("Percent chance to reclaim one resource.")]
            public float reclaimPercent = 1f;
        }

        #endregion
    }
}