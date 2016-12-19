/*
 * Created by SharpDevelop.
 * User: hepadmin
 * Date: 12/19/2016
 * Time: 4:58 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Diagnostics;


namespace Mu2e_SiPM_Test
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			

			Debug.WriteLine("Running Mu2e_SiPM_Test");
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new sipmTestForm());
		}
		
	}
}
