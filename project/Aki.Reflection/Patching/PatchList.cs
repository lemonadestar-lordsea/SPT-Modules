using System;
using System.Collections;
using System.Collections.Generic;

namespace Aki.Reflection.Patching
{
    public class PatchList : IEnumerable<ModulePatch>
    {
        private readonly List<ModulePatch> _list;

        /// <summary>
        /// Amount of items in list
        /// </summary>
        public int Count
        {
            get => _list.Count;
        }

        public PatchList()
        {
            _list = new List<ModulePatch>();
        }

        /// <summary>
        /// Enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<ModulePatch> GetEnumerator()
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
        private ModulePatch Get(Type type)
        {
            foreach (ModulePatch item in _list)
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
        public ModulePatch Get<T>() where T : ModulePatch
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// Is patch in the list?
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>If the patch is in the list</returns>
        public bool Contains<T>() where T : ModulePatch
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
        public void Add<T>() where T : ModulePatch, new()
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
        public void Add(ModulePatch patch)
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
        public void Remove<T>() where T : ModulePatch
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
        public void Enable<T>() where T : ModulePatch
        {
            ModulePatch patch = Get<T>();

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
            foreach (ModulePatch patch in _list)
            {
                patch.Enable();
            }
        }

        /// <summary>
        /// Disable patch
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public void Disable<T>() where T : ModulePatch
        {
            ModulePatch patch = Get<T>();

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
            foreach (ModulePatch patch in _list)
            {
                patch.Disable();
            }
        }
    }
}
