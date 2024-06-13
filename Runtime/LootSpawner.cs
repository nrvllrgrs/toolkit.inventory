using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static ToolkitEngine.ObjectSpawner;

namespace ToolkitEngine.Inventory
{
	[RequireComponent(typeof(Loot))]
    public class LootSpawner : MonoBehaviour
    {
		#region Fields

		[SerializeField]
		private Set[] m_sets;

		[SerializeField]
		private Transform[] m_points = new Transform[] { };

		[SerializeField]
		private OrderMode m_order;

		[SerializeField]
		private Transform m_parent;

		[SerializeField]
		private SpawnSpace m_spawnSpace = SpawnSpace.SpawnAtPoint;

		[SerializeField]
		private Vector3 m_position, m_rotation;

		[SerializeField, Tooltip("Indicates whether spawns on start.")]
		protected bool m_spawnOnStart;

		private Loot m_loot;
		private int m_index;
		private IList<Transform> m_availablePoints = new List<Transform>();

		#endregion

		#region Events

		public UnityEvent<SpawnerEventArgs> m_onSpawned;

		#endregion

		#region Properties

		//public UnityEvent<SpawnerEventArgs> onSpawning => m_spawner.onSpawning;
		public UnityEvent<SpawnerEventArgs> onSpawned => m_onSpawned;
		//public UnityEvent<SpawnerEventArgs> onDespawned => m_spawner.onDespawned;

		public bool hasPoints => m_spawnSpace == SpawnSpace.SpawnAtPoint && (m_points?.Length ?? 0) > 0;
		public Transform[] points
		{
			get
			{
				if (m_spawnSpace != SpawnSpace.SpawnAtPoint)
					return new Transform[] { };

				return m_points != null && m_points.Length > 0
					? m_points
					: new Transform[] { transform };
			}
		}

		#endregion

		#region Methods

		protected virtual void Awake()
		{
			m_loot = GetComponent<Loot>();
			m_availablePoints = new List<Transform>(m_points);
		}

		protected virtual void Start()
		{
			if (m_spawnOnStart)
			{
				Spawn();
			}
		}

		#endregion

		#region Spawn Methods

		public void Spawn()
		{
			var drops = m_loot.Generate();
			switch (m_spawnSpace)
			{
				case SpawnSpace.SpawnAtPoint:
					foreach (var drop in drops)
					{
						InstantiateNext(drop);
					}
					break;

				case SpawnSpace.SpawnInLocalSpace:
					foreach (var drop in drops)
					{
						Instantiate(drop, m_parent.position, Quaternion.Euler(m_rotation), m_parent);
					}
					break;

				case SpawnSpace.SpawnInWorldSpace:
					foreach (var drop in drops)
					{
						Instantiate(drop, Vector3.zero, Quaternion.identity, m_parent);
					}
					break;

				case SpawnSpace.PositionAndRotation:
					foreach (var drop in drops)
					{
						Instantiate(drop, m_position, Quaternion.Euler(m_rotation), m_parent);
					}
					break;
			}
		}

		private void InstantiateNext(DropEntry drop)
		{
			if (m_spawnSpace != SpawnSpace.SpawnAtPoint)
				return;

			Transform point = null;
			if (!hasPoints)
			{
				point = transform;
			}
			else
			{
				switch (m_order)
				{
					case OrderMode.Sequence:
						point = m_points[m_index];
						m_index = (m_index + 1).Mod(m_points.Length);
						break;

					case OrderMode.Random:
						point = m_points[Random.Range(0, m_points.Length)];
						break;

					case OrderMode.RandomWithoutRepeating:
						if (m_index == 0)
						{
							m_availablePoints = m_availablePoints.Shuffle();
						}
						point = m_availablePoints[m_index];
						m_index = (m_index + 1).Mod(m_points.Length);
						break;
				}
			}

			if (point != null)
			{
				Instantiate(drop, point.position, point.rotation, null);
			}
		}

		private void Instantiate(DropEntry drop, Vector3 position, Quaternion rotation, Transform parent)
		{
			switch (drop.dropType)
			{
				case DropEntry.DropType.Item:
					drop.itemType.Instantiate(position, rotation, parent, PostSpawn);
					break;
			}
		}
		
		private void PostSpawn(GameObject spawnedObject, params object[] args)
		{
			foreach (var set in m_sets)
			{
				set.Add(spawnedObject);
			}

			m_onSpawned?.Invoke(new SpawnerEventArgs(null, spawnedObject));
		}

		#endregion
	}
}