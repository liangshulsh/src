// Ai Software Control Library.
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace Ai.Control {
    /// <summary>
    /// Class to provides data for the collection event.
    /// </summary>
    public class CollectionEventArgs : EventArgs {
        /// <summary>
        /// Determine the event type raised by the collection.
        /// </summary>
        [Description("Determine the event type raised by the collection.")]
        public enum EventType { 
            OnClear,
            OnClearComplete,
            OnInsert,
            OnInsertComplete,
            OnRemove,
            OnRemoveComplete,
            OnSet,
            OnSetComplete
        }
        int _index = -1;
        EventType _type = EventType.OnClear;
        object _item = null;
        object _oldValue = null;
        object _newValue = null;
        /// <summary>
        /// Create an instance of CollectionEventArgs that have no arguments.
        /// </summary>
        public CollectionEventArgs() : base() { }
        /// <summary>
        /// Create an instance of CollectionEventArgs that have type arguments.
        /// </summary>
        public CollectionEventArgs(EventType type) : base() { _type = type; }
        /// <summary>
        /// Create an instance of CollectionEventArgs that have index and item arguments.
        /// </summary>
        public CollectionEventArgs(EventType type, int index, object item) : base() {
            _type = type;
            _index = index;
            _item = item;
        }
        /// <summary>
        /// Create an instance of CollectionEventArgs that have index, oldvalue, and newvalue arguments.
        /// </summary>
        public CollectionEventArgs(EventType type, int index, object oldValue, object newValue) : base() {
            _type = type;
            _index = index;
            _oldValue = oldValue;
            _newValue = newValue;
        }
        /// <summary>
        /// Gets the type of the event raised.
        /// </summary>
        public EventType Type {
            get { return _type; }
        }
        /// <summary>
        /// Gets the item index in the collection.
        /// </summary>
        public int Index {
            get { return _index; }
        }
        /// <summary>
        /// Gets the item of the collection associated with the event.
        /// </summary>
        public object Item {
            get { return _item; }
        }
        /// <summary>
        /// Gets the old value of the item associated with the event.
        /// </summary>
        public object OldValue {
            get { return _oldValue; }
        }
        /// <summary>
        /// Gets the new value of the item associated with the event.
        /// </summary>
        public object NewValue {
            get { return _newValue; }
        }
    }
}