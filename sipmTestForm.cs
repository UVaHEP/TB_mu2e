/*
 * Created by SharpDevelop.
 * User: hepadmin
 * Date: 12/19/2016
 * Time: 4:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Mu2e_SiPM_Test
{
	/// <summary>
	/// Description of sipmTestForm.
	/// </summary>
	public partial class sipmTestForm : Form
	{
		public sipmTestForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void Sipm1Click(object sender, EventArgs e)
		{
			Debug.WriteLine("Sipm 1");
		}
		void Sipm2Click(object sender, EventArgs e)
		{
			Debug.WriteLine("Sipm 2");
		}
		void Sipm3Click(object sender, EventArgs e)
		{
			Debug.WriteLine("Sipm 3");
		}
		void Sipm4Click(object sender, EventArgs e)
		{
			Debug.WriteLine("Sipm 4");
		}
		void Sipm5Click(object sender, EventArgs e)
		{
			Debug.WriteLine("Sipm 5");
		}
		void Sipm6Click(object sender, EventArgs e)
		{
			Debug.WriteLine("Sipm 6");
		}
		void Sipm7Click(object sender, EventArgs e)
		{
			Debug.WriteLine("Sipm 7");
		}
		void Sipm8Click(object sender, EventArgs e)
		{
			Debug.WriteLine("Sipm 8");
		}
		void AutomeasureClick(object sender, EventArgs e)
		{
			Debug.WriteLine("AutoMeasure");
		}
		void LastMeasureClick(object sender, EventArgs e)
		{
			Debug.WriteLine("Opening Last Measurement Form");
		}
		void ConnectClick(object sender, EventArgs e)
		{
			Debug.WriteLine("Connecting");
		}
		
		
	}
}
