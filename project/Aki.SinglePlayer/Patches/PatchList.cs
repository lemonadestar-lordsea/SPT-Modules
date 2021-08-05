using System;
using System.Collections;
using System.Collections.Generic;

namespace Aki.SinglePlayer.Patches
{
    public class PatchList : IEnumerable<Patch>
    {
        private readonly List<Patch> _list;

        /// <summary>
        /// Amount of items in list
        /// </summary>
        public int Count
        {
            get => _list.Count;
        }

        public PatchList()
        {
            _list = new List<Patch>();
        }

        /// <summary>
        /// Enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<Patch> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Get patch from list
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Patch</returns>
        private Patch Get(Type type)
        {
            foreach (Patch item in _list)
            {
                if (item.GetType() == type)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Get patch from list
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Patch</returns>
        public Patch Get<T>() where T : Patch
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// Is patch in the list?
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>If the patch is in the list</returns>
        public bool Contains<T>() where T : Patch
        {
            return (Get(typeof(T)) != null);
        }

        /// <summary>
        /// Is patch in the list?
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>If the patch is in the list</returns>
        private bool Contains(Type type)
        {
            return (Get(type) != null);
        }

        /// <summary>
        /// Add patch to list
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public void Add<T>() where T : Patch, new()
        {
            if (!Contains<T>())
            {
                _list.Add(new T());
            }
        }

        /// <summary>
        /// Add patch to list
        /// </summary>
        /// <param name="patch">Patch</param>
        public void Add(Patch patch)
        {
            if (!Contains(patch.GetType()))
            {
                _list.Add(patch);
            }
        }

        /// <summary>
        /// Remove patch from list
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public void Remove<T>() where T : Patch
        {
            if (Contains<T>())
            {
                _list.Remove(Get<T>());
            }
        }

        /// <summary>
        /// Remove all patches from list
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Enable patch
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public void Enable<T>() where T : Patch
        {
            Patch patch = Get<T>();

            if (patch != null)
            {
                patch.Enable();
            }
        }

        /// <summary>
        /// Enable all patches
        /// </summary>
        public void EnableAll()
        {
            foreach (Patch patch in _list)
            {
                patch.Enable();
            }
        }

        /// <summary>
        /// Disable patch
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public void Disable<T>() where T : Patch
        {
            Patch patch = Get<T>();

            if (patch != null)
            {
                patch.Disable();
            }
        }

        /// <summary>
        /// Disable all patches
        /// </summary>
        public void DisableAll()
        {
            foreach (Patch patch in _list)
            {
                patch.Disable();
            }
        }
    }
}
