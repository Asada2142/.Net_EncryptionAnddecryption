using KeyGen;
using LicenseCommonUtil;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SystemInfoGenerator
{
  public class FormGenerateSystemInfo : Form
  {
    private RichTextBox MachineCode;
    private Button btnGenerateMachineCode;
    private Button btnSaveLicense;
    private Label label1;
    private Label label2;
    private TextBox LicenseCode;
    private Button btnGenerateLicense;
    private Button btnSaveMachineCode;
    private Button btnCheckLicense;
    private ComboBox licenseTimeLimit;
    private Label label4;
    private LicGenerate licGenerate;

    public FormGenerateSystemInfo()
    {
      this.InitializeComponent();
    }

    private void InitializeComponent()
    {
            this.MachineCode = new System.Windows.Forms.RichTextBox();
            this.btnGenerateMachineCode = new System.Windows.Forms.Button();
            this.btnSaveLicense = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSaveMachineCode = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.LicenseCode = new System.Windows.Forms.TextBox();
            this.btnGenerateLicense = new System.Windows.Forms.Button();
            this.btnCheckLicense = new System.Windows.Forms.Button();
            this.licenseTimeLimit = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MachineCode
            // 
            this.MachineCode.Location = new System.Drawing.Point(58, 10);
            this.MachineCode.Name = "MachineCode";
            this.MachineCode.ReadOnly = true;
            this.MachineCode.Size = new System.Drawing.Size(520, 49);
            this.MachineCode.TabIndex = 0;
            this.MachineCode.Text = "";
            // 
            // btnGenerateMachineCode
            // 
            this.btnGenerateMachineCode.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnGenerateMachineCode.Location = new System.Drawing.Point(394, 65);
            this.btnGenerateMachineCode.Name = "btnGenerateMachineCode";
            this.btnGenerateMachineCode.Size = new System.Drawing.Size(75, 37);
            this.btnGenerateMachineCode.TabIndex = 1;
            this.btnGenerateMachineCode.Text = "生成机器码";
            this.btnGenerateMachineCode.UseVisualStyleBackColor = true;
            this.btnGenerateMachineCode.Click += new System.EventHandler(this.btnGenerateMachineCode_Click);
            // 
            // btnSaveLicense
            // 
            this.btnSaveLicense.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSaveLicense.Enabled = false;
            this.btnSaveLicense.Location = new System.Drawing.Point(422, 192);
            this.btnSaveLicense.Name = "btnSaveLicense";
            this.btnSaveLicense.Size = new System.Drawing.Size(75, 37);
            this.btnSaveLicense.TabIndex = 2;
            this.btnSaveLicense.Text = "保存注册码";
            this.btnSaveLicense.UseVisualStyleBackColor = true;
            this.btnSaveLicense.Click += new System.EventHandler(this.btnSaveLicense_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "机器码：";
            // 
            // btnSaveMachineCode
            // 
            this.btnSaveMachineCode.Enabled = false;
            this.btnSaveMachineCode.Location = new System.Drawing.Point(503, 65);
            this.btnSaveMachineCode.Name = "btnSaveMachineCode";
            this.btnSaveMachineCode.Size = new System.Drawing.Size(75, 37);
            this.btnSaveMachineCode.TabIndex = 4;
            this.btnSaveMachineCode.Text = "保存机器码";
            this.btnSaveMachineCode.UseVisualStyleBackColor = true;
            this.btnSaveMachineCode.Click += new System.EventHandler(this.btnSaveMachineCode_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 107);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "注册码：";
            // 
            // LicenseCode
            // 
            this.LicenseCode.Location = new System.Drawing.Point(58, 107);
            this.LicenseCode.Margin = new System.Windows.Forms.Padding(2);
            this.LicenseCode.Multiline = true;
            this.LicenseCode.Name = "LicenseCode";
            this.LicenseCode.ReadOnly = true;
            this.LicenseCode.Size = new System.Drawing.Size(520, 80);
            this.LicenseCode.TabIndex = 6;
            this.LicenseCode.TextChanged += new System.EventHandler(this.LicenseCode_TextChanged);
            // 
            // btnGenerateLicense
            // 
            this.btnGenerateLicense.Location = new System.Drawing.Point(342, 192);
            this.btnGenerateLicense.Margin = new System.Windows.Forms.Padding(2);
            this.btnGenerateLicense.Name = "btnGenerateLicense";
            this.btnGenerateLicense.Size = new System.Drawing.Size(75, 37);
            this.btnGenerateLicense.TabIndex = 7;
            this.btnGenerateLicense.Text = "生成注册码";
            this.btnGenerateLicense.UseVisualStyleBackColor = true;
            this.btnGenerateLicense.Click += new System.EventHandler(this.btnGenerateLicense_Click);
            // 
            // btnCheckLicense
            // 
            this.btnCheckLicense.Enabled = false;
            this.btnCheckLicense.Location = new System.Drawing.Point(503, 192);
            this.btnCheckLicense.Name = "btnCheckLicense";
            this.btnCheckLicense.Size = new System.Drawing.Size(75, 37);
            this.btnCheckLicense.TabIndex = 8;
            this.btnCheckLicense.Text = "验证注册码";
            this.btnCheckLicense.UseVisualStyleBackColor = true;
            this.btnCheckLicense.Click += new System.EventHandler(this.btnCheckLicense_Click);
            // 
            // licenseTimeLimit
            // 
            this.licenseTimeLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.licenseTimeLimit.FormattingEnabled = true;
            this.licenseTimeLimit.Items.AddRange(new object[] {
            "一星期",
            "一个月",
            "三个月"});
            this.licenseTimeLimit.Location = new System.Drawing.Point(127, 201);
            this.licenseTimeLimit.Name = "licenseTimeLimit";
            this.licenseTimeLimit.Size = new System.Drawing.Size(174, 20);
            this.licenseTimeLimit.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(56, 204);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "试用期限：";
            // 
            // FormGenerateSystemInfo
            // 
            this.AcceptButton = this.btnGenerateMachineCode;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnSaveLicense;
            this.ClientSize = new System.Drawing.Size(586, 243);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.licenseTimeLimit);
            this.Controls.Add(this.btnCheckLicense);
            this.Controls.Add(this.btnGenerateLicense);
            this.Controls.Add(this.LicenseCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSaveMachineCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSaveLicense);
            this.Controls.Add(this.btnGenerateMachineCode);
            this.Controls.Add(this.MachineCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGenerateSystemInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KeyGen";
            this.Load += new System.EventHandler(this.FormGenerateSystemInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    private void FormGenerateSystemInfo_Load(object sender, EventArgs e)
    {
        licenseTimeLimit.SelectedIndex = 0;
    }

    private void btnGenerateMachineCode_Click(object sender, EventArgs e)
    {
      try
      {
        this.SetUIEnable(false);
        StringBuilder stringBuilder = new StringBuilder();

        byte[] bytes = LicenseUtil.ByteConvertor.GetBytes(LicenseUtil.SystemInfo);
        LicenseUtil.GetStringFromByte(bytes);
        string str = Convert.ToBase64String(bytes);

        stringBuilder.Append(str);
        this.MachineCode.Text = stringBuilder.ToString();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.ToString(), "错误");
      }
      finally
      {
        this.SetUIEnable(true);
      }
    }

    private void SetUIEnable(bool enable)
    {
       this.btnSaveMachineCode.Enabled = enable;
      Cursor.Current = enable ? Cursors.Default : Cursors.WaitCursor;
      this.btnSaveMachineCode.Enabled = this.btnSaveMachineCode.Enabled ? enable : this.btnSaveMachineCode.Enabled;
    }

    private void MachineCode_TextChanged(object sender, EventArgs e)
    {
      this.btnSaveLicense.Enabled = this.MachineCode.TextLength > 0;
    }


    private void btnGenerateLicense_Click(object sender, EventArgs e)
    {
        try
        {
            licGenerate = new LicGenerate(MachineCode.Text.ToString().Trim());

            licGenerate.fromDateTime = DateTime.Now.ToString();

            //试用期限
            switch(licenseTimeLimit.SelectedIndex)
            {
                case 0:
                    //7天
                    licGenerate.toDateTime = DateTime.Now.AddDays(7).ToString();
                    break;
                case 1:
                    //一个月
                    licGenerate.toDateTime = DateTime.Now.AddMonths(1).ToString();
                    break;
                case 2:
                    //三个月
                    licGenerate.toDateTime = DateTime.Now.AddMonths(3).ToString();
                    break;
            }

            if (MachineCode.Text.ToString().Trim() != "")
            {
                licGenerate.hackExcute();
                LicenseCode.Text = licGenerate.KeyLicenseCode + "\n" + licGenerate.TimeLicenseCode;
                MessageBox.Show("注册码生成成功！");
            }
            else
            {
                MessageBox.Show("不能为空");
            }
        }
        catch
        {
            MessageBox.Show("注册码生成失败！");
        }
        finally
        {
            this.SetUIEnable2(true);
        }
    }

    private void btnSaveLicense_Click(object sender, EventArgs e)
    {
        LicenseUtil.SaveFile(this.LicenseCode.Text.Replace("\n", Environment.NewLine), ".lic", "License(*.lic)|*.lic");
    }
    
    private void btnCheckLicense_Click(object sender, EventArgs e)
    {
        string licenseFilePath = "";
        string message = "";
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.Multiselect = false;
        fileDialog.RestoreDirectory = true;
        fileDialog.Filter = "License(*.lic)|*.lic";
        if(DialogResult.OK == fileDialog.ShowDialog())
        {
            licenseFilePath = fileDialog.FileName;
        }

        try
        {
            if (LicenseClient.LicenseClient.CheckLicense(licenseFilePath, out message))
            {
                MessageBox.Show("验证通过！");
            }
            else
            {
                MessageBox.Show("验证失败！");
            }
        }
        catch
        {
            MessageBox.Show("验证失败！");
        }
        
    }

    private void btnSaveMachineCode_Click(object sender, EventArgs e)
    {
        LicenseUtil.SaveFile(this.MachineCode.Text.Replace("\n", Environment.NewLine), ".txt", "文本文件(*.txt)|*.txt");
    }

    private void SetUIEnable2(bool enable)
    {
        this.btnSaveLicense.Enabled = enable;
        this.btnCheckLicense.Enabled = enable;
        Cursor.Current = enable ? Cursors.Default : Cursors.WaitCursor;
        this.btnSaveLicense.Enabled = this.btnSaveLicense.Enabled ? enable : this.btnSaveLicense.Enabled;
        this.btnCheckLicense.Enabled = this.btnCheckLicense.Enabled ? enable : this.btnCheckLicense.Enabled;
    }

    private void LicenseCode_TextChanged(object sender, EventArgs e)
    {
        this.btnSaveLicense.Enabled = this.LicenseCode.TextLength > 0;
        this.btnCheckLicense.Enabled = this.LicenseCode.TextLength > 0;
    }
  }
}
