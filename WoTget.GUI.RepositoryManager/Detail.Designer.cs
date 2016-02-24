namespace WoTget.GUI.RepositoryManager
{
    partial class Detail
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label authorsLabel;
            System.Windows.Forms.Label descriptionLabel;
            System.Windows.Forms.Label nameLabel;
            System.Windows.Forms.Label ownersLabel;
            System.Windows.Forms.Label projectUrlLabel;
            System.Windows.Forms.Label versionLabel;
            System.Windows.Forms.Label label2;
            this.authorsTextBox = new System.Windows.Forms.TextBox();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.ownersTextBox = new System.Windows.Forms.TextBox();
            this.projectUrlTextBox = new System.Windows.Forms.TextBox();
            this.versionTextBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.iPackageBindingSource = new System.Windows.Forms.BindingSource(this.components);
            authorsLabel = new System.Windows.Forms.Label();
            descriptionLabel = new System.Windows.Forms.Label();
            nameLabel = new System.Windows.Forms.Label();
            ownersLabel = new System.Windows.Forms.Label();
            projectUrlLabel = new System.Windows.Forms.Label();
            versionLabel = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.iPackageBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // authorsLabel
            // 
            authorsLabel.AutoSize = true;
            authorsLabel.Location = new System.Drawing.Point(12, 122);
            authorsLabel.Name = "authorsLabel";
            authorsLabel.Size = new System.Drawing.Size(46, 13);
            authorsLabel.TabIndex = 8;
            authorsLabel.Text = "Authors:";
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.Location = new System.Drawing.Point(12, 44);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new System.Drawing.Size(63, 13);
            descriptionLabel.TabIndex = 2;
            descriptionLabel.Text = "Description:";
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Location = new System.Drawing.Point(12, 18);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new System.Drawing.Size(38, 13);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Name:";
            // 
            // ownersLabel
            // 
            ownersLabel.AutoSize = true;
            ownersLabel.Location = new System.Drawing.Point(12, 96);
            ownersLabel.Name = "ownersLabel";
            ownersLabel.Size = new System.Drawing.Size(46, 13);
            ownersLabel.TabIndex = 6;
            ownersLabel.Text = "Owners:";
            // 
            // projectUrlLabel
            // 
            projectUrlLabel.AutoSize = true;
            projectUrlLabel.Location = new System.Drawing.Point(12, 148);
            projectUrlLabel.Name = "projectUrlLabel";
            projectUrlLabel.Size = new System.Drawing.Size(59, 13);
            projectUrlLabel.TabIndex = 10;
            projectUrlLabel.Text = "Project Url:";
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.Location = new System.Drawing.Point(12, 70);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new System.Drawing.Size(45, 13);
            versionLabel.TabIndex = 4;
            versionLabel.Text = "Version:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 174);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(34, 13);
            label2.TabIndex = 12;
            label2.Text = "Tags:";
            // 
            // authorsTextBox
            // 
            this.authorsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authorsTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.iPackageBindingSource, "Authors", true));
            this.authorsTextBox.Location = new System.Drawing.Point(81, 119);
            this.authorsTextBox.Name = "authorsTextBox";
            this.authorsTextBox.Size = new System.Drawing.Size(363, 20);
            this.authorsTextBox.TabIndex = 9;
            this.authorsTextBox.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.iPackageBindingSource, "Description", true));
            this.descriptionTextBox.Location = new System.Drawing.Point(81, 41);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(363, 20);
            this.descriptionTextBox.TabIndex = 3;
            this.descriptionTextBox.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.iPackageBindingSource, "Name", true));
            this.nameTextBox.Location = new System.Drawing.Point(81, 15);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(363, 20);
            this.nameTextBox.TabIndex = 1;
            this.nameTextBox.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // ownersTextBox
            // 
            this.ownersTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ownersTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.iPackageBindingSource, "Owners", true));
            this.ownersTextBox.Location = new System.Drawing.Point(81, 93);
            this.ownersTextBox.Name = "ownersTextBox";
            this.ownersTextBox.Size = new System.Drawing.Size(363, 20);
            this.ownersTextBox.TabIndex = 7;
            this.ownersTextBox.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // projectUrlTextBox
            // 
            this.projectUrlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projectUrlTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.iPackageBindingSource, "ProjectUrl", true));
            this.projectUrlTextBox.Location = new System.Drawing.Point(81, 145);
            this.projectUrlTextBox.Name = "projectUrlTextBox";
            this.projectUrlTextBox.Size = new System.Drawing.Size(363, 20);
            this.projectUrlTextBox.TabIndex = 11;
            this.projectUrlTextBox.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // versionTextBox
            // 
            this.versionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.versionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.iPackageBindingSource, "Version", true));
            this.versionTextBox.Location = new System.Drawing.Point(81, 67);
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.Size = new System.Drawing.Size(363, 20);
            this.versionTextBox.TabIndex = 5;
            this.versionTextBox.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.iPackageBindingSource, "TagsString", true));
            this.textBox1.Location = new System.Drawing.Point(81, 171);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(363, 20);
            this.textBox1.TabIndex = 13;
            this.textBox1.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(369, 221);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(289, 221);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 14;
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // iPackageBindingSource
            // 
            this.iPackageBindingSource.DataSource = typeof(WoTget.GUI.RepositoryManager.PackageModel);
            // 
            // Detail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 266);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(authorsLabel);
            this.Controls.Add(this.authorsTextBox);
            this.Controls.Add(descriptionLabel);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(nameLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(ownersLabel);
            this.Controls.Add(this.ownersTextBox);
            this.Controls.Add(label2);
            this.Controls.Add(projectUrlLabel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.projectUrlTextBox);
            this.Controls.Add(versionLabel);
            this.Controls.Add(this.versionTextBox);
            this.Name = "Detail";
            this.Text = "Detail";
            ((System.ComponentModel.ISupportInitialize)(this.iPackageBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource iPackageBindingSource;
        private System.Windows.Forms.TextBox authorsTextBox;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.TextBox ownersTextBox;
        private System.Windows.Forms.TextBox projectUrlTextBox;
        private System.Windows.Forms.TextBox versionTextBox;
        private System.Windows.Forms.TextBox textBox1;
        protected System.Windows.Forms.Button button1;
        protected System.Windows.Forms.Button button2;
        protected System.Windows.Forms.TextBox nameTextBox;
    }
}