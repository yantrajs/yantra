using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Objects;

namespace YantraJS.Core.Core.Storage
{

    #region Old

    //public struct ElementArray
    //{

    //    private JSProperty[] Storage;
    //    private UInt32Map<uint> Map;
    //    private bool IsSparse;
    //    private uint length;
    //    public ElementArray(int size)
    //    {
    //        Storage = new JSProperty[size < 16 ? 16 : size];
    //        Map = UInt32Map<uint>.Null;
    //        IsSparse = false;
    //        length = 0;
    //    }

    //    public bool IsNull => Storage == null;

    //    public JSProperty this[uint index]
    //    {
    //        get
    //        {
    //            if(!IsSparse)
    //            {
    //                if (index < Storage.Length)
    //                    return Storage[index];
    //            }
    //            if (Map.TryGetValue(index, out var i))
    //                return Storage[i];
    //            return JSProperty.Empty;
    //        }
    //        set
    //        {
    //            uint key;
    //            if (IsSparse)
    //            {
    //                if(Map.TryGetValue(index, out key))
    //                {
    //                    Storage[key] = value;
    //                    return;
    //                }
    //                key = length++;
    //                Map.Save(index, length);
    //                if (key >= Storage.Length)
    //                {
    //                    Array.Resize(ref Storage, Storage.Length + 8);
    //                }
    //                Storage[key] = value;
    //                return;
    //            }
    //            if (index < 64)
    //            {
    //                if (index >= length)
    //                {
    //                    length = index + 1;
    //                }

    //                // extend array...
    //                if (index < Storage.Length)
    //                {
    //                    Storage[index] = value;
    //                    return;
    //                }
    //                if(index < 32)
    //                {
    //                    Array.Resize(ref Storage, 32);
    //                } else
    //                {
    //                    Array.Resize(ref Storage, 64);
    //                }
    //                Storage[index] = value;
    //                return;
    //            }

    //            // create map...
    //            IsSparse = true;
    //            key = length++;
    //            Map.Save(index, key);
    //            if (key >= Storage.Length)
    //            {
    //                Array.Resize(ref Storage, Storage.Length + 8);
    //            }
    //            Storage[key] = value;
    //        }
    //    }

    //    public bool TryGetValue(uint key, out JSProperty value)
    //    {
    //        if (IsSparse)
    //        {
    //            if (Map.TryGetValue(key, out var index))
    //            {
    //                value = Storage[index];
    //                return !value.IsEmpty;
    //            }
    //            value = JSProperty.Empty;
    //            return false;
    //        }
    //        if (key < Storage.Length)
    //        {
    //            value = Storage[key];
    //        } else
    //        {
    //            value = JSProperty.Empty;
    //            return false;
    //        }
    //        return !value.IsEmpty;
    //    }

    //    public bool TryRemove(uint key, out JSProperty value)
    //    {
    //        if (IsSparse)
    //        {
    //            if (Map.TryRemove(key, out var index))
    //            {
    //                value = Storage[index];
    //                Storage[index] = JSProperty.Empty;
    //                return true;
    //            }
    //            value = JSProperty.Empty;
    //            return false;
    //        }
    //        if (key < Storage.Length)
    //        {
    //            value = Storage[key];
    //            Storage[key] = JSProperty.Empty;
    //            return true;
    //        }
    //        value = JSProperty.Empty;
    //        return false;
    //    }
    //    public bool RemoveAt(uint key)
    //    {
    //        if (IsSparse)
    //        {
    //            if(Map.TryRemove(key,out var index))
    //            {
    //                Storage[index] = JSProperty.Empty;
    //                return true;
    //            }
    //            return false;
    //        }
    //        if (key < Storage.Length)
    //        {
    //            Storage[key] = JSProperty.Empty;
    //            return true;
    //        }
    //        return false;
    //    }
    //    public IEnumerable<(uint Key, JSProperty Value)> AllValues()
    //    {
    //        if(IsSparse)
    //        {
    //            for(uint i = 0; i < length; i++)
    //            {
    //                if(Map.TryGetValue(i, out var v))
    //                {
    //                    yield return (i , Storage[v]);
    //                }
    //            }
    //            yield break;
    //        }
    //        if (Storage == null)
    //            yield break;
    //        for (uint i = 0; i < Storage.Length; i++)
    //        {
    //            yield return (i, Storage[i]);
    //        }
    //    }
    //    public bool HasKey(uint key)
    //    {
    //        if (IsSparse)
    //        {
    //            return Map.HasKey(key);
    //        }
    //        if (key >= length)
    //        {
    //            return false;
    //        }
    //        ref var p = ref Storage[key];
    //        return !p.IsEmpty;
    //    }
    //}

    //public struct ElementArray
    //{
    //    public UInt32Map<JSProperty> storage;

