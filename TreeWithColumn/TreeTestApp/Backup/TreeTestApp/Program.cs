using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TreeTestApp
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			CommonTools.Tracing.EnableTrace();
			CommonTools.Tracing.AddId(0);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
			//Application.Run(new Form2());
			//Application.Run(new FolderView());

			CommonTools.Tracing.Terminate();
		}
	}
}