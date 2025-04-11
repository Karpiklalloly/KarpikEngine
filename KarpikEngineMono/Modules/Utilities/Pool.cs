using System;
using System.Runtime.CompilerServices;

namespace KarpikEngineMono.Modules.Utilities
{
    //DragonECS-based
    public class Pool<T>
        where T : struct
    {
        public static Pool<T> Instance { get; } = new();
    
        private int[] _mapping;// index = entityID / value = itemIndex;/ value = 0 = no entityID
        private T[] _items;
        private int[] _recycledItems;
        private int _itemsCount;
        private int _recycledItemsCount = 0;

        public Pool()
        {
            _mapping = new int[4];
            _items = new T[4];
            _recycledItems = new int[4];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Add(int id)
        {
            ValidateMapping(id);
            ref int itemIndex = ref _mapping[id];
#if (DEBUG && !DISABLE_DEBUG) || ENABLE_DRAGONECS_ASSERT_CHEKS
            if (itemIndex > 0)
            {
                throw new Exception(id.ToString());
            }
#endif
            if (_recycledItemsCount > 0)
            {
                itemIndex = _recycledItems[--_recycledItemsCount];
                _itemsCount++;
            }
            else
            {
                itemIndex = ++_itemsCount;
                if (itemIndex >= _items.Length)
                {
                    Array.Resize(ref _items, _items.Length << 1);
                }
            }
            ref var result = ref _items[itemIndex];
            return ref result;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int id)
        {
            ValidateMapping(id);
#if (DEBUG && !DISABLE_DEBUG)
            if (!Has(id)) { throw new Exception(id.ToString()); }
#endif
            return ref _items[_mapping[id]];
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T Read(int entityID)
        {
#if (DEBUG && !DISABLE_DEBUG)
            if (!Has(entityID)) { throw new Exception(entityID.ToString()); }
#endif
            return ref _items[_mapping[entityID]];
        }
        
        public ref T TryAddOrGet(int id)
        {
            ValidateMapping(id);
            ref int itemIndex = ref _mapping[id];
            if (itemIndex <= 0)
            {
                if (_recycledItemsCount > 0)
                {
                    itemIndex = _recycledItems[--_recycledItemsCount];
                    _itemsCount++;
                }
                else
                {
                    itemIndex = ++_itemsCount;
                    if (itemIndex >= _items.Length)
                    {
                        Array.Resize(ref _items, _items.Length << 1);
                    }
                }
            }
            return ref _items[itemIndex];
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int id)
        {
            ValidateMapping(id);
            return _mapping[id] > 0;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Del(int id)
        {
            ref int itemIndex = ref _mapping[id];
#if (DEBUG && !DISABLE_DEBUG)
            if (itemIndex <= 0) { throw new Exception(id.ToString()); }
#endif
            if (_recycledItemsCount >= _recycledItems.Length)
            {
                Array.Resize(ref _recycledItems, _recycledItems.Length << 1);
            }
            _recycledItems[_recycledItemsCount++] = itemIndex;
            itemIndex = 0;
            _itemsCount--;
        }
    
        public void TryDel(int id)
        {
            if (Has(id))
            {
                Del(id);
            }
        }
    
        public void ClearAll()
        {
            _recycledItemsCount = 0;
            if (_itemsCount <= 0) { return; }
            for (int i = 0; i < _mapping.Length; i++)
            {
                TryDel(i);
            }
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Copy(ref T from, ref T to)
        {
            to = from;
        }

        private void ValidateMapping(int id)
        {
            if (_mapping.Length <= id)
            {
                Array.Resize(ref _mapping, _mapping.Length << 1);
            }
        }
    }
}