    //    public JSProperty this [uint index]
    //}
    #endregion

    public struct ElementArray
    {

        const int MaxArraySize = 1024 * 1024;

        private SAUint32Map<JSProperty> Storage;
        private JSValue[] array;

        private bool isDictionary;

        // private UInt32Map<JSProperty> Storage;
        private uint length;

        public uint Length => length;

        //public void Put(uint index, JSFunction getter, JSFunction setter, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        //{
        //    this.EnsureCapacity(index);
        //    Put(index) = JSProperty.Property(getter, setter, attributes);
        //}


        public void Put(uint index, JSValue value, JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            if (attributes != JSPropertyAttributes.EnumerableConfigurableValue)
            {
                this.TransferToDictionary();
            }
            else
            {
                this.EnsureCapacity(index + 1);
                if (array != null)
                {
                    array[index] = value;
                    return;
                }
            }
            Put(index) = JSProperty.Property(value, attributes);
        }

        public void Put(uint index, in JSProperty p)
        {
            this.Put(index, p.value, p.Attributes);
        }

        private void EnsureCapacity(uint size)
        {
            if (this.isDictionary)
            {
                return;
            }
            if (this.length > size)
            {
                return;
            }
            this.length = size;
            if (this.length < MaxArraySize)
            {
                if (this.array == null)
                {
                    int newLength = this.length < 4 ? 4 : ((((int)this.length >> 2) + 1) << 2);
                    // lets allocate...
                    this.array = new JSValue[newLength];
                    return;
                }
                if (this.array.Length < this.length)
                {
                    var length2 = (int)this.array.Length * 2;
                    var newLength = this.length < length2 ? length2 : (int)this.length;
                    if (newLength < MaxArraySize)
                    {
                        System.Array.Resize(ref this.array, newLength);
                        return;
                    }
                }
            }
            TransferToDictionary();
        }

        private void TransferToDictionary()
        {
            if (this.isDictionary)
            {
                return;   
            }
            this.isDictionary = true;
            var i = 0u;
            if (this.array == null)
            {
                return;
            }
            Storage.Resize((int)this.length);
            foreach (var item in this.array)
            {
                if (item == null)
                {
                    i++;
                    continue;
                }
                Storage.Put(i++) = JSProperty.Property(item, JSPropertyAttributes.EnumerableConfigurableValue);
            }
            this.array = null;
        }

        public void Put(uint index, JSValue value)
        {
            this.EnsureCapacity(index + 1);
            if (array != null)
            {
                array[index] = value;
                return;
            }
            Put(index) = JSProperty.Property( value, JSPropertyAttributes.EnumerableConfigurableValue);
        }

        private ref JSProperty Put(uint index)
        {
            if(index >= length)
            {
                length = index + 1;
            }
            return ref Storage.Put(index);
        }

        public JSProperty Get(uint index)
        {
            if (index >= length)
            {
                return default;
            }

            if (this.array != null)
            {
                var value = this.array[index];
                if (value == null)
                {
                    return default;
                }
                return JSProperty.Property(value, JSPropertyAttributes.EnumerableConfigurableValue);
            }

            if (!this.isDictionary)
            {
                return default;
            }
            ref var p = ref Storage.GetRefOrDefault(index, ref JSProperty.Empty);
            return p;
        }
        
        //public ref JSProperty Get(uint index)
        //{
        //    if (index >= length)
        //    {
        //        return ref JSProperty.Empty;
        //    }
        //    if (!this.isDictionary)
        //    {
        //        if(this.array == null)
        //        {
        //            return ref JSProperty.Empty;
        //        }
        //    }
        //    return ref Storage.GetRefOrDefault(index, ref JSProperty.Empty);
        //}

        public JSProperty this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // ref var p = ref Storage.GetRefOrDefault(index, ref JSProperty.Empty);
                return this.Get(index);
            }
            //[Obsolete("Use Put")]
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            //set
            //{
            //    if (index >= length)
            //    {
            //        length = index + 1;
            //    }
            //    Storage.Put(index) = value;
            //}
        }

        public bool IsNull => length == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(uint key, out JSProperty value)
        {
            if (key >= length)
            {
                value = default;
                return false;
            }
            if (this.array != null)
            {
                var v = this.array[key];
                if (v == null)
                {
                    value = default;
                    return false;
                }
                value = JSProperty.Property(v, JSPropertyAttributes.EnumerableConfigurableValue);
                return true;
            }
            return Storage.TryGetValue(key, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemove(uint key, out JSProperty value)
        {
            if (key >= length)
            {
                value = default;
                return false;
            }
            if (this.array != null)
            {
                var v = this.array[key];
                if (v == null)
                {
                    value = default;
                    return false;
                }
                this.array[key] = null;
                value = JSProperty.Property(v, JSPropertyAttributes.EnumerableConfigurableValue);
                return true;
            }
            return Storage.TryRemove(key, out value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveAt(uint key)
        {
            if (this.array != null)
            {
                if (key >= length)
                {
                    return false;
                }
                this.array[key] = null;
                return true;
            }
            return Storage.RemoveAt(key);
        }
        public IEnumerable<(uint Key, JSProperty Value)> AllValues()
        {
            if (this.array != null)
            {
                for (var i = 0u; i < this.length; i++)
                {
                    var v = this.array[i];
                    if (v == null)
                    {
                        continue;
                    }
                    yield return (i, JSProperty.Property(v, JSPropertyAttributes.EnumerableConfigurableValue));
                }
            }
            else
            {
                for (uint i = 0; i < length; i++)
                {
                    if (Storage.TryGetValue(i, out var v))
                    {
                        yield return (i, v);
                    }
                }
            }
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public bool HasKey(uint key)
        //{
        //    return Storage.HasKey(key);
        //}

        public void Resize(uint size)
        {
            if (length <= size)
            {
                if (this.isDictionary)
                {
                    this.length = size;
                    var n = size > 1024 ? 1024 : size;
                    Storage.Resize((int)n);
                } else
                {
                    this.EnsureCapacity(size);
                }
            }
        }

        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Sorts a array using the quicksort algorithm.
        /// </summary>
        /// <param name="comparer"> A comparison function. </param>
        /// <param name="start"> The first index in the range. </param>
        /// <param name="end"> The last index in the range. </param>
        internal void QuickSort(Comparison<JSValue> comparer, uint start, uint end)
        {
            if (end - start < 30)
            {
                // Insertion sort is faster than quick sort for small arrays.
                InsertionSort(comparer, start, end);
                return;
            }

            // Choose a random pivot.
            uint pivotIndex = start + (uint)(JSMath.RandomNumber() * (end - start));

            // Get the pivot value.
            var pivotValue = this[pivotIndex];

            // Send the pivot to the back.
            Swap(pivotIndex, end);

            // Sweep all the low values to the front of the array and the high values to the back
            // of the array.  This version of quicksort never gets into an infinite loop even if
            // the comparer function is not consistent.
            uint newPivotIndex = start;
            for (uint i = start; i < end; i++)
            {
                if (comparer(this[i].value, pivotValue.value) <= 0)
                {
                    Swap(i, newPivotIndex);
                    newPivotIndex++;
                }
            }

            // Swap the pivot back to where it belongs.
            Swap(end, newPivotIndex);

            // Quick sort the array to the left of the pivot.
            if (newPivotIndex > start)
                QuickSort(comparer, start, newPivotIndex - 1);

            // Quick sort the array to the right of the pivot.
            if (newPivotIndex < end)
                QuickSort(comparer, newPivotIndex + 1, end);
        }

        /// <summary>
        /// Sorts a array using the insertion sort algorithm.
        /// </summary>
        /// <param name="comparer"> A comparison function. </param>
        /// <param name="start"> The first index in the range. </param>
        /// <param name="end"> The last index in the range. </param>
        private void InsertionSort(Comparison<JSValue> comparer, uint start, uint end)
        {
            for (uint i = start + 1; i <= end; i++)
            {
                var value = this[i];
                uint j;
                for (j = i - 1; j > start && comparer(this[j].value, value.value) > 0; j--)
                    this.Put(j + 1,this[j]);

                // Normally the for loop above would continue until j < start but since we are
                // using uint it doesn't work when start == 0.  Therefore the for loop stops one
                // short of start then the extra loop iteration runs below.
                if (j == start && comparer(this[j].value, value.value) > 0)
                {
                    this.Put(j + 1, this[j]);
                    j--;
                }

                this.Put(j + 1, value);
            }
        }

        /// <summary>
        /// Swaps the elements at two locations in the array.
        /// </summary>
        /// <param name="index1"> The location of the first element. </param>
        /// <param name="index2"> The location of the second element. </param>
        private void Swap(uint index1, uint index2)
        {
            var temp = this[index1];
            this.Put(index1, this[index2]);
            this.Put(index2, temp);
        }

        internal void Initialize(JSValue[] items)
        {
            this.array = items;
            this.length = (uint)items.Length;
            this.isDictionary = false;
        }

        internal void Initialize(IList<JSValue> items)
        {
            this.array = new JSValue[items.Count];
            this.length = (uint)items.Count;
            var i = 0;
            foreach(var item in items) {
                this.array[i++] = item;
            }
            this.isDictionary = false;
        }

        internal void Initialize(List<JSValue> items)
        {
            this.array = new JSValue[items.Count];
            this.length = (uint)items.Count;
            var i = 0;
            foreach (var item in items)
            {
                this.array[i++] = item;
            }
            this.isDictionary = false;
        }
    }

}
