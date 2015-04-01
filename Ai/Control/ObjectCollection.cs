// Ai Software Control Library.
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace Ai.Control {
    /// <summary>
    /// A class to represent a collection of objects.
    /// </summary>
    public class ObjectCollection : CollectionBase {
        public ObjectCollection() : base() { }
        /// <summary>
        /// Gets an object in the collection specified by its index.
        /// </summary>
        [Description("Gets an object in the collection specified by its index.")]
        public object this[int index] {
            get {
                if (index >= 0 && index < List.Count) return List[index];
                return null;
            }
        }
        /// <summary>
        /// Gets the index of an object in the collection.
        /// </summary>
        [Description("Gets the index of an object in the collection.")]
        public int IndexOf(object obj) { return List.IndexOf(obj); }
        /// <summary>
        /// Add an object to the collection.
        /// </summary>
        [Description("Add an object to the collection.")]
        public object Add(object obj) {
            int index = List.Add(obj);
            return List[index];
        }
        /// <summary>
        /// Add an object collection to the collection.
        /// </summary>
        [Description("Add an object collection to the collection."),
            System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public void AddRange(CollectionBase objects) {
            foreach (object obj in objects) this.Add(obj);
        }
        /// <summary>
        /// Insert an object to the collection at specified index.
        /// </summary>
        [Description("Insert an object to the collection at specified index.")]
        public void Insert(int index, object obj) {
            List.Insert(index, obj);
        }
        /// <summary>
        /// Remove an object from the collection.
        /// </summary>
        [Description("Remove an object from the collection.")]
        public void Remove(object obj) {
            if (List.Contains(obj)) List.Remove(obj);
        }
        /// <summary>
        /// Determine whether an object exist in the collection.
        /// </summary>
        /// <returns></returns>
        [Description("Determine whether an object exist in the collection.")]
        public bool Contains(object obj) { return List.Contains(obj); }
    }
}