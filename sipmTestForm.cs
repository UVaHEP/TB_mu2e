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

			//Setup the colors and other settings for each of the buttons and labels 

			this.sipm1.BackColor = Color.Red; 
			this.sipm2.BackColor = Color.Red; 
			this.sipm3.BackColor = Color.Red; 
			this.sipm4.BackColor = Color.Red; 
			this.sipm5.BackColor = Color.Red; 
			this.sipm6.BackColor = Color.Red; 
			this.sipm7.BackColor = Color.Red; 
			this.sipm8.BackColor = Color.Red; 
			this.connectBtn.BackColor = Color.Red; 

			this.FEBConnected.ForeColor = Color.Red; 



		}


		//Each of the SiPMClicks will eventually open a form containing the data for the previous measurement sequence
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
