namespace ShanDain.AIM
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.listView_searchResult = new System.Windows.Forms.ListView();
            this.button_search = new System.Windows.Forms.Button();
            this.textBox_search = new System.Windows.Forms.TextBox();
            this.Label = new System.Windows.Forms.Label();
            this.label_selected = new System.Windows.Forms.Label();
            this.listView_version = new System.Windows.Forms.ListView();
            this.button_satrt = new System.Windows.Forms.Button();
            this.textBox_showlog = new System.Windows.Forms.TextBox();
            this.button_unload = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage_searchresult = new System.Windows.Forms.TabPage();
            this.tabPage_loaded = new System.Windows.Forms.TabPage();
            this.listView_loaded = new System.Windows.Forms.ListView();
            this.label_serverstatus = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPage_searchresult.SuspendLayout();
            this.tabPage_loaded.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView_searchResult
            // 
            this.listView_searchResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_searchResult.FullRowSelect = true;
            this.listView_searchResult.Location = new System.Drawing.Point(3, 3);
            this.listView_searchResult.Name = "listView_searchResult";
            this.listView_searchResult.Size = new System.Drawing.Size(647, 361);
            this.listView_searchResult.TabIndex = 0;
            this.listView_searchResult.UseCompatibleStateImageBehavior = false;
            this.listView_searchResult.View = System.Windows.Forms.View.Details;
            this.listView_searchResult.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_searchResult_ItemSelectionChanged);
            // 
            // button_search
            // 
            this.button_search.Location = new System.Drawing.Point(152, 12);
            this.button_search.Name = "button_search";
            this.button_search.Size = new System.Drawing.Size(75, 23);
            this.button_search.TabIndex = 1;
            this.button_search.Text = "搜索";
            this.button_search.UseVisualStyleBackColor = true;
            this.button_search.Click += new System.EventHandler(this.button_search_Click);
            // 
            // textBox_search
            // 
            this.textBox_search.Location = new System.Drawing.Point(12, 12);
            this.textBox_search.Name = "textBox_search";
            this.textBox_search.Size = new System.Drawing.Size(134, 21);
            this.textBox_search.TabIndex = 2;
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(693, 51);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(35, 12);
            this.Label.TabIndex = 3;
            this.Label.Text = "详情:";
            // 
            // label_selected
            // 
            this.label_selected.AutoSize = true;
            this.label_selected.Location = new System.Drawing.Point(693, 79);
            this.label_selected.Name = "label_selected";
            this.label_selected.Size = new System.Drawing.Size(77, 12);
            this.label_selected.TabIndex = 4;
            this.label_selected.Text = "--详情描述--";
            // 
            // listView_version
            // 
            this.listView_version.Location = new System.Drawing.Point(679, 317);
            this.listView_version.Name = "listView_version";
            this.listView_version.Size = new System.Drawing.Size(193, 68);
            this.listView_version.TabIndex = 5;
            this.listView_version.UseCompatibleStateImageBehavior = false;
            this.listView_version.View = System.Windows.Forms.View.Details;
            this.listView_version.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_version_ItemSelectionChanged);
            // 
            // button_satrt
            // 
            this.button_satrt.Location = new System.Drawing.Point(797, 402);
            this.button_satrt.Name = "button_satrt";
            this.button_satrt.Size = new System.Drawing.Size(75, 23);
            this.button_satrt.TabIndex = 6;
            this.button_satrt.Text = "下载";
            this.button_satrt.UseVisualStyleBackColor = true;
            this.button_satrt.Click += new System.EventHandler(this.button_satrt_Click);
            // 
            // textBox_showlog
            // 
            this.textBox_showlog.Location = new System.Drawing.Point(19, 440);
            this.textBox_showlog.Multiline = true;
            this.textBox_showlog.Name = "textBox_showlog";
            this.textBox_showlog.ReadOnly = true;
            this.textBox_showlog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_showlog.Size = new System.Drawing.Size(865, 122);
            this.textBox_showlog.TabIndex = 7;
            // 
            // button_unload
            // 
            this.button_unload.Enabled = false;
            this.button_unload.Location = new System.Drawing.Point(683, 402);
            this.button_unload.Name = "button_unload";
            this.button_unload.Size = new System.Drawing.Size(75, 23);
            this.button_unload.TabIndex = 8;
            this.button_unload.Text = "卸载";
            this.button_unload.UseVisualStyleBackColor = true;
            this.button_unload.Click += new System.EventHandler(this.button_unload_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage_searchresult);
            this.tabControl.Controls.Add(this.tabPage_loaded);
            this.tabControl.Location = new System.Drawing.Point(12, 41);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(661, 393);
            this.tabControl.TabIndex = 9;
            // 
            // tabPage_searchresult
            // 
            this.tabPage_searchresult.Controls.Add(this.listView_searchResult);
            this.tabPage_searchresult.Location = new System.Drawing.Point(4, 22);
            this.tabPage_searchresult.Name = "tabPage_searchresult";
            this.tabPage_searchresult.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_searchresult.Size = new System.Drawing.Size(653, 367);
            this.tabPage_searchresult.TabIndex = 0;
            this.tabPage_searchresult.Text = "查询结果";
            this.tabPage_searchresult.UseVisualStyleBackColor = true;
            // 
            // tabPage_loaded
            // 
            this.tabPage_loaded.Controls.Add(this.listView_loaded);
            this.tabPage_loaded.Location = new System.Drawing.Point(4, 22);
            this.tabPage_loaded.Name = "tabPage_loaded";
            this.tabPage_loaded.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_loaded.Size = new System.Drawing.Size(653, 367);
            this.tabPage_loaded.TabIndex = 1;
            this.tabPage_loaded.Text = "已装载插件";
            this.tabPage_loaded.UseVisualStyleBackColor = true;
            this.tabPage_loaded.Click += new System.EventHandler(this.tabPage_loaded_Click);
            // 
            // listView_loaded
            // 
            this.listView_loaded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_loaded.FullRowSelect = true;
            this.listView_loaded.Location = new System.Drawing.Point(3, 3);
            this.listView_loaded.Name = "listView_loaded";
            this.listView_loaded.Size = new System.Drawing.Size(647, 361);
            this.listView_loaded.TabIndex = 0;
            this.listView_loaded.UseCompatibleStateImageBehavior = false;
            this.listView_loaded.View = System.Windows.Forms.View.Details;
            this.listView_loaded.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LocalView_ItemSelectionChanged);
            // 
            // label_serverstatus
            // 
            this.label_serverstatus.AutoSize = true;
            this.label_serverstatus.Location = new System.Drawing.Point(262, 23);
            this.label_serverstatus.Name = "label_serverstatus";
            this.label_serverstatus.Size = new System.Drawing.Size(0, 12);
            this.label_serverstatus.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 574);
            this.Controls.Add(this.label_serverstatus);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.button_unload);
            this.Controls.Add(this.textBox_showlog);
            this.Controls.Add(this.button_satrt);
            this.Controls.Add(this.listView_version);
            this.Controls.Add(this.label_selected);
            this.Controls.Add(this.Label);
            this.Controls.Add(this.textBox_search);
            this.Controls.Add(this.button_search);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "善点:插件下载器";
            this.tabControl.ResumeLayout(false);
            this.tabPage_searchresult.ResumeLayout(false);
            this.tabPage_loaded.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView_searchResult;
        private System.Windows.Forms.Button button_search;
        private System.Windows.Forms.TextBox textBox_search;
        private System.Windows.Forms.Label Label;
        private System.Windows.Forms.Label label_selected;
        private System.Windows.Forms.ListView listView_version;
        private System.Windows.Forms.Button button_satrt;
        private System.Windows.Forms.TextBox textBox_showlog;
        private System.Windows.Forms.Button button_unload;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage_searchresult;
        private System.Windows.Forms.TabPage tabPage_loaded;
        private System.Windows.Forms.ListView listView_loaded;
        private System.Windows.Forms.Label label_serverstatus;
    }
}

