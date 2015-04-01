using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace TreeTestApp
{
	public partial class FolderView : UserControl
	{
		public FolderView()
		{
			InitializeComponent();
			m_folderTree.AddDrives();
		}
	}

	public class FolderViewTree : CommonTools.TreeListView
	{
		public enum eColumns
		{
			// this must match the order at which the columns are added
			Name = 0,
			CreatedTime = 1,
			Size = 2,
		}
		public enum eIncludeTypes
		{
			File		= 0x0001,
			Directory	= 0x0002,
			Hidden		= 0x0004,
			System		= 0x0008,
		};
		eIncludeTypes m_includeFlags = eIncludeTypes.File | eIncludeTypes.Directory | eIncludeTypes.Hidden;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public eIncludeTypes IncludeTypes
		{
			get { return m_includeFlags; }
			set { m_includeFlags = value;}
		}
		protected bool IsIncluded(eIncludeTypes type)
		{
			return (int)(m_includeFlags & type) > 0;
		}

		public FolderViewTree()
		{
			ContextMenuStrip = new ContextMenuStrip();

			ToolStripMenuItem item = new ToolStripMenuItem("Size - Bytes", null, new EventHandler(OnSizeSelect));
			item.Tag = eSizeDisplayType.Bytes;
			ContextMenuStrip.Items.Add(item);
			
			item = new ToolStripMenuItem("Size - KB", null, new EventHandler(OnSizeSelect));
			item.Tag = eSizeDisplayType.KBytes;
			ContextMenuStrip.Items.Add(item);
			
			item = new ToolStripMenuItem("Size - MB", null, new EventHandler(OnSizeSelect));
			item.Tag = eSizeDisplayType.MBytes;
			ContextMenuStrip.Items.Add(item);
			
			item = new ToolStripMenuItem("Size - Best", null, new EventHandler(OnSizeSelect));
			item.Tag = eSizeDisplayType.Best;
			ContextMenuStrip.Items.Add(item);
			ContextMenuStrip.Items.Add(new ToolStripSeparator());

			item = new ToolStripMenuItem("Full Scan", null, new EventHandler(OnFullScan));
			item.Tag = "fullScan";
			ContextMenuStrip.Items.Add(item);
		}
		public void AddDrives()
		{
			foreach (string drive in Directory.GetLogicalDrives())
			{
				DriveInfo info = new DriveInfo(drive);
				if (info.DriveType != DriveType.Fixed)
					continue; 
				CommonTools.Node drivenode = new CommonTools.Node();
				drivenode[(int)eColumns.Name] = drive;
				SetSize(drive, drivenode);
				try
				{
					AddPath(GetPath(drivenode), drivenode);
					drivenode.Expanded = true;
					Nodes.Add(drivenode);
				}
				catch {}
				{
				}
			}
		}
		protected virtual string GetModifiedTime(string path)
		{
			string lastaccess = Directory.GetLastAccessTime(path).ToShortDateString();
			lastaccess += " " + Directory.GetLastAccessTime(path).ToShortTimeString();
			return lastaccess;
		}
		protected virtual bool CanAdd(string path)
		{
			FileAttributes attr = File.GetAttributes(path);
			if ((int)(attr & FileAttributes.Hidden) > 0 && IsIncluded(eIncludeTypes.Hidden) == false)
				return false;
			if ((int)(attr & FileAttributes.System) > 0 && IsIncluded(eIncludeTypes.System) == false)
				return false;
			if ((int)(attr & FileAttributes.Directory) == 0 && IsIncluded(eIncludeTypes.File) == false)
				return false;
			return true;
		}
		void AddNode(string path, CommonTools.Node parent, bool isDirectory)
		{
			CommonTools.Node node = new CommonTools.Node();
			node[(int)eColumns.Size] = null;
			node[(int)eColumns.CreatedTime] = GetModifiedTime(path);
			node[(int)eColumns.Name] = Path.GetFileName(path);
			SetSize(path, node);
			parent.Nodes.Add(node);
			if (isDirectory)
			{
				node.HasChildren = AnySubDirs(GetPath(node));
				if (node.HasChildren == false && IsIncluded(eIncludeTypes.File))
					node.HasChildren = AnyFiles(GetPath(node));
			}
		}
		void AddPath(string originalPath, CommonTools.Node parent)
		{
			if (IsIncluded(eIncludeTypes.Directory))
			{
				string[] directories = Directory.GetDirectories(originalPath);
				foreach (string path in directories)
				{
					if (CanAdd(path) == false)
						continue;
					AddNode(path, parent, true);
				}
			}
			if (IsIncluded(eIncludeTypes.File))
			{
				string[] files = Directory.GetFiles(originalPath);
				foreach (string path in files)
				{
					if (CanAdd(path) == false)
						continue;
					AddNode(path, parent, false);
				}
			}
		}

		Dictionary<string, Bitmap> m_iconCache = new Dictionary<string,Bitmap>();
		protected override Image GetNodeBitmap(CommonTools.Node node)
		{
			string path = GetPath(node);
			string key = Path.GetExtension(path);
			if (key.Length == 0)
				key = path;
			Bitmap icon = null;
			if (key.Length > 0 && m_iconCache.TryGetValue(key, out icon))
				return icon;
			try
			{
				icon = CommonTools.IconUtil.GetIcon(path);
				m_iconCache[key] = icon;
				return icon;
			}
			catch 
			{
			}
			return null;
		}

		string GetPath(CommonTools.Node node)
		{
			StringBuilder sb = new StringBuilder();
			while (node != null)
			{
				if (node[(int)eColumns.Name] == null)
					return sb.ToString();

				if (node.Parent != null && node.Parent.Parent != null)
					sb.Insert(0, '\\' + node[(int)eColumns.Name].ToString());
				else
					sb.Insert(0, node[(int)eColumns.Name].ToString());
				node = node.Parent;
			}
			return sb.ToString();
		}
		bool AnySubDirs(string path)
		{
			// this should be optimized to return tru as soon as one file or sub dir is found
			try
			{
				if (Directory.GetDirectories(path).Length > 0)
					return true;
			}
			catch 
			{
			}
			return false;
		}
		bool AnyFiles(string path)
		{
			// this should be optimized to return tru as soon as one file or sub dir is found
			try
			{
				if (Directory.GetFiles(path).Length > 0)
					return true;
			}
			catch 
			{
			}
			return false;
		}

		public override void OnNotifyBeforeExpand(CommonTools.Node node, bool expanding)
		{
			if (node.Nodes.Count == 0)
				AddPath(GetPath(node), node);
		}
		
		enum eSizeDisplayType
		{
			Bytes,
			KBytes,
			MBytes,
			Best,
		}
		eSizeDisplayType m_sizeDisplayType = eSizeDisplayType.Best;
		class SizeInfo
		{
			public enum SizeType
			{
				Complete,
				Partial,
			}
			public SizeType Type;
			public long Size;
			public SizeInfo(long size, SizeType type)
			{
				Size = size;
				Type = type;
			}
		}
		
		string GetSizeString(long size, eSizeDisplayType displayType)
		{
			double gb = 1024*1024*1024;
			double mb = 1024*1024;
			double kb = 1024;

			if (displayType == eSizeDisplayType.Bytes)
				return string.Format("{0} bytes", size);
			if (displayType == eSizeDisplayType.KBytes)
				return string.Format("{0} KB", (int)Math.Ceiling((double)size / kb));
			if (displayType == eSizeDisplayType.MBytes)
			{
				double dsize = Math.Round((double)size / mb, 2);
				return string.Format("{0:f2} MB", dsize);
			}
			if (displayType == eSizeDisplayType.Best)
			{
				long sizekb = (long)Math.Ceiling((double)size / kb);
				if (size <= 100)
					return string.Format("{0} KB", sizekb);

				if (size > (100 * gb))
					return string.Format("{0:f2} GB", Math.Round((double)size / gb, 2));
				if (size > (100 * mb))
					return string.Format("{0:f2} MB", Math.Round((double)size / mb, 2));
				return string.Format("{0} KB", (int)Math.Ceiling(size / kb));
			}
			return string.Format("{0} KB", (int)Math.Ceiling(size / kb));
		}
		string GetSizeString(CommonTools.Node node, eSizeDisplayType displayType)
		{
			object data = node[(int)eColumns.Size];
			if (data == null)
				return string.Empty;
			if (data is string)
				return data.ToString();
			
			SizeInfo info = data as SizeInfo;
			if (info.Type == SizeInfo.SizeType.Partial)
			{
				return "> 2GB";
			}
			return GetSizeString(info.Size, displayType);
		}
		void SetSize(string fullpath, CommonTools.Node node)
		{
			string root = Path.GetPathRoot(fullpath);
			if (root == fullpath)
			{
				DriveInfo info = new DriveInfo(root);
				double freesize = info.TotalFreeSpace;
				double usedsize = info.TotalSize - info.TotalFreeSpace;
				node[(int)eColumns.Size] = GetSizeString((long)freesize, eSizeDisplayType.Best) + " free of " + GetSizeString((long)info.TotalSize, eSizeDisplayType.Best);
				return;
			}
			FileInfo fileinfo = new FileInfo(fullpath);
			if ((int)(fileinfo.Attributes & FileAttributes.Directory) == 0)
				node[(int)eColumns.Size] = new SizeInfo(fileinfo.Length, SizeInfo.SizeType.Complete);
		}
		void OnSizeSelect(object sender, EventArgs e)
		{
			ToolStripItem item = sender as ToolStripItem;
			m_sizeDisplayType = (eSizeDisplayType)item.Tag;
			Invalidate();
		}
		
		protected override void BeforeShowContextMenu()
		{
			SizeInfo sizeinfo = FocusedNode[(int)eColumns.Size] as SizeInfo;
			bool fullScanEnabled = sizeinfo != null && sizeinfo.Type == SizeInfo.SizeType.Partial;
			FindContextMenuItem("fullScan").Enabled = fullScanEnabled;
		}
		ToolStripItem FindContextMenuItem(string tagname)
		{
			foreach (ToolStripItem item in ContextMenuStrip.Items)
			{
				if (item.Tag is string && (string)item.Tag == tagname)
					return item;
			}
			return null;
		}
		
		List<CommonTools.Node> m_foldersToScan = new List<CommonTools.Node>();
		void AddFolderToScan(CommonTools.Node node)
		{
			bool startscan = false;
			lock (m_foldersToScan)
			{
				if (m_foldersToScan.Contains(node))
					return;
				node[(int)eColumns.Size] = "Scanning...";
				startscan = m_foldersToScan.Count == 0;
				m_foldersToScan.Add(node);
			}
			Invalidate();
			if (startscan)
			{
			System.Threading.Thread thread = new System.Threading.Thread(DoScan);
			thread.Start();
			}
		}
		long GetScanSize(DirectoryInfo info, long parentsize, long maxsize, ref bool exceededMax)
		{
			long size = 0;
			try
			{
				FileInfo[] files = info.GetFiles();
				foreach (FileInfo finfo in files)
				{
					size += finfo.Length;
					if (size > maxsize)
					{
						exceededMax = true;
						break;
					}
				}

				DirectoryInfo[] dinfos = info.GetDirectories();
				foreach (DirectoryInfo dinfo in dinfos)
				{
					size += GetScanSize(dinfo, size, maxsize, ref exceededMax);
					if (size > maxsize || exceededMax)
					{
						exceededMax = true;
						break;
					}
				}
			}
			catch
			{
			}
			return size;
		}
		delegate void DoScanHandler(CommonTools.Node node, long maxsize);
		void DoScan(CommonTools.Node node, long maxsize)
		{
			node[(int)eColumns.Size] = "Scanning...";
			this.BeginInvoke(new MethodInvoker(Invalidate));

			string fullpath = GetPath(node);
			bool exceededMax = false;
			long size = GetScanSize(new DirectoryInfo(fullpath), 0, maxsize, ref exceededMax);
			if (exceededMax)
				node[(int)eColumns.Size] = new SizeInfo(size, SizeInfo.SizeType.Partial);
			else
				node[(int)eColumns.Size] = new SizeInfo(size, SizeInfo.SizeType.Complete);
			this.BeginInvoke(new MethodInvoker(Invalidate));
		}
		void DoScan()
		{
			while (true)
			{
				CommonTools.Node node = null;
				lock (m_foldersToScan)
				{
					if (m_foldersToScan.Count == 0)
						break;
					node = m_foldersToScan[0];
				}
				
				// scan
				DoScan(node, 2000000000);
				
				lock (m_foldersToScan)
				{
					this.BeginInvoke(new MethodInvoker(Invalidate));
					if (m_foldersToScan.Count == 0)
						break;
					m_foldersToScan.RemoveAt(0);
				}
			}
		}
		void OnFullScan(object sender, EventArgs e)
		{
			SizeInfo sizeinfo = FocusedNode[(int)eColumns.Size] as SizeInfo;
			if (sizeinfo != null && sizeinfo.Type == SizeInfo.SizeType.Partial)
			{
				DoScanHandler d = new DoScanHandler(DoScan);
				d.BeginInvoke(FocusedNode, long.MaxValue, null, null);
			}
		}
		protected override void OnAfterSelect(CommonTools.Node node)
		{
			if (node[(int)eColumns.Size] != null)
				return;

			string fullpath = GetPath(node);
			string root = Path.GetPathRoot(fullpath);

			FileInfo info = new FileInfo(fullpath);
			if ((int)(info.Attributes & FileAttributes.Directory) > 0)
				AddFolderToScan(node);
		}
		protected override object GetData(CommonTools.Node node, CommonTools.TreeListColumn column)
		{
			object data = base.GetData(node, column);
			if (data is string)
				return data;
			if (data != null && column.Index == (int)eColumns.Size)
				return GetSizeString(node, m_sizeDisplayType);
			return data;
		}
	}

}