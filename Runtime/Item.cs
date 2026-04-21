using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Inventory
{
	public class Item : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private ItemType m_itemType;

		[SerializeField, Min(0)]
		private int m_amount = 1;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<Item> m_onChanged;

		[SerializeField]
		private UnityEvent<ItemEventArgs> m_onCollected;

		#endregion

		#region Properties

		public ItemType itemType
		{
			get => m_itemType;
			set
			{
				// No change, skip
				if (m_itemType == value)
					return;

				m_itemType = value;
				m_onChanged?.Invoke(this);
			}
		}

		public int amount
		{
			get => m_amount;
			set
			{
				// No change, skip
				if (m_amount == value)
					return;

				m_amount = value;
				m_onChanged?.Invoke(this);
			}
		}

		public UnityEvent<Item> onChanged => m_onChanged;
		public UnityEvent<ItemEventArgs> onCollected => m_onCollected;

		#endregion

		#region Methods

		public void Set(ItemType itemType, int amount = 1)
		{
			m_itemType = itemType;
			m_amount = amount;
			m_onChanged?.Invoke(this);
		}

		public void Collect(InventoryList inventory)
		{
			if (inventory == null)
				return;

			if (inventory.AddItem(this))
			{
				Set(null, 0);
				m_onCollected?.Invoke(new ItemEventArgs(inventory, null));
			}
		}

		#endregion

		#region Dismantle Methods

		public bool Dismantle()
		{
			return Dismantle(transform.position, transform.rotation, null);
		}

		public bool Dismantle(InventoryList inventory, out int[] overflows)
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