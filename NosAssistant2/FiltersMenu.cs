using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NosAssistant2.Configs;
using NosAssistant2.GameData;
using NosAssistant2.GUIElements;
using NosAssistant2.Helpers;

namespace NosAssistant2;

public class FiltersMenu : Form
{
	private Panel filteredPanel;

	private TextBox newFilterTextBox;

	private Label modeLabel;

	private Label ruleLabel;

	private string clickedFilterLabel = "";

	public FiltersMenu()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FiltersMenu";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Filters Menu";
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = NosAssistant2.Helpers.NAStyles.MainThemeLighter;
		base.ClientSize = new System.Drawing.Size(800, 600);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Region = System.Drawing.Region.FromHrgn(NosAssistant2.Helpers.DllImports.CreateRoundRectRgn(0, 0, base.Width, base.Height, 25, 25));
		base.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
		base.ResumeLayout(false);
		System.Windows.Forms.Label label = new System.Windows.Forms.Label();
		label.Text = "Filters";
		label.Font = new System.Drawing.Font(" Microsoft Sans Serif", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		label.BackColor = System.Drawing.Color.Transparent;
		label.ForeColor = NosAssistant2.Helpers.NAStyles.CounterForeColor;
		label.Width = 150;
		label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		label.Left = base.Width / 2 - label.Width / 2;
		label.Top = 10;
		base.Controls.Add(label);
		System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
		panel.BackColor = NosAssistant2.Helpers.NAStyles.MainThemeDarker;
		panel.Left = 50;
		panel.Top = 50;
		panel.Width = 200;
		panel.Height = 500;
		NosAssistant2.GUI.SetRoundShape(panel, 10);
		base.Controls.Add(panel);
		base.SuspendLayout();
		int num = 0;
		foreach (NosAssistant2.GameData.NostaleCharacterInfo nostaleCharacterInfo in NosAssistant2.GUI._nostaleCharacterInfoList)
		{
			System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
			label2.Name = (System.IntPtr)nostaleCharacterInfo.hwnd + "_FilterNicknameLabel";
			label2.Text = nostaleCharacterInfo.nickname;
			label2.BackColor = System.Drawing.Color.Transparent;
			label2.ForeColor = NosAssistant2.Helpers.NAStyles.CounterForeColor;
			label2.Location = new System.Drawing.Point(10, 10 + num * 40);
			label2.Font = new System.Drawing.Font(" Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			label2.Width = 130;
			label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			panel.Controls.Add(label2);
			System.Windows.Forms.Button button = new System.Windows.Forms.Button();
			button.Name = (System.IntPtr)nostaleCharacterInfo.hwnd + "_FilterToggleButton";
			button.BackColor = (NosAssistant2.Helpers.PacketLogger.activeChars[nostaleCharacterInfo.hwnd] ? NosAssistant2.Helpers.NAStyles.ButtonTrueColor : NosAssistant2.Helpers.NAStyles.ButtonFalseColor);
			button.Location = new System.Drawing.Point(150, 10 + num * 40);
			button.Size = new System.Drawing.Size(30, 30);
			button.FlatAppearance.BorderSize = 0;
			button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button.Click += new System.EventHandler(ToggleButton_Click);
			panel.Controls.Add(button);
			num++;
		}
		this.filteredPanel = new System.Windows.Forms.Panel();
		this.filteredPanel.BackColor = NosAssistant2.Helpers.NAStyles.MainThemeDarker;
		this.filteredPanel.Left = 550;
		this.filteredPanel.Top = panel.Top;
		this.filteredPanel.Width = panel.Width;
		this.filteredPanel.Height = panel.Height;
		NosAssistant2.GUI.SetRoundShape(this.filteredPanel, 10);
		base.Controls.Add(this.filteredPanel);
		int num2 = 0;
		foreach (string filter in NosAssistant2.Configs.Settings.config.PacketLoggerSettings.Filters)
		{
			System.Windows.Forms.Label label3 = new System.Windows.Forms.Label();
			label3.Text = filter;
			label3.Width = 180;
			label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			label3.BackColor = System.Drawing.Color.Transparent;
			label3.ForeColor = NosAssistant2.Helpers.NAStyles.CounterForeColor;
			label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			label3.Click += new System.EventHandler(FilteredLabel_Click);
			label3.Location = new System.Drawing.Point(10, 10 + num2 * 40);
			this.filteredPanel.Controls.Add(label3);
			num2++;
		}
		NosAssistant2.GUIElements.NAButton nAButton = new NosAssistant2.GUIElements.NAButton();
		nAButton.Text = "Remove";
		nAButton.Name = "RemoveFilterButton";
		nAButton.Location = new System.Drawing.Point(this.filteredPanel.Left - 120, this.filteredPanel.Top);
		nAButton.Click += new System.EventHandler(RemoveButton_Click);
		base.Controls.Add(nAButton);
		NosAssistant2.GUIElements.NAButton nAButton2 = new NosAssistant2.GUIElements.NAButton();
		nAButton2.Text = "Add";
		nAButton2.Name = "AddFilterButton";
		nAButton2.Location = new System.Drawing.Point(this.filteredPanel.Left - 120, nAButton.Bottom + 20);
		nAButton2.Click += new System.EventHandler(AddButton_Click);
		base.Controls.Add(nAButton2);
		this.newFilterTextBox = new System.Windows.Forms.TextBox();
		this.newFilterTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.newFilterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.newFilterTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.newFilterTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.newFilterTextBox.Location = new System.Drawing.Point(this.filteredPanel.Left - 120, nAButton2.Bottom + 20);
		this.newFilterTextBox.Name = "newFilterTextBox";
		this.newFilterTextBox.Size = new System.Drawing.Size(nAButton2.Width, 26);
		this.newFilterTextBox.TabIndex = 20;
		this.newFilterTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		base.Controls.Add(this.newFilterTextBox);
		NosAssistant2.GUIElements.NAButton nAButton3 = new NosAssistant2.GUIElements.NAButton();
		nAButton3.Text = "Mode";
		nAButton3.Name = "ModeButton";
		nAButton3.Location = new System.Drawing.Point(this.filteredPanel.Left - 120, this.filteredPanel.Bottom - 80);
		nAButton3.Click += new System.EventHandler(ModeButton_Click);
		nAButton3.ForeColor = NosAssistant2.Helpers.NAStyles.CounterForeColor;
		nAButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		base.Controls.Add(nAButton3);
		this.modeLabel = new System.Windows.Forms.Label();
		this.modeLabel.Text = (NosAssistant2.Configs.Settings.config.PacketLoggerSettings.LoggerMode ? "Blacklist" : "Whitelist");
		this.modeLabel.Name = "ModeLabel";
		this.modeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.modeLabel.BackColor = System.Drawing.Color.Transparent;
		this.modeLabel.ForeColor = NosAssistant2.Helpers.NAStyles.CounterForeColor;
		this.modeLabel.Font = new System.Drawing.Font(" Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.modeLabel.Location = new System.Drawing.Point(this.filteredPanel.Left - 120, nAButton3.Bottom + 10);
		this.modeLabel.Width = nAButton3.Width;
		base.Controls.Add(this.modeLabel);
		NosAssistant2.GUIElements.NAButton nAButton4 = new NosAssistant2.GUIElements.NAButton();
		nAButton4.Text = "Rule";
		nAButton4.Name = "RuleButton";
		nAButton4.Location = new System.Drawing.Point(this.filteredPanel.Left - 120, nAButton3.Top - 100);
		nAButton4.Click += new System.EventHandler(RuleButton_Click);
		nAButton4.ForeColor = NosAssistant2.Helpers.NAStyles.CounterForeColor;
		nAButton4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		base.Controls.Add(nAButton4);
		this.ruleLabel = new System.Windows.Forms.Label();
		this.ruleLabel.Text = (NosAssistant2.Configs.Settings.config.PacketLoggerSettings.LoggerRule ? "StartsWith" : "Contains");
		this.ruleLabel.Name = "RuleLabel";
		this.ruleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ruleLabel.BackColor = System.Drawing.Color.Transparent;
		this.ruleLabel.ForeColor = NosAssistant2.Helpers.NAStyles.CounterForeColor;
		this.ruleLabel.Font = new System.Drawing.Font(" Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.ruleLabel.Location = new System.Drawing.Point(this.filteredPanel.Left - 120, nAButton4.Bottom + 10);
		this.ruleLabel.Width = nAButton3.Width;
		base.Controls.Add(this.ruleLabel);
		NosAssistant2.GUIElements.NASmallButton nASmallButton = new NosAssistant2.GUIElements.NASmallButton();
		nASmallButton.Location = new System.Drawing.Point(758, 10);
		nASmallButton.Click += new System.EventHandler(CloseButton_Click);
		base.Controls.Add(nASmallButton);
		NosAssistant2.GUI.tooltips.Add(new NosAssistant2.GUIElements.NATooltip(nAButton, "Removes selected element from the list.", NosAssistant2.Configs.Settings.config.showTooltips));
		NosAssistant2.GUI.tooltips.Add(new NosAssistant2.GUIElements.NATooltip(nAButton2, "Add element in textbox to the list.", NosAssistant2.Configs.Settings.config.showTooltips));
		NosAssistant2.GUI.tooltips.Add(new NosAssistant2.GUIElements.NATooltip(nAButton4, "Switches filter rule.\nStartsWith - Displays only packets that start with the filtered values.\nContains - Displays only packets that contain any of the filtered values.", NosAssistant2.Configs.Settings.config.showTooltips));
		NosAssistant2.GUI.tooltips.Add(new NosAssistant2.GUIElements.NATooltip(nAButton3, "Switches filter mode.\nBlacklist - Ignores packets meeting filters' criteria.\nWhitelist - Displays only packets that meet filters' criteria.", NosAssistant2.Configs.Settings.config.showTooltips));
	}

	private void ModeButton_Click(object? sender, EventArgs e)
	{
		modeLabel.Text = ((modeLabel.Text == "Whitelist") ? "Blacklist" : "Whitelist");
		Settings.config.PacketLoggerSettings.LoggerMode = !Settings.config.PacketLoggerSettings.LoggerMode;
		Settings.SaveSettings();
	}

	private void RuleButton_Click(object? sender, EventArgs e)
	{
		ruleLabel.Text = ((ruleLabel.Text == "StartsWith") ? "Contains" : "StartsWith");
		Settings.config.PacketLoggerSettings.LoggerRule = !Settings.config.PacketLoggerSettings.LoggerRule;
		Settings.SaveSettings();
	}

	private void AddButton_Click(object? sender, EventArgs e)
	{
		if (newFilterTextBox.Text.Length != 0)
		{
			Settings.config.PacketLoggerSettings.Filters.Add(newFilterTextBox.Text);
			UpdateFiltersPanel();
			newFilterTextBox.Clear();
			Settings.SaveSettings();
		}
	}

	private void RemoveButton_Click(object? sender, EventArgs e)
	{
		if (clickedFilterLabel != null)
		{
			Settings.config.PacketLoggerSettings.Filters.Remove(clickedFilterLabel);
			UpdateFiltersPanel();
			Settings.SaveSettings();
		}
	}

	private void OnMouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			DllImports.ReleaseCapture();
			DllImports.SendMessage(base.Handle, 161, 2, 0);
		}
	}

	private void CloseButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void ToggleButton_Click(object sender, EventArgs e)
	{
		Button button = (Button)sender;
		button.BackColor = ((button.BackColor == NAStyles.ButtonTrueColor) ? NAStyles.ButtonFalseColor : NAStyles.ButtonTrueColor);
		NostaleCharacterInfo nostaleCharacterInfo = GUI._nostaleCharacterInfoList.Find((NostaleCharacterInfo x) => ((IntPtr)x.hwnd).ToString() == button.Name.Split("_").ElementAt(0));
		if (nostaleCharacterInfo != null)
		{
			PacketLogger.activeChars[nostaleCharacterInfo.hwnd] = !PacketLogger.activeChars[nostaleCharacterInfo.hwnd];
			nostaleCharacterInfo.config.packetLoggerFilterState = PacketLogger.activeChars[nostaleCharacterInfo.hwnd];
			Settings.SaveSettings();
		}
	}

	private void FilteredLabel_Click(object sender, EventArgs e)
	{
		Label label = (Label)sender;
		if (label == null)
		{
			return;
		}
		foreach (Control control in label.Parent.Controls)
		{
			if (control is Label label2)
			{
				label2.BackColor = Color.Transparent;
			}
		}
		label.BackColor = NAStyles.SelectedPanelColor;
		clickedFilterLabel = label.Text;
	}

	private void UpdateFiltersPanel()
	{
		filteredPanel.Controls.Clear();
		int num = 0;
		foreach (string filter in Settings.config.PacketLoggerSettings.Filters)
		{
			Label label = new Label();
			label.Text = filter;
			label.Width = 180;
			label.TextAlign = ContentAlignment.MiddleCenter;
			label.BackColor = Color.Transparent;
			label.ForeColor = NAStyles.CounterForeColor;
			label.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label.Click += FilteredLabel_Click;
			label.Location = new Point(10, 10 + num * 40);
			filteredPanel.Controls.Add(label);
			num++;
		}
	}
}
