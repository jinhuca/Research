namespace DatabaseFiller
{
    partial class FrmDatabaseInitializer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LblConnectionString = new System.Windows.Forms.Label();
            this.TxtConnectionString = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.BtnTestConnection = new System.Windows.Forms.Button();
            this.LblConnectionStatus = new System.Windows.Forms.Label();
            this.BtnFillValues = new System.Windows.Forms.Button();
            this.TxtStatus = new System.Windows.Forms.TextBox();
            this.LblStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.LblConnectionString, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TxtConnectionString, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.TxtStatus, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.LblStatus, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.BtnFillValues, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(758, 360);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // LblConnectionString
            // 
            this.LblConnectionString.AutoSize = true;
            this.LblConnectionString.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblConnectionString.Location = new System.Drawing.Point(3, 15);
            this.LblConnectionString.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.LblConnectionString.Name = "LblConnectionString";
            this.LblConnectionString.Size = new System.Drawing.Size(94, 22);
            this.LblConnectionString.TabIndex = 0;
            this.LblConnectionString.Text = "Connection String:";
            this.LblConnectionString.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // TxtConnectionString
            // 
            this.TxtConnectionString.Location = new System.Drawing.Point(103, 13);
            this.TxtConnectionString.Name = "TxtConnectionString";
            this.TxtConnectionString.Size = new System.Drawing.Size(640, 20);
            this.TxtConnectionString.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.BtnTestConnection);
            this.flowLayoutPanel1.Controls.Add(this.LblConnectionStatus);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(100, 37);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(658, 27);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // BtnTestConnection
            // 
            this.BtnTestConnection.Location = new System.Drawing.Point(3, 3);
            this.BtnTestConnection.Name = "BtnTestConnection";
            this.BtnTestConnection.Size = new System.Drawing.Size(123, 23);
            this.BtnTestConnection.TabIndex = 8;
            this.BtnTestConnection.Text = "Test Connection";
            this.BtnTestConnection.UseVisualStyleBackColor = true;
            this.BtnTestConnection.Click += new System.EventHandler(this.BtnTestConnection_Click);
            // 
            // LblConnectionStatus
            // 
            this.LblConnectionStatus.AutoSize = true;
            this.LblConnectionStatus.Location = new System.Drawing.Point(132, 8);
            this.LblConnectionStatus.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.LblConnectionStatus.Name = "LblConnectionStatus";
            this.LblConnectionStatus.Size = new System.Drawing.Size(24, 13);
            this.LblConnectionStatus.TabIndex = 9;
            this.LblConnectionStatus.Text = "test";
            this.LblConnectionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnFillValues
            // 
            this.BtnFillValues.Location = new System.Drawing.Point(103, 94);
            this.BtnFillValues.Name = "BtnFillValues";
            this.BtnFillValues.Size = new System.Drawing.Size(86, 21);
            this.BtnFillValues.TabIndex = 10;
            this.BtnFillValues.Text = "Fill Values";
            this.BtnFillValues.UseVisualStyleBackColor = true;
            this.BtnFillValues.Click += new System.EventHandler(this.BtnFillValues_Click);
            // 
            // TxtStatus
            // 
            this.TxtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtStatus.Location = new System.Drawing.Point(103, 121);
            this.TxtStatus.Multiline = true;
            this.TxtStatus.Name = "TxtStatus";
            this.TxtStatus.ReadOnly = true;
            this.TxtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtStatus.Size = new System.Drawing.Size(652, 236);
            this.TxtStatus.TabIndex = 11;
            // 
            // LblStatus
            // 
            this.LblStatus.AutoSize = true;
            this.LblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblStatus.Location = new System.Drawing.Point(3, 118);
            this.LblStatus.Name = "LblStatus";
            this.LblStatus.Size = new System.Drawing.Size(94, 242);
            this.LblStatus.TabIndex = 12;
            this.LblStatus.Text = "Status:";
            this.LblStatus.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // FrmDatabaseInitializer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 360);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FrmDatabaseInitializer";
            this.Text = "Database Initializer";
            this.Load += new System.EventHandler(this.FrmDatabaseInitializer_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label LblConnectionString;
        private System.Windows.Forms.TextBox TxtConnectionString;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button BtnTestConnection;
        private System.Windows.Forms.Label LblConnectionStatus;
        private System.Windows.Forms.Button BtnFillValues;
        private System.Windows.Forms.TextBox TxtStatus;
        private System.Windows.Forms.Label LblStatus;
    }
}

