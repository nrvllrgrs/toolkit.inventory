using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Inventory
{
    [CreateAssetMenu(menuName = "Toolkit/Inventory/Recipes")]
    public class Recipes : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private ItemType[] m_items = default;

        #endregion

        #region Properties

        public ItemType[] items => m_items;

        #endregion

        #region Methods

        public ItemType[] GetRecipesByAnyIngredient(Inventory inventory)
        {
            return GetRecipesByAnyIngredient(inventory.items.Select(x => x.slotType));
        }

        public ItemType[] GetRecipesByAnyIngredient(IEnumerable<ItemType> itemTypes)
        {
            // Get items from recipes that contain any ingredient from itemTypes
            return m_items.Where(item => item.ingredients.Select(ingredient => ingredient.item).Intersect(itemTypes).Any()).ToArray();
        }

        public ItemType[] GetRecipesByAnyIngredient(IEnumerable<Item> items)
        {
            return GetRecipesByAnyIngredient(items.Select(x => x.itemType));
        }

        public ItemType[] GetRecipesByAllIngredients(Inventory inventory)
        {
            return GetRecipesByAllIngredients(inventory.items.Select(x => x.slotType));
        }

        public ItemType[] GetRecipesByAllIngredients(IEnumerable<ItemType> itemTypes)
        {
            return m_items.Where(x => !x.ingredients.Select(y => y.item).Except(itemTypes).Any()).ToArray();
        }

        public ItemType[] GetRecipesByAllIngredients(IEnumerable<Item> items)
        {
            return GetRecipesByAllIngredients(items.Select(x => x.itemType));
        }

        #endregion
    }
}