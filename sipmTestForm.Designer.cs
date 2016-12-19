/*
 * Created by SharpDevelop.
 * User: hepadmin
 * Date: 12/19/2016
 * Time: 4:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Mu2e_SiPM_Test
{
	partial class sipmTestForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TextBox hostname;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button sipm1;
		private System.Windows.Forms.Button sipm2;
		private System.Windows.Forms.Button sipm3;
		private System.Windows.Forms.Button sipm4;
		private System.Windows.Forms.Button sipm5;
		private System.Windows.Forms.Button sipm6;
		private System.Windows.Forms.Button sipm7;
		private System.Windows.Forms.Button sipm8;
		private System.Windows.Forms.Button automeasureBtn;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown avgCount;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripProgressBar MeasureProgress;
		private System.Windows.Forms.ToolStripStatusLabel FEBConnected;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox dicounter;
		private System.Windows.Forms.Button lastMeasure;
		private System.Windows.Forms.Button connectBtn;
	
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
			this.hostname = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.sipm1 = new System.Windows.Forms.Button();
			this.sipm2 = new System.Windows.Forms.Button();
			this.sipm3 = new System.Windows.Forms.Button();
			this.sipm4 = new System.Windows.Forms.Button();
			this.sipm5 = new System.Windows.Forms.Button();
			this.sipm6 = new System.Windows.Forms.Button();
			this.sipm7 = new System.Windows.Forms.Button();
			this.sipm8 = new System.Windows.Forms.Button();
			this.automeasureBtn = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.avgCount = new System.Windows.Forms.NumericUpDown();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.MeasureProgress = new System.Windows.Forms.ToolStripProgressBar();
			this.FEBConnected = new System.Windows.Forms.ToolStripStatusLabel();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.dicounter = new System.Windows.Forms.TextBox();
			this.connectBtn = new System.Windows.Forms.Button();
			this.lastMeasure = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.avgCount)).BeginInit();
			this.statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// hostname
			// 
			this.hostname.Location = new System.Drawing.Point(86, 12);
			this.hostname.MaxLength = 256;
			this.hostname.Name = "hostname";
			this.hostname.Size = new System.Drawing.Size(100, 20);
			this.hostname.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 19);
			this.label1.TabIndex = 1;
			this.label1.Text = "Host";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(481, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 24);
			this.label2.TabIndex = 2;
			this.label2.Text = "SiPMs";
			// 
			// sipm1
			// 
			this.sipm1.Location = new System.Drawing.Point(470, 34);
			this.sipm1.Name = "sipm1";
			this.sipm1.Size = new System.Drawing.Size(20, 23);
			this.sipm1.TabIndex = 3;
			this.sipm1.Text = "1";
			this.sipm1.UseVisualStyleBackColor = true;
			this.sipm1.Click += new System.EventHandler(this.Sipm1Click);
			// 
			// sipm2
			// 
			this.sipm2.Location = new System.Drawing.Point(496, 34);
			this.sipm2.Name = "sipm2";
			this.sipm2.Size = new System.Drawing.Size(20, 23);
			this.sipm2.TabIndex = 4;
			this.sipm2.Text = "2";
			this.sipm2.UseVisualStyleBackColor = true;
			this.sipm2.Click += new System.EventHandler(this.Sipm2Click);
			// 
			// sipm3
			// 
			this.sipm3.Location = new System.Drawing.Point(522, 34);
			this.sipm3.Name = "sipm3";
			this.sipm3.Size = new System.Drawing.Size(20, 23);
			this.sipm3.TabIndex = 5;
			this.sipm3.Text = "3";
			this.sipm3.UseVisualStyleBackColor = true;
			this.sipm3.Click += new System.EventHandler(this.Sipm3Click);
			// 
			// sipm4
			// 
			this.sipm4.Location = new System.Drawing.Point(548, 34);
			this.sipm4.Name = "sipm4";
			this.sipm4.Size = new System.Drawing.Size(20, 23);
			this.sipm4.TabIndex = 6;
			this.sipm4.Text = "4";
			this.sipm4.UseVisualStyleBackColor = true;
			this.sipm4.Click += new System.EventHandler(this.Sipm4Click);
			// 
			// sipm5
			// 
			this.sipm5.Location = new System.Drawing.Point(470, 63);
			this.sipm5.Name = "sipm5";
			this.sipm5.Size = new System.Drawing.Size(20, 23);
			this.sipm5.TabIndex = 8;
			this.sipm5.Text = "5";
			this.sipm5.UseVisualStyleBackColor = true;
			this.sipm5.Click += new System.EventHandler(this.Sipm5Click);
			// 
			// sipm6
			// 
			this.sipm6.Location = new System.Drawing.Point(496, 63);
			this.sipm6.Name = "sipm6";
			this.sipm6.Size = new System.Drawing.Size(20, 23);
			this.sipm6.TabIndex = 9;
			this.sipm6.Text = "6";
			this.sipm6.UseVisualStyleBackColor = true;
			this.sipm6.Click += new System.EventHandler(this.Sipm6Click);
			// 
			// sipm7
			// 
			this.sipm7.Location = new System.Drawing.Point(522, 63);
			this.sipm7.Name = "sipm7";
			this.sipm7.Size = new System.Drawing.Size(20, 23);
			this.sipm7.TabIndex = 10;
			this.sipm7.Text = "7";
			this.sipm7.UseVisualStyleBackColor = true;
			this.sipm7.Click += new System.EventHandler(this.Sipm7Click);
			// 
			// sipm8
			// 
			this.sipm8.Location = new System.Drawing.Point(548, 63);
			this.sipm8.Name = "sipm8";
			this.sipm8.Size = new System.Drawing.Size(20, 23);
			this.sipm8.TabIndex = 11;
			this.sipm8.Text = "8";
			this.sipm8.UseVisualStyleBackColor = true;
			this.sipm8.Click += new System.EventHandler(this.Sipm8Click);
			// 
			// automeasureBtn
			// 
			this.automeasureBtn.Location = new System.Drawing.Point(470, 148);
			this.automeasureBtn.Name = "automeasureBtn";
			this.automeasureBtn.Size = new System.Drawing.Size(98, 23);
			this.automeasureBtn.TabIndex = 12;
			this.automeasureBtn.Text = "Auto Measure";
			this.automeasureBtn.UseVisualStyleBackColor = true;
			this.automeasureBtn.Click += new System.EventHandler(this.AutomeasureClick);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(406, 177);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 20);
			this.label3.TabIndex = 13;
			this.label3.Text = "Avg#:";
			// 
			// avgCount
			// 
			this.avgCount.Location = new System.Drawing.Point(496, 177);
			this.avgCount.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.avgCount.Name = "avgCount";
			this.avgCount.Size = new System.Drawing.Size(68, 20);
			this.avgCount.TabIndex = 14;
			this.avgCount.Value = new decimal(new int[] {
			10,
			0,
			0,
			0});
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.MeasureProgress,
			this.FEBConnected});
			this.statusStrip1.Location = new System.Drawing.Point(0, 418);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.statusStrip1.Size = new System.Drawing.Size(657, 22);
			this.statusStrip1.TabIndex = 15;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// MeasureProgress
			// 
			this.MeasureProgress.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
			this.MeasureProgress.Name = "MeasureProgress";
			this.MeasureProgress.Size = new System.Drawing.Size(100, 16);
			// 
			// FEBConnected
			// 
			this.FEBConnected.Name = "FEBConnected";
			this.FEBConnected.Size = new System.Drawing.Size(26, 17);
			this.FEBConnected.Text = "FEB";
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.DecimalPlaces = 4;
			this.numericUpDown1.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.numericUpDown1.Location = new System.Drawing.Point(496, 218);
			this.numericUpDown1.Maximum = new decimal(new int[] {
			15,
			0,
			0,
			65536});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(68, 20);
			this.numericUpDown1.TabIndex = 16;
			this.numericUpDown1.Value = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(406, 215);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 23);
			this.label4.TabIndex = 17;
			this.label4.Text = "IThresh:";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(406, 257);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(84, 23);
			this.label5.TabIndex = 18;
			this.label5.Text = "Dicounter:";
			// 
			// dicounter
			// 
			this.dicounter.Location = new System.Drawing.Point(496, 259);
			this.dicounter.MaxLength = 16;
			this.dicounter.Name = "dicounter";
			this.dicounter.Size = new System.Drawing.Size(68, 20);
			this.dicounter.TabIndex = 19;
			// 
			// connectBtn
			// 
			this.connectBtn.Location = new System.Drawing.Point(193, 8);
			this.connectBtn.Name = "connectBtn";
			this.connectBtn.Size = new System.Drawing.Size(75, 23);
			this.connectBtn.TabIndex = 20;
			this.connectBtn.Text = "Connect";
			this.connectBtn.UseVisualStyleBackColor = true;
			this.connectBtn.Click += new System.EventHandler(this.ConnectClick);
			// 
			// lastMeasure
			// 
			this.lastMeasure.Location = new System.Drawing.Point(575, 33);
			this.lastMeasure.Name = "lastMeasure";
			this.lastMeasure.Size = new System.Drawing.Size(45, 23);
			this.lastMeasure.TabIndex = 21;
			this.lastMeasure.Text = "Last";
			this.lastMeasure.UseVisualStyleBackColor = true;
			this.lastMeasure.Click += new System.EventHandler(this.LastMeasureClick);
			// 
			// sipmTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(657, 440);
			this.Controls.Add(this.lastMeasure);
			this.Controls.Add(this.connectBtn);
			this.Controls.Add(this.dicounter);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.avgCount);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.automeasureBtn);
			this.Controls.Add(this.sipm8);
			this.Controls.Add(this.sipm7);
			this.Controls.Add(this.sipm6);
			this.Controls.Add(this.sipm5);
			this.Controls.Add(this.sipm4);
			this.Controls.Add(this.sipm3);
			this.Controls.Add(this.sipm2);
			this.Controls.Add(this.sipm1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.hostname);
			this.Name = "sipmTestForm";
			this.Text = "Mu2e SiPM Test";
			((System.ComponentModel.ISupportInitialize)(this.avgCount)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}