// Ai Software Library.

using System;
using System.Runtime.InteropServices;

namespace Ai.Control {
    /// <summary>
    /// Create a ghost image of the drag drop operation.  http://www.codeproject.com/KB/tree/TreeViewDragDrop.aspx
    /// </summary>
    internal class DragHelper {
        [DllImport("comctl32.dll")]
        public static extern bool InitCommonControls();

        /// <summary>
        /// Creates a temporary image list that is used for dragging.
        /// </summary>
        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_BeginDrag(IntPtr hImlTrack, int iTrack, int dxHotSpot, int dyHotSpot);

        /// <summary>
        /// Moves the image that is being dragged during a drag-and-drop operation.
        /// </summary>
        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragMove(int x, int y);

        /// <summary>
        /// Ends a drag operation.
        /// </summary>
        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern void ImageList_EndDrag();

        /// <summary>
        /// Displays the drag image at the specified position within the window.
        /// </summary>
        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragEnter(IntPtr hwndLock, int x, int y);

        /// <summary>
        /// Unlocks the specified window and hides the drag image, allowing the window to be updated.
        /// </summary>
        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragLeave(IntPtr hwndLock);

        /// <summary>
        /// Shows or hides the image being dragged.
        /// </summary>
        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragShowNolock(bool fShow);

        public enum DragEffect { None, Stop, Swap, Insert}

        static DragHelper() { InitCommonControls(); }
    }
}