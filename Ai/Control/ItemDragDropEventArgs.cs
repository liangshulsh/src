using System;
using System.Windows.Forms;

namespace Ai.Control {
    public class ItemDragDropEventArgs : EventArgs {
        object _itemsSource;
        object _itemTarget;
        object _itemHover;
        int _keyState;
        DragDropEffects _effects;
        public ItemDragDropEventArgs(object sources, object target, object hover, int keyState) : base() {
            _itemsSource = sources;
            _itemTarget = target;
            _itemHover = hover;
            _keyState = keyState;
            _effects = DragDropEffects.None;
        }
        public object ItemsSource { get { return _itemsSource; } }
        public object ItemTarget { get { return _itemTarget; } }
        public object ItemHover { get { return _itemHover; } }
        public int KeyState { get { return _keyState; } }
        public DragDropEffects Effects {
            get { return _effects; }
            set { _effects = value; }
        }
    }
}