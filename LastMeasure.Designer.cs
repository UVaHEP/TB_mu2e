/*
 * Created by SharpDevelop.
 * User: hepadmin
 * Date: 12/19/2016
 * Time: 5:42 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Mu2e_SiPM_Test
{
	partial class LastMeasure
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn SiPM;
		private System.Windows.Forms.DataGridViewTextBoxColumn IMeasured;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.SiPM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.IMeasured = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
			this.SiPM,
			this.IMeasured});
			this.dataGridView1.Location = new System.Drawing.Point(12, 41);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(260, 209);
			this.dataGridView1.TabIndex = 0;
			// 
			// SiPM
			// 
			this.SiPM.HeaderText = "SiPM #";
			this.SiPM.Name = "SiPM";
			// 
			// IMeasured
			// 
			this.IMeasured.HeaderText = "Measured Current";
			this.IMeasured.Name = "IMeasured";
			// 
			// LastMeasure
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.dataGridView1);
			this.Name = "LastMeasure";
			this.Text = "Last Averaged Measurements";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);

		}
	}
}
