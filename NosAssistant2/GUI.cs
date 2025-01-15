using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NetCoreServer;
using Newtonsoft.Json;
using NosAssistant2.Configs;
using NosAssistant2.Dtos;
using NosAssistant2.Dtos.Input;
using NosAssistant2.Dtos.Output;
using NosAssistant2.EnumsID;
using NosAssistant2.GameData;
using NosAssistant2.GameObjects;
using NosAssistant2.GUIElements;
using NosAssistant2.Helpers;
using NosAssistant2.Properties;
using SharpPcap.LibPcap;

namespace NosAssistant2;

public class GUI : Form
{
	public static GUI form;

	public static RaidForm? RaidModeForm;

	public static DmgContributionCounterWindow? dmgContributionForm;

	public static MobFilterWindow? mob_filters_window = null;

	public static NostaleCharacterInfo? Main = null;

	public static NostaleCharacterInfo? Inviter = null;

	public static NostaleCharacterInfo? Mapper = null;

	public static ControlPanelItem? SelectedPanel = null;

	public static RaidsHisotryItem? SelectedHistoryitem = null;

	public static NostaleCharacterInfo? SelectedClient = null;

	public static RaiderPanelItem? draggedItem = null;

	public static int draggedItemXOffset = 0;

	private static Panel? currentPanel;

	public static int raids_history_page = 1;

	public static int current_marathon_page = 1;

	public static int family_records_page = 1;

	public static int ranking_page = 1;

	public static bool show_marathons = true;

	public static string accessNickname = "";

	public static string raidHost = "";

	public static bool PacketLoggerPrintRecv = false;

	public static bool PacketLoggerPrintSent = false;

	public static float MainMonitorScalingFactor = 100f;

	public static bool AutoFull = false;

	public static bool AutoRespawn = false;

	public static bool first_run = true;

	public static List<PictureBox> ranking_bosses_icons = new List<PictureBox>();

	public static List<PictureBox> ranking_sps_icons = new List<PictureBox>();

	public static readonly ConcurrentQueue<string> popUpsQueue = new ConcurrentQueue<string>();

	public static string version = "1.106";

	public static string currentlyModifiedKey = "";

	public static NALabel? currentlyModifiedKeyLabel = null;

	public static DateTime last_raid_notifiaction_shown = DateTime.UtcNow;

	public static DateTime last_raids_bars_refresh = DateTime.UtcNow;

	public static DateTime last_raids_bars_update = DateTime.UtcNow;

	public static DateTime last_version_check = DateTime.UtcNow;

	public static DateTime license_valid_until = DateTime.UtcNow;

	public static bool CountdownStaretd = false;

	public static DateTime CountDownEnd = DateTime.UtcNow;

	public static string CountdownMessage = "";

	public string watermark = "youtube.com/@NosAssistant";

	public static string stripe_url = "https://buy.stripe.com/28og0x5NS7mTek0dQU";

	private IContainer components;

	private BackgroundWorker packetListener;

	private BackgroundWorker connectionsUpdater;

	private BackgroundWorker packetHandler;

	private Timer timer_map_tick;

	private Panel SideMenu;

	private Panel LogoPanel;

	private Label WelcomeUserLabel;

	private NAMenuButton SwitchToControlPanelButton;

	private PictureBox LogoPicture;

	private NAMenuButton SwitchToSettingsButton;

	private NAMenuButton SwitchToMLButton;

	private NAMenuButton SwitchToRaidsButton;

	private NAMenuButton ExitButton;

	private NAMenuButton SwitchToMapButton;

	private DoubleBufferedPanel ControlPanelPanel;

	private Panel MapPanel;

	private Panel MinilandPanel;

	private Panel SettingsPanel;

	private Panel RaidsPanel;

	private PictureBox map_picture;

	private Label change_map_label;

	private NAButton InjectDLLButton;

	private Label BrowseDLLLabel;

	private Label DLLFileLabel;

	private NAButton InviterButton;

	private NAButton MapperButton;

	private Label InviterLabel;

	private Label MapperLabel;

	private Panel ControlPanelBottomPanel;

	private Label MapXLabel;

	private Label MapYLabel;

	private Panel MapBottomPanel;

	private Label MapPanelMapperLabel;

	private Label MapLabel;

	private Button CloseButton;

	private Button MinimizeButton;

	private Label accessLabel;

	private TextBox HostNameTextBox;

	private Label AutoJoinToggleLabel;

	private NAButton AutoJoinToggleButton;

	private NAButton JoinListButton;

	private Label AutoFullToggleLabel;

	private NAButton AutoFullToggleButton;

	private TrackBar AutofullThresholdTrackbar;

	private Label AutoFullThresholdLabel;

	private Label RaidHostInfoLabel;

	private DoubleBufferedPanel RaidersHPStatusPanel;

	private NAButton prepareMLButton;

	private NAButton inviteToMLButton;

	private Label MLTabInviterLabel;

	private NAButton useBuffsButton;

	private Label autoConfirmLabel;

	private NAButton autoConfirmToggleButton;

	private NAButton stackWindowsButton;

	private NAButton waterfallButton;

	private NAButton changeSPButton;

	private NAButton useSelfBuffsButton;

	private NAButton leaveRaidButton;

	private NAButton resetTitlesButton;

	private NAButton closeAltsButton;

	private NAMenuButton SwitchToPacketLoggerButton;

	private Panel PacketLoggerPanel;

	private Panel PacketLoggerBottomPanel;

	private NAButton PacketLoggerPrintRecvButton;

	private Label PacketLogerPrintRecvStatusLabel;

	private NAButton ClearPacketLoggerButton;

	private NAButton PacketLoggerPrintSentButton;

	private Label PacketLogerPrintSentStatusLabel;

	private NAButton OpenPacketFiltersButton;

	private TextBox SettingsDelayBuffTextBox;

	private NAButton AddLicenseButton;

	private Label SettingsDelayBuffLabel;

	private Button SettingsRenameButton;

	private Label RenameSettingLabel;

	private TextBox RandomRangeTextBox;

	private Label RandomRangeLabel;

	private ConsoleWindow PacketsConsole;

	private Panel MainControlPanel;

	private Panel MainMapPanel;

	private Panel MainRaidsPanel;

	private Panel MainMinilandPanel;

	private Panel MainPacketLoggerPanel;

	private Panel MainSettingsPanel;

	private Panel MainNoAccessPanel;

	private Label MainControlPanelLabel;

	private Label MainMapPanelLabel;

	private Label MainRaidsPanelLabel;

	private Label MainMinilandPanelLabel;

	private Label MainPacketLoggerPanelLabel;

	private Label MainSettingsPanelLabel;

	private Label NoAccessPanelMainLabel;

	private Panel NoAccessPanel;

	private NAButton NoAccessLicenseConfirmButton;

	private Label NoAccessInfoLabel;

	private TextBox NoAccessLicenseTextBox;

	private PopUpPanel popUpPanel;

	private NAScrollBar ControlPanelScrollbar;

	private FlowLayoutPanel flowLayoutCharactersPanel;

	private FlowLayoutPanel flowLayoutRaidsPanel;

	private NALabel NicknameHeaderLabel;

	private NALabel AttackerHeaderLabel;

	private NALabel DisableHeaderLabel;

	private NALabel MoverHeaderLabel;

	private NALabel OtherHeaderLabel;

	private NALabel BufferHeaderLabel;

	private NALabel RaiderHeaderLabel;

	private NALabel AutofullHeaderLabel;

	private Panel LoadingScreenPanel;

	private Panel panel2;

	private NALabel LoadingLabel;

	private NAProgressBar LoadingScreenBar;

	private NAScrollBar PacketLoggerScrollbar;

	private TextBox SettingsDelayInviteTextBox;

	private Label SettingsDelayInviteLabel;

	private TextBox SettingsDelayMoveTextBox;

	private Label SettingsDelayMoveLabel;

	private TextBox SettingsDelayRaidTextBox;

	private Label SettingsDelayRaidLabel;

	private NACheckbox MapShowMapperCheckBox;

	private NALabel MapShowMapperLabel;

	private NALabel MapShowPetsLabel;

	private NACheckbox MapShowPetsCheckBox;

	private NALabel MapShowAltsLabel;

	private NACheckbox MapShowAltsCheckBox;

	private NALabel MapShowEntitiesLabel;

	private NACheckbox MapShowEntitiesCheckBox;

	private NALabel MapShowMobsLabel;

	private NACheckbox MapShowMobsCheckBox;

	private NALabel MapShowPlayersLabel;

	private NACheckbox MapShowPlayersCheckBox;

	private Label VersionLabel;

	private Label useBuffsSettingsLabel;

	private PictureBox editUseBufssControlPictureBox;

	private PictureBox editWearSPControlPictureBox;

	private Label wearSPSettingsLabel;

	private PictureBox editMassHealControlPictureBox;

	private Label MassHealSettingsLabel;

	private PictureBox editInviteControlPictureBox;

	private Label InviteSettingsLabel;

	private PictureBox editUseSelfBuffsControlPictureBox;

	private Label useSelfBuffsSettingsLabel;

	private NALabel InviteControlLabel;

	private NALabel MassHealControlLabel;

	private NALabel WearSPControlLabel;

	private NALabel useSelfBuffsControlLabel;

	private NALabel useBuffsControlLabel;

	private PictureBox editJoinListControlPictureBox;

	private Label JoinListSettingsLabel;

	private NALabel JoinListControlLabel;

	private Button SettingsHotkeysButton;

	private Label HotkeysSettingLabel;

	private PictureBox editExitRaidControlPictureBox;

	private Label ExitRaidSettingsLabel;

	private NALabel ExitRaidControlLabel;

	private NAButton AddToInviteListButton;

	private Panel InviteListPanel;

	private NATextBox NewNicknameToInviteListTextBox;

	private NAButton RemoveFromInviteListButton;

	private CounterDataGrid InviteListDataGrid;

	private BindingSource inviteListBindingSource;

	private NAButton DefaultSettingsButton;

	private NAComboBox WindowSizeComboBox;

	private Label WindowSizeLabel;

	private Button SettingsSoundsButton;

	private Label SoundsSettingsLabel;

	private NAComboBox NetworkDeviceCombobox;

	private Label NetworkDeviceLabel;

	private PictureBox HideMenuButton;

	private PictureBox ResetUseSelfBuffsButton;

	private PictureBox ResetWearSPButton;

	private PictureBox ResetMassHealButton;

	private PictureBox ResetExitRaidButton;

	private PictureBox ResetJoinListButton;

	private PictureBox ResetInviteButton;

	private PictureBox ResetUseBuffsButton;

	private TextBox SettingsDelayItemsTextBox;

	private Label SettingsDelayItemsLabel;

	private NAButton BuffsetsButton;

	private PictureBox ResetUseBuffset1Button;

	private PictureBox editUseBuffset1ControlPictureBox;

	private Label useBuffset1SettingsLabel;

	private NALabel useBuffset1ControlLabel;

	private PictureBox ResetUseBuffset3Button;

	private PictureBox editUseBuffset3ControlPictureBox;

	private Label useBuffset3SettingsLabel;

	private NALabel useBuffset3ControlLabel;

	private PictureBox ResetUseBuffset2Button;

	private PictureBox editUseBuffset2ControlPictureBox;

	private Label useBuffset2SettingsLabel;

	private NALabel useBuffset2ControlLabel;

	private Label ServerLabel;

	private Label ChannelLabel;

	private NAButton OpenDmgContributionWindow;

	private Button SettingsLowSpecButton;

	private Label LowSpecSettingsLabel;

	private NAScrollBar RaidersPanelScrollbar;

	private DataGridViewTextBoxColumn nickname;

	private NAButton UseSelfBuffsRaidsButton;

	private NAButton UseBuffsRaidsButton;

	private NAButton MimicMouseButton;

	private NAButton MimicKeyboardButton;

	private Label MimicKeyboardLabel;

	private Label MimicMouseLabel;

	private Button SettingsTooltipsButton;

	private Label TooltipsSettingsLabel;

	private NAButton AutoRespawnButton;

	private Label AutoRespawnLabel;

	private PictureBox ResetUseDebuffsButton;

	private PictureBox editUseDebuffsControlPictureBox;

	private Label useDebuffsSettingsLabel;

	private NALabel useDebuffsControlLabel;

	private NAButton useDebuffsButton;

	private Panel RaidsHistoryPanel;

	private NAButton RaidsHistoryBackButton;

	private FlowLayoutPanel RaidsHistoryFlowLayoutPanel;

	private NALabel RaidsHistoryLabel;

	private FlowLayoutPanel RaidsHistorySelectedRaidPlayersFlowLayoutPanel;

	private FlowLayoutPanel RaidsHistoryDetailsPanel;

	private Panel RaidsHistoryListViewBorder;

	private CounterDataGrid RaidsHistoryDetailsGridView;

	private DoubleBufferedPanel RaidsHistoryDoubleBufferedPanel;

	private PictureBox RaidsHistoryNextPageButton;

	private PictureBox RaidsHistoryPreviousPageButton;

	private NALabel RaidsHistoryPageLabel;

	private PictureBox BackArrowRaidsHistory;

	private NALabel RaidsHistoryTabLabel;

	private NALabel FamRecordsTabLabel;

	private FlowLayoutPanel FamRecordsPanel;

	private PictureBox FamRecordsPreviousPageButton;

	private PictureBox FamRecordsNextPageButton;

	private NAMenuButton SwitchToCounterButton;

	private NAMenuButton SwitchToAnalyticsButton;

	private NALabel PlayersTabLabel;

	private Panel AnalyticsPlayersTab;

	private NAButton SearchPlayerButton;

	private NAComboBox SearchServerComboBox;

	private NATextBox SearchNicknameTextBox;

	private NALabel SearchedPlayerFamily;

	private NALabel SearchedPlayerNickname;

	private PictureBox SearchedPlayerAvatar;

	private NALabel SearchedPlayerClassSex;

	private NALabel SearchedPlayerLVLCLVL;

	private PictureBox Reputation;

	private PictureBox Armor;

	private NALabel SearchedPlayerArmorLabel;

	private NALabel SecondaryWeaponUpgrade;

	private PictureBox SecondaryWeapon;

	private NALabel SearchedPlayerSecondWeaponLabel;

	private NALabel MainWeaponUpgrade;

	private PictureBox MainWeapon;

	private NALabel SearchedPlayerMainWeaponLabel;

	private PictureBox Wings;

	private NALabel SearchedPlayerWingsLabel;

	private PictureBox WeaponSkin;

	private NALabel SearchedPlayerWeaponSkinLabel;

	private PictureBox CostumeHat;

	private NALabel SearchedPlayerCostumeHatLabel;

	private PictureBox Costume;

	private NALabel SearchedPlayerCostumeLabel;

	private PictureBox FlyingPet;

	private NALabel SearchedPlayerFlyingPetLabel;

	private PictureBox Mask;

	private NALabel SearchedPlayerMaskLabel;

	private PictureBox Hat;

	private NALabel SearchedPlayerHatLabel;

	private NALabel ArmorUpgrade;

	private FlowLayoutPanel SearchedPlayerFairiesFlowLayoutPanel;

	private FlowLayoutPanel SearchedPlayerSPsFlowLayoutPanel;

	private NALabel SearchedPlayerLastUpdateLabel;

	private NALabel SearchedPlayerLastUpdateDateLabel;

	private PictureBox AnalyticsBackArrow;

	private NALabel SearchedPlayerFamilyRole;

	private NALabel SearchedPlayerTitle;

	private Panel ShellInfoMainPanel;

	private PictureBox SwitchShellTypeButton;

	private NALabel ShellItemTypeLabel;

	private NAButton ShowMarathonTotalButton;

	private PictureBox SwitchToRuneButton;

	private PictureBox SwitchToShellButton;

	private Panel SPDetailsPanel;

	private NALabel SPDetailsPerfectionLabel;

	private NALabel SPDetailsJobLabel;

	private PictureBox SPDetailsAvatar;

	private PictureBox SPDetailsAttackImage;

	private NALabel SPDetailsShadowLabel;

	private PictureBox SPDetailsShadowImage;

	private NALabel SPDetailsLightLabel;

	private PictureBox SPDetailsLightImage;

	private NALabel SPDetailsWaterLabel;

	private PictureBox SPDetailsWaterImage;

	private NALabel SPDetailsFireLabel;

	private PictureBox SPDetailsFireImage;

	private NALabel SPDetailsEnergyLabel;

	private PictureBox SPDetailsEnergyImage;

	private NALabel SPDetailsPropertyLabel;

	private PictureBox SPDetailsPropertyImage;

	private NALabel SPDetailsDefenceLabel;

	private PictureBox SPDetailsDefenceImage;

	private NALabel SPDetailsAttackLabel;

	private PictureBox CloseSPDetailsButton;

	private Panel SPDetailsBorderPanel;

	private FlowLayoutPanel ShellEffectsFlowLayoutPanel;

	private PictureBox Tattoo1Icon;

	private NALabel Tattoo2UpgradeLabel;

	private PictureBox Tattoo2Icon;

	private NALabel Tattoo1UpgradeLabel;

	private NALabel RuneLevelLabel;

	private NAButton SaveConfigButton;

	private NAButton LoadConfigButton;

	private Panel FairyDetailsPanel;

	private FlowLayoutPanel FairyEffectsFlowLayoutPanel;

	private PictureBox FairyDetailsIcon;

	private PictureBox CloseFairyDetailsButton;

	private NALabel FairyDetailsLabel;

	private Panel MainFairyDetailsPanel;

	private NALabel FairyUpgradePercentLabel;

	private Panel RankingTabPanel;

	private NALabel RankingTabLabel;

	private FlowLayoutPanel RankingFlowLayoutPanel;

	private FlowLayoutPanel RankingSPFilterFlowLayoutPanel;

	private FlowLayoutPanel RankingRaidTypeFilterFlowLayoutPanel;

	private NAButton ClearRankingFiltersButton;

	private NALabel RankingModeLabel;

	private NAButton RankingModeButton;

	private NAButton RankingSearchButton;

	private PictureBox RankingPreviousPageButton;

	private PictureBox RankingNextPageButton;

	private NALabel RankingPageLabel;

	private DoubleBufferedPanel RankingDoubleBufferedPanel;

	private NAButton SettingsSoundsMenuButton;

	private NAButton SaveWindowsButton;

	private NAButton LoadWindowsButton;

	private NAButton ResizeWindowsButton;

	private TextBox ResizeHeightTextBox;

	private TextBox ResizeWidthTextBox;

	private Label ResizeWidthLabel;

	private Label ResizeHeightLabel;

	private NAButton LimitFPSButton;

	private Label FPSLabel;

	private TextBox FPSTextBox;

	private Label OpenBoxesLabel;

	private NAButton OpenBoxesButton;

	private PictureBox ResetArcaneWisdomButton;

	private PictureBox editArcaneWisdomControlPictureBox;

	private Label arcaneWisdomLabel;

	private NALabel arcaneWisdomControlLabel;

	private NAMenuButton JoinDiscordButton;

	private NAButton MapMobFilterButton;

	private NAButton RaidsNotificationsButton;

	private FlowLayoutPanel PetTrainerFlowLayoutPanel;

	private NALabel PetTrainerTimerLabel1;

	private Panel panel1;

	private PictureBox PetTrainerIcon1;

	private Panel panel3;

	private PictureBox PetTrainerIcon2;

	private NALabel PetTrainerTimerLabel2;

	private Panel panel4;

	private PictureBox PetTrainerIcon3;

	private NALabel PetTrainerTimerLabel3;

	private Panel panel5;

	private PictureBox TrainedPetIcon1;

	private NALabel TrainedPetInfoLabel1;

	private Panel panel6;

	private PictureBox TrainedPetIcon2;

	private NALabel TrainedPetInfoLabel2;

	private Panel panel7;

	private PictureBox TrainedPetIcon3;

	private NALabel TrainedPetInfoLabel3;

	private PictureBox SwitchRaidersTabPanel;

	private FlowLayoutPanel RaidsBarsStatusFlowLayoutPanel;

	private NALabel RefreshRaidsBarsLabel;

	private NATextBox RaidsHistoryFilterTextBox;

	private Panel CountDownPanel;

	private NALabel CountDownLabel;

	private NALabel BuyLicenseLabel;

	private NALabel ForgotLicenseLabel;

	private NAButton ChangeNicknameButton;

	private NAButton BuyLicenseButton;

	private DataGridViewTextBoxColumn ID;

	private DataGridViewTextBoxColumn CharacterID;

	private DataGridViewTextBoxColumn Lp;

	private DataGridViewTextBoxColumn PlayerName;

	private DataGridViewTextBoxColumn CLvl;

	private DataGridViewTextBoxColumn Family;

	private DataGridViewTextBoxColumn Total;

	private DataGridViewTextBoxColumn MaxHit;

	private DataGridViewImageColumn MaxHitIcon;

	private DataGridViewTextBoxColumn Pets;

	private DataGridViewTextBoxColumn Special;

	private DataGridViewTextBoxColumn MobDmg;

	private DataGridViewTextBoxColumn OnyxDmg;

	private DataGridViewTextBoxColumn All;

	private DataGridViewTextBoxColumn Gold;

	private DataGridViewTextBoxColumn Average;

	private DataGridViewTextBoxColumn Hit;

	private DataGridViewTextBoxColumn Miss;

	private DataGridViewTextBoxColumn Crit;

	private DataGridViewTextBoxColumn Bon;

	private DataGridViewTextBoxColumn BonCrit;

	private DataGridViewTextBoxColumn Dbf;

	private DataGridViewTextBoxColumn Dead;

	private DataGridViewTextBoxColumn MBHit;

	private DataGridViewTextBoxColumn AllHits;

	private DataGridViewTextBoxColumn AllMiss;

	private NALabel LicesneExpirationDateLabel;

	private NALabel RaidsHistoryBestTimeLabel;

	private NALabel RaidsHistoryAverageTimeLabel;

	private Panel PlayerRaidsStatisticsPanel;

	private FlowLayoutPanel PlayerRaidsSelectionFlowLayoutPanel;

	private NAButton PlayerRaidsStatisticsButton;

	private NAButton PlayerEquipementButton;

	private TableLayoutPanel PlayerRaidsStatisticsRaidsStatisticsTablePanel;

	private NALabel PlayerRaidsStatisticsBestTimeLabel;

	private NALabel PlayerRaidsStatisticsRaidsFinishedLabel;

	private NALabel PlayerRaidsStatisticsTotalMaxHitLabel;

	private NALabel PlayerRaidsStatisticsTotalMaxHitRankLabel;

	private NALabel PlayerRaidsStatisticsBestTimeRankLabel;

	private NALabel PlayerRaidsStatisticsRaidsFinishedRankLabel;

	private NALabel PlayerRaidsStatisticsBossNameLabel;

	private PictureBox PlayerRaidsStatisticsBossIcon;

	private NALabel RankingRaidsDoneLabel;

	private NALabel RankingBestTimesLabel;

	private NALabel RankingMaxHitsLabel;

	private NALabel RankingAverageDMGLabel;

	private NALabel PlayersRaidsStatisticsLoadingLabel;

	private NAButton SettingsWaypointsMenuButton;

	private NALabel ChangeNicknameLabel;

	private Panel MainQuestsPanel;

	private Label QuestsLabel;

	private NAMenuButton SwitchToQuestsButton;

	private Panel QuestsPanel;

	private FlowLayoutPanel QuestsFlowLayoutPanel;

	private PictureBox QuestsTabMap;

	private NALabel QuestPathLabel;

	private FlowLayoutPanel QuestObjectiveIconsFlowLayoutPanel;

	private NALabel QuestObjectiveLabel;

	private Panel QuestSearchTypesPanel;

	private NALabel QuestMobTypeLabel;

	private NALabel QuestNPCTypeLabel;

	private NATextBox QuestSearchTextBox;

	private NALabel QuestTSTypeLabel;

	private NALabel QuestMapTypeLabel;

	private NAButton QuestNavigateButton;

	private FlowLayoutPanel QuestSearchResultsFlowLayoutPanel;

	private PictureBox ShowTimeSpaceMapButton;

	private NALabel AccounstCountLabel;

	private NAButton StopNavigatingButton;

	private PictureBox SwitchMapModeButton;

	private PictureBox SwitchMapModeQuestButton;

	private PictureBox OpenMapInNewWindowMapTabButton;

	public static List<NostaleCharacterInfo> _nostaleCharacterInfoList { get; set; } = new List<NostaleCharacterInfo>();


	public static List<GameField> fields { get; set; } = new List<GameField>();


	public static List<GameEntity> entities { get; set; } = new List<GameEntity>();


	public static List<GameMonster> monsters { get; set; } = new List<GameMonster>();


	public static List<GamePlayer> players { get; set; } = new List<GamePlayer>();


	public static List<GameTimeSpace> time_spaces { get; set; } = new List<GameTimeSpace>();


	public static List<SpawnTimedMob> spawn_timed_mobs { get; set; } = new List<SpawnTimedMob>();


	public static List<NATooltip> tooltips { get; set; } = new List<NATooltip>();


	public static List<GamePlayer> miniland_state { get; set; } = new List<GamePlayer>();


	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = base.CreateParams;
			createParams.Style |= 131072;
			createParams.ClassStyle |= 8;
			return createParams;
		}
	}

	public GUI()
	{
		InitializeComponent();
		form = this;
		Application.Idle += Application_Idle;
	}

	private void Application_Idle(object sender, EventArgs e)
	{
		Application.Idle -= Application_Idle;
		if (!VersionManager.CheckIfNpcapIsInstalled())
		{
			new NAMessageBox("Npcap not installed!", "Error", error: true).ShowDialog();
			//Application.Exit();
			//return;
		}
		if (CheckAccess.isNAAlreadyRunning())
		{
			//new NAMessageBox("NosAssistant2 is already running!", "Error", error: true).ShowDialog();
			//Application.Exit();
			//return;
		}
		BlockAccess();
		if (VersionManager.CheckForUpdates())
		{
			VersionManager.DownloadMissingFiles();
		}
	}

	public static void StartRoutine()
	{
		PacketsManager.UpdateNostaleCharacterInfosList();
		Settings.LoadSettings();
		NAHttpClient.SetHttpClient();
		CheckAccess.checkAccess();
		Settings.LoadCharsConfig();
		MainMonitorScalingFactor = getScalingFactor();
		RaidModeForm = new RaidForm();
		RaidModeForm.Show();
		RaidModeForm.Hide();
		dmgContributionForm = new DmgContributionCounterWindow();
		dmgContributionForm.Show();
		dmgContributionForm.Hide();
		Controller.renamedClients = Settings.config.renameClients;
		Controller.RenameClients(false);
		PacketLogger.updatePacketsLoggerDict();
        GUI.form.connectionsUpdater.RunWorkerAsync();
        GUI.form.packetListener.RunWorkerAsync();
        GUI.form.packetHandler.RunWorkerAsync();
		KeyboardManager.StartListeningKeyboard();
		KeyboardManager.StartListeningMouse();
		ScaleRaidsHistoryDGV();
		NAvigator.FetchGameWorld();
		AppDomain.CurrentDomain.UnhandledException += NALogger.UnhandledExceptionHandler;
		if (Mapper != null)
		{
			QuestManager.UpdateQuestList(Mapper.config.quests);
		}
		if (Main == null)
		{
			ShowNoAccessPanel();
		}
	}

	private void GUI_Load(object sender, EventArgs e)
	{
		Settings.LoadSettings();
		MainMonitorScalingFactor = getScalingFactor();
		setGuiElements();
		SetTooltips();
		base.Width = Convert.ToInt32(MainMonitorScalingFactor / 100f * (float)base.Width);
		base.Height = Convert.ToInt32(MainMonitorScalingFactor / 100f * (float)base.Height);
		ScaleControls(base.Controls);
	}

	public static int getScalingFactor()
	{
		return Settings.config.window_size switch
		{
			0 => 100, 
			1 => 85, 
			2 => 75, 
			_ => 100, 
		};
	}

	public static void ScaleRaidsHistoryDGV()
	{
        GUI.form.RaidsHistoryDetailsGridView.DefaultCellStyle.Font = new Font("Segoe UI", Convert.ToInt32(MainMonitorScalingFactor / 100f * 9f));
        GUI.form.RaidsHistoryDetailsGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", Convert.ToInt32(MainMonitorScalingFactor / 100f * 9f));
		foreach (DataGridViewColumn column in GUI.form.RaidsHistoryDetailsGridView.Columns)
		{
			column.Width = Convert.ToInt32(MainMonitorScalingFactor / 100f * (float)column.Width);
		}
	}

	public static void ScaleControl(Control control)
	{
		if (MainMonitorScalingFactor == 100f)
		{
			return;
		}
		float num = MainMonitorScalingFactor / 100f * ((float)GUI.form.ClientSize.Width / (float)GUI.form.Width);
		float num2 = MainMonitorScalingFactor / 100f * ((float)GUI.form.ClientSize.Height / (float)GUI.form.Height);
		if (control.Name.Contains("shell_label") || control.Name.Contains("rune_label"))
		{
			float num3 = control.Font.Size * num * num;
			if (num3 <= 0f)
			{
				num3 = 1f;
			}
			control.Font = new Font(control.Font.FontFamily, num3, control.Font.Style);
		}
		else
		{
			if (control is DataGridView)
			{
				return;
			}
			if (control.Name == "RankingFlowLayoutPanel")
			{
				control.Size = ((control.Parent != null) ? control.Parent.Size : control.Size);
				control.Location = new Point(0, 0);
				return;
			}
			if (control.Name == "PlayerRaidsStatisticsRaidsStatisticsTablePanel")
			{
				control.Location = new Point(15, 135);
			}
			control.Size = new Size((int)Math.Floor((float)control.Width * num), (int)Math.Floor((float)control.Height * num2));
			control.Location = new Point((int)Math.Floor((float)control.Location.X * num), (int)Math.Floor((float)control.Location.Y * num2));
			if (control is RankingItem)
			{
				int num4 = ((Settings.config.window_size != 2) ? (-1) : 0);
				control.Margin = new Padding((int)((float)control.Margin.Left * num), (int)((float)control.Margin.Top * num2) - num4, (int)((float)control.Margin.Right * num), (int)((float)control.Margin.Bottom * num2));
			}
			if (control is NAButton nAButton)
			{
				float num5 = nAButton.Font.Size * num * 0.8f;
				if (num5 <= 0f)
				{
					num5 = 1f;
				}
				nAButton.Font = new Font(nAButton.Font.FontFamily, num5, nAButton.Font.Style);
			}
			else if (control is NALabel nALabel)
			{
				float num6 = nALabel.Font.Size * num * 0.8f;
				if (num6 <= 0f)
				{
					num6 = 1f;
				}
				nALabel.Font = new Font(nALabel.Font.FontFamily, num6, nALabel.Font.Style);
			}
			else if (control is ConsoleWindow)
			{
				if (control is ConsoleWindow consoleWindow)
				{
					foreach (ColumnHeader column in consoleWindow.Columns)
					{
						column.Width = (int)((float)column.Width * num);
					}
				}
			}
			else if (control is NAScrollBar && control.Name == "PacketLoggerScrollbar")
			{
				control.Width = 20;
				if (MainMonitorScalingFactor == 85f)
				{
					control.Left -= (int)(4f * num);
				}
				else
				{
					control.Left -= (int)(6f * num);
				}
			}
			else
			{
				float num7 = control.Font.Size * num;
				if (num7 <= 0f)
				{
					num7 = 1f;
				}
				control.Font = new Font(control.Font.FontFamily, num7, control.Font.Style);
			}
			if (control.Controls.Count > 0)
			{
				foreach (Control control2 in control.Controls)
				{
					ScaleControl(control2);
				}
			}
			if (control is PictureBox { Image: not null } pictureBox)
			{
				pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox.Refresh();
			}
			if (control is Button { Image: not null } button && !string.IsNullOrEmpty(button.Text))
			{
				button.Padding = new Padding((int)Math.Floor((float)button.Padding.Left * num), (int)Math.Floor((float)button.Padding.Top * num2), (int)Math.Floor((float)button.Padding.Right * num), (int)Math.Floor((float)button.Padding.Bottom * num2));
			}
			if (control.Parent != null && control.Parent.Name == "ComboBoxBorder")
			{
				control.Width -= 2;
				control.Height -= 2;
				control.Left += 2;
				control.Top += 2;
			}
			if (control.Parent != null && control.Parent is ComboBoxItem)
			{
				control.Width -= 2;
				control.Height--;
				control.Left += 2;
				control.Top++;
			}
			if ((control is RaidsHisotryItem || control is MarathonItem) && MainMonitorScalingFactor != 100f)
			{
				double num8 = ((MainMonitorScalingFactor == 85f) ? 0.97 : 0.92);
				control.Height = (int)Math.Floor((double)control.Height * num8);
				control.Top = (int)Math.Floor((double)control.Top * num8);
			}
			if (control is FamilyRecordsItem)
			{
				Padding margin = control.Margin;
				Padding margin2 = new Padding((int)((float)margin.Left * num), (int)((float)margin.Top * num), (int)((float)margin.Right * num), (int)((float)margin.Bottom * num));
				control.Margin = margin2;
			}
		}
	}

	public static void ScaleControls(Control.ControlCollection controls)
	{
		foreach (Control control in controls)
		{
			ScaleControl(control);
		}
        GUI.form.Region = System.Drawing.Region.FromHrgn(DllImports.CreateRoundRectRgn(0, 0, GUI.form.Width, GUI.form.Height, 25, 25));
		RoundAllTabsPanels();
	}

	public static void BlockAccess()
	{
		Utils.InvokeIfRequired(GUI.form.MainNoAccessPanel, delegate
		{
            GUI.form.MainNoAccessPanel.Enabled = true;
            GUI.form.MainNoAccessPanel.Show();
            GUI.form.SideMenu.Show();
		});
		foreach (Panel p in GUI.form.Controls.OfType<Panel>().ToList())
		{
			if (p.Name == "SideMenu" || p.Name == "popUpPanel")
			{
				foreach (Button button in p.Controls.OfType<Button>().ToList())
				{
					if (!button.Name.Contains("Exit") && !button.Name.Contains("Discord"))
					{
						Utils.InvokeIfRequired(button, delegate
						{
							button.Enabled = false;
						});
					}
				}
				continue;
			}
			Utils.InvokeIfRequired(p, delegate
			{
				if (p.Name != "MainNoAccessPanel" && p.Name != "LoadingScreenPanel")
				{
					p.Hide();
					p.Enabled = false;
				}
			});
		}
		if (RaidModeForm != null)
		{
			Utils.InvokeIfRequired(RaidModeForm, RaidModeForm.Hide);
		}
		if (dmgContributionForm != null)
		{
			Utils.InvokeIfRequired(dmgContributionForm, dmgContributionForm.Hide);
		}
		Main = null;
		accessNickname = "";
		updateWelcomeLabel("");
	}

	public static void ShowNoAccessPanel()
	{
		Utils.InvokeIfRequired(GUI.form.MainNoAccessPanel, delegate
		{
            GUI.form.MainNoAccessPanel.Enabled = true;
            GUI.form.MainNoAccessPanel.Show();
            GUI.form.LoadingScreenPanel.Hide();
		});
	}

	public static void GrantAccess()
	{
		Utils.InvokeIfRequired(GUI.form, delegate
		{
			if (currentPanel != null)
			{
				currentPanel.Show();
				Button button2 = GUI.form.SideMenu.Controls.OfType<Button>().ToList().Find((Button x) => x.Name.Contains(currentPanel.Name.Substring(4, 3)));
				if (button2 == null)
				{
					return;
				}
				button2.BackColor = NAStyles.MenuButtonPressedColor;
			}
			else
			{
                GUI.form.MainControlPanel.Show();
				currentPanel = GUI.form.MainControlPanel;
                GUI.form.SwitchToControlPanelButton.BackColor = NAStyles.MenuButtonPressedColor;
			}
            GUI.form.LicesneExpirationDateLabel.Text = "License Expires: " + license_valid_until.ToLocalTime().ToString("dd/MM/yyyy");
		});
		foreach (Panel p in GUI.form.Controls.OfType<Panel>().ToList())
		{
			if (p.Name == "SideMenu")
			{
				foreach (Button button in p.Controls.OfType<Button>().ToList())
				{
					if (!button.Name.Contains("Exit"))
					{
						Utils.InvokeIfRequired(button, delegate
						{
							button.Enabled = true;
						});
					}
				}
				continue;
			}
			Utils.InvokeIfRequired(p, delegate
			{
				if (p.Name != "MainNoAccessPanel" && p.Name != "LoadingScreenPanel")
				{
					p.Enabled = true;
				}
				else
				{
					p.Hide();
					p.Enabled = false;
				}
			});
		}
	}

	private void packetListener_DoWork(object sender, DoWorkEventArgs e)
	{
		PacketsManager.Listen(sender, e);
	}

	private void connectionsUpdater_DoWork(object sender, DoWorkEventArgs e)
	{
		PacketsManager.UpdateConnectionsData(sender, e);
	}

	private void packetHandler_DoWork(object sender, DoWorkEventArgs e)
	{
		PacketsManager.HandlePackets(sender, e);
	}

	private void setGuiElements()
	{
		base.Region = System.Drawing.Region.FromHrgn(DllImports.CreateRoundRectRgn(0, 0, form.Width, form.Height, 25, 25));
		RoundAllTabsPanels();
		VersionLabel.Text = "version " + version;
		currentPanel = MainControlPanel;
		DLLFileLabel.Text = Settings.config.dllPath.Split("\\").Last();
		AutofullThresholdTrackbar.Value = Settings.config.autoFullThreshold;
		AutoFullThresholdLabel.Text = Settings.config.autoFullThreshold + "%";
		raidHost = Settings.config.raidHostName;
		HostNameTextBox.Text = raidHost;
		autoConfirmLabel.Text = (Settings.config.autoconfirm ? "On" : "Off");
		AutoJoinToggleLabel.Text = (Settings.config.autoJoinList ? "On" : "Off");
		SettingsRenameButton.BackColor = (Settings.config.renameClients ? NAStyles.ButtonTrueColor : NAStyles.ButtonFalseColor);
		SettingsHotkeysButton.BackColor = (Settings.config.enableHotkeys ? NAStyles.ButtonTrueColor : NAStyles.ButtonFalseColor);
		SettingsSoundsButton.BackColor = (Settings.config.playSounds ? NAStyles.ButtonTrueColor : NAStyles.ButtonFalseColor);
		SettingsLowSpecButton.BackColor = (Settings.config.low_spec ? NAStyles.ButtonTrueColor : NAStyles.ButtonFalseColor);
		SettingsTooltipsButton.BackColor = (Settings.config.showTooltips ? NAStyles.ButtonTrueColor : NAStyles.ButtonFalseColor);
		RandomRangeTextBox.Text = Settings.config.randomizeCordsRange.ToString();
		SettingsDelayBuffTextBox.Text = Settings.config.DelaySettings.Buff.ToString();
		SettingsDelayRaidTextBox.Text = Settings.config.DelaySettings.Raid.ToString();
		SettingsDelayMoveTextBox.Text = Settings.config.DelaySettings.Move.ToString();
		SettingsDelayInviteTextBox.Text = Settings.config.DelaySettings.Invite.ToString();
		SettingsDelayItemsTextBox.Text = Settings.config.DelaySettings.Items.ToString();
		useBuffsControlLabel.Text = (Settings.config.ControlsSettings.useBuffs.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.useBuffs.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.useBuffs.Item1));
		InviteControlLabel.Text = (Settings.config.ControlsSettings.invite.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.invite.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.invite.Item1));
		JoinListControlLabel.Text = (Settings.config.ControlsSettings.joinList.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.joinList.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.joinList.Item1));
		ExitRaidControlLabel.Text = (Settings.config.ControlsSettings.exitRaid.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.exitRaid.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.exitRaid.Item1));
		MassHealControlLabel.Text = (Settings.config.ControlsSettings.massHeal.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.massHeal.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.massHeal.Item1));
		WearSPControlLabel.Text = (Settings.config.ControlsSettings.wearSP.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.wearSP.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.wearSP.Item1));
		useSelfBuffsControlLabel.Text = (Settings.config.ControlsSettings.useSelfBuffs.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.useSelfBuffs.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.useSelfBuffs.Item1));
		useBuffset1ControlLabel.Text = (Settings.config.ControlsSettings.useBuffset1.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.useBuffset1.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.useBuffset1.Item1));
		useBuffset2ControlLabel.Text = (Settings.config.ControlsSettings.useBuffset2.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.useBuffset2.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.useBuffset2.Item1));
		useBuffset3ControlLabel.Text = (Settings.config.ControlsSettings.useBuffset3.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.useBuffset3.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.useBuffset3.Item1));
		useDebuffsControlLabel.Text = (Settings.config.ControlsSettings.useDebuffs.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.useDebuffs.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.useDebuffs.Item1));
		arcaneWisdomControlLabel.Text = (Settings.config.ControlsSettings.arcaneWisdom.Item2 ? KeyboardManager.KeyToString((Keys)Settings.config.ControlsSettings.arcaneWisdom.Item1) : KeyboardManager.IntToMouseButtonName(Settings.config.ControlsSettings.arcaneWisdom.Item1));
        GUI.form.ControlPanelScrollbar.targetPanel = GUI.form.flowLayoutCharactersPanel;
        GUI.form.flowLayoutCharactersPanel.MouseWheel += FlowLayoutCharactersPanel_MouseWheel;
        GUI.form.ControlPanelScrollbar.MouseWheel += FlowLayoutCharactersPanel_MouseWheel;
        GUI.form.ControlPanelScrollbar.updateScrollButtonSize(GUI.form.flowLayoutCharactersPanel.Controls.Count * 40);
        GUI.form.PacketLoggerScrollbar.targetPanel = PacketsConsole;
        GUI.form.RaidersPanelScrollbar.targetPanel = flowLayoutRaidsPanel;
        GUI.form.flowLayoutRaidsPanel.MouseWheel += FlowLayoutRaidsPanel_MouseWheel;
        GUI.form.RaidersPanelScrollbar.MouseWheel += FlowLayoutRaidsPanel_MouseWheel;
        GUI.form.RaidersPanelScrollbar.updateScrollButtonSize(GUI.form.flowLayoutRaidsPanel.Controls.Count * 35);
		MapShowAltsCheckBox.SetState(Settings.config.MapSettings.Alts, NAStyles.AltColor);
		MapShowPlayersCheckBox.SetState(Settings.config.MapSettings.Players, NAStyles.PlayersColor);
		MapShowPetsCheckBox.SetState(Settings.config.MapSettings.Pets, NAStyles.PetsColor);
		MapShowMapperCheckBox.SetState(Settings.config.MapSettings.Mapper, NAStyles.MapperColor);
		MapShowEntitiesCheckBox.SetState(Settings.config.MapSettings.Entities, NAStyles.EntitiesColor);
		MapShowMobsCheckBox.SetState(Settings.config.MapSettings.Mobs, NAStyles.MonstersColor);
		List<string> list = new List<string> { "large", "medium", "small" };
		WindowSizeComboBox.setItems(list);
		WindowSizeComboBox.setState(list.ElementAt(Settings.config.window_size));
		WindowSizeComboBox.setAction(changeWindowSize);
		SetRoundShape(PetTrainerFlowLayoutPanel, 15);
		NetworkDeviceCombobox.onDropdownClick = SetListOfNetworkDevices;
		NetworkDeviceCombobox.onItemChangedAction = SetDefaultNetworkDevice;
		SearchServerComboBox.setItems(NostaleServers.GetAllServersNames());
		SearchServerComboBox.setState(SearchServerComboBox.items.ElementAt(1));
		SearchServerComboBox.setAction(delegate
		{
			Analytics.SearchServer = SearchServerComboBox.currentState;
		});
		SetRoundShape(SearchedPlayerFairiesFlowLayoutPanel, 15);
		SetRoundShape(SearchedPlayerSPsFlowLayoutPanel, 15);
		CreateRankingIcons();
		UpdateQuestsFlowLayoutPanel();
		SetRoundShape(QuestsFlowLayoutPanel, 15);
		SetRoundShape(QuestSearchResultsFlowLayoutPanel, 15);
		QuestNPCTypeLabel.ForeColor = NAStyles.NotActiveCharColor;
	}

	public static void changeWindowSize()
	{
		if (GUI.form.WindowSizeComboBox.currentState == "large")
		{
			Settings.config.window_size = 0;
		}
		if (GUI.form.WindowSizeComboBox.currentState == "medium")
		{
			Settings.config.window_size = 1;
		}
		if (GUI.form.WindowSizeComboBox.currentState == "small")
		{
			Settings.config.window_size = 2;
		}
		Settings.SaveSettings();
		ShowPopUp("Reset to apply changes", isNotification: true);
	}

	private static void RoundAllTabsPanels()
	{
		foreach (Panel item in GUI.form.Controls.OfType<Panel>().ToList())
		{
			if (item.Name != "SideMenu")
			{
				SetRoundShape(item, 15);
			}
		}
		SetRoundShape(GUI.form.InviteListPanel, 15);
		SetRoundShape(GUI.form.RaidsHistoryFlowLayoutPanel, 15);
		SetRoundShape(GUI.form.RaidsHistorySelectedRaidPlayersFlowLayoutPanel, 15);
	}

	public static void AddCharacterToControlPanel(NostaleCharacterInfo character)
	{
		Utils.InvokeIfRequired(GUI.form.ControlPanelPanel, delegate
		{
			ControlPanelItem controlPanelItem = new ControlPanelItem
			{
				character = character
			};
			ScaleControl(controlPanelItem);
            GUI.form.flowLayoutCharactersPanel.Controls.Add(controlPanelItem);
            GUI.form.ControlPanelScrollbar.updateScrollButtonSize(GUI.form.flowLayoutCharactersPanel.Controls.Count * 40);
            GUI.form.AccounstCountLabel.Text = $"Accounts: {GUI.form.flowLayoutCharactersPanel.Controls.Count}";
		});
	}

	public static void RemoveCharacterPanel(NostaleCharacterInfo character)
	{
		ControlPanelItem currentControl = GUI.form.flowLayoutCharactersPanel.Controls.OfType<ControlPanelItem>().ToList().Find((ControlPanelItem x) => x.character?.hwnd == character.hwnd);
		if (currentControl != null)
		{
			Utils.InvokeIfRequired(GUI.form.flowLayoutCharactersPanel, delegate
			{
                GUI.form.flowLayoutCharactersPanel.Controls.Remove(currentControl);
                GUI.form.ControlPanelScrollbar.updateScrollButtonSize(GUI.form.flowLayoutCharactersPanel.Controls.Count * 40);
				currentControl.Dispose();
                GUI.form.AccounstCountLabel.Text = $"Accounts: {GUI.form.flowLayoutCharactersPanel.Controls.Count}";
			});
		}
	}

	public static void UpdateCharacterPanel(NostaleCharacterInfo character)
	{
		ControlPanelItem currentControl = GUI.form.flowLayoutCharactersPanel.Controls.OfType<ControlPanelItem>().ToList().Find((ControlPanelItem x) => x.character?.hwnd == character.hwnd);
		if (currentControl != null)
		{
			Utils.InvokeIfRequired(GUI.form.flowLayoutCharactersPanel, delegate
			{
				currentControl.updateInfo();
				currentControl.updateAvatar();
			});
		}
	}

	public static void AddCharacterToRaidersPanel(NostaleCharacterInfo character)
	{
		if (character.config.isRaider && !character.config.isDisabled && !(character.status == "offline"))
		{
			Utils.InvokeIfRequired(GUI.form.flowLayoutRaidsPanel, delegate
			{
				RaiderPanelItem raiderPanelItem = new RaiderPanelItem
				{
					character = character
				};
				ScaleControl(raiderPanelItem);
                GUI.form.flowLayoutRaidsPanel.Controls.Add(raiderPanelItem);
				RaidModeForm?.AddCharacterToRaidersPanel(character);
			});
			if (GUI.form.flowLayoutRaidsPanel.Visible)
			{
                GUI.form.RaidersPanelScrollbar.updateScrollButtonSize(GUI.form.flowLayoutRaidsPanel.Controls.Count * 35);
			}
		}
	}

	public static void RemoveRaiderPanel(NostaleCharacterInfo character)
	{
		RaiderPanelItem currentControl = GUI.form.flowLayoutRaidsPanel.Controls.OfType<RaiderPanelItem>().ToList().Find((RaiderPanelItem x) => x.character?.hwnd == character.hwnd);
		if (currentControl != null)
		{
			Utils.InvokeIfRequired(GUI.form.flowLayoutRaidsPanel, delegate
			{
                GUI.form.flowLayoutRaidsPanel.Controls.Remove(currentControl);
				currentControl.Dispose();
			});
			if (GUI.form.flowLayoutRaidsPanel.Visible)
			{
                GUI.form.RaidersPanelScrollbar.updateScrollButtonSize(GUI.form.flowLayoutRaidsPanel.Controls.Count * 35);
			}
			RaidModeForm?.RemoveRaiderPanel(character);
		}
	}

	public static void UpdateRaiderPanel(NostaleCharacterInfo character)
	{
		RaiderPanelItem raiderPanelItem = GUI.form.flowLayoutRaidsPanel.Controls.OfType<RaiderPanelItem>().ToList().Find((RaiderPanelItem x) => x.character?.hwnd == character.hwnd);
		if (raiderPanelItem != null)
		{
			raiderPanelItem.updateHpBar();
			raiderPanelItem.updateAvatar();
			RaidModeForm?.UpdateRaiderPanel(character);
		}
	}

	public static void SetRoundShape(Control control, int radius)
	{
		GraphicsPath graphicsPath = new GraphicsPath();
		graphicsPath.AddArc(control.ClientRectangle.Right - radius, control.ClientRectangle.Top, radius, radius, 270f, 90f);
		graphicsPath.AddArc(control.ClientRectangle.Right - radius, control.ClientRectangle.Bottom - radius, radius, radius, 0f, 90f);
		graphicsPath.AddArc(control.ClientRectangle.Left, control.ClientRectangle.Bottom - radius, radius, radius, 90f, 90f);
		graphicsPath.AddArc(control.ClientRectangle.Left, control.ClientRectangle.Top, radius, radius, 180f, 90f);
		graphicsPath.CloseFigure();
		control.Region = new Region(graphicsPath);
	}

	public static void updateWelcomeLabel(string nickname)
	{
		Utils.InvokeIfRequired(GUI.form.WelcomeUserLabel, delegate
		{
			if (nickname != "")
			{
                GUI.form.WelcomeUserLabel.Text = "Welcome " + nickname;
                GUI.form.accessLabel.Text = "Access granted";
                GUI.form.accessLabel.ForeColor = Color.Green;
			}
			else
			{
                GUI.form.WelcomeUserLabel.Text = "Login char with license";
                GUI.form.accessLabel.Text = "No access";
                GUI.form.accessLabel.ForeColor = Color.Red;
			}
		});
	}

	public static void updateMapInfo(int map_id)
	{
		string text = $"images\\maps\\{map_id}.png";
		string text2 = $"images\\maps\\{map_id}_org.png";
		if (Path.Exists(text) && Path.Exists(text2))
		{
			NAStyles.BitmapLarge = new Bitmap(text);
			NAStyles.BitmapOriginal = new Bitmap(text2);
		}
	}

    public static void UpdateMapBottomPanel()
    {
        Utils.InvokeIfRequired((Control)GUI.form.MapperLabel, (Action)(() =>
        {
            if (GUI.Mapper == null)
                return;
            GUI.form.MapXLabel.Text = "X: " + GUI.Mapper.x_pos.ToString();
            GUI.form.MapYLabel.Text = "Y: " + GUI.Mapper.y_pos.ToString();
            Label mapLabel = GUI.form.MapLabel;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 3);
            interpolatedStringHandler.AppendLiteral("Map: ");
            interpolatedStringHandler.AppendFormatted(MapID.GetMapName(GUI.Mapper.real_map_id));
            interpolatedStringHandler.AppendLiteral(" (");
            interpolatedStringHandler.AppendFormatted<int>(GUI.Mapper.real_map_id);
            interpolatedStringHandler.AppendLiteral(" : ");
            interpolatedStringHandler.AppendFormatted<int>(GUI.Mapper.map_id);
            interpolatedStringHandler.AppendLiteral(")");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            mapLabel.Text = stringAndClear;
            GUI.form.MapPanelMapperLabel.Text = "Mapper: " + GUI.Mapper.nickname;
            GUI.form.ServerLabel.Text = "Server: " + GUI.Mapper.server;
            GUI.form.ChannelLabel.Text = "CH: " + GUI.Mapper.channel.ToString();
        }));
    }

    public static void updateMapperLabel()
	{
		if (Mapper != null)
		{
			Utils.InvokeIfRequired(GUI.form.MapperLabel, delegate
			{
                GUI.form.MapperLabel.Text = Mapper.nickname;
			});
		}
	}

	private void HostNameTextBox_TextChanged(object sender, EventArgs e)
	{
		HostNameTextBox.Text = HostNameTextBox.Text.RemoveWhiteSpace();
		raidHost = HostNameTextBox.Text;
		Settings.config.raidHostName = raidHost;
		Settings.SaveSettings();
	}

	private void AutofullThresholdTrackbar_Scroll(object sender, EventArgs e)
	{
		Settings.config.autoFullThreshold = AutofullThresholdTrackbar.Value;
		AutoFullThresholdLabel.Text = AutofullThresholdTrackbar.Value + "%";
		Settings.SaveSettings();
	}

	private void RandomRangeTextBox_TextChanged(object sender, EventArgs e)
	{
		if (RandomRangeTextBox.Text.Length != 0)
		{
			Settings.config.randomizeCordsRange = Convert.ToInt32(RandomRangeTextBox.Text);
			Settings.SaveSettings();
		}
	}

	private void RandomRangeTextBox_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
		{
			e.Handled = true;
		}
	}

	public static void PacketsConsolePrint(string text)
	{
		Utils.InvokeIfRequired(GUI.form.PacketsConsole, delegate
		{
            GUI.form.PacketsConsole.WriteLine(text + Environment.NewLine);
            GUI.form.PacketLoggerScrollbar.updateScrollButtonSize();
		});
	}

	private void map_picture_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right && Mapper != null && sender is PictureBox)
		{
			PictureBox pictureBox = (PictureBox)sender;
			Raids.MoveByMapClick(e.X, e.Y, pictureBox.Width, pictureBox.Height, pictureBox.Name);
		}
	}

	private void GUI_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			DllImports.ReleaseCapture();
			DllImports.SendMessage(base.Handle, 161, 2, 0);
		}
	}

	private void BrowseDLLLabel_MouseEnter(object sender, EventArgs e)
	{
		BrowseDLLLabel.ForeColor = NAStyles.NotActiveCharColor;
	}

	private void BrowseDLLLabel_MouseLeave(object sender, EventArgs e)
	{
		BrowseDLLLabel.ForeColor = NAStyles.CounterForeColor;
	}

	private void BrowseDLLLabel_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
		{
			return;
		}
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "DLL Files (*.dll)|*.dll|All Files (*.*)|*.*";
		openFileDialog.FilterIndex = 1;
		openFileDialog.RestoreDirectory = true;
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = openFileDialog.FileName;
			Settings.config.dllPath = fileName;
			DLLFileLabel.Text = fileName.Split("\\").Last();
			Settings.SaveSettings();
		}
	}

	private void InjectDLLButton_MouseDown(object sender, MouseEventArgs e)
	{
		if (SelectedClient != null)
		{
			DLLHandler.InjectDLL(SelectedClient.process_id);
		}
		else
		{
			ShowPopUp("Select a client");
		}
	}

	private void InviterButton_Click(object sender, EventArgs e)
	{
		if (SelectedClient != null)
		{
			miniland_state.Clear();
			Inviter = SelectedClient;
			updateInviterLabel();
		}
		else
		{
			ShowPopUp("Select a client");
		}
	}

	public static void updateInviterLabel()
	{
		string nickname = ((Inviter == null) ? "None" : Inviter.nickname);
		Utils.InvokeIfRequired(form.MLTabInviterLabel, delegate
		{
			form.MLTabInviterLabel.Text = "Inviter: " + nickname;
			form.InviterLabel.Text = "Inviter: " + nickname;
		});
	}

	private void MapperButton_Click(object sender, EventArgs e)
	{
		if (RaidManager.raidStarted)
		{
			ShowPopUp("Can not change Mapper during raid");
		}
		else if (SelectedClient != null)
		{
			if (Mapper?.real_map_id != SelectedClient.real_map_id || Mapper.server != SelectedClient.server || Mapper.channel != SelectedClient.channel)
			{
				PacketsManager.ResetMapperData();
			}
			Mapper = SelectedClient;
			MapperLabel.Text = SelectedClient.nickname;
			updateMapInfo(Mapper.map_id);
			if (dmgContributionForm != null && dmgContributionForm.Visible)
			{
				DmgContributionCounterWindow.setMapper();
			}
			if (MapID.isFamMobbingMap(Mapper.map_id))
			{
				NAStyles.SetTSStonesData();
			}
			Miniland.ClearTrainerData();
			QuestManager.UnselectQuest();
			QuestManager.UpdateQuestList(Mapper.config.quests);
		}
		else
		{
			ShowPopUp("Select a client");
		}
	}

	private void CloseButton_Click(object sender, EventArgs e)
	{
		if (RaidManager.raidStarted)
		{
			if (new NAMessageBox("Are you sure you want to exit?", "Exit").ShowDialog() != DialogResult.Yes)
			{
				return;
			}
			form.Close();
		}
		form.Close();
	}

	private void MinimizeButton_Click(object sender, EventArgs e)
	{
		base.WindowState = FormWindowState.Minimized;
	}

	private void AutoJoinToggleButton_Click(object sender, EventArgs e)
	{
		Settings.config.autoJoinList = !Settings.config.autoJoinList;
		AutoJoinToggleLabel.Text = (Settings.config.autoJoinList ? "On" : "Off");
		Settings.SaveSettings();
		if (!Settings.config.autoJoinList)
		{
			Raids.readyRaiders.Clear();
		}
	}

	private void AutoFullToggleButton_Click(object sender, EventArgs e)
	{
		AutoFull = !AutoFull;
		AutoFullToggleLabel.Text = (AutoFull ? "On" : "Off");
		AutofullThresholdTrackbar.Visible = AutoFull;
		AutoFullThresholdLabel.Visible = AutoFull;
	}

	private void JoinListButton_Click(object sender, EventArgs e)
	{
		Raids.JoinList();
	}

	private void useBuffsButton_Click(object sender, EventArgs e)
	{
		Miniland.UseBuffs(0);
	}

	private void inviteToMLButton_Click(object sender, EventArgs e)
	{
		Miniland.InvitePlayersToML();
	}

	private void prepareMLButton_Click(object sender, EventArgs e)
	{
		Miniland.InviteBuffersToML();
	}

	private void autoConfirmToggleButton_Click(object sender, EventArgs e)
	{
		Settings.config.autoconfirm = !Settings.config.autoconfirm;
		autoConfirmLabel.Text = (Settings.config.autoconfirm ? "On" : "Off");
		Settings.SaveSettings();
	}

	private void changeSPButton_Click(object sender, EventArgs e)
	{
		Raids.TransformSP();
	}

	private void stackWindowsButton_Click(object sender, EventArgs e)
	{
		Controller.stackWindows();
	}

	private void waterfallButton_Click(object sender, EventArgs e)
	{
		Controller.windowsToWaterfall();
	}

	private void useSelfBuffsButton_Click(object sender, EventArgs e)
	{
		Miniland.UseSelfBuffs();
	}

	private async void leaveRaidButton_Click(object sender, EventArgs e)
	{
		await Raids.ExitRaid();
	}

	private void resetTitlesButton_Click(object sender, EventArgs e)
	{
		Controller.RenameClients(false);
	}

	private void closeAltsButton_Click(object sender, EventArgs e)
	{
		Raids.closeAlts();
	}

	private void PacketLoggerPrintButton_Click(object sender, EventArgs e)
	{
		PacketLoggerPrintRecv = !PacketLoggerPrintRecv;
		PacketLogerPrintRecvStatusLabel.Text = (PacketLoggerPrintRecv ? "On" : "Off");
		if (!PacketLoggerPrintRecv)
		{
			PacketLoggerScrollbar.scrollButtonResizeCooldown = 1;
			PacketLoggerScrollbar.updateScrollButtonSize();
		}
	}

	private void ClearPacketLoggerButton_Click(object sender, EventArgs e)
	{
		PacketsConsole.Items.Clear();
		PacketLoggerScrollbar.Visible = false;
	}

	private void PacketLoggerPrintSentButton_Click(object sender, EventArgs e)
	{
		PacketLoggerPrintSent = !PacketLoggerPrintSent;
		PacketLogerPrintSentStatusLabel.Text = (PacketLoggerPrintSent ? "On" : "Off");
		if (!PacketLoggerPrintSent)
		{
			PacketLoggerScrollbar.scrollButtonResizeCooldown = 1;
			PacketLoggerScrollbar.updateScrollButtonSize();
		}
	}

	private void OpenPacketFiltersButton_Click(object sender, EventArgs e)
	{
		bool packetLoggerPrintSent = PacketLoggerPrintSent;
		bool packetLoggerPrintRecv = PacketLoggerPrintRecv;
		PacketLoggerPrintSent = false;
		PacketLoggerPrintRecv = false;
		new FiltersMenu().ShowDialog();
		PacketLoggerPrintSent = packetLoggerPrintSent;
		PacketLoggerPrintRecv = packetLoggerPrintRecv;
	}

	private void SettingsRenameButton_MouseClick(object sender, MouseEventArgs e)
	{
		SettingsRenameButton.BackColor = ((SettingsRenameButton.BackColor == NAStyles.ButtonFalseColor) ? NAStyles.ButtonTrueColor : NAStyles.ButtonFalseColor);
		Settings.config.renameClients = !Settings.config.renameClients;
		Settings.SaveSettings();
	}

	private void SettingsPanel_Click(object sender, EventArgs e)
	{
		((Panel)sender).Focus();
	}

	private void AddLicenseButton_Click(object sender, EventArgs e)
	{
		new EnterLicenseWindow().ShowDialog();
	}

	private async void timer_map_tick_Tick(object sender, EventArgs e)
	{
		if (Mapper == null || Main == null || NAStyles.BitmapLarge == null || NAStyles.BitmapOriginal == null)
		{
			return;
		}
		NAStyles.map_width = NAStyles.BitmapOriginal.Width;
		NAStyles.map_height = NAStyles.BitmapOriginal.Height;
		double ratioX = (double)NAStyles.BitmapLarge.Width / (double)NAStyles.map_width * 2.0;
		double ratioY = (double)NAStyles.BitmapLarge.Height / (double)NAStyles.map_height * 2.0;
		Bitmap fresh_map = new Bitmap(NAStyles.BitmapLarge);
		Bitmap bitmap = null;
		try
		{
			fresh_map = NAStyles.DrawDangerousCirclesOnBitmap(fresh_map, ratioX, ratioY);
			fresh_map = NAStyles.DrawFieldsOnBitmap(fresh_map, ratioX, ratioY);
			fresh_map = NAStyles.DrawMonstersOnBitmap(fresh_map, ratioX, ratioY);
			fresh_map = NAStyles.DrawEntitiesOnBitmap(fresh_map, ratioX, ratioY);
			fresh_map = NAStyles.DrawTimeSpacesOnBitmap(fresh_map, ratioX, ratioY);
			fresh_map = NAStyles.DrawPlayersOnBitmap(fresh_map, ratioX, ratioY);
			fresh_map = NAStyles.DrawSpecialMonstersOnBitmap(fresh_map, ratioX, ratioY);
			fresh_map = NAStyles.DrawQuestNavigatorOnBitmap(fresh_map, ratioX, ratioY);
			fresh_map = NAStyles.DrawSelfOnBitmap(fresh_map, ratioX, ratioY);
			fresh_map = NAStyles.DrawLeversOnBitmap(fresh_map, ratioX, ratioY);
			if (MapID.isFamMobbingMap(Mapper.real_map_id))
			{
				fresh_map = NAStyles.DrawSpawnTimersOnMap(fresh_map, ratioX, ratioY);
			}
			if (RaidManager.IsInUltimateArmaBossroom)
			{
				fresh_map = NAStyles.ZoomUltArmaBossRoom(fresh_map, ratioX, ratioY);
			}
			if (TimeSpaceManager.ts_started && NAStyles.ts_map != null && TimeSpaceManager.current_ts != null)
			{
				bitmap = (Bitmap)NAStyles.ts_map.Clone();
				bitmap = await NAStyles.DrawTSDataMap(bitmap, TimeSpaceManager.current_ts.ID);
			}
			RaidModeForm?.updateMap(fresh_map);
			if (bitmap != null && NAStyles.force_live_map_draw)
			{
				fresh_map = NAStyles.OverlayMinimap((Bitmap)fresh_map.Clone(), bitmap);
			}
			else if (bitmap != null)
			{
				fresh_map = NAStyles.OverlayMinimap((Bitmap)bitmap.Clone(), fresh_map);
			}
			form.map_picture.Image = fresh_map;
			form.map_picture.Size = new Size(MapPanel.Width, MapPanel.Height);
			if (!QuestManager.ShowQuestSearchInstanceMap && !NAvigator.show_time_space_map)
			{
				form.QuestsTabMap.Image = fresh_map;
			}
		}
		catch (Exception)
		{
		}
		UpdateMapBottomPanel();
		RaidManager.GraphsUpdate();
		RaidManager.UpdateRanks();
		if (Miniland.pet_trainer_mobs_list.Count != 0)
		{
			UpdatePetTrainerSection();
		}
		if (last_version_check.AddMinutes(15.0) <= DateTime.UtcNow && !CountdownStaretd && !CheckAccess.isAdmin)
		{
			bool? flag = VersionManager.GetVersion(close: false);
			if (flag.HasValue && !flag.Value)
			{
				//StartCountdown("New Version is Avaiable: NA2 will close in:", DateTime.UtcNow.AddMinutes(120.0));
			}
			last_version_check = DateTime.UtcNow;
		}
		if (license_valid_until.AddMinutes(-120.0) <= DateTime.UtcNow && !CountdownStaretd && !CheckAccess.isAdmin)
		{
			//StartCountdown("Your License is expiring soon: NA2 will close in:", license_valid_until);
		}
		if (CountdownStaretd)
		{
			UpdateCountDown();
		}
		if (currentPanel == MainRaidsPanel)
		{
			if (last_raids_bars_refresh.AddSeconds(120.0) <= DateTime.UtcNow)
			{
				await Analytics.RefreshBarStatusData();
			}
			if (last_raids_bars_update.AddMilliseconds(800.0) <= DateTime.UtcNow)
			{
				UpdateRaidBarsStatus();
			}
		}
		if (RaidManager.UltArmaBoxesSpawn)
		{
			RaidManager.MarkUltArmaNotInFieldRaiders();
		}
	}

	private void SidePanelButton_Click(object sender, EventArgs e)
	{
		Button button = (Button)sender;
		switch (button.Name)
		{
		case "ExitButton":
			form.Close();
			break;
		case "SwitchToControlPanelButton":
			form.MainControlPanel.Show();
			currentPanel = form.MainControlPanel;
			break;
		case "SwitchToMapButton":
			form.MainMapPanel.Show();
			currentPanel = MainMapPanel;
			break;
		case "SwitchToRaidsButton":
			UpdateRaidBarsStatus();
			form.MainRaidsPanel.Show();
			currentPanel = MainRaidsPanel;
			break;
		case "SwitchToMLButton":
			form.MainMinilandPanel.Show();
			currentPanel = MainMinilandPanel;
			if (first_run)
			{
				UpdateInviteList();
				first_run = false;
			}
			InviteListDataGrid.ClearSelection();
			break;
		case "SwitchToSettingsButton":
			form.MainSettingsPanel.Show();
			currentPanel = MainSettingsPanel;
			break;
		case "SwitchToPacketLoggerButton":
			form.MainPacketLoggerPanel.Show();
			currentPanel = MainPacketLoggerPanel;
			break;
		case "SwitchToQuestsButton":
			form.MainQuestsPanel.Show();
			currentPanel = MainQuestsPanel;
			break;
		case "SwitchToAnalyticsButton":
			OpenAnalytics();
			return;
		case "SwitchToCounterButton":
			RaidModeForm?.Show();
			return;
		}
		foreach (Button item in SideMenu.Controls.OfType<Button>())
		{
			item.BackColor = ((item == button) ? NAStyles.MenuButtonPressedColor : NAStyles.MainThemeDarker);
		}
		CloseButton.BringToFront();
		MinimizeButton.BringToFront();
		HideMenuButton.BringToFront();
		foreach (Panel item2 in from panel in form.Controls.OfType<Panel>()
			where panel.Name != "SideMenu" && panel.Name != "popUpPanel"
			select panel)
		{
			if (item2 != currentPanel)
			{
				item2.Hide();
			}
		}
		if (button.Name != "SwitchToPacketLogger")
		{
			PacketLoggerPrintRecv = false;
			PacketLoggerPrintSent = false;
			form.PacketLogerPrintRecvStatusLabel.Text = "Off";
			form.PacketLogerPrintSentStatusLabel.Text = "Off";
		}
	}

	private async void WelcomeUserLabel_Click(object sender, EventArgs e)
	{
		foreach (NostaleCharacterInfo nostaleCharacterInfo in _nostaleCharacterInfoList)
		{
			_ = nostaleCharacterInfo;
		}
	}

	private static void HandleMessageReceived(object message)
	{
	}

	private void accessLabel_Click(object sender, EventArgs e)
	{
		QuestManager.PrintQuestList();
	}

	private void NoAccessLicenseConfirmButton_Click(object sender, EventArgs e)
	{
		if (NoAccessLicenseTextBox.Text.Length != 16)
		{
			//ShowPopUp("Invalid License Format");
			//return;
		}
		if (!Settings.config.licenseKeys.Contains(NoAccessLicenseTextBox.Text.Trim()))
		{
			Settings.config.licenseKeys.Add(NoAccessLicenseTextBox.Text.Trim());
			Settings.SaveSettings();
			NoAccessLicenseTextBox.Text = "";
			ShowPopUp("License Added Successfully", isNotification: true);
		}
		else
		{
			NoAccessLicenseTextBox.Text = "";
			ShowPopUp("License already exists");
		}
		if (Main == null)
		{
			CheckAccess.checkAccess();
		}
	}

	public static void ShowPopUp(string message, bool isNotification = false)
	{
		string sound = (isNotification ? "Notification Sound" : "Error Sound");
		form.popUpPanel.ShowPopUp(message, sound);
	}

	public static void ShowRaidNotification(string message)
	{
		form.popUpPanel.ShowPopUp(message, "Raid Notification");
	}

	private void StartRaidModeButton_Click(object sender, EventArgs e)
	{
		RaidModeForm?.Show();
	}

	public static void FlowLayoutCharactersPanel_MouseWheel(object sender, MouseEventArgs e)
	{
		form.ControlPanelScrollbar.OnOwnerMouseWheel(e.Delta);
	}

	public static void ConsoleWindow_MouseWheel(object sender, MouseEventArgs e)
	{
		form.PacketLoggerScrollbar.OnOwnerMouseWheel(e.Delta);
	}

	public static void FlowLayoutRaidsPanel_MouseWheel(object sender, MouseEventArgs e)
	{
		form.RaidersPanelScrollbar.OnOwnerMouseWheel(e.Delta);
	}

	public static void UpdateLoadingBar(int current_count, int max_count)
	{
		if (form.LoadingScreenPanel.Visible)
		{
			double progress = (double)current_count / (double)max_count;
			form.LoadingScreenBar.updateProgress(progress);
		}
	}

	private void SettingsDelayBuffTextBox_Leave(object sender, EventArgs e)
	{
		int num = 200;
		if (int.TryParse(form.SettingsDelayBuffTextBox.Text, out var result))
		{
			if (result < 0)
			{
				result = 0;
			}
			if (!ShowDelayWarning(result, num))
			{
				form.SettingsDelayBuffTextBox.Text = num.ToString();
				result = num;
			}
			Settings.config.DelaySettings.Buff = result;
			Settings.SaveSettings();
		}
		else
		{
			form.SettingsDelayBuffTextBox.Text = Settings.config.DelaySettings.Buff.ToString();
		}
	}

	private void SettingsDelayRaidTextBox_Leave(object sender, EventArgs e)
	{
		int num = 500;
		if (int.TryParse(form.SettingsDelayRaidTextBox.Text, out var result))
		{
			if (result < 0)
			{
				result = 0;
			}
			if (!ShowDelayWarning(result, num))
			{
				form.SettingsDelayRaidTextBox.Text = num.ToString();
				result = num;
			}
			Settings.config.DelaySettings.Raid = result;
			Settings.SaveSettings();
		}
		else
		{
			form.SettingsDelayRaidTextBox.Text = Settings.config.DelaySettings.Raid.ToString();
		}
	}

	private void SettingsDelayMoveTextBox_Leave(object sender, EventArgs e)
	{
		int num = 500;
		if (int.TryParse(form.SettingsDelayMoveTextBox.Text, out var result))
		{
			if (result < 0)
			{
				result = 0;
			}
			if (!ShowDelayWarning(result, num))
			{
				form.SettingsDelayMoveTextBox.Text = num.ToString();
				result = num;
			}
			Settings.config.DelaySettings.Move = result;
			Settings.SaveSettings();
		}
		else
		{
			form.SettingsDelayMoveTextBox.Text = Settings.config.DelaySettings.Move.ToString();
		}
	}

	private void SettingsDelayInviteTextBox_Leave(object sender, EventArgs e)
	{
		int num = 200;
		if (int.TryParse(form.SettingsDelayInviteTextBox.Text, out var result))
		{
			if (result < 0)
			{
				result = 0;
			}
			if (!ShowDelayWarning(result, num))
			{
				form.SettingsDelayInviteTextBox.Text = num.ToString();
				result = num;
			}
			Settings.config.DelaySettings.Invite = result;
			Settings.SaveSettings();
		}
		else
		{
			form.SettingsDelayInviteTextBox.Text = Settings.config.DelaySettings.Invite.ToString();
		}
	}

	private void SettingsDelayItemsTextBox_Leave(object sender, EventArgs e)
	{
		int num = 700;
		if (int.TryParse(form.SettingsDelayItemsTextBox.Text, out var result))
		{
			if (result < 0)
			{
				result = 0;
			}
			if (!ShowDelayWarning(result, num))
			{
				form.SettingsDelayItemsTextBox.Text = num.ToString();
				result = num;
			}
			Settings.config.DelaySettings.Items = result;
			Settings.SaveSettings();
		}
		else
		{
			form.SettingsDelayItemsTextBox.Text = Settings.config.DelaySettings.Items.ToString();
		}
	}

	private void MapShowPlayersCheckBox_Click(object sender, EventArgs e)
	{
		Settings.config.MapSettings.Players = !Settings.config.MapSettings.Players;
		Settings.SaveSettings();
		MapShowPlayersCheckBox.SetState(Settings.config.MapSettings.Players, NAStyles.PlayersColor);
	}

	private void MapShowMobsCheckBox_Click(object sender, EventArgs e)
	{
		Settings.config.MapSettings.Mobs = !Settings.config.MapSettings.Mobs;
		Settings.SaveSettings();
		MapShowMobsCheckBox.SetState(Settings.config.MapSettings.Mobs, NAStyles.MonstersColor);
	}

	private void MapShowEntitiesCheckBox_Click(object sender, EventArgs e)
	{
		Settings.config.MapSettings.Entities = !Settings.config.MapSettings.Entities;
		Settings.SaveSettings();
		MapShowEntitiesCheckBox.SetState(Settings.config.MapSettings.Entities, NAStyles.EntitiesColor);
	}

	private void MapShowMapperCheckBox_Click(object sender, EventArgs e)
	{
		Settings.config.MapSettings.Mapper = !Settings.config.MapSettings.Mapper;
		Settings.SaveSettings();
		MapShowMapperCheckBox.SetState(Settings.config.MapSettings.Mapper, NAStyles.MapperColor);
	}

	private void MapShowAltsCheckBox_Click(object sender, EventArgs e)
	{
		Settings.config.MapSettings.Alts = !Settings.config.MapSettings.Alts;
		Settings.SaveSettings();
		MapShowAltsCheckBox.SetState(Settings.config.MapSettings.Alts, NAStyles.AltColor);
	}

	private void MapShowPetsCheckBox_Click(object sender, EventArgs e)
	{
		Settings.config.MapSettings.Pets = !Settings.config.MapSettings.Pets;
		Settings.SaveSettings();
		MapShowPetsCheckBox.SetState(Settings.config.MapSettings.Pets, NAStyles.PetsColor);
	}

	private int GetChildIndexAtPoint(FlowLayoutPanel panel, Point clientPoint)
	{
		for (int i = 0; i < panel.Controls.Count; i++)
		{
			Rectangle bounds = panel.Controls[i].Bounds;
			bounds.Inflate(panel.Controls[i].Margin.Size);
			if (bounds.Contains(clientPoint))
			{
				return i;
			}
		}
		return -1;
	}

	private void flowLayoutRaidsPanel_DragDrop(object sender, DragEventArgs e)
	{
		flowLayoutRaidsPanel.ResumeLayout();
		if (e.Data != null && e.Data.GetData(typeof(RaiderPanelItem)) is RaiderPanelItem child)
		{
			Point clientPoint = flowLayoutRaidsPanel.PointToClient(new Point(e.X, e.Y));
			int childIndexAtPoint = GetChildIndexAtPoint(flowLayoutRaidsPanel, clientPoint);
			if (childIndexAtPoint != -1)
			{
				flowLayoutRaidsPanel.Controls.SetChildIndex(child, childIndexAtPoint);
			}
			else
			{
				flowLayoutRaidsPanel.Controls.SetChildIndex(child, flowLayoutRaidsPanel.Controls.Count - 1);
			}
			_nostaleCharacterInfoList = (from item in flowLayoutRaidsPanel.Controls.OfType<RaiderPanelItem>()
				select item.character ?? new NostaleCharacterInfo()).ToList();
		}
	}

	private void flowLayoutRaidsPanel_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data != null && e.Data.GetDataPresent(typeof(RaiderPanelItem)))
		{
			flowLayoutRaidsPanel.SuspendLayout();
			e.Effect = DragDropEffects.Move;
		}
	}

	private void flowLayoutRaidsPanel_DragOver(object sender, DragEventArgs e)
	{
		if (draggedItem != null)
		{
			Point location = flowLayoutRaidsPanel.PointToClient(new Point(e.X, e.Y));
			location.X -= draggedItemXOffset;
			draggedItem.Location = location;
		}
	}

	private void flowLayoutRaidsPanel_DragLeave(object sender, EventArgs e)
	{
		flowLayoutRaidsPanel.ResumeLayout();
	}

	private void GUI_FormClosing(object sender, FormClosingEventArgs e)
	{
		connectionsUpdater.Dispose();
		packetListener.Dispose();
		packetHandler.Dispose();
	}

	private void editControlPictureBox_MouseEnter(object sender, EventArgs e)
	{
		((PictureBox)sender).Image = Resources.edit_icon_hover;
	}

	private void editControlPictureBox_MouseLeave(object sender, EventArgs e)
	{
		((PictureBox)sender).Image = Resources.edit_icon;
	}

	private void editUseBufssControl_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		useBuffsControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = useBuffsControlLabel;
		currentlyModifiedKey = "useBuffs";
	}

	private void editInviteControl_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		InviteControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = InviteControlLabel;
		currentlyModifiedKey = "invite";
	}

	private void editJoinListControlPictureBox_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		JoinListControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = JoinListControlLabel;
		currentlyModifiedKey = "joinList";
	}

	private void editExitRaidControlPictureBox_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		ExitRaidControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = ExitRaidControlLabel;
		currentlyModifiedKey = "exitRaid";
	}

	private void editMoveControl_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		MassHealControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = MassHealControlLabel;
		currentlyModifiedKey = "massHeal";
	}

	private void editWearSPControl_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		WearSPControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = WearSPControlLabel;
		currentlyModifiedKey = "wearSP";
	}

	private void editUseSelfBuffsControl_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		useSelfBuffsControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = useSelfBuffsControlLabel;
		currentlyModifiedKey = "useSelfBuffs";
	}

	private void SettingsHotkeysButton_Click(object sender, EventArgs e)
	{
		SettingsHotkeysButton.BackColor = ((SettingsHotkeysButton.BackColor == NAStyles.ButtonFalseColor) ? NAStyles.ButtonTrueColor : NAStyles.ButtonFalseColor);
		Settings.config.enableHotkeys = !Settings.config.enableHotkeys;
		Settings.SaveSettings();
	}

	private void AddToInviteListButton_Click(object sender, EventArgs e)
	{
		string newNickname = NewNicknameToInviteListTextBox.Text.Trim();
		if (!string.IsNullOrEmpty(newNickname))
		{
			if (Settings.config.inviteList.Any((InviteItem item) => item.nickname == newNickname))
			{
				ShowPopUp("Nickname already in list");
				NewNicknameToInviteListTextBox.Clear();
				return;
			}
			Settings.config.inviteList.Add(new InviteItem
			{
				nickname = newNickname,
				active = true
			});
			NewNicknameToInviteListTextBox.Clear();
			UpdateInviteList();
		}
	}

	private void RemoveFromInviteListButton_Click(object sender, EventArgs e)
	{
		if (InviteListDataGrid.SelectedCells.Count == 0)
		{
			return;
		}
		string selectedNickname = InviteListDataGrid.SelectedCells[0]?.Value?.ToString();
		if (!string.IsNullOrEmpty(selectedNickname))
		{
			Settings.config.inviteList.RemoveAll((InviteItem x) => x.nickname == selectedNickname);
			UpdateInviteList();
		}
	}

	private void UpdateInviteList()
	{
		Settings.SaveSettings();
		InviteListDataGrid.DataSource = Settings.config.inviteList.Select((InviteItem invite) => new InviteItem
		{
			nickname = invite.nickname
		}).ToList();
		InviteListDataGrid.ClearSelection();
		foreach (DataGridViewRow row in (IEnumerable)InviteListDataGrid.Rows)
		{
			if (row.Cells["nickname"].Value != null)
			{
				InviteItem inviteItem = Settings.config.inviteList.FirstOrDefault((InviteItem x) => x.nickname == row.Cells["nickname"].Value.ToString());
				if (inviteItem != null)
				{
					row.Cells["nickname"].Style.BackColor = (inviteItem.active ? NAStyles.MainThemeDarker : NAStyles.NotActiveCharColor);
				}
			}
		}
	}

	private void DefaultSettingsButton_Click(object sender, EventArgs e)
	{
		Settings.LoadDefaults();
		setGuiElements();
	}

	public static void setLoadingLabelText(string text)
	{
		Utils.InvokeIfRequired(form.LoadingLabel, delegate
		{
			form.LoadingLabel.Text = text;
		});
	}

	private void SettingsSoundsButton_Click(object sender, EventArgs e)
	{
		SettingsSoundsButton.BackColor = ((SettingsSoundsButton.BackColor == NAStyles.ButtonTrueColor) ? NAStyles.ButtonFalseColor : NAStyles.ButtonTrueColor);
		Settings.config.playSounds = !Settings.config.playSounds;
		Settings.SaveSettings();
	}

	public static void SetNetworkDeviceState(string name)
	{
		Utils.InvokeIfRequired(form.NetworkDeviceCombobox, delegate
		{
			form.NetworkDeviceCombobox.setState(name);
		});
	}

	public static void SetListOfNetworkDevices()
	{
		List<string> list = new List<string>();
		foreach (NetworkInterface item in from x in NetworkInterface.GetAllNetworkInterfaces()
			where x.OperationalStatus == OperationalStatus.Up
			select x)
		{
			if (item.GetIPv4Statistics().BytesSent > 0)
			{
				list.Add(item.Name);
			}
		}
		form.NetworkDeviceCombobox.setItems(list);
	}

	public static void SetDefaultNetworkDevice()
	{
		NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces().ToList().Find((NetworkInterface x) => x.Name == form.NetworkDeviceCombobox.currentState);
		if (networkInterface != null)
		{
			Settings.config.defaultNetwordDeviceID = networkInterface.Id;
			Settings.SaveSettings();
			if (LibPcapLiveDeviceList.Instance.Where((LibPcapLiveDevice x) => x.ToString().Contains(Settings.config.defaultNetwordDeviceID)).FirstOrDefault() != null)
			{
				SetNetworkDeviceState(networkInterface.Name);
				ShowPopUp("Reset to apply changes", isNotification: true);
			}
			else
			{
				ShowPopUp("Failed while selecting new device");
			}
		}
		else
		{
			ShowPopUp("Could not find the device");
		}
	}

	private void InviteListDataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
		{
			return;
		}
		DataGridViewCell cell = InviteListDataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
		if (cell != null && cell.Value != null)
		{
			InviteItem inviteItem = Settings.config.inviteList.Find((InviteItem x) => x.nickname == cell.Value.ToString());
			if (inviteItem != null)
			{
				inviteItem.active = !inviteItem.active;
				Settings.SaveSettings();
				cell.Style.BackColor = (inviteItem.active ? NAStyles.MainThemeDarker : NAStyles.NotActiveCharColor);
			}
		}
	}

	private void HideMenuButton_Click(object sender, EventArgs e)
	{
		SideMenu.Visible = !SideMenu.Visible;
		if (SideMenu.Visible)
		{
			base.Width += SideMenu.Width;
			currentPanel.Left = SideMenu.Width;
			CloseButton.Left += SideMenu.Width;
			MinimizeButton.Left += SideMenu.Width;
			HideMenuButton.Left += SideMenu.Width;
			HideMenuButton.Image = Resources.triangleRight;
			base.Left -= SideMenu.Width;
		}
		else
		{
			base.Width -= SideMenu.Width;
			currentPanel.Left -= SideMenu.Width;
			CloseButton.Left -= SideMenu.Width;
			MinimizeButton.Left -= SideMenu.Width;
			HideMenuButton.Left -= SideMenu.Width;
			HideMenuButton.Image = Resources.triangleLeft;
			base.Left += SideMenu.Width;
		}
	}

	private void HideMenuButton_MouseEnter(object sender, EventArgs e)
	{
		HideMenuButton.Image = (SideMenu.Visible ? Resources.triangleRightHover : Resources.triangleLeftHover);
	}

	private void HideMenuButton_MouseLeave(object sender, EventArgs e)
	{
		HideMenuButton.Image = (SideMenu.Visible ? Resources.triangleRight : Resources.triangleLeft);
	}

	private void ResetUseBuffsButton_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "useBuffs"))
		{
			Settings.config.ControlsSettings.useBuffs = (0, true);
			Settings.SaveSettings();
			useBuffsControlLabel.Text = "None";
		}
	}

	private void ResetInviteButton_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "invite"))
		{
			Settings.config.ControlsSettings.invite = (0, true);
			Settings.SaveSettings();
			InviteControlLabel.Text = "None";
		}
	}

	private void ResetJoinListButton_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "joinList"))
		{
			Settings.config.ControlsSettings.joinList = (0, true);
			Settings.SaveSettings();
			JoinListControlLabel.Text = "None";
		}
	}

	private void ResetExitRaidButton_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "exitRaid"))
		{
			Settings.config.ControlsSettings.exitRaid = (0, true);
			Settings.SaveSettings();
			ExitRaidControlLabel.Text = "None";
		}
	}

	private void ResetMassHealButton_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "massHeal"))
		{
			Settings.config.ControlsSettings.massHeal = (0, true);
			Settings.SaveSettings();
			MassHealControlLabel.Text = "None";
		}
	}

	private void ResetWearSPButton_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "wearSP"))
		{
			Settings.config.ControlsSettings.wearSP = (0, true);
			Settings.SaveSettings();
			WearSPControlLabel.Text = "None";
		}
	}

	private void ResetUseSelfBuffsButton_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "useSelfBuffs"))
		{
			Settings.config.ControlsSettings.useSelfBuffs = (0, true);
			Settings.SaveSettings();
			useSelfBuffsControlLabel.Text = "None";
		}
	}

	private void BuffsetsButton_Click(object sender, EventArgs e)
	{
		new BuffsetsMenu().ShowDialog();
	}

	private void ResetUseBuffset1Button_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "useBuffset1"))
		{
			Settings.config.ControlsSettings.useBuffset1 = (0, true);
			Settings.SaveSettings();
			useBuffset1ControlLabel.Text = "None";
		}
	}

	private void ResetUseBuffset2Button_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "useBuffset2"))
		{
			Settings.config.ControlsSettings.useBuffset2 = (0, true);
			Settings.SaveSettings();
			useBuffset2ControlLabel.Text = "None";
		}
	}

	private void ResetUseBuffset3Button_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "useBuffset3"))
		{
			Settings.config.ControlsSettings.useBuffset3 = (0, true);
			Settings.SaveSettings();
			useBuffset3ControlLabel.Text = "None";
		}
	}

	private void ResetArcaneWisdomButton_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "arcaneWisdom"))
		{
			Settings.config.ControlsSettings.arcaneWisdom = (0, true);
			Settings.SaveSettings();
			arcaneWisdomControlLabel.Text = "None";
		}
	}

	private void editUseBuffset1ControlPictureBox_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		useBuffset1ControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = useBuffset1ControlLabel;
		currentlyModifiedKey = "useBuffset1";
	}

	private void editUseBuffset2ControlPictureBox_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		useBuffset2ControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = useBuffset2ControlLabel;
		currentlyModifiedKey = "useBuffset2";
	}

	private void editUseBuffset3ControlPictureBox_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		useBuffset3ControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = useBuffset3ControlLabel;
		currentlyModifiedKey = "useBuffset3";
	}

	private void editArcaneWisdomControlPictureBox_Click(object sender, EventArgs e)
	{
		KeyboardManager.RestoreEditedHotkeyLabel();
		form.ActiveControl = null;
		arcaneWisdomControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = arcaneWisdomControlLabel;
		currentlyModifiedKey = "arcaneWisdom";
	}

	private void OpenDmgContributionWindow_Click(object sender, EventArgs e)
	{
		if (dmgContributionForm != null)
		{
			dmgContributionForm.Show();
			DmgContributionCounterWindow.clear();
		}
	}

	private void LowSpecSoundsButton_Click(object sender, EventArgs e)
	{
		SettingsLowSpecButton.BackColor = ((SettingsLowSpecButton.BackColor == NAStyles.ButtonFalseColor) ? NAStyles.ButtonTrueColor : NAStyles.ButtonFalseColor);
		Settings.config.low_spec = !Settings.config.low_spec;
		Settings.SaveSettings();
	}

	private void SettingsTooltipsButton_Click(object sender, EventArgs e)
	{
		SettingsTooltipsButton.BackColor = ((SettingsTooltipsButton.BackColor == NAStyles.ButtonTrueColor) ? NAStyles.ButtonFalseColor : NAStyles.ButtonTrueColor);
		Settings.config.showTooltips = !Settings.config.showTooltips;
		ChangeTooltipState(Settings.config.showTooltips);
		Settings.SaveSettings();
	}

	private void UseBuffsRaidsButton_Click(object sender, EventArgs e)
	{
		Miniland.UseBuffs(0);
	}

	private void UseSelfBuffsRaidsButton_Click(object sender, EventArgs e)
	{
		Miniland.UseSelfBuffs();
	}

	private void MimicKeyboardButton_Click(object sender, EventArgs e)
	{
		KeyboardManager.mimic_keyboard = !KeyboardManager.mimic_keyboard;
		MimicKeyboardLabel.Text = (KeyboardManager.mimic_keyboard ? "On" : "Off");
	}

	private void MimicMouseButton_Click(object sender, EventArgs e)
	{
		KeyboardManager.mimic_mouse = !KeyboardManager.mimic_mouse;
		MimicMouseLabel.Text = (KeyboardManager.mimic_mouse ? "On" : "Off");
	}

	private void SetTooltips()
	{
		tooltips.Add(new NATooltip(NoAccessLicenseConfirmButton, "Add the license key entered above\nLogin to the game if license is already provided", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(HideMenuButton, "Hide/Show Side Menu", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(InviterButton, "Selects Inviter (Must be selected in the Control Panel list).\nInviter is used to invite players to his miniland.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(MapperButton, "Selects Main (Must be selected in the Control Panel list)\nMain is used to display map, raid damage and damage contribution.\nBeware that many functions will work after map change.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(closeAltsButton, "Closes all non attackers after confirmation.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(resetTitlesButton, "Whether to rename clients on Nostale Title Bar\nYou can notice that only if windowed.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(BrowseDLLLabel, "Opens dll file select window.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(InjectDLLButton, "Injects choosen dll file to the selected client.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(NicknameHeaderLabel, "Nickname of your character.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(AttackerHeaderLabel, "Attackers are prioritized in some functions.\nSelf buffs work only for attackers.\n Attackers are ignored by:\n-Closing all alts\n-Transform SP", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(RaiderHeaderLabel, "Raiders join list, transform SP and change amulets. They react to raid move commands if are also movers.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(MoverHeaderLabel, "Movers react to move commands in raids (Zenas, Erenia etc.) - must be marked as raider.\nThey also move when you click on the map in Map view (or Raid view)", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(BufferHeaderLabel, "Buffers are characters that use buffs (all if no buffset is set).", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(AutofullHeaderLabel, "Autofull allows characters to automatically use full potion.\nRemember to set it in Raids View (disabled by default).", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(DisableHeaderLabel, "Disabled characters are ignored by all functions.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(OtherHeaderLabel, "Opens additional character specific settings window.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(RandomRangeLabel, "Random range of pixels to spread characters.\nIf set to 0 all characters will move to the exact same location.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(AutoFullToggleButton, "Activates auto use full potion for characters that have this option enabled in the Control Panel.\nFull potions must be on hotbar.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(AutoJoinToggleButton, "Activates auto joining of Raiders when a raid hosted by 'Raid Host Nick' appears in the raids list.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(JoinListButton, "All Raiders will join the raid hosted by: 'Raid Host Nick'.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(leaveRaidButton, "All Raiders will leave the current raid.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(changeSPButton, "All Raiders will transform SP and SPP.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(MimicMouseButton, "All Raiders will copy your mouse click (triggered by right mouse click).\nAll raiders should have the same resolution.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(MimicKeyboardButton, "All Raiders will copy your key strokes.\nHotkeys are disabled while this is active.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(BuffsetsButton, "Opens additional Buff Sets window.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(UseBuffsRaidsButton, "All Buffers will use *ALL* Buffs.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(UseSelfBuffsRaidsButton, "All Attackers will use Self Buffs (Tattoos, pet buffs, SP buffs).", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(stackWindowsButton, "Places all windows in the same location in the middle of the screen (Attackers on top).", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(waterfallButton, "Generates a cascading waterfall layout for windows, beginning from the top-right corner and adjusting the offset based on the number of open windows.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(useDebuffsButton, "Debuffers use their debuffs (Target must be selected manually).", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(SaveWindowsButton, "Saves current windows' locations. The state can be later restored using the Load Windows button.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(LoadWindowsButton, "Restores saved windows' locations.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(ResizeWindowsButton, "Resizes all windows to the specified width and height for all raiders. Attackers are ignored.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(LimitFPSButton, "Limits FPS of all raiders. Attackers are ignored.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(OpenBoxesButton, "All raiders start opening raid boxes/sealed vessels located on their hotbars.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(useBuffsButton, "All Buffers will use *ALL* Buffs.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(inviteToMLButton, "Inviter will invite all characters with nicknames that are present in the list on right\nInviter must be in his own miniland.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(prepareMLButton, "Inviter will invite all your Buffers to his miniland.\nAll buffers will accept the invitation automatically\nInviter must be in miniland.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(useSelfBuffsButton, "All Attackers will use Self Buffs (Tattoos, pet buffs, SP buffs).", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(autoConfirmToggleButton, "All Attackers will confirm the invitation to Inviter's miniland automatically.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(OpenDmgContributionWindow, "Opens Damage Contribution Window.\nShows percent representation of damage dealt to mobs by players in the current map\nNot recommended to open this while character is in raid.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(RemoveFromInviteListButton, "Removes selected character from the list above.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(AddToInviteListButton, "Adds the nickname entered below to the list above.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(AutoRespawnButton, "Automatically respawns a character after 21-26 seconds after dying. (Does not work for attackers)", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(PacketLoggerPrintRecvButton, "Prints received packets from the game (Character must be toggled in filters menu).\nDisplaying too many packets will slow down the entire application dramatically.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(PacketLoggerPrintSentButton, "Prints sent packets from your clients (Character must be toggled in filters menu).", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(ClearPacketLoggerButton, "Clear the packets from the list above.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(OpenPacketFiltersButton, "Oppens additional Filters window.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(AddLicenseButton, "Opens the Add license window.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(DefaultSettingsButton, "Reset all settings to default.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(SettingsDelayBuffLabel, "Approximate delay of all Buff commands.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(SettingsDelayRaidLabel, "Approximate delay of all Raid related commands.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(SettingsDelayMoveLabel, "Approximate delay of all Move commands.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(SettingsDelayInviteLabel, "Approximate delay of Inviting to miniland by Inviter.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(SettingsDelayItemsLabel, "Approximate delay of using attack pot or vale/alza pot.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(NetworkDeviceLabel, "Network card name to capture packets from.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(RenameSettingLabel, "Whether to Rename clients on app startup.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(HotkeysSettingLabel, "Whether to listen for hotkeys.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(LowSpecSettingsLabel, "Whether to display images on map or just colored dots.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(SoundsSettingsLabel, "Whether to play a sound during specific action.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(TooltipsSettingsLabel, "Whether to display tooltips.", Settings.config.showTooltips));
		tooltips.Add(new NATooltip(arcaneWisdomLabel, "Changes Hotbar Tab, changes all items on the buttons 0-9, uses tattoos, restores the equipement and returns to the original hotbar tab.", Settings.config.showTooltips));
	}

	public static void ChangeTooltipState(bool state)
	{
		foreach (NATooltip tooltip in tooltips)
		{
			tooltip.Active = state;
		}
	}

	private void AutoRespawnButton_Click(object sender, EventArgs e)
	{
		AutoRespawn = !AutoRespawn;
		AutoRespawnLabel.Text = (AutoRespawn ? "On" : "Off");
	}

	private void editUseDebuffsControlPictureBox_Click(object sender, EventArgs e)
	{
		form.ActiveControl = null;
		useDebuffsControlLabel.Text = "Press Key";
		KeyboardManager.new_binding = true;
		currentlyModifiedKeyLabel = useDebuffsControlLabel;
		currentlyModifiedKey = "useDebuffs";
	}

	private void ResetUseDebuffsButton_Click(object sender, EventArgs e)
	{
		if (!(currentlyModifiedKey == "useDebuffs"))
		{
			Settings.config.ControlsSettings.useDebuffs = (0, true);
			Settings.SaveSettings();
			useDebuffsControlLabel.Text = "None";
		}
	}

	private void useDebuffsButton_Click(object sender, EventArgs e)
	{
		Raids.UseDebuffs();
	}

	private void DisplayFamilyMaxHitsAllRaids()
	{
		form.FamRecordsPanel.Controls.Clear();
		for (int i = (family_records_page - 1) * 8; i < family_records_page * 8 && i <= Analytics.family_max_hits_all_raids.Keys.Count - 1; i++)
		{
			FamilyRecordsItem familyRecordsItem = new FamilyRecordsItem();
			ScaleControl(familyRecordsItem);
			SetRoundShape(familyRecordsItem, 15);
			familyRecordsItem.setData(Analytics.family_max_hits_all_raids[Analytics.family_max_hits_all_raids.Keys.ElementAt(i)]);
			FamRecordsPanel.Controls.Add(familyRecordsItem);
		}
	}

	private async void OpenAnalytics()
	{
		Analytics.Clear();
		await Analytics.GetMarathons();
		raids_history_page = 1;
		current_marathon_page = 1;
		RaidsHistoryPreviousPageButton.Hide();
		DisplayMarathonsHistory();
		form.RaidsHistoryPanel.Show();
		form.RaidsHistoryPanel.BringToFront();
		form.CloseButton.BringToFront();
		form.MinimizeButton.BringToFront();
		form.HideMenuButton.Hide();
		form.SideMenu.Hide();
		SetRankingTabsColor();
		Analytics.ranking_data_mode = "average_damage";
		RankingAverageDMGLabel.BackColor = NAStyles.MainThemeDarker;
		HideFamRecordsTab();
		HidePlayersTab();
		HideRankingTab();
		RaidsHistoryTabLabel_Click(new object(), new EventArgs());
		int serverIdFromName = NostaleServers.GetServerIdFromName(Mapper.server);
		if (serverIdFromName != 0)
		{
			PlayerInfoData playerInfoData = await NAHttpClient.FetchPlayerData(new ServerNicknameDto
			{
				server_id = serverIdFromName,
				nickname = Mapper.nickname
			});
			if (playerInfoData != null)
			{
				SetSearchedPlayerInfo(playerInfoData);
			}
		}
	}

	private void RaidsHistoryBackButton_Click(object sender, EventArgs e)
	{
		form.RaidsHistoryPanel.Hide();
		form.RaidsHistoryDetailsPanel.Hide();
		form.RaidsHistoryFlowLayoutPanel.Show();
		form.SideMenu.Show();
		show_marathons = true;
		form.HideMenuButton.Show();
		if (SelectedHistoryitem != null)
		{
			SelectedHistoryitem.BackColor = NAStyles.ActiveCharColor;
			SelectedHistoryitem = null;
		}
		AnalyticsCleanUp();
	}

	private void DisplayMarathonsHistory()
	{
		show_marathons = true;
		RaidsHistoryFilterTextBox.Show();
		if (Analytics.marathons_filter != "")
		{
			Analytics.marathons = Analytics.marathons_org.Where((MarathonData x) => GameBoss.getBossName(x.boss_id).ToLower().Contains(Analytics.marathons_filter.ToLower())).ToList();
		}
		else
		{
			Analytics.marathons = Analytics.marathons_org;
		}
		if (Analytics.marathons == null)
			Analytics.marathons = new();
        GUI.form.RaidsHistoryPreviousPageButton.Visible = raids_history_page > 1;
        GUI.form.RaidsHistoryNextPageButton.Visible = (double)raids_history_page < Math.Max(1.0, Math.Ceiling((double)Analytics.marathons.Count / 15.0));
        GUI.form.RaidsHistoryLabel.Text = "Marathons History";
		RaidsHistoryPageLabel.Text = $"Page: {raids_history_page}/{Math.Max(1.0, Math.Ceiling((double)Analytics.marathons.Count / 15.0))}";
        GUI.form.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Controls.Clear();
		RaidsHistoryFlowLayoutPanel.Controls.Clear();
		for (int i = (raids_history_page - 1) * 15; i < raids_history_page * 15; i++)
		{
			if (i > Analytics.marathons.Count - 1)
			{
				RaidsHistoryNextPageButton.Hide();
				break;
			}
			MarathonItem marathonItem = new MarathonItem();
			ScaleControl(marathonItem);
			SetRoundShape(marathonItem, 15);
			marathonItem.setData(Analytics.marathons.ElementAt(i));
			if (RaidsHistoryFlowLayoutPanel.Controls.Count == 0)
			{
				marathonItem.Margin = new Padding(20, 20, 0, 0);
			}
			RaidsHistoryFlowLayoutPanel.Controls.Add(marathonItem);
		}
        GUI.form.BackArrowRaidsHistory.Hide();
		AnalyticsBackArrow.Show();
		ShowMarathonTotalButton.Hide();
		RaidsHistoryBestTimeLabel.Hide();
		RaidsHistoryAverageTimeLabel.Hide();
	}

	public void DisplayRaidsHistory()
	{
		show_marathons = false;
		RaidsHistoryFilterTextBox.Hide();
        GUI.form.RaidsHistoryLabel.Text = "Raids in the Marathon";
		RaidsHistoryPageLabel.Text = $"Page: {current_marathon_page}/{Math.Max(1.0, Math.Ceiling((double)Analytics.current_marathon.Count / 15.0))}";
		RaidsHistoryFlowLayoutPanel.Controls.Clear();
		for (int i = (current_marathon_page - 1) * 15; i < current_marathon_page * 15; i++)
		{
			if (i > Analytics.current_marathon.Count - 1)
			{
				RaidsHistoryNextPageButton.Hide();
				break;
			}
			RaidsHisotryItem raidsHisotryItem = new RaidsHisotryItem();
			ScaleControl(raidsHisotryItem);
			SetRoundShape(raidsHisotryItem, 15);
			raidsHisotryItem.setData(Analytics.current_marathon.ElementAt(i));
			if (RaidsHistoryFlowLayoutPanel.Controls.Count == 0)
			{
				raidsHisotryItem.Margin = new Padding(20, 20, 0, 0);
			}
			RaidsHistoryFlowLayoutPanel.Controls.Add(raidsHisotryItem);
		}
		if ((double)current_marathon_page < Math.Max(1.0, Math.Ceiling((double)Analytics.current_marathon.Count / 15.0)))
		{
			RaidsHistoryNextPageButton.Show();
		}
		if (current_marathon_page > 1)
		{
			RaidsHistoryPreviousPageButton.Show();
		}
		AnalyticsBackArrow.Hide();
		BackArrowRaidsHistory.Show();
		ShowMarathonTotalButton.Show();
		double num = -1.0;
		double num2 = 0.0;
		foreach (RaidData item in Analytics.current_marathon)
		{
			if (num == -1.0 || num >= item.finished_in)
			{
				num = item.finished_in;
			}
			num2 += item.finished_in;
		}
		num2 /= (double)Analytics.current_marathon.Count;
		RaidsHistoryBestTimeLabel.Text = $"Best Time: {TimeSpan.FromSeconds(num):mm\\:ss}";
		RaidsHistoryAverageTimeLabel.Text = $"Average Time: {TimeSpan.FromSeconds(num2):mm\\:ss}";
		RaidsHistoryBestTimeLabel.Show();
		RaidsHistoryAverageTimeLabel.Show();
	}

	public static void updateRaidsHistoryRaidersPanel(RaidData raid_data)
	{
		Utils.InvokeIfRequired(form.RaidsHistorySelectedRaidPlayersFlowLayoutPanel, delegate
		{
			form.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Controls.Clear();
			List<RaidPlayer> list = raid_data.players.OrderBy((RaidPlayer x) => x.damage).Reverse().ToList();
			long num = list.Take(Math.Min(2, list.Count)).Sum((RaidPlayer x) => x.damage);
			int num2 = 0;
			foreach (RaidPlayer item in list)
			{
				GraphsItem graphsItem = new GraphsItem();
				graphsItem.ModifyForRaidsHisotry();
				graphsItem.setNickname(item.nickname);
				graphsItem.setPortait(item.getPortrait());
				graphsItem.setValue((item.damage != 0L) ? Math.Max((double)item.damage / (double)num, 0.01) : 0.01);
				if (item.nickname == Mapper?.nickname)
				{
					graphsItem.BackColor = NAStyles.NotActiveCharColor;
				}
				ScaleControl(graphsItem);
				form.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Controls.Add(graphsItem);
				num2++;
				if (num2 == 15)
				{
					break;
				}
			}
		});
	}

	public static void showRaidDetails(RaidData raid_data)
	{
		Utils.InvokeIfRequired(form.RaidsHistoryPanel, delegate
		{
			form.RaidsHistoryDetailsGridView.Rows.Clear();
			int num = 1;
			foreach (RaidPlayer item in raid_data.players.OrderBy((RaidPlayer x) => x.damage).Reverse().ToList())
			{
				addCharacterRaidsDetails(item, num);
				num++;
			}
			form.RaidsHistoryDetailsPanel.Show();
			form.RaidsHistoryFlowLayoutPanel.Hide();
		});
	}

	public static void addCharacterRaidsDetails(RaidPlayer player, int index)
	{
		Utils.InvokeIfRequired(form.RaidsHistoryDetailsGridView, delegate
		{
			CounterDataGrid raidsHistoryDetailsGridView = form.RaidsHistoryDetailsGridView;
			int index2 = raidsHistoryDetailsGridView.Rows.Add();
			DataGridViewRow dataGridViewRow = raidsHistoryDetailsGridView.Rows[index2];
			dataGridViewRow.Cells["ID"].Value = index;
			dataGridViewRow.Cells["Lp"].Value = index;
			dataGridViewRow.Cells["CLvl"].Value = $"{player.lvl}+{player.clvl}";
			dataGridViewRow.Cells["CharacterID"].Value = player.character_id;
			dataGridViewRow.Cells["PlayerName"].Value = player.nickname;
			dataGridViewRow.Cells["Total"].Value = player.damage;
			dataGridViewRow.Cells["OnyxDmg"].Value = player.damage_onyx;
			dataGridViewRow.Cells["MaxHit"].Value = player.max_hit;
			dataGridViewRow.Cells["MaxHitIcon"].Value = SPCard.getSkillIcon(player.max_hit_skill_id);
			dataGridViewRow.Cells["Pets"].Value = player.pets;
			dataGridViewRow.Cells["Special"].Value = player.damage_miniboss;
			dataGridViewRow.Cells["Gold"].Value = player.gold;
			dataGridViewRow.Cells["Average"].Value = player.average;
			dataGridViewRow.Cells["Hit"].Value = player.hit;
			dataGridViewRow.Cells["Miss"].Value = player.miss;
			dataGridViewRow.Cells["Crit"].Value = player.crit;
			dataGridViewRow.Cells["Bon"].Value = player.bon;
			dataGridViewRow.Cells["BonCrit"].Value = player.boncrit;
			dataGridViewRow.Cells["Dbf"].Value = player.debuffs;
			dataGridViewRow.Cells["Dead"].Value = player.dead;
			dataGridViewRow.Cells["All"].Value = player.all_damage;
			dataGridViewRow.Cells["MobDmg"].Value = player.mob_damage;
			dataGridViewRow.Cells["AllHits"].Value = player.all_hits;
			dataGridViewRow.Cells["AllMiss"].Value = player.all_miss;
		});
	}

	private void HideRaidDetailsPictureBox_Click(object sender, EventArgs e)
	{
		Utils.InvokeIfRequired(form, delegate
		{
			form.RaidsHistoryDetailsPanel.Hide();
			form.RaidsHistoryFlowLayoutPanel.Show();
		});
	}

	private void RaidsHistoryNextPageButton_MouseEnter(object sender, EventArgs e)
	{
		form.RaidsHistoryNextPageButton.Image = Resources.triangleRightHover;
	}

	private void RaidsHistoryNextPageButton_MouseLeave(object sender, EventArgs e)
	{
		form.RaidsHistoryNextPageButton.Image = Resources.triangleRight;
	}

	private void RaidsHistoryPreviousPageButton_MouseEnter(object sender, EventArgs e)
	{
		form.RaidsHistoryPreviousPageButton.Image = Resources.triangleLeftHover;
	}

	private void RaidsHistoryPreviousPageButton_MouseLeave(object sender, EventArgs e)
	{
		form.RaidsHistoryPreviousPageButton.Image = Resources.triangleLeft;
	}

	private void RaidsHistoryPreviousPageButton_Click(object sender, EventArgs e)
	{
		RaidsHistoryNextPageButton.Show();
		if (show_marathons)
		{
			raids_history_page--;
			RaidsHistoryPageLabel.Text = $"Page: {raids_history_page}/{Math.Max(1.0, Math.Ceiling((double)Analytics.marathons.Count / 15.0))}";
			if (raids_history_page <= 1)
			{
				RaidsHistoryPreviousPageButton.Hide();
			}
			DisplayMarathonsHistory();
			HideRaidDetailsPictureBox_Click(sender, e);
			return;
		}
		current_marathon_page--;
		RaidsHistoryPageLabel.Text = $"Page: {current_marathon_page}/{Math.Max(1.0, Math.Ceiling((double)Analytics.current_marathon.Count / 15.0))}";
		if (current_marathon_page <= 1)
		{
			RaidsHistoryPreviousPageButton.Hide();
		}
		DisplayRaidsHistory();
		HideRaidDetailsPictureBox_Click(sender, e);
		form.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Controls.Clear();
		if (SelectedHistoryitem != null)
		{
			SelectedHistoryitem.BackColor = NAStyles.ActiveCharColor;
			SelectedHistoryitem = null;
		}
	}

	private void RaidsHistoryNextPageButton_Click(object sender, EventArgs e)
	{
		RaidsHistoryPreviousPageButton.Show();
		if (show_marathons)
		{
			raids_history_page++;
			RaidsHistoryPageLabel.Text = $"Page: {raids_history_page}/{Math.Max(1.0, Math.Ceiling((double)Analytics.marathons.Count / 15.0))}";
			DisplayMarathonsHistory();
			HideRaidDetailsPictureBox_Click(sender, e);
			if ((double)raids_history_page == Math.Max(1.0, Math.Ceiling((double)Analytics.marathons.Count / 15.0)))
			{
				RaidsHistoryNextPageButton.Hide();
			}
			return;
		}
		current_marathon_page++;
		RaidsHistoryPageLabel.Text = $"Page: {current_marathon_page}/{Math.Max(1.0, Math.Ceiling((double)Analytics.current_marathon.Count / 15.0))}";
		DisplayRaidsHistory();
		HideRaidDetailsPictureBox_Click(sender, e);
		if (SelectedHistoryitem != null)
		{
			SelectedHistoryitem.BackColor = NAStyles.ActiveCharColor;
			SelectedHistoryitem = null;
		}
		if ((double)current_marathon_page == Math.Max(1.0, Math.Ceiling((double)Analytics.current_marathon.Count / 15.0)))
		{
			RaidsHistoryNextPageButton.Hide();
		}
	}

	private void BackArrowRaidsHistory_Click(object sender, EventArgs e)
	{
		if (form.RaidsHistoryDetailsPanel.Visible)
		{
			form.RaidsHistoryDetailsPanel.Hide();
			form.RaidsHistoryFlowLayoutPanel.Show();
		}
		else
		{
			DisplayMarathonsHistory();
		}
	}

	private void BackArrowRaidsHistory_MouseEnter(object sender, EventArgs e)
	{
		form.BackArrowRaidsHistory.Image = Resources.back_arrow_hover;
	}

	private void BackArrowRaidsHistory_MouseLeave(object sender, EventArgs e)
	{
		form.BackArrowRaidsHistory.Image = Resources.back_arrow;
	}

	private void FamRecordsNextPageButton_MouseEnter(object sender, EventArgs e)
	{
		form.FamRecordsNextPageButton.Image = Resources.triangleRightHover;
	}

	private void FamRecordsNextPageButton_MouseLeave(object sender, EventArgs e)
	{
		form.FamRecordsNextPageButton.Image = Resources.triangleRight;
	}

	private void FamRecordsPreviousPageButton_MouseEnter(object sender, EventArgs e)
	{
		form.FamRecordsPreviousPageButton.Image = Resources.triangleLeftHover;
	}

	private void FamRecordsPreviousPageButton_MouseLeave(object sender, EventArgs e)
	{
		form.FamRecordsPreviousPageButton.Image = Resources.triangleLeft;
	}

	private void FamRecordsNextPageButton_Click(object sender, EventArgs e)
	{
		family_records_page++;
		if (family_records_page != 1)
		{
			FamRecordsPreviousPageButton.Show();
		}
		if ((double)family_records_page >= Math.Max(1.0, Math.Ceiling((double)Analytics.family_max_hits_all_raids.Keys.Count / 8.0)))
		{
			FamRecordsNextPageButton.Hide();
		}
		DisplayFamilyMaxHitsAllRaids();
	}

	private void FamRecordsPreviousPageButton_Click(object sender, EventArgs e)
	{
		family_records_page--;
		if (family_records_page == 1)
		{
			form.FamRecordsPreviousPageButton.Hide();
		}
		if ((double)family_records_page < Math.Max(1.0, Math.Ceiling((double)Analytics.family_max_hits_all_raids.Keys.Count / 8.0)))
		{
			FamRecordsNextPageButton.Show();
		}
		DisplayFamilyMaxHitsAllRaids();
	}

	private void RaidsHistoryTabLabel_Click(object sender, EventArgs e)
	{
		HideFamRecordsTab();
		HidePlayersTab();
		HideRankingTab();
		RaidsHistoryTabLabel.BackColor = NAStyles.MainThemeDarker;
		RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Show();
		RaidsHistoryDoubleBufferedPanel.Show();
		RaidsHistoryPageLabel.Show();
		if (show_marathons)
		{
			AnalyticsBackArrow.Show();
			form.RaidsHistoryLabel.Text = "Marathons History";
			if ((double)raids_history_page < Math.Max(1.0, Math.Ceiling((double)Analytics.marathons.Count / 15.0)))
			{
				RaidsHistoryNextPageButton.Show();
			}
			if (raids_history_page > 1)
			{
				RaidsHistoryPreviousPageButton.Show();
			}
			RaidsHistoryFilterTextBox.Show();
		}
		else
		{
			form.RaidsHistoryLabel.Text = "Raids in the Marathon";
			if ((double)current_marathon_page < Math.Max(1.0, Math.Ceiling((double)Analytics.current_marathon.Count / 15.0)))
			{
				RaidsHistoryNextPageButton.Show();
			}
			if (current_marathon_page > 1)
			{
				RaidsHistoryPreviousPageButton.Show();
			}
			AnalyticsBackArrow.Hide();
			BackArrowRaidsHistory.Show();
			ShowMarathonTotalButton.Show();
			RaidsHistoryFilterTextBox.Hide();
			RaidsHistoryBestTimeLabel.Show();
			RaidsHistoryAverageTimeLabel.Show();
		}
		RaidsHistoryBackButton.Show();
	}

	private void HideRaidsHistoryTab()
	{
		RaidsHistoryTabLabel.BackColor = NAStyles.NotSelectedTabColor;
		RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Hide();
		RaidsHistoryDoubleBufferedPanel.Hide();
		RaidsHistoryPageLabel.Hide();
		RaidsHistoryPreviousPageButton.Hide();
		RaidsHistoryNextPageButton.Hide();
		BackArrowRaidsHistory.Hide();
		AnalyticsBackArrow.Show();
		ShowMarathonTotalButton.Hide();
		RaidsHistoryBackButton.Hide();
		RaidsHistoryFilterTextBox.Hide();
		RaidsHistoryBestTimeLabel.Hide();
		RaidsHistoryAverageTimeLabel.Hide();
	}

	private async void FamRecordsLabel_Click(object sender, EventArgs e)
	{
		HideRaidsHistoryTab();
		HidePlayersTab();
		HideRankingTab();
		AnalyticsBackArrow.Show();
		if (Analytics.family_max_hits_all_raids.Keys.Count == 0)
		{
			await Analytics.GetFamilyMaxHitsAllRaids();
			family_records_page = 1;
			DisplayFamilyMaxHitsAllRaids();
		}
		FamRecordsTabLabel.BackColor = NAStyles.MainThemeDarker;
		form.RaidsHistoryLabel.Text = "Family Records";
		FamRecordsPanel.Show();
		if ((double)family_records_page < Math.Max(1.0, Math.Ceiling((double)Analytics.family_max_hits_all_raids.Keys.Count / 8.0)))
		{
			FamRecordsNextPageButton.Show();
		}
		if (family_records_page != 1)
		{
			FamRecordsPreviousPageButton.Show();
		}
	}

	private void HideFamRecordsTab()
	{
		FamRecordsPanel.Hide();
		FamRecordsNextPageButton.Hide();
		FamRecordsPreviousPageButton.Hide();
		FamRecordsTabLabel.BackColor = NAStyles.NotSelectedTabColor;
	}

	public void PlayersTabLabel_Click(object sender, EventArgs e)
	{
		if (RaidForm.isPlayerDetailsTabVisible())
		{
			ShowPopUp("You can't use this function when the Counter Player Details tab is open!");
			return;
		}
		form.ShowPlayersTab();
		form.HideRankingTab();
	}

	public void ShowPlayersTab()
	{
		form.PlayersTabLabel.BackColor = NAStyles.MainThemeDarker;
		form.AnalyticsPlayersTab.Show();
		form.AnalyticsBackArrow.Show();
		form.RaidsHistoryLabel.Text = "Search For Players";
		form.SearchNicknameTextBox.Text = Analytics.SearchNickname;
		form.SearchServerComboBox.setState(Analytics.SearchServer);
		form.HideFamRecordsTab();
		form.HideRaidsHistoryTab();
	}

	private void HidePlayersTab()
	{
		AnalyticsPlayersTab.Hide();
		PlayersTabLabel.BackColor = NAStyles.NotSelectedTabColor;
		PlayerRaidsStatisticsPanel.Hide();
		ClearPlayersRaidsStatistics();
	}

	private void CreateRankingIcons()
	{
		foreach (int boss_id in RaidID.GetAllRaidBosses())
		{
			if (!RaidID.IsPercentDamageRaid(RaidID.GetRaidID(boss_id)))
			{
				PictureBox boss_icon = new PictureBox();
				boss_icon.Name = $"RankingRaidTypeFilter_{boss_id}";
				boss_icon.Image = GameMonster.GetIcon(boss_id);
				boss_icon.Size = new Size(57, 57);
				boss_icon.SizeMode = PictureBoxSizeMode.Zoom;
				boss_icon.Margin = new Padding(1, 1, 1, 1);
				boss_icon.Click += delegate
				{
					Analytics.UpdateRankingRaidTypeFilter(boss_id, boss_icon);
				};
				ScaleControl(boss_icon);
				ranking_bosses_icons.Add(boss_icon);
			}
		}
		foreach (int sp_id in SPID.GetCombatSPs())
		{
			if (sp_id != 30 && sp_id != 43)
			{
				PictureBox sp_icon = new PictureBox();
				sp_icon.Name = $"RankingSPFilter_{sp_id}";
				sp_icon.Image = GamePlayer.GetIcon(sp_id, 0, SPID.GetSPClass(sp_id));
				sp_icon.Size = new Size(51, 51);
				sp_icon.SizeMode = PictureBoxSizeMode.Zoom;
				sp_icon.Margin = new Padding(1, 1, 1, 1);
				sp_icon.Click += delegate
				{
					Analytics.UpdateRankingSPFilter(sp_id, sp_icon);
				};
				ScaleControl(sp_icon);
				ranking_sps_icons.Add(sp_icon);
			}
		}
	}

	private void RankingTabLabel_Click(object sender, EventArgs e)
	{
		RankingTabLabel.BackColor = NAStyles.MainThemeDarker;
		form.RaidsHistoryLabel.Text = "Ranking";
		if (RankingRaidTypeFilterFlowLayoutPanel.Controls.Count == 0)
		{
			foreach (PictureBox ranking_bosses_icon in ranking_bosses_icons)
			{
				RankingRaidTypeFilterFlowLayoutPanel.Controls.Add(ranking_bosses_icon);
			}
		}
		if (RankingSPFilterFlowLayoutPanel.Controls.Count == 0)
		{
			foreach (PictureBox ranking_sps_icon in ranking_sps_icons)
			{
				RankingSPFilterFlowLayoutPanel.Controls.Add(ranking_sps_icon);
			}
		}
		SetRankingPanels();
		HideFamRecordsTab();
		HideRaidsHistoryTab();
		HidePlayersTab();
		RankingTabPanel.Show();
		RankingTabPanel.BringToFront();
	}

	public void HideRankingTab()
	{
		RankingTabPanel.Hide();
		RankingTabLabel.BackColor = NAStyles.NotSelectedTabColor;
	}

	private void SearchNicknameTextBox_TextChanged(object sender, EventArgs e)
	{
		string text = SearchNicknameTextBox.Text;
		if (text.Length > 18 || text.Length < 0)
		{
			SearchNicknameTextBox.Text = Analytics.SearchNickname;
		}
		else
		{
			Analytics.SearchNickname = text.Trim();
		}
	}

	public void SetSearchedPlayerInfo(PlayerInfoData player)
	{
		if (player == null || player.vanity == null || !player.reputation.HasValue)
		{
			ShowPopUp("No data about this player", isNotification: true);
			return;
		}
		Analytics.current_player_data = player;
		MainFairyDetailsPanel.Hide();
		SPDetailsBorderPanel.Hide();
		List<string> list = player.vanity.Split('.').ToList();
		List<int> equipment = new List<int>();
		foreach (string item in list)
		{
			if (item.Contains("-1") && item.Length > 2)
			{
				equipment.Add(Convert.ToInt32(item.Substring(0, item.Length - 2)));
				equipment.Add(-1);
			}
			else
			{
				equipment.Add(Convert.ToInt32(item));
			}
		}
		PlayerItem main_weapon = player.items.Find((PlayerItem x) => x.type == 1 && x.item_id == Convert.ToInt32(equipment.ElementAt(2)));
		PlayerItem secondary_weapon = player.items.Find((PlayerItem x) => x.type == 2 && x.item_id == Convert.ToInt32(equipment.ElementAt(3)));
		PlayerItem armor = player.items.Find((PlayerItem x) => x.type == 0 && x.item_id == Convert.ToInt32(equipment.ElementAt(1)));
		MainWeapon.Image = ((main_weapon != null) ? ItemID.GetIcon(main_weapon.item_id) : ItemID.GetIcon(-1));
		SecondaryWeapon.Image = ItemID.GetIcon(equipment.ElementAt(3));
		Armor.Image = ((armor != null) ? ItemID.GetIcon(armor.item_id) : ItemID.GetIcon(-1));
		Hat.Image = ItemID.GetIcon(equipment.ElementAt(0));
		Mask.Image = ItemID.GetIcon(equipment.ElementAt(4));
		FlyingPet.Image = ItemID.GetIcon(equipment.ElementAt(10));
		Costume.Image = ItemID.GetIcon(equipment.ElementAt(6));
		CostumeHat.Image = ItemID.GetIcon(equipment.ElementAt(7));
		WeaponSkin.Image = ItemID.GetIcon(equipment.ElementAt(8));
		Wings.Image = ItemID.GetIcon(equipment.ElementAt(9));
		DateTime updatedAt = player.updatedAt;
		SearchedPlayerNickname.Text = player.nickname;
		if (player.family != null && player.family_role_id.HasValue)
		{
			SearchedPlayerFamily.Text = $"{player.family} [lvl{player.family_lvl}]";
			SearchedPlayerFamilyRole.Text = GamePlayer.id_to_fam_role[player.family_role_id];
		}
		else
		{
			SearchedPlayerFamily.Text = "-";
			SearchedPlayerFamilyRole.Text = "-";
		}
		SearchedPlayerLVLCLVL.Text = $"{player.lvl}+{player.clvl}";
		SearchedPlayerClassSex.Text = GamePlayer.id_to_class[player.class_id] + ":" + ((player.sex == 0) ? "Male" : "Female");
		SearchedPlayerTitle.Text = ((!player.title.HasValue) ? "-" : (GamePlayer.title_to_name.ContainsKey(player.title) ? GamePlayer.title_to_name[player.title] : ((player.title == 0) ? "-" : player.title.ToString())));
		MainWeaponUpgrade.Text = ((main_weapon != null) ? $"R{main_weapon.item_rarity}+{main_weapon.item_upgrade}" : "None");
		if (secondary_weapon != null && secondary_weapon.item_upgrade.HasValue && secondary_weapon.item_rarity.HasValue)
		{
			SecondaryWeaponUpgrade.Text = ((secondary_weapon != null) ? $"R{secondary_weapon.item_rarity}+{secondary_weapon.item_upgrade}" : "None");
		}
		else
		{
			SecondaryWeaponUpgrade.Text = "";
		}
		ArmorUpgrade.Text = ((armor != null) ? $"R{armor.item_rarity}+{armor.item_upgrade}" : "None");
		if (player.sp_cards.Count == 0)
		{
			SearchedPlayerAvatar.Image = GamePlayer.GetIcon(0, player.sex, player.class_id);
		}
		else
		{
			List<PlayerSP> list2 = (from x in player.sp_cards
				group x by x.sp_upgrade into g
				orderby g.Key descending
				select g).FirstOrDefault()?.ToList();
			int? num = ((list2 != null && list2.Count > 0) ? list2[new Random().Next(list2.Count)].sp_id : 0);
			SearchedPlayerAvatar.Image = GamePlayer.GetIcon(num.Value, player.sex, player.class_id);
		}
		if (player.reputation.HasValue)
		{
			Reputation.Image = GamePlayer.GetReputationIcon(player.reputation.Value);
		}
		SearchedPlayerFairiesFlowLayoutPanel.Controls.Clear();
		int? num2 = 0;
		foreach (PlayerFairy item2 in from x in player.fairies
			orderby x.element, x.updatedAt descending
			select x)
		{
			if (item2.element != num2)
			{
				num2 = item2.element;
				FairyInfoItem fairyInfoItem = new FairyInfoItem();
				fairyInfoItem.setData(player, item2, ShowFairyDetails);
				ScaleControl(fairyInfoItem);
				SearchedPlayerFairiesFlowLayoutPanel.Controls.Add(fairyInfoItem);
				if (item2.updatedAt > updatedAt)
				{
					updatedAt = item2.updatedAt;
				}
			}
		}
		SearchedPlayerSPsFlowLayoutPanel.Controls.Clear();
		player.sp_cards = (from card in player.sp_cards
			group card by card.sp_id into @group
			select @group.OrderByDescending((PlayerSP card) => card.updatedAt).First()).ToList();
		foreach (PlayerSP item3 in player.sp_cards.OrderBy((PlayerSP x) => x.sp_id))
		{
			_ = item3.sp_id;
			_ = item3.sp_wings;
			_ = item3.sp_upgrade;
			if (player.class_id == SPID.SPIDToClass[item3.sp_id])
			{
				SPInfoItem sPInfoItem = new SPInfoItem();
				sPInfoItem.setData(player, item3, ShowSPDetails);
				ScaleControl(sPInfoItem);
				SearchedPlayerSPsFlowLayoutPanel.Controls.Add(sPInfoItem);
				if (item3.updatedAt > updatedAt)
				{
					updatedAt = item3.updatedAt;
				}
			}
		}
		Analytics.searcherd_player_items_with_shells = (from x in player.items
			where x.shell != null && (x.item_id == main_weapon?.item_id || x.item_id == secondary_weapon?.item_id || x.item_id == armor?.item_id)
			orderby x.type
			select x).ToList();
		if (Analytics.searcherd_player_items_with_shells.Count == 0)
		{
			ShellInfoMainPanel.Hide();
		}
		else
		{
			RuneLevelLabel.Hide();
			ShellInfoMainPanel.Show();
			Analytics.shown_shell_type = 1;
			while (Analytics.searcherd_player_items_with_shells.Find((PlayerItem x) => x.type == Analytics.shown_shell_type) == null)
			{
				Analytics.shown_shell_type = (Analytics.shown_shell_type + 1) % 3;
			}
			PlayerItem playerItem = Analytics.searcherd_player_items_with_shells.Find((PlayerItem x) => x.type == Analytics.shown_shell_type);
			if (playerItem != null)
			{
				SetShellInfo(new GameShell(playerItem.shell.Split(" ").ToList(), (playerItem.type != 0) ? 1 : 0), form.ShellEffectsFlowLayoutPanel);
				ShellItemTypeLabel.Text = ((Analytics.shown_shell_type == 0) ? "Armor" : ((Analytics.shown_shell_type == 1) ? "Main" : "Secondary"));
			}
			SwitchShellTypeButton.Visible = Analytics.searcherd_player_items_with_shells.Count > 1;
			SwitchToRuneButton.Visible = Analytics.searcherd_player_items_with_shells.Find((PlayerItem x) => x.type == Analytics.shown_shell_type && x.rune != null) != null;
			SwitchToShellButton.Visible = Analytics.searcherd_player_items_with_shells.Find((PlayerItem x) => x.type == Analytics.shown_shell_type && x.shell != null) != null;
			ShellItemTypeLabel.Text = ((Analytics.shown_shell_type == 0) ? "Armor" : ((Analytics.shown_shell_type == 1) ? "Main" : "Secondary"));
		}
		if (player.tattoos != null && player.tattoos.Count != 0)
		{
			List<PictureBox> list3 = new List<PictureBox>();
			CollectionsMarshal.SetCount(list3, 2);
			Span<PictureBox> span = CollectionsMarshal.AsSpan(list3);
			int num3 = 0;
			span[num3] = Tattoo1Icon;
			num3++;
			span[num3] = Tattoo2Icon;
			num3++;
			List<PictureBox> list4 = list3;
			List<NALabel> list5 = new List<NALabel>();
			CollectionsMarshal.SetCount(list5, 2);
			Span<NALabel> span2 = CollectionsMarshal.AsSpan(list5);
			num3 = 0;
			span2[num3] = Tattoo1UpgradeLabel;
			num3++;
			span2[num3] = Tattoo2UpgradeLabel;
			num3++;
			List<NALabel> list6 = list5;
			for (int i = 0; i < list4.Count; i++)
			{
				if (player.tattoos.Count > i)
				{
					PlayerTattoo playerTattoo = player.tattoos[i];
					list4[i].Image = EffectsID.GetIcon(playerTattoo.tattoo_id);
					list6[i].Text = $"+{playerTattoo.upgrade}";
					if (playerTattoo.updatedAt > updatedAt)
					{
						updatedAt = playerTattoo.updatedAt;
					}
				}
				else
				{
					list4[i].Image = null;
					list6[i].Text = "";
				}
			}
		}
		else
		{
			Tattoo1Icon.Image = null;
			Tattoo1UpgradeLabel.Text = "";
			Tattoo2Icon.Image = null;
			Tattoo2UpgradeLabel.Text = "";
		}
		SearchedPlayerLastUpdateDateLabel.Text = updatedAt.ToLocalTime().ToString();
	}

	public void SetSearchPlayerServerInfo()
	{
		SearchServerComboBox.setState(Analytics.SearchServer);
		SearchNicknameTextBox.Text = Analytics.SearchNickname;
	}

	private async void SearchPlayerButton_Click(object sender, EventArgs e)
	{
		PlayerInfoData playerInfoData = await Analytics.SearchForPlayer();
		if (playerInfoData != null)
		{
			SetSearchedPlayerInfo(playerInfoData);
		}
	}

	private void AnalyticsBackArrow_Click(object sender, EventArgs e)
	{
		RaidsHistoryBackButton_Click(sender, e);
	}

	private void AnalyticsBackArrow_MouseEnter(object sender, EventArgs e)
	{
		AnalyticsBackArrow.Image = Resources.back_arrow_hover;
	}

	private void AnalyticsBackArrow_MouseLeave(object sender, EventArgs e)
	{
		AnalyticsBackArrow.Image = Resources.back_arrow;
	}

	public static void SetShellInfo(GameShell shell, FlowLayoutPanel panel)
	{
		Utils.InvokeIfRequired(panel, delegate
		{
			panel.Controls.Clear();
			foreach (Label item in shell.toLabels(panel))
			{
				panel.Controls.Add(item);
			}
		});
	}

	public static void SetRuneInfo(GameRune rune, FlowLayoutPanel panel)
	{
		Utils.InvokeIfRequired(panel, delegate
		{
			panel.Controls.Clear();
			foreach (Label item in rune.toLabels(panel))
			{
				panel.Controls.Add(item);
			}
		});
	}

	private void SwitchShellTypeButton_MouseLeave(object sender, EventArgs e)
	{
		SwitchShellTypeButton.Image = Resources.switch_icon;
	}

	private void SwitchShellTypeButton_MouseEnter(object sender, EventArgs e)
	{
		SwitchShellTypeButton.Image = Resources.switch_icon_hover;
	}

	private void SwitchShellTypeButton_Click(object sender, EventArgs e)
	{
		if (Analytics.searcherd_player_items_with_shells.Count >= 1)
		{
			Analytics.shown_shell_type = (Analytics.shown_shell_type + 1) % 3;
			Analytics.show_item_rune = false;
			while (Analytics.searcherd_player_items_with_shells.Find((PlayerItem x) => x.type == Analytics.shown_shell_type) == null)
			{
				Analytics.shown_shell_type = (Analytics.shown_shell_type + 1) % 3;
			}
			PlayerItem playerItem = Analytics.searcherd_player_items_with_shells.Find((PlayerItem x) => x.type == Analytics.shown_shell_type);
			if (playerItem != null)
			{
				SwitchToRuneButton.Visible = playerItem.rune != null;
				SetShellInfo(new GameShell(playerItem.shell.Split(" ").ToList(), (playerItem.type != 0) ? 1 : 0), form.ShellEffectsFlowLayoutPanel);
				ShellItemTypeLabel.Text = ((Analytics.shown_shell_type == 0) ? "Armor" : ((Analytics.shown_shell_type == 1) ? "Main" : "Secondary"));
				RuneLevelLabel.Hide();
			}
		}
	}

	private void ShowMarathonTotalButton_Click(object sender, EventArgs e)
	{
		RaidData raid_data = Analytics.CalculateTotal();
		showRaidDetails(raid_data);
		updateRaidsHistoryRaidersPanel(raid_data);
	}

	private void SwitchToShellButton_Click(object sender, EventArgs e)
	{
		RuneLevelLabel.Hide();
		Analytics.show_item_rune = false;
		PlayerItem playerItem = Analytics.searcherd_player_items_with_shells.Find((PlayerItem x) => x.type == Analytics.shown_shell_type);
		if (playerItem != null)
		{
			SetShellInfo(new GameShell(playerItem.shell.Split(" ").ToList(), (playerItem.type != 0) ? 1 : 0), form.ShellEffectsFlowLayoutPanel);
			if (Analytics.shown_shell_type > 1)
			{
				SwitchShellTypeButton.Show();
			}
		}
	}

	private void SwitchToRuneButton_Click(object sender, EventArgs e)
	{
		RuneLevelLabel.Show();
		Analytics.show_item_rune = true;
		PlayerItem playerItem = Analytics.searcherd_player_items_with_shells.Find((PlayerItem x) => x.type == Analytics.shown_shell_type && x.rune != null);
		if (playerItem != null)
		{
			GameRune gameRune = new GameRune(playerItem.rune);
			SetRuneInfo(gameRune, form.ShellEffectsFlowLayoutPanel);
			RuneLevelLabel.Text = $"{gameRune.effects.Sum((RuneEffect x) => x.level)}/21";
		}
	}

	private void CloseSPDetailsButton_Click(object sender, EventArgs e)
	{
		SPDetailsBorderPanel.Hide();
	}

	public static void SetSPDetails(PlayerSP sp_details, Image avatar)
	{
		form.SPDetailsAvatar.Image = avatar;
		form.SPDetailsJobLabel.Text = $"Job: {sp_details.job} + {sp_details.sp_upgrade}";
		form.SPDetailsPerfectionLabel.Text = $"Perfection: {sp_details.perfection}";
		List<int> source = (from x in sp_details.build.Split('.')
			select Convert.ToInt32(x)).ToList();
		List<int> source2 = (from x in sp_details.pp.Split('.')
			select Convert.ToInt32(x)).ToList();
		List<int> source3 = (from x in sp_details.sl.Split('.')
			select Convert.ToInt32(x)).ToList();
		List<int> source4 = (from x in SPID.SPIDToBaseResistance[sp_details.sp_id].Split('.')
			select Convert.ToInt32(x)).ToList();
		form.SPDetailsAttackLabel.Text = $"{source.ElementAt(0) + source3.ElementAt(0)} ({source2.ElementAt(0)})";
		form.SPDetailsDefenceLabel.Text = $"{source.ElementAt(1) + source3.ElementAt(1)} ({source2.ElementAt(1)})";
		form.SPDetailsPropertyLabel.Text = $"{source.ElementAt(2) + source3.ElementAt(2)} ({source2.ElementAt(2)})";
		form.SPDetailsEnergyLabel.Text = $"{source.ElementAt(3) + source3.ElementAt(3)} ({source2.ElementAt(3)})";
		form.SPDetailsFireLabel.Text = $"{source4.ElementAt(0)} ({source2.ElementAt(4)})";
		form.SPDetailsWaterLabel.Text = $"{source4.ElementAt(1)} ({source2.ElementAt(5)})";
		form.SPDetailsLightLabel.Text = $"{source4.ElementAt(2)} ({source2.ElementAt(6)})";
		form.SPDetailsShadowLabel.Text = $"{source4.ElementAt(3)} ({source2.ElementAt(7)})";
	}

	public static void ShowSPDetails(PlayerSP sp_details, Image avatar)
	{
		if (form.SPDetailsBorderPanel != null && sp_details != null && sp_details.job.HasValue && sp_details.perfection.HasValue && sp_details.pp != null && sp_details.build != null && sp_details.sl != null)
		{
			SetSPDetails(sp_details, avatar);
			form.SPDetailsBorderPanel.Show();
			form.SPDetailsBorderPanel.BringToFront();
		}
	}

	public static bool isAnatliticsPlayerTabVisible()
	{
		return form.AnalyticsPlayersTab.Visible;
	}

	private void SaveConfigButton_Click(object sender, EventArgs e)
	{
		string text = Path.Combine(Directory.GetCurrentDirectory(), "configs");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.InitialDirectory = text;
		saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
		saveFileDialog.Title = "Save Configuration File";
		saveFileDialog.DefaultExt = "json";
		saveFileDialog.FileName = "characters_config.json";
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(Settings.config.charsConfigs, Formatting.Indented));
		}
	}

	private void LoadConfigButton_Click(object sender, EventArgs e)
	{
		string text = Path.Combine(Directory.GetCurrentDirectory(), "configs");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		using (OpenFileDialog openFileDialog = new OpenFileDialog())
		{
			openFileDialog.InitialDirectory = text;
			openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
			openFileDialog.Title = "Select Configuration File";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					List<CharConfig> charsConfigs = JsonConvert.DeserializeObject<List<CharConfig>>(File.ReadAllText(openFileDialog.FileName));
					Settings.config.charsConfigs = charsConfigs;
					Settings.SaveSettings();
					Settings.LoadCharsConfig();
				}
				catch (Exception)
				{
					ShowPopUp("Failed while loading configuration file");
				}
			}
		}
		flowLayoutCharactersPanel.Controls.Clear();
		foreach (NostaleCharacterInfo nostaleCharacterInfo in _nostaleCharacterInfoList)
		{
			AddCharacterToControlPanel(nostaleCharacterInfo);
		}
	}

	private void CloseFairyDetailsButton_Click(object sender, EventArgs e)
	{
		MainFairyDetailsPanel.Hide();
	}

	public static void ShowFairyDetails(PlayerFairy fairy_details, Image avatar)
	{
		if (form.MainFairyDetailsPanel != null && fairy_details != null)
		{
			SetFairyDetails(fairy_details, avatar);
			form.MainFairyDetailsPanel.Show();
			form.MainFairyDetailsPanel.BringToFront();
		}
	}

	public static void SetFairyDetails(PlayerFairy fairy_details, Image avatar)
	{
		form.FairyEffectsFlowLayoutPanel.Controls.Clear();
		form.FairyDetailsIcon.Image = avatar;
		form.FairyDetailsLabel.Text = GameFairy.IDToName((fairy_details?.fairy_id).GetValueOrDefault()) ?? "";
		form.FairyUpgradePercentLabel.Text = $"{(fairy_details?.upgrade).GetValueOrDefault()}/9 ({(fairy_details?.percent).GetValueOrDefault()}%)";
		if (fairy_details == null || fairy_details.effects == null)
		{
			return;
		}
		List<FairyEffect> effects = GameFairy.StringToEffects(fairy_details.effects);
		foreach (Label item in FairyEffect.toLabels(form.FairyEffectsFlowLayoutPanel, effects))
		{
			form.FairyEffectsFlowLayoutPanel.Controls.Add(item);
		}
	}

	private void SearchNicknameTextBox_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\r')
		{
			SearchPlayerButton_Click(sender, e);
			e.Handled = true;
		}
	}

	private void ClearRankingFiltersButton_Click(object sender, EventArgs e)
	{
		Analytics.ClearRankingFilters();
	}

	public static void UpdateRankingModeLabel()
	{
		string text = ((Analytics.ranking_raid_type_filter != 0) ? GameBoss.getBossName(Analytics.ranking_raid_type_filter) : "No Raid Selected");
		form.RankingModeLabel.Text = Analytics.ranking_mode + " - " + text;
	}

	private void RankingModeButton_Click(object sender, EventArgs e)
	{
		Analytics.ranking_mode = ((Analytics.ranking_mode == "Global") ? "Family" : "Global");
		UpdateRankingModeLabel();
	}

	private void SetRankingPanels()
	{
		if (RankingFlowLayoutPanel.Controls.Count == 0)
		{
			for (int i = 0; i < 50; i++)
			{
				RankingItem rankingItem = new RankingItem();
				SetRoundShape(rankingItem, 15);
				ScaleControl(rankingItem);
				RankingFlowLayoutPanel.Controls.Add(rankingItem);
			}
		}
	}

	private async void RankingSearchButton_Click(object sender, EventArgs e)
	{
		if (Analytics.ranking_raid_type_filter == 0)
		{
			ShowPopUp("Raid was not selected");
			return;
		}
		ranking_page = 1;
		UpdateRanking(reload: true);
	}

	public async void UpdateRanking(bool reload)
	{
		switch (Analytics.ranking_data_mode)
		{
		case "average_damage":
			if (reload)
			{
				Analytics.real_ranking_mode = Analytics.ranking_mode;
				List<int> sp_ids4 = ((Analytics.ranking_sps_filter.Count == 0) ? SPID.GetCombatSPs() : Analytics.ranking_sps_filter);
				Analytics.ranking_data_average_damage = await NAHttpClient.GetFullRanking(new GetFullRankingDto
				{
					server_id = NostaleServers.GetServerIdFromName(Mapper.server),
					boss_id = Analytics.ranking_raid_type_filter,
					sp_ids = sp_ids4
				});
			}
			UpdateRankingAverageDamage();
			break;
		case "max_hits":
			if (reload)
			{
				Analytics.real_ranking_mode = Analytics.ranking_mode;
				List<int> sp_ids3 = ((Analytics.ranking_sps_filter.Count == 0) ? new List<int>() : Analytics.ranking_sps_filter);
				Analytics.ranking_data_max_hits = await NAHttpClient.GetMaxHitsRanking(new RankingInfoDto
				{
					boss_id = Analytics.ranking_raid_type_filter,
					sp_ids = sp_ids3
				});
			}
			UpdateRankingMaxHit();
			break;
		case "best_times":
			if (reload)
			{
				Analytics.real_ranking_mode = Analytics.ranking_mode;
				List<int> sp_ids2 = ((Analytics.ranking_sps_filter.Count == 0) ? new List<int>() : Analytics.ranking_sps_filter);
				Analytics.ranking_data_best_times = await NAHttpClient.GetBestTimesRanking(new RankingInfoDto
				{
					boss_id = Analytics.ranking_raid_type_filter,
					sp_ids = sp_ids2
				});
			}
			UpdateRankingBestTimes();
			break;
		case "raids_done":
			if (reload)
			{
				Analytics.real_ranking_mode = Analytics.ranking_mode;
				List<int> sp_ids = ((Analytics.ranking_sps_filter.Count == 0) ? new List<int>() : Analytics.ranking_sps_filter);
				Analytics.ranking_data_raids_done = await NAHttpClient.GetRaidsDoneRanking(new RankingInfoDto
				{
					boss_id = Analytics.ranking_raid_type_filter,
					sp_ids = sp_ids
				});
				if (Analytics.ranking_sps_filter.Count == 0)
				{
					List<RankingRaidsDone> list = new List<RankingRaidsDone>();
					foreach (RankingRaidsDone item in Analytics.ranking_data_raids_done)
					{
						RankingRaidsDone rankingRaidsDone = list.Find((RankingRaidsDone x) => x.character_id == item.character_id && x.server_id == item.server_id);
						if (rankingRaidsDone != null)
						{
							rankingRaidsDone.total_raids += item.total_raids;
						}
						else
						{
							list.Add(item);
						}
					}
					Analytics.ranking_data_raids_done = list.OrderByDescending((RankingRaidsDone x) => x.total_raids).ToList();
				}
			}
			UpdateRankingRaidsDone();
			break;
		}
	}

	private async void UpdateRankingAverageDamage()
	{
		int i = 0;
		List<PlayerFullRankingInfo> players_info_list = ((Analytics.real_ranking_mode == "Global") ? Analytics.ranking_data_average_damage : Analytics.ranking_data_average_damage.Where((PlayerFullRankingInfo x) => x.family == Mapper?.family_name && x.server_id == NostaleServers.GetServerIdFromName(Mapper?.server)).ToList());
		if (players_info_list == null)
		{
			ClearRankingItems();
			ShowPopUp("Could not access the data");
			return;
		}
		foreach (Control control in RankingFlowLayoutPanel.Controls)
		{
			int index = (ranking_page - 1) * 50 + i;
			if (!(control is RankingItem rank_item))
			{
				continue;
			}
			if (players_info_list.Count <= index)
			{
				rank_item.resetData();
				continue;
			}
			PlayerFullRankingInfo player_info = players_info_list.ElementAt(index);
			if (player_info == null)
			{
				continue;
			}
			RaidRankingInfo raidRankingInfo = null;
			if (Analytics.ranking_avg_stddev_values.Count != 0)
			{
				raidRankingInfo = Analytics.ranking_avg_stddev_values.Find((RaidRankingInfo x) => x.sp_id == player_info.sp_id && x.server_id == player_info.server_id && x.boss_id == player_info.boss_id);
			}
			if (raidRankingInfo == null)
			{
				RaidRankingInfo raidRankingInfo2 = await NAHttpClient.GetRaidRankingInfo(new GetRaidRankingInfoDto
				{
					server_id = player_info.server_id,
					sp_id = player_info.sp_id,
					boss_id = player_info.boss_id
				});
				Analytics.ranking_avg_stddev_values.Add(raidRankingInfo2);
				raidRankingInfo = raidRankingInfo2;
				if (raidRankingInfo == null)
				{
					continue;
				}
			}
			int rank = Analytics.AssignPlayersRank(player_info.player_avg_damage, raidRankingInfo.mean_damage, raidRankingInfo.stddev_damage);
			rank_item.setData(index + 1, player_info.nickname, player_info.sp_id, player_info.player_avg_damage.ToString("N0", new CultureInfo("en-US")), player_info.sex, Analytics.GetRankIcon(rank), player_info.server_id);
			rank_item.BackColor = ((player_info.family == Mapper?.family_name && player_info.server_id == NostaleServers.GetServerIdFromName(Mapper?.server ?? "")) ? NAStyles.RankingFamMemberColor : NAStyles.ActiveCharColor);
			rank_item.BackColor = ((player_info.nickname == Mapper?.nickname && player_info.server_id == NostaleServers.GetServerIdFromName(Mapper?.server ?? "")) ? NAStyles.NotActiveCharColor : rank_item.BackColor);
			i++;
		}
		UpdateRankingArrowsVisibility();
	}

	private void UpdateRankingMaxHit()
	{
		int num = 0;
		List<RankingMaxHit> list = ((Analytics.real_ranking_mode == "Global") ? Analytics.ranking_data_max_hits : Analytics.ranking_data_max_hits.Where((RankingMaxHit x) => x.family == Mapper?.family_name && x.server_id == NostaleServers.GetServerIdFromName(Mapper?.server)).ToList());
		if (list == null)
		{
			ClearRankingItems();
			ShowPopUp("Could not access the data");
			return;
		}
		foreach (Control control in RankingFlowLayoutPanel.Controls)
		{
			int num2 = (ranking_page - 1) * 50 + num;
			if (control is RankingItem rankingItem)
			{
				if (list.Count <= num2)
				{
					rankingItem.resetData();
					continue;
				}
				RankingMaxHit rankingMaxHit = list.ElementAt(num2);
				rankingItem.setData(num2 + 1, rankingMaxHit.nickname, rankingMaxHit.sp_id, rankingMaxHit.max_hit.ToString("N0", new CultureInfo("en-US")), rankingMaxHit.sex, SPCard.getSkillIcon(rankingMaxHit.max_hit_skill_id), rankingMaxHit.server_id);
				rankingItem.BackColor = ((rankingMaxHit.family == Mapper?.family_name && rankingMaxHit.server_id == NostaleServers.GetServerIdFromName(Mapper?.server ?? "")) ? NAStyles.RankingFamMemberColor : NAStyles.ActiveCharColor);
				rankingItem.BackColor = ((rankingMaxHit.nickname == Mapper?.nickname && rankingMaxHit.server_id == NostaleServers.GetServerIdFromName(Mapper?.server ?? "")) ? NAStyles.NotActiveCharColor : rankingItem.BackColor);
				num++;
			}
		}
		UpdateRankingArrowsVisibility();
	}

	private void UpdateRankingBestTimes()
	{
		int num = 0;
		List<RankingBestTime> list = ((Analytics.real_ranking_mode == "Global") ? Analytics.ranking_data_best_times : Analytics.ranking_data_best_times.Where((RankingBestTime x) => x.family == Mapper?.family_name && x.server_id == NostaleServers.GetServerIdFromName(Mapper?.server)).ToList());
		if (list == null)
		{
			ClearRankingItems();
			ShowPopUp("Could not access the data");
			return;
		}
		foreach (Control control in RankingFlowLayoutPanel.Controls)
		{
			int num2 = (ranking_page - 1) * 50 + num;
			if (!(control is RankingItem rankingItem))
			{
				continue;
			}
			if (list.Count <= num2)
			{
				rankingItem.resetData();
				continue;
			}
			RankingBestTime rankingBestTime = list.ElementAt(num2);
			if (rankingBestTime.nickname == Mapper?.nickname && rankingBestTime.server_id == NostaleServers.GetServerIdFromName(Mapper?.server ?? ""))
			{
				rankingItem.setData(num2 + 1, rankingBestTime.nickname, rankingBestTime.sp_id, $"{TimeSpan.FromSeconds(rankingBestTime.finished_in):mm\\:ss}", rankingBestTime.sex, null, rankingBestTime.server_id);
				rankingItem.BackColor = NAStyles.NotActiveCharColor;
			}
			else if (Mapper?.family_name != "-" && rankingBestTime.family == Mapper?.family_name && rankingBestTime.server_id == NostaleServers.GetServerIdFromName(Mapper?.server ?? ""))
			{
				rankingItem.setData(num2 + 1, rankingBestTime.nickname, rankingBestTime.sp_id, $"{TimeSpan.FromSeconds(rankingBestTime.finished_in):mm\\:ss}", rankingBestTime.sex, null, rankingBestTime.server_id);
				rankingItem.BackColor = NAStyles.RankingFamMemberColor;
			}
			else
			{
				if (!BossID.isA8A9RaidBoss(Analytics.ranking_raid_type_filter))
				{
					rankingItem.setData(num2 + 1, "", rankingBestTime.sp_id, $"{TimeSpan.FromSeconds(rankingBestTime.finished_in):mm\\:ss}", rankingBestTime.sex, null, rankingBestTime.server_id);
				}
				else
				{
					rankingItem.setData(num2 + 1, rankingBestTime.nickname, rankingBestTime.sp_id, $"{TimeSpan.FromSeconds(rankingBestTime.finished_in):mm\\:ss}", rankingBestTime.sex, null, rankingBestTime.server_id);
				}
				rankingItem.BackColor = NAStyles.ActiveCharColor;
			}
			num++;
		}
		UpdateRankingArrowsVisibility();
	}

	public void UpdateRankingRaidsDone()
	{
		int num = 0;
		List<RankingRaidsDone> list = ((Analytics.real_ranking_mode == "Global") ? Analytics.ranking_data_raids_done : Analytics.ranking_data_raids_done.Where((RankingRaidsDone x) => x.family == Mapper?.family_name && x.server_id == NostaleServers.GetServerIdFromName(Mapper?.server)).ToList());
		if (list == null)
		{
			ClearRankingItems();
			ShowPopUp("Could not access the data");
			return;
		}
		foreach (Control control in RankingFlowLayoutPanel.Controls)
		{
			int num2 = (ranking_page - 1) * 50 + num;
			if (!(control is RankingItem rankingItem))
			{
				continue;
			}
			if (list.Count <= num2)
			{
				rankingItem.resetData();
				continue;
			}
			RankingRaidsDone rankingRaidsDone = list.ElementAt(num2);
			if (rankingRaidsDone.nickname == Mapper?.nickname && rankingRaidsDone.server_id == NostaleServers.GetServerIdFromName(Mapper?.server ?? ""))
			{
				rankingItem.setData(num2 + 1, rankingRaidsDone.nickname, rankingRaidsDone.sp_id, rankingRaidsDone.total_raids.ToString(), rankingRaidsDone.sex, null, rankingRaidsDone.server_id);
				rankingItem.BackColor = NAStyles.NotActiveCharColor;
			}
			else if (Mapper?.family_name != "-" && rankingRaidsDone.family == Mapper?.family_name && rankingRaidsDone.server_id == NostaleServers.GetServerIdFromName(Mapper?.server ?? ""))
			{
				rankingItem.setData(num2 + 1, rankingRaidsDone.nickname, rankingRaidsDone.sp_id, rankingRaidsDone.total_raids.ToString(), rankingRaidsDone.sex, null, rankingRaidsDone.server_id);
				rankingItem.BackColor = NAStyles.RankingFamMemberColor;
			}
			else
			{
				rankingItem.setData(num2 + 1, "", rankingRaidsDone.sp_id, rankingRaidsDone.total_raids.ToString(), rankingRaidsDone.sex, null, rankingRaidsDone.server_id);
				rankingItem.BackColor = NAStyles.ActiveCharColor;
			}
			num++;
		}
		UpdateRankingArrowsVisibility();
	}

	public static void ClearRankingItems()
	{
		foreach (Control control in form.RankingFlowLayoutPanel.Controls)
		{
			(control as RankingItem).resetData();
		}
		form.RankingNextPageButton.Hide();
		form.RankingPreviousPageButton.Hide();
		form.RankingPageLabel.Hide();
	}

	private void RankingNextPageButton_MouseEnter(object sender, EventArgs e)
	{
		RankingNextPageButton.Image = Resources.triangleRightHover;
	}

	private void RankingNextPageButton_MouseLeave(object sender, EventArgs e)
	{
		RankingNextPageButton.Image = Resources.triangleRight;
	}

	private void RankingPreviousPageButton_MouseEnter(object sender, EventArgs e)
	{
		RankingPreviousPageButton.Image = Resources.triangleLeftHover;
	}

	private void RankingPreviousPageButton_MouseLeave(object sender, EventArgs e)
	{
		RankingPreviousPageButton.Image = Resources.triangleLeft;
	}

	private void RankingPreviousPageButton_Click(object sender, EventArgs e)
	{
		if (ranking_page != 1)
		{
			ranking_page--;
			UpdateRanking(reload: false);
		}
	}

	private void RankingNextPageButton_Click(object sender, EventArgs e)
	{
		int num = 0;
		if (!((decimal)ranking_page == Math.Ceiling((decimal)num / 50m)))
		{
			ranking_page++;
			UpdateRanking(reload: false);
		}
	}

	private void UpdateRankingArrowsVisibility()
	{
		int rankingItemsCount = GetRankingItemsCount();
		RankingPreviousPageButton.Visible = ranking_page != 1;
		if (Analytics.real_ranking_mode == "Global")
		{
			RankingNextPageButton.Visible = (decimal)ranking_page < Math.Ceiling((decimal)rankingItemsCount / 50m);
		}
		else if (Mapper != null)
		{
			RankingNextPageButton.Visible = (decimal)ranking_page < Math.Ceiling((decimal)rankingItemsCount / 50m);
		}
		UpdateRankingPage();
	}

	private void UpdateRankingPage()
	{
		int rankingItemsCount = GetRankingItemsCount();
		RankingPageLabel.Show();
		if (Analytics.real_ranking_mode == "Global")
		{
			RankingPageLabel.Text = $"{ranking_page}/{Math.Max(Math.Ceiling((decimal)rankingItemsCount / 50m), 1m)}";
		}
		else if (Mapper != null)
		{
			RankingPageLabel.Text = $"{ranking_page}/{Math.Max(Math.Ceiling((decimal)rankingItemsCount / 50m), 1m)}";
		}
	}

	private int GetRankingItemsCount()
	{
		int result = 0;
		switch (Analytics.ranking_data_mode)
		{
		case "average_damage":
			result = ((Analytics.real_ranking_mode == "Global") ? Analytics.ranking_data_average_damage.Count : ((int)Math.Ceiling((decimal)Analytics.ranking_data_average_damage.Where((PlayerFullRankingInfo x) => x.family == Mapper.family_name && x.server_id == NostaleServers.GetServerIdFromName(Mapper.server)).ToList().Count)));
			break;
		case "max_hits":
			result = ((Analytics.real_ranking_mode == "Global") ? Analytics.ranking_data_max_hits.Count : ((int)Math.Ceiling((decimal)Analytics.ranking_data_max_hits.Where((RankingMaxHit x) => x.family == Mapper.family_name && x.server_id == NostaleServers.GetServerIdFromName(Mapper.server)).ToList().Count)));
			break;
		case "best_times":
			result = ((Analytics.real_ranking_mode == "Global") ? Analytics.ranking_data_best_times.Count : ((int)Math.Ceiling((decimal)Analytics.ranking_data_best_times.Where((RankingBestTime x) => x.family == Mapper.family_name && x.server_id == NostaleServers.GetServerIdFromName(Mapper.server)).ToList().Count)));
			break;
		case "raids_done":
			result = ((Analytics.real_ranking_mode == "Global") ? Analytics.ranking_data_raids_done.Count : ((int)Math.Ceiling((decimal)Analytics.ranking_data_raids_done.Where((RankingRaidsDone x) => x.family == Mapper.family_name && x.server_id == NostaleServers.GetServerIdFromName(Mapper.server)).ToList().Count)));
			break;
		}
		return result;
	}

	private void AnalyticsCleanUp()
	{
		Analytics.Clear();
		RankingRaidTypeFilterFlowLayoutPanel.Controls.Clear();
		RankingSPFilterFlowLayoutPanel.Controls.Clear();
		RaidsHistoryFlowLayoutPanel.Controls.Clear();
		SearchedPlayerFairiesFlowLayoutPanel.Controls.Clear();
		ShellEffectsFlowLayoutPanel.Controls.Clear();
		SearchedPlayerSPsFlowLayoutPanel.Controls.Clear();
		RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Controls.Clear();
		FamRecordsPanel.Controls.Clear();
		RankingFlowLayoutPanel.Controls.Clear();
	}

	private void SettingsSoundsMenuButton_Click(object sender, EventArgs e)
	{
		new SoundsPanelForm().Show();
	}

	private void SaveWindowsButton_Click(object sender, EventArgs e)
	{
		Controller.SaveWindowsPositions();
	}

	private void LoadWindowsButton_Click(object sender, EventArgs e)
	{
		Controller.LoadWindowsPositions();
	}

	private void ResizeWindowsButton_Click(object sender, EventArgs e)
	{
		int result = 0;
		int result2 = 0;
		if (!int.TryParse(ResizeWidthTextBox.Text, out result) || !int.TryParse(ResizeHeightTextBox.Text, out result2))
		{
			new NAMessageBox("Invalid width or height value!", "Error", error: true).Show();
		}
		else if ((result2 >= 550 && result >= 600) || new NAMessageBox("Too low resolution! Joining list and moving might not work properly. Do you want to continue?", "Warning!").ShowDialog() != DialogResult.No)
		{
			Controller.ResizeWindows(result, result2);
		}
	}

	private void ResizeWidthTextBox_TextChanged(object sender, EventArgs e)
	{
		if (sender is TextBox textBox)
		{
			textBox.Text = Regex.Replace(textBox.Text, "[^0-9]", "");
			textBox.SelectionStart = textBox.Text.Length;
		}
	}

	private void ResizeHeightTextBox_TextChanged(object sender, EventArgs e)
	{
		if (sender is TextBox textBox)
		{
			textBox.Text = Regex.Replace(textBox.Text, "[^0-9]", "");
			textBox.SelectionStart = textBox.Text.Length;
		}
	}

	private void FPSTextBox_TextChanged(object sender, EventArgs e)
	{
		if (sender is TextBox textBox)
		{
			textBox.Text = Regex.Replace(textBox.Text, "[^0-9]", "");
			textBox.SelectionStart = textBox.Text.Length;
		}
	}

	private void LimitFPSButton_Click(object sender, EventArgs e)
	{
		int result = 0;
		if (!int.TryParse(FPSTextBox.Text, out result))
		{
			new NAMessageBox("Invalid fps value!", "Error", error: true).Show();
			return;
		}
		result = Math.Max(result, 1);
		if (result <= 5 && new NAMessageBox("Too low FPS! Joining list might not work properly. Do you want to continue?", "Warning!").ShowDialog() == DialogResult.No)
		{
			return;
		}
		foreach (NostaleCharacterInfo item in _nostaleCharacterInfoList.Where((NostaleCharacterInfo x) => x.config.isRaider && !x.config.isAttacker && !x.config.isDisabled))
		{
			Controller.LimitFPS((int)item.process_id, result);
		}
	}

	private void OpenBoxesButton_Click(object sender, EventArgs e)
	{
		if (Raids.isOpeningBoxes)
		{
			Raids.isOpeningBoxes = false;
			SetOpenBoxesLabelText("Off");
		}
		else
		{
			SetOpenBoxesLabelText("On");
			Raids.OpenBoxes();
		}
	}

	public void SetOpenBoxesLabelText(string text)
	{
		OpenBoxesLabel.Text = text;
	}

	private void JoinDiscordButton_Click(object sender, EventArgs e)
	{
		string fileName = "https://discord.gg/TxHraSx59D";
		Process.Start(new ProcessStartInfo
		{
			FileName = fileName,
			UseShellExecute = true
		});
	}

	private bool ShowDelayWarning(int delay, int min_delay)
	{
		if (delay <= min_delay)
		{
			return new NAMessageBox("A low delay increases the risk of a ban. Are you sure you want to proceed?", "Warning!").ShowDialog() == DialogResult.Yes;
		}
		return true;
	}

	private void MapMobFilterButton_Click(object sender, EventArgs e)
	{
		mob_filters_window = new MobFilterWindow();
		mob_filters_window.Show();
	}

	private void RaidsNotificationsButton_Click(object sender, EventArgs e)
	{
		new RaidsNotificationsForm().Show();
	}

	public void UpdatePetTrainerSection()
	{
		List<NALabel> list = (from x in (from x in PetTrainerFlowLayoutPanel.GetAllControls()
				where x.Name.Contains("PetTrainerTimer")
				select x).OfType<NALabel>()
			orderby x.Name
			select x).ToList();
		List<PictureBox> list2 = (from PictureBox x in from x in PetTrainerFlowLayoutPanel.GetAllControls().OfType<Control>()
				where x.Name.Contains("PetTrainerIcon")
				select x
			orderby x.Name
			select x).ToList();
		int current_trainer_duration = 0;
		for (int i = 0; i < list.Count; i++)
		{
			switch (i)
			{
			case 0:
				current_trainer_duration = 900;
				break;
			case 1:
				current_trainer_duration = 1800;
				break;
			case 2:
				current_trainer_duration = 3600;
				break;
			}
			PetTrainerMob petTrainerMob = Miniland.pet_trainer_mobs_list.Find((PetTrainerMob x) => x.duration == current_trainer_duration);
			if (petTrainerMob == null)
			{
				list[i].Text = $"Trainer #{i + 1}: 00:00";
				list2[i].Image = null;
			}
			else if (petTrainerMob.despawn_time < DateTime.UtcNow)
			{
				list[i].Text = $"Trainer #{i + 1}: 00:00";
				list2[i].Image = null;
				Miniland.pet_trainer_mobs_list.Remove(petTrainerMob);
				Controller.PlaySound("Trainer Finished");
			}
			else
			{
				TimeSpan timeSpan = petTrainerMob.despawn_time - DateTime.UtcNow;
				list[i].Text = $"Trainer #{i + 1}: {$"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}"}";
				list2[i].Image = petTrainerMob.icon;
			}
		}
		List<NALabel> list3 = (from NALabel x in from x in PetTrainerFlowLayoutPanel.GetAllControls().OfType<Control>()
				where x.Name.Contains("TrainedPetInfo")
				select x
			orderby x.Name
			select x).ToList();
		List<PictureBox> list4 = (from PictureBox x in from x in PetTrainerFlowLayoutPanel.GetAllControls().OfType<Control>()
				where x.Name.Contains("TrainedPetIcon")
				select x
			orderby x.Name
			select x).ToList();
		List<MinilandPet> list5 = (from x in Miniland.pets_list
			where x.xp_changed
			orderby x.last_update
			select x).ToList();
		foreach (MinilandPet item in list5)
		{
			if (Math.Abs((item.last_update - DateTime.UtcNow).TotalSeconds) >= 15.0)
			{
				item.xp_changed = false;
			}
		}
		list5 = (from x in list5
			where x.xp_changed
			orderby x.pet_server_id
			select x).ToList();
		int num = 0;
		for (int j = 0; j < list3.Count; j++)
		{
			if (list5.Count <= j)
			{
				list3[j].Text = $"Pet #{j + 1}: -";
				list4[j].Image = null;
				continue;
			}
			MinilandPet minilandPet = list5[j];
			list3[j].Text = $"Pet #{j + 1}: {minilandPet.lvl}* (+{minilandPet.clvl}) {minilandPet.current_xp} / {minilandPet.max_xp}";
			list4[j].Image = minilandPet.icon;
			if (Math.Abs((minilandPet.last_update - DateTime.UtcNow).TotalSeconds) < 15.0)
			{
				num++;
			}
		}
		if (num < Miniland.trained_pets_count)
		{
			if (!Miniland.trained_pets_count_change_date.HasValue)
			{
				Miniland.trained_pets_count_change_date = DateTime.UtcNow;
			}
			if (Math.Abs((Miniland.trained_pets_count_change_date.Value - DateTime.UtcNow).TotalSeconds) >= 15.0)
			{
				Controller.PlaySound("Trainer Finished");
				Miniland.trained_pets_count = num;
				Miniland.trained_pets_count_change_date = null;
			}
		}
		else
		{
			Miniland.trained_pets_count_change_date = null;
			if (num > Miniland.trained_pets_count)
			{
				Miniland.trained_pets_count = Math.Min(num, Miniland.pet_trainer_mobs_list.Count);
			}
		}
	}

	private void SwitchRaidersTabPanel_MouseLeave(object sender, EventArgs e)
	{
		SwitchRaidersTabPanel.Image = Resources.switch_icon;
	}

	private void SwitchRaidersTabPanel_MouseEnter(object sender, EventArgs e)
	{
		SwitchRaidersTabPanel.Image = Resources.switch_icon_hover;
	}

	private void SwitchRaidersTabPanel_Click(object sender, EventArgs e)
	{
		flowLayoutRaidsPanel.Visible = !flowLayoutRaidsPanel.Visible;
		if (!flowLayoutRaidsPanel.Visible)
		{
			RaidersPanelScrollbar.Visible = false;
		}
		else
		{
			form.RaidersPanelScrollbar.updateScrollButtonSize(form.flowLayoutRaidsPanel.Controls.Count * 35);
		}
		RaidsBarsStatusFlowLayoutPanel.Visible = !RaidsBarsStatusFlowLayoutPanel.Visible;
		RefreshRaidsBarsLabel.Visible = !RefreshRaidsBarsLabel.Visible;
	}

	private async void CreateRaidBarsStatus()
	{
		if (Analytics.raids_bar_status_data.Count == 0)
		{
			await Analytics.RefreshBarStatusData();
		}
		RaidsBarsStatusFlowLayoutPanel.Controls.Clear();
		foreach (BarStatusDto raids_bar_status_datum in Analytics.raids_bar_status_data)
		{
			RaidStatusBarItem raidStatusBarItem = new RaidStatusBarItem();
			raidStatusBarItem.SetData(raids_bar_status_datum);
			ScaleControl(raidStatusBarItem);
			RaidsBarsStatusFlowLayoutPanel.Controls.Add(raidStatusBarItem);
		}
	}

	private void UpdateRaidBarsStatus()
	{
		if (RaidsBarsStatusFlowLayoutPanel.Controls.Count == 0)
		{
			CreateRaidBarsStatus();
		}
		if (Analytics.raids_bar_status_data.Count == RaidsBarsStatusFlowLayoutPanel.Controls.Count)
		{
			for (int i = 0; i < RaidsBarsStatusFlowLayoutPanel.Controls.Count; i++)
			{
				((RaidStatusBarItem)RaidsBarsStatusFlowLayoutPanel.Controls[i]).UpdateData(Analytics.raids_bar_status_data[i]);
			}
			last_raids_bars_update = DateTime.UtcNow;
		}
	}

	private void RefreshRaidsBarsLabel_MouseEnter(object sender, EventArgs e)
	{
		RefreshRaidsBarsLabel.ForeColor = NAStyles.NotActiveCharColor;
	}

	private void RefreshRaidsBarsLabel_MouseLeave(object sender, EventArgs e)
	{
		RefreshRaidsBarsLabel.ForeColor = NAStyles.CounterForeColor;
	}

	private async void RefreshRaidsBarsLabel_MouseClick(object sender, MouseEventArgs e)
	{
		await Analytics.RefreshBarStatusData();
		UpdateRaidBarsStatus();
	}

	private void RaidsHistoryFilterTextBox_TextChanged(object sender, EventArgs e)
	{
		Analytics.marathons_filter = RaidsHistoryFilterTextBox.Text;
		raids_history_page = 1;
		DisplayMarathonsHistory();
	}

	private void DataGridView_Paint(object sender, PaintEventArgs e)
	{
		DrawWatermark(e.Graphics, form.RaidsHistoryDetailsGridView.ClientRectangle);
	}

	private void DrawWatermark(Graphics g, Rectangle bounds)
	{
		Font font = new Font("Microsoft Sans Serif", 24f, FontStyle.Bold);
		Color color = Color.FromArgb(30, NAStyles.ButtonTrueColor);
		SizeF sizeF = g.MeasureString(form.watermark, font);
		g.DrawString(point: new PointF(((float)bounds.Width - sizeF.Width) / 2f, (float)bounds.Height - sizeF.Height - 10f), s: form.watermark, font: font, brush: new SolidBrush(color));
	}

	private void RefreshControlPanelButton_Click(object sender, EventArgs e)
	{
		if (RaidManager.raidStarted)
		{
			ShowPopUp("You can't do it during raid!");
			return;
		}
		_nostaleCharacterInfoList.Clear();
		flowLayoutCharactersPanel.Controls.Clear();
		PacketsManager.ResetMapperData();
		PacketsManager.UpdateNostaleCharacterInfosList();
		Settings.LoadSettings();
		CheckAccess.checkAccess();
		Settings.LoadCharsConfig();
		Controller.renamedClients = Settings.config.renameClients;
		Controller.RenameClients(false);
		miniland_state.Clear();
		Inviter = null;
		InviterLabel.Text = "None";
		updateMapInfo(Mapper?.map_id ?? (-1));
	}

	public void StartCountdown(string count_down_message, DateTime _CountDownEnd)
	{
		CountdownStaretd = true;
		CountdownMessage = count_down_message;
		CountDownPanel.Show();
		CountDownEnd = _CountDownEnd;
		string text = (CountDownEnd - DateTime.UtcNow).ToString("h\\:mm\\:ss");
		CountDownLabel.Text = CountdownMessage + " " + text;
		Controller.PlaySound("Notification Sound");
	}

	public void UpdateCountDown()
	{
		string text = (CountDownEnd - DateTime.UtcNow).ToString("h\\:mm\\:ss");
		CountDownLabel.Text = CountdownMessage + " " + text;
		if (DateTime.UtcNow > CountDownEnd)
		{
			Close();
		}
	}

	private void ForgotLicenseLabel_MouseEnter(object sender, EventArgs e)
	{
		ForgotLicenseLabel.ForeColor = NAStyles.NotActiveCharColor;
	}

	private void ForgotLicenseLabel_MouseLeave(object sender, EventArgs e)
	{
		ForgotLicenseLabel.ForeColor = NAStyles.CounterForeColor;
	}

	private void BuyLicenseLabel_MouseEnter(object sender, EventArgs e)
	{
		BuyLicenseLabel.ForeColor = NAStyles.NotActiveCharColor;
	}

	private void BuyLicenseLabel_MouseLeave(object sender, EventArgs e)
	{
		BuyLicenseLabel.ForeColor = NAStyles.CounterForeColor;
	}

	private void ForgotLicenseLabel_Click(object sender, EventArgs e)
	{
		new RemindLicenseForm().Show();
	}

	private void BuyLicenseLabel_Click(object sender, EventArgs e)
	{
		CheckAccess.BuyLicense();
	}

	private void ChangeNicknameButton_Click(object sender, EventArgs e)
	{
		new ChangeNicknameForm().Show();
	}

	private void BuyLicenseButton_Click(object sender, EventArgs e)
	{
		CheckAccess.BuyLicense();
	}

	private void PlayerRaidsStatisticsButton_Click(object sender, EventArgs e)
	{
		PlayerRaidsStatisticsPanel.Show();
		PlayerRaidsStatisticsPanel.BringToFront();
		PlayerRaidsSelectionFlowLayoutPanel.Controls.Clear();
		foreach (int boss_id in RaidID.GetAllRaidBosses())
		{
			if (!RaidID.IsPercentDamageRaid(RaidID.GetRaidID(boss_id)))
			{
				PictureBox boss_icon = new PictureBox();
				boss_icon.Name = $"PlayerRaidsStatisticsRaidFilter_{boss_id}";
				boss_icon.Image = GameMonster.GetIcon(boss_id);
				boss_icon.Size = new Size(42, 42);
				boss_icon.SizeMode = PictureBoxSizeMode.Zoom;
				boss_icon.Margin = new Padding(1, 1, 1, 1);
				boss_icon.Click += delegate
				{
					Analytics.ChangePlayerRaidStatisticsRaidType(boss_id, boss_icon);
				};
				ScaleControl(boss_icon);
				PlayerRaidsSelectionFlowLayoutPanel.Controls.Add(boss_icon);
			}
		}
	}

	private void PlayerEquipementButton_Click(object sender, EventArgs e)
	{
		PlayerRaidsStatisticsPanel.Hide();
		ClearPlayersRaidsStatistics();
	}

	public async void SetPlayersRaidsStatisticsData(PlayerInfoData player)
	{
		if (player == null)
		{
			return;
		}
		PlayersRaidsStatisticsLoadingLabel.Show();
		List<PlayerFullRankingInfo> averages_players = await NAHttpClient.GetFullRanking(new GetFullRankingDto
		{
			server_id = NostaleServers.GetServerIdFromName(Analytics.SearchServer),
			boss_id = Analytics.players_raid_statistics_raid_type,
			sp_ids = SPID.GetCombatSPs()
		});
		if (averages_players == null)
		{
			PlayersRaidsStatisticsLoadingLabel.Hide();
			return;
		}
		List<int> ranks = new List<int>();
		foreach (PlayerSP sp_card in player.sp_cards.OrderBy((PlayerSP x) => x.sp_id))
		{
			_ = sp_card.sp_id;
			_ = sp_card.sp_wings;
			_ = sp_card.sp_upgrade;
			if (player.class_id == SPID.SPIDToClass[sp_card.sp_id])
			{
				RaidRankingInfo raidRankingInfo = await NAHttpClient.GetRaidRankingInfo(new GetRaidRankingInfoDto
				{
					server_id = NostaleServers.GetServerIdFromName(Analytics.SearchServer),
					sp_id = sp_card.sp_id,
					boss_id = Analytics.players_raid_statistics_raid_type
				});
				PlayerFullRankingInfo playerFullRankingInfo = averages_players.Find((PlayerFullRankingInfo x) => x.nickname == player.nickname && x.server_id == NostaleServers.GetServerIdFromName(Analytics.SearchServer) && x.sp_id == sp_card.sp_id);
				int item2 = ((playerFullRankingInfo != null && raidRankingInfo != null) ? Analytics.AssignPlayersRank(playerFullRankingInfo.player_avg_damage, raidRankingInfo.mean_damage, raidRankingInfo.stddev_damage) : 0);
				ranks.Add(item2);
			}
		}
		List<RankingMaxHit> max_hits = await NAHttpClient.GetMaxHitsRanking(new RankingInfoDto
		{
			boss_id = Analytics.players_raid_statistics_raid_type,
			sp_ids = new List<int>()
		});
		List<RankingBestTime> list = (((!(player.family == Mapper?.family_name) || !(Analytics.SearchServer == Mapper.server)) && !CheckAccess.isAdmin) ? null : (await NAHttpClient.GetBestTimesRanking(new RankingInfoDto
		{
			boss_id = Analytics.players_raid_statistics_raid_type,
			sp_ids = new List<int>()
		})));
		List<RankingBestTime> best_times = list;
		List<RankingRaidsDone> list2 = (((!(player.family == Mapper?.family_name) || !(Analytics.SearchServer == Mapper.server)) && !CheckAccess.isAdmin) ? null : (await NAHttpClient.GetRaidsDoneRanking(new RankingInfoDto
		{
			boss_id = Analytics.players_raid_statistics_raid_type,
			sp_ids = new List<int>()
		})));
		List<RankingRaidsDone> list3 = list2;
		if (list3 != null)
		{
			List<RankingRaidsDone> list4 = new List<RankingRaidsDone>();
			foreach (RankingRaidsDone item in list3)
			{
				RankingRaidsDone rankingRaidsDone = list4.Find((RankingRaidsDone x) => x.character_id == item.character_id && x.server_id == item.server_id);
				if (rankingRaidsDone != null)
				{
					rankingRaidsDone.total_raids += item.total_raids;
				}
				else
				{
					list4.Add(item);
				}
			}
			list3 = list4.OrderByDescending((RankingRaidsDone x) => x.total_raids).ToList();
		}
		PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Clear();
		List<string> obj = new List<string>(7) { "SP", "Rank", "Position", "Avg. Damage", "Position", "Max Hit", "Icon" };
		int num = 0;
		foreach (string item3 in obj)
		{
			NALabel nALabel = new NALabel();
			nALabel.Text = item3;
			nALabel.TextAlign = ContentAlignment.MiddleCenter;
			nALabel.Anchor = AnchorStyles.None;
			nALabel.Padding = new Padding(0);
			nALabel.Margin = new Padding(0);
			ScaleControl(nALabel);
			PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Add(nALabel, num, 0);
			num++;
			nALabel.Dock = DockStyle.Fill;
			nALabel.BackColor = NAStyles.ButtonFalseColor;
		}
		player.sp_cards = (from card in player.sp_cards
			group card by card.sp_id into @group
			select @group.OrderByDescending((PlayerSP card) => card.updatedAt).First()).ToList();
		int num2 = 1;
		foreach (PlayerSP item4 in player.sp_cards.OrderBy((PlayerSP x) => x.sp_id))
		{
			_ = item4.sp_id;
			_ = item4.sp_wings;
			_ = item4.sp_upgrade;
			if (player.class_id == SPID.SPIDToClass[item4.sp_id])
			{
				PictureBox pictureBox = new PictureBox();
				pictureBox.Image = GamePlayer.GetIcon(item4.sp_id, player.sex, player.class_id);
				pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox.Anchor = AnchorStyles.None;
				pictureBox.Padding = new Padding(0);
				pictureBox.Margin = new Padding(0);
				ScaleControl(pictureBox);
				PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Add(pictureBox, 0, num2);
				num2++;
				pictureBox.Dock = DockStyle.Fill;
				pictureBox.BackColor = ((num2 % 2 == 0) ? NAStyles.SelectedPanelColor : NAStyles.ButtonFalseColor);
			}
		}
		num2 = 1;
		foreach (PlayerSP item5 in player.sp_cards.OrderBy((PlayerSP x) => x.sp_id))
		{
			_ = item5.sp_id;
			_ = item5.sp_wings;
			_ = item5.sp_upgrade;
			if (player.class_id == SPID.SPIDToClass[item5.sp_id])
			{
				PictureBox pictureBox2 = new PictureBox();
				pictureBox2.Image = Analytics.GetRankIcon(ranks[num2 - 1]);
				pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox2.Anchor = AnchorStyles.None;
				pictureBox2.Padding = new Padding(0);
				pictureBox2.Margin = new Padding(0);
				ScaleControl(pictureBox2);
				PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Add(pictureBox2, 1, num2);
				num2++;
				pictureBox2.Dock = DockStyle.Fill;
				pictureBox2.BackColor = ((num2 % 2 == 0) ? NAStyles.SelectedPanelColor : NAStyles.ButtonFalseColor);
			}
		}
		num2 = 1;
		foreach (PlayerSP sp_card in player.sp_cards.OrderBy((PlayerSP x) => x.sp_id))
		{
			_ = sp_card.sp_id;
			_ = sp_card.sp_wings;
			_ = sp_card.sp_upgrade;
			if (player.class_id == SPID.SPIDToClass[sp_card.sp_id])
			{
				NALabel nALabel2 = new NALabel();
				List<PlayerFullRankingInfo> list5 = averages_players.Where((PlayerFullRankingInfo x) => x.sp_id == sp_card.sp_id).ToList();
				PlayerFullRankingInfo playerFullRankingInfo2 = list5.Find((PlayerFullRankingInfo x) => x.nickname == player.nickname && x.server_id == NostaleServers.GetServerIdFromName(Analytics.SearchServer));
				string text = ((playerFullRankingInfo2 == null) ? "-" : $"#{list5.IndexOf(playerFullRankingInfo2) + 1}");
				nALabel2.Text = text;
				nALabel2.TextAlign = ContentAlignment.MiddleCenter;
				nALabel2.Anchor = AnchorStyles.None;
				nALabel2.Padding = new Padding(0);
				nALabel2.Margin = new Padding(0);
				ScaleControl(nALabel2);
				PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Add(nALabel2, 2, num2);
				num2++;
				nALabel2.Dock = DockStyle.Fill;
				nALabel2.BackColor = ((num2 % 2 == 0) ? NAStyles.SelectedPanelColor : NAStyles.ButtonFalseColor);
			}
		}
		num2 = 1;
		foreach (PlayerSP sp_card in player.sp_cards.OrderBy((PlayerSP x) => x.sp_id))
		{
			_ = sp_card.sp_id;
			_ = sp_card.sp_wings;
			_ = sp_card.sp_upgrade;
			if (player.class_id == SPID.SPIDToClass[sp_card.sp_id])
			{
				NALabel nALabel3 = new NALabel();
				nALabel3.Text = (averages_players.Find((PlayerFullRankingInfo x) => x.nickname == player.nickname && x.server_id == NostaleServers.GetServerIdFromName(Analytics.SearchServer) && x.sp_id == sp_card.sp_id)?.player_avg_damage ?? 0).ToString("N0", new CultureInfo("es-ES"));
				nALabel3.TextAlign = ContentAlignment.MiddleCenter;
				nALabel3.Anchor = AnchorStyles.None;
				nALabel3.Padding = new Padding(0);
				nALabel3.Margin = new Padding(0);
				ScaleControl(nALabel3);
				PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Add(nALabel3, 3, num2);
				num2++;
				nALabel3.Dock = DockStyle.Fill;
				nALabel3.BackColor = ((num2 % 2 == 0) ? NAStyles.SelectedPanelColor : NAStyles.ButtonFalseColor);
			}
		}
		num2 = 1;
		foreach (PlayerSP sp_card in player.sp_cards.OrderBy((PlayerSP x) => x.sp_id))
		{
			_ = sp_card.sp_id;
			_ = sp_card.sp_wings;
			_ = sp_card.sp_upgrade;
			if (player.class_id == SPID.SPIDToClass[sp_card.sp_id])
			{
				NALabel nALabel4 = new NALabel();
				List<RankingMaxHit> list6 = max_hits.Where((RankingMaxHit x) => x.sp_id == sp_card.sp_id).ToList();
				RankingMaxHit rankingMaxHit = list6.Find((RankingMaxHit x) => x.nickname == player.nickname && x.server_id == NostaleServers.GetServerIdFromName(Analytics.SearchServer) && x.sp_id == sp_card.sp_id);
				string text2 = ((rankingMaxHit == null) ? "-" : $"#{list6.IndexOf(rankingMaxHit) + 1}");
				nALabel4.Text = text2;
				nALabel4.TextAlign = ContentAlignment.MiddleCenter;
				nALabel4.Anchor = AnchorStyles.None;
				nALabel4.Padding = new Padding(0);
				nALabel4.Margin = new Padding(0);
				ScaleControl(nALabel4);
				PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Add(nALabel4, 4, num2);
				num2++;
				nALabel4.Dock = DockStyle.Fill;
				nALabel4.BackColor = ((num2 % 2 == 0) ? NAStyles.SelectedPanelColor : NAStyles.ButtonFalseColor);
			}
		}
		num2 = 1;
		foreach (PlayerSP sp_card in player.sp_cards.OrderBy((PlayerSP x) => x.sp_id))
		{
			_ = sp_card.sp_id;
			_ = sp_card.sp_wings;
			_ = sp_card.sp_upgrade;
			if (player.class_id == SPID.SPIDToClass[sp_card.sp_id])
			{
				NALabel nALabel5 = new NALabel();
				nALabel5.Text = (max_hits.Find((RankingMaxHit x) => x.nickname == player.nickname && x.server_id == NostaleServers.GetServerIdFromName(Analytics.SearchServer) && x.sp_id == sp_card.sp_id)?.max_hit ?? 0).ToString("N0", new CultureInfo("es-ES"));
				nALabel5.TextAlign = ContentAlignment.MiddleCenter;
				nALabel5.Anchor = AnchorStyles.None;
				nALabel5.Padding = new Padding(0);
				nALabel5.Margin = new Padding(0);
				ScaleControl(nALabel5);
				PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Add(nALabel5, 5, num2);
				num2++;
				nALabel5.Dock = DockStyle.Fill;
				nALabel5.BackColor = ((num2 % 2 == 0) ? NAStyles.SelectedPanelColor : NAStyles.ButtonFalseColor);
			}
		}
		num2 = 1;
		foreach (PlayerSP sp_card in player.sp_cards.OrderBy((PlayerSP x) => x.sp_id))
		{
			_ = sp_card.sp_id;
			_ = sp_card.sp_wings;
			_ = sp_card.sp_upgrade;
			if (player.class_id == SPID.SPIDToClass[sp_card.sp_id])
			{
				int skill_id = max_hits.Find((RankingMaxHit x) => x.nickname == player.nickname && x.server_id == NostaleServers.GetServerIdFromName(Analytics.SearchServer) && x.sp_id == sp_card.sp_id)?.max_hit_skill_id ?? 0;
				Panel panel = new Panel();
				panel.Padding = new Padding(0);
				panel.Margin = new Padding(0);
				panel.Dock = DockStyle.Fill;
				PictureBox pictureBox3 = new PictureBox();
				pictureBox3.Image = SPCard.getSkillIcon(skill_id);
				pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox3.Padding = new Padding(0);
				pictureBox3.Margin = new Padding(0);
				pictureBox3.Size = new Size(40, 40);
				pictureBox3.Anchor = AnchorStyles.None;
				panel.Controls.Add(pictureBox3);
				ScaleControl(panel);
				pictureBox3.Location = new Point((panel.Width - pictureBox3.Width) / 2, (panel.Height - pictureBox3.Height) / 2);
				PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Add(panel, 6, num2);
				num2++;
				panel.BackColor = ((num2 % 2 == 0) ? NAStyles.SelectedPanelColor : NAStyles.ButtonFalseColor);
			}
		}
		if (list3 != null)
		{
			RankingRaidsDone rankingRaidsDone2 = list3.Find((RankingRaidsDone x) => x.nickname == player.nickname && x.server_id == NostaleServers.GetServerIdFromName(Analytics.SearchServer));
			string text3 = ((rankingRaidsDone2 == null) ? "-" : $"#{list3.IndexOf(rankingRaidsDone2) + 1}");
			PlayerRaidsStatisticsRaidsFinishedLabel.Text = ((rankingRaidsDone2 == null) ? "Raids Done: -" : $"Raids Done: {rankingRaidsDone2.total_raids}");
			PlayerRaidsStatisticsRaidsFinishedRankLabel.Text = text3;
			PlayerRaidsStatisticsRaidsFinishedLabel.Show();
			PlayerRaidsStatisticsRaidsFinishedRankLabel.Show();
		}
		else
		{
			PlayerRaidsStatisticsRaidsFinishedLabel.Hide();
			PlayerRaidsStatisticsRaidsFinishedRankLabel.Hide();
		}
		if (best_times != null)
		{
			RankingBestTime rankingBestTime = best_times.Find((RankingBestTime x) => x.nickname == player.nickname && x.server_id == NostaleServers.GetServerIdFromName(Analytics.SearchServer));
			string text4 = ((rankingBestTime == null) ? "-" : $"#{best_times.IndexOf(rankingBestTime) + 1}");
			PlayerRaidsStatisticsBestTimeLabel.Text = ((rankingBestTime == null) ? "Best Time: -" : $"Best Time: {TimeSpan.FromSeconds(rankingBestTime.finished_in):mm\\:ss}");
			PlayerRaidsStatisticsBestTimeRankLabel.Text = text4;
			PlayerRaidsStatisticsBestTimeLabel.Show();
			PlayerRaidsStatisticsBestTimeRankLabel.Show();
		}
		else
		{
			PlayerRaidsStatisticsBestTimeLabel.Hide();
			PlayerRaidsStatisticsBestTimeRankLabel.Hide();
		}
		RankingMaxHit rankingMaxHit2 = max_hits.Find((RankingMaxHit x) => x.nickname == player.nickname && x.server_id == NostaleServers.GetServerIdFromName(Analytics.SearchServer));
		string text5 = ((rankingMaxHit2 == null) ? "-" : $"#{max_hits.IndexOf(rankingMaxHit2) + 1}");
		PlayerRaidsStatisticsTotalMaxHitRankLabel.Text = text5;
		text5 = ((rankingMaxHit2 == null) ? "-" : rankingMaxHit2.max_hit.ToString("N0", new CultureInfo("es-ES")));
		PlayerRaidsStatisticsTotalMaxHitLabel.Text = "Total Max Hit: " + text5;
		PlayerRaidsStatisticsBossIcon.Image = Analytics.players_raid_statistics_raid_type_picture_box?.Image ?? null;
		PlayerRaidsStatisticsBossNameLabel.Text = GameBoss.getBossName(Analytics.players_raid_statistics_raid_type);
		PlayersRaidsStatisticsLoadingLabel.Hide();
		ShowPlayersRaidsStatisticsElements();
	}

	public void ClearPlayersRaidsStatistics()
	{
		PlayerRaidsStatisticsRaidsStatisticsTablePanel.Controls.Clear();
		PlayerRaidsStatisticsBossNameLabel.Hide();
		PlayerRaidsStatisticsBossIcon.Hide();
		PlayerRaidsStatisticsTotalMaxHitLabel.Hide();
		PlayerRaidsStatisticsTotalMaxHitRankLabel.Hide();
		PlayerRaidsStatisticsBestTimeLabel.Hide();
		PlayerRaidsStatisticsBestTimeRankLabel.Hide();
		PlayerRaidsStatisticsRaidsFinishedLabel.Hide();
		PlayerRaidsStatisticsRaidsFinishedRankLabel.Hide();
	}

	public void ShowPlayersRaidsStatisticsElements()
	{
		PlayerRaidsStatisticsBossNameLabel.Show();
		PlayerRaidsStatisticsBossIcon.Show();
		PlayerRaidsStatisticsTotalMaxHitLabel.Show();
		PlayerRaidsStatisticsTotalMaxHitRankLabel.Show();
	}

	private void SetRankingTabsColor()
	{
		RankingAverageDMGLabel.BackColor = NAStyles.NotSelectedTabColor;
		RankingMaxHitsLabel.BackColor = NAStyles.NotSelectedTabColor;
		RankingBestTimesLabel.BackColor = NAStyles.NotSelectedTabColor;
		RankingRaidsDoneLabel.BackColor = NAStyles.NotSelectedTabColor;
		ranking_page = 1;
	}

	private void RankingAverageDMGLabel_Click(object sender, EventArgs e)
	{
		if (!(Analytics.ranking_data_mode == "average_damage"))
		{
			SetRankingTabsColor();
			Analytics.ranking_data_mode = "average_damage";
			RankingAverageDMGLabel.BackColor = NAStyles.MainThemeDarker;
			UpdateRanking(reload: true);
		}
	}

	private void RankingMaxHitsLabel_Click(object sender, EventArgs e)
	{
		if (!(Analytics.ranking_data_mode == "max_hits"))
		{
			SetRankingTabsColor();
			Analytics.ranking_data_mode = "max_hits";
			RankingMaxHitsLabel.BackColor = NAStyles.MainThemeDarker;
			UpdateRanking(reload: true);
		}
	}

	private void RankingBestTimesLabel_Click(object sender, EventArgs e)
	{
		if (!(Analytics.ranking_data_mode == "best_times"))
		{
			SetRankingTabsColor();
			Analytics.ranking_data_mode = "best_times";
			RankingBestTimesLabel.BackColor = NAStyles.MainThemeDarker;
			UpdateRanking(reload: true);
		}
	}

	private void RankingRaidsDoneLabel_Click(object sender, EventArgs e)
	{
		if (!(Analytics.ranking_data_mode == "raids_done"))
		{
			SetRankingTabsColor();
			Analytics.ranking_data_mode = "raids_done";
			RankingRaidsDoneLabel.BackColor = NAStyles.MainThemeDarker;
			UpdateRanking(reload: true);
		}
	}

	private void SettingsWaypointsMenuButton_Click(object sender, EventArgs e)
	{
		new WaypointsConfigMenu().Show();
	}

	private void ChangeNicknameLabel_MouseEnter(object sender, EventArgs e)
	{
		ChangeNicknameLabel.ForeColor = NAStyles.NotActiveCharColor;
	}

	private void ChangeNicknameLabel_MouseLeave(object sender, EventArgs e)
	{
		ChangeNicknameLabel.ForeColor = NAStyles.CounterForeColor;
	}

	private void ChangeNicknameLabel_Click(object sender, EventArgs e)
	{
		new ChangeNicknameForm().Show();
	}

	public void UpdateQuestsPanel()
	{
		UpdateQuestsFlowLayoutPanel();
		UpdateQuestPath(QuestManager.path_to_quest_target);
		UpdateQuestObjectiveControls();
	}

	public void UpdateQuestsFlowLayoutPanel()
	{
		Utils.InvokeIfRequired(QuestsFlowLayoutPanel, delegate
		{
			QuestsFlowLayoutPanel.Controls.Clear();
			if (QuestManager.quests.Count == 0)
			{
				NALabel nALabel = new NALabel
				{
					Size = new Size(150, 50),
					AutoSize = false,
					Text = "Relog to load quests data",
					TextAlign = ContentAlignment.MiddleCenter
				};
				ScaleControl(nALabel);
				QuestsFlowLayoutPanel.Controls.Add(nALabel);
				return;
			}
			int num = 1;
			int num2 = 1;
			foreach (GameQuest quest in QuestManager.quests)
			{
				if (quest.questID != 0)
				{
					NALabel quest_label = new NALabel();
					quest_label.Size = new Size(150, 16);
					quest_label.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold, GraphicsUnit.Point, 238);
					quest_label.AutoSize = false;
					if (QuestManager.followed_quest != null && quest.questPosition == QuestManager.followed_quest.questPosition)
					{
						quest_label.ForeColor = NAStyles.NotActiveCharColor;
					}
					string text = "";
					if (quest.questPosition == 5)
					{
						text = "Main";
					}
					else if (quest.questPosition < 5)
					{
						text = $"Flower #{num2}";
						num2++;
					}
					else if (quest.questPosition > 5)
					{
						text = $"Side #{num}";
						num++;
					}
					quest_label.Text = text ?? "";
					quest_label.TextAlign = ContentAlignment.MiddleCenter;
					quest_label.MouseEnter += delegate(object? sender, EventArgs e)
					{
						if (sender != null)
						{
							NALabel nALabel3 = (NALabel)sender;
							if (nALabel3.ForeColor != NAStyles.NotActiveCharColor)
							{
								nALabel3.ForeColor = NAStyles.PlayersColor;
							}
						}
					};
					quest_label.MouseLeave += delegate(object? sender, EventArgs e)
					{
						if (sender != null)
						{
							NALabel nALabel2 = (NALabel)sender;
							if (nALabel2.ForeColor != NAStyles.NotActiveCharColor)
							{
								nALabel2.ForeColor = NAStyles.CounterForeColor;
							}
						}
					};
					quest_label.Click += delegate(object? sender, EventArgs e)
					{
						QuestManager.UpdateCurrentQuestClick(quest_label.Text);
						foreach (Control control in QuestsFlowLayoutPanel.Controls)
						{
							control.ForeColor = NAStyles.CounterForeColor;
						}
						if (sender != null)
						{
							((NALabel)sender).ForeColor = NAStyles.NotActiveCharColor;
							QuestManager.ShowQuestSearchInstanceMap = false;
							QuestManager.quest_search_selected_item = "";
							QuestManager.navigating_instance_map_id = -1;
							UpdateQuestSearchResults();
							ShowTimeSpaceMapButton.Hide();
							NAvigator.show_time_space_map = false;
						}
					};
					ScaleControl(quest_label);
					QuestsFlowLayoutPanel.Controls.Add(quest_label);
				}
			}
		});
	}

	public void UpdateQuestPath(List<int> path_map_ids)
	{
		string path = "Path: ";
		foreach (int path_map_id in path_map_ids)
		{
			path += MapID.GetMapName(path_map_id);
			if (path_map_id != path_map_ids.Last())
			{
				path += " => ";
			}
		}
		Utils.InvokeIfRequired(QuestPathLabel, delegate
		{
			QuestPathLabel.Text = path;
		});
	}

	public void UpdateQuestObjectiveControls()
	{
		if (QuestManager.followed_quest == null)
		{
			Utils.InvokeIfRequired(MainQuestsPanel, delegate
			{
				QuestObjectiveIconsFlowLayoutPanel.Controls.Clear();
				QuestObjectiveIconsFlowLayoutPanel.Hide();
				QuestObjectiveLabel.Hide();
			});
			return;
		}
		string objective = "";
		if (new List<int> { 8, 31, 32, 23, 26 }.Contains(QuestManager.followed_quest.questType))
		{
			Utils.InvokeIfRequired(MainQuestsPanel, delegate
			{
				QuestObjectiveIconsFlowLayoutPanel.Controls.Clear();
				QuestObjectiveIconsFlowLayoutPanel.Hide();
				QuestObjectiveLabel.Show();
				if (QuestManager.followed_quest.questType == 8)
				{
					objective = "Produce Item";
				}
				else if (QuestManager.followed_quest.questType == 31)
				{
					objective = "Tame Pets";
				}
				else if (QuestManager.followed_quest.questType == 32)
				{
					objective = "Train Pets Level";
				}
				else if (QuestManager.followed_quest.questType == 23)
				{
					objective = "Collect Reward";
				}
				else if (QuestManager.followed_quest.questType == 26 && QuestManager.followed_quest_data != null)
				{
					string value = ((Mapper == null || Mapper.lvl == -1) ? "your lvl -10" : $"{Mapper.lvl - 10}");
					objective = $"Kill {QuestManager.followed_quest_data.data.ElementAt(0).ElementAt(0)} monsters with higher level than {value}";
				}
				if ((QuestManager.followed_quest.questType == 31 || QuestManager.followed_quest.questType == 32) && QuestManager.followed_quest_data != null && QuestManager.followed_quest_data.data.Count > 0 && QuestManager.followed_quest_data.data.ElementAt(0).Count >= 2)
				{
					objective = $"{objective}: {QuestManager.followed_quest_data.data.ElementAt(0).ElementAt(0)} Stars x{QuestManager.followed_quest_data.data.ElementAt(0).ElementAt(1)}";
				}
				QuestObjectiveLabel.Text = objective;
			});
			return;
		}
		Utils.InvokeIfRequired(MainQuestsPanel, delegate
		{
			int num = 100;
			QuestObjectiveIconsFlowLayoutPanel.Controls.Clear();
			QuestObjectiveIconsFlowLayoutPanel.Show();
			QuestObjectiveLabel.Show();
			switch (QuestManager.quest_target_type)
			{
			case "Map":
				objective = ((QuestManager.followed_quest.questType == 4) ? "Collect Items" : "Go To");
				QuestObjectiveLabel.Text = $"{objective}: {MapID.GetMapName(QuestManager.quest_target_id)} ({QuestManager.target_location.X}:{QuestManager.target_location.Y})";
				break;
			case "TimeSpace":
			{
				QuestObjectiveLabel.Text = $"Finish Time Space: {QuestManager.time_space_name} ({QuestManager.target_location_map})";
				PictureBox pictureBox3 = new PictureBox
				{
					Image = (File.Exists("images\\npcs\\time_space.png") ? Image.FromFile("images\\npcs\\time_space.png") : null),
					Size = new Size(num, num),
					Margin = new Padding(0),
					Padding = new Padding(0),
					SizeMode = PictureBoxSizeMode.Zoom
				};
				ScaleControl(pictureBox3);
				QuestObjectiveIconsFlowLayoutPanel.Controls.Add(pictureBox3);
				break;
			}
			case "NPC":
			{
				objective = ((QuestManager.followed_quest.questType == 2 || QuestManager.followed_quest.questType == 4 || QuestManager.followed_quest.questType == 14) ? "Deliver Items To" : "Talk With");
				QuestObjectiveLabel.Text = $"{objective}: {GameMonster.GetNameById(QuestManager.quest_target_id)} ({QuestManager.target_location_map})";
				PictureBox pictureBox2 = new PictureBox
				{
					Image = (File.Exists($"images\\npcs\\{QuestManager.quest_target_id}.png") ? Image.FromFile($"images\\npcs\\{QuestManager.quest_target_id}.png") : null),
					Size = new Size(num, num),
					Margin = new Padding(0),
					Padding = new Padding(0),
					SizeMode = PictureBoxSizeMode.Zoom
				};
				ScaleControl(pictureBox2);
				QuestObjectiveIconsFlowLayoutPanel.Controls.Add(pictureBox2);
				break;
			}
			case "Mob":
			{
				string text2 = "";
				foreach (int item in QuestManager.mobs_to_hunt)
				{
					PictureBox pictureBox4 = new PictureBox
					{
						Image = (File.Exists($"images\\npcs\\{item}.png") ? Image.FromFile($"images\\npcs\\{item}.png") : null),
						Size = new Size(num, num),
						Margin = new Padding(0),
						Padding = new Padding(0),
						SizeMode = PictureBoxSizeMode.Zoom
					};
					ScaleControl(pictureBox4);
					QuestObjectiveIconsFlowLayoutPanel.Controls.Add(pictureBox4);
					text2 = text2 + " " + GameMonster.GetNameById(item) + ",";
				}
				if (text2.Length > 0)
				{
					text2 = text2.Remove(text2.Length - 1);
				}
				objective = ((QuestManager.followed_quest.questType == 6 || QuestManager.followed_quest.questType == 5) ? "Capture" : "Hunt");
				if (QuestManager.followed_quest.questType == 17 || QuestManager.followed_quest.questType == 3)
				{
					objective = "Drop Items From";
				}
				QuestObjectiveLabel.Text = $"{objective}:{text2} on the map {QuestManager.target_location_map}";
				break;
			}
			case "Raid":
			{
				objective = "Finish Raid";
				string text = "";
				foreach (int item2 in QuestManager.mobs_to_hunt)
				{
					PictureBox pictureBox = new PictureBox
					{
						Image = (File.Exists($"images\\npcs\\{item2}.png") ? Image.FromFile($"images\\npcs\\{item2}.png") : null),
						Size = new Size(num, num),
						Margin = new Padding(0),
						Padding = new Padding(0),
						SizeMode = PictureBoxSizeMode.Zoom
					};
					ScaleControl(pictureBox);
					QuestObjectiveIconsFlowLayoutPanel.Controls.Add(pictureBox);
					text = text + " " + GameBoss.getBossName(item2) + ",";
				}
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1);
				}
				QuestObjectiveLabel.Text = objective + ":" + text;
				break;
			}
			}
		});
	}

	public void SetQuestSearchType(object sender, EventArgs e)
	{
		if (sender == null)
		{
			return;
		}
		NALabel nALabel = (NALabel)sender;
		foreach (Control control in form.QuestSearchTypesPanel.Controls)
		{
			control.ForeColor = NAStyles.CounterForeColor;
		}
		nALabel.ForeColor = NAStyles.NotActiveCharColor;
		QuestManager.quest_search_type = nALabel.Text;
		UpdateQuestSearchResults();
	}

	public void QuestSearchTypeLabel_MouseEnter(object sender, EventArgs e)
	{
		if (sender != null)
		{
			NALabel nALabel = (NALabel)sender;
			if (!(nALabel.ForeColor == NAStyles.NotActiveCharColor))
			{
				nALabel.ForeColor = NAStyles.PlayersColor;
			}
		}
	}

	public void QuestSearchTypeLabel_MouseLeave(object sender, EventArgs e)
	{
		if (sender != null)
		{
			NALabel nALabel = (NALabel)sender;
			if (!(nALabel.ForeColor == NAStyles.NotActiveCharColor))
			{
				nALabel.ForeColor = NAStyles.CounterForeColor;
			}
		}
	}

	private void QuestNavigateButton_Click(object sender, EventArgs e)
	{
		NAvigator.show_time_space_map = false;
		QuestManager.NavigateToNonQuest();
		ShowTimeSpaceMapButton.Hide();
	}

	private void UpdateQuestSearchResults()
	{
		Utils.InvokeIfRequired(form.QuestSearchResultsFlowLayoutPanel, delegate
		{
			form.QuestSearchResultsFlowLayoutPanel.Controls.Clear();
			string searched_phrase = form.QuestSearchTextBox.Text;
			if (searched_phrase == "")
			{
				return;
			}
			List<EntityDto> list = new List<EntityDto>();
			if (QuestManager.quest_search_type == "Mob")
			{
				list = NAvigator.world_monsters.Where((MobDto x) => x.name.ToLower().Contains(searched_phrase.ToLower())).Cast<EntityDto>().ToList();
			}
			else if (QuestManager.quest_search_type == "Map")
			{
				list = NAvigator.world_maps.Where((MapDto x) => x.name.ToLower().Contains(searched_phrase.ToLower())).Cast<EntityDto>().ToList();
			}
			else if (QuestManager.quest_search_type == "NPC")
			{
				list = NAvigator.world_NPCs.Where((NPCDto x) => x.name.ToLower().Contains(searched_phrase.ToLower())).Cast<EntityDto>().ToList();
			}
			else if (QuestManager.quest_search_type == "TS")
			{
				list = NAvigator.world_time_spaces.Where((TimeSpaceDto x) => x.name.ToLower().Contains(searched_phrase.ToLower())).Cast<EntityDto>().ToList();
			}
			int num = 0;
			foreach (EntityDto item in list)
			{
				NALabel nALabel = new NALabel
				{
					Size = new Size(275, 22),
					Font = new Font("Microsoft Sans Serif", 11f, FontStyle.Bold, GraphicsUnit.Point, 238),
					AutoSize = false,
					Text = item.name
				};
				if (item.name == QuestManager.quest_search_selected_item)
				{
					nALabel.ForeColor = NAStyles.NotActiveCharColor;
				}
				nALabel.MouseEnter += delegate(object? sender, EventArgs e)
				{
					if (sender != null)
					{
						NALabel nALabel3 = (NALabel)sender;
						if (nALabel3.ForeColor != NAStyles.NotActiveCharColor)
						{
							nALabel3.ForeColor = NAStyles.PlayersColor;
						}
					}
				};
				nALabel.MouseLeave += delegate(object? sender, EventArgs e)
				{
					if (sender != null)
					{
						NALabel nALabel2 = (NALabel)sender;
						if (nALabel2.ForeColor != NAStyles.NotActiveCharColor)
						{
							nALabel2.ForeColor = NAStyles.CounterForeColor;
						}
					}
				};
				nALabel.Click += delegate(object? sender, EventArgs e)
				{
					if (sender != null)
					{
						QuestSearchItemClicked(sender, e);
					}
				};
				ScaleControl(nALabel);
				form.QuestSearchResultsFlowLayoutPanel.Controls.Add(nALabel);
				num++;
				if (num == 20)
				{
					break;
				}
			}
		});
	}

	private void QuestSearchTextBox_TextChanged(object sender, EventArgs e)
	{
		UpdateQuestSearchResults();
	}

	private void QuestSearchItemClicked(object sender, EventArgs e)
	{
		foreach (Control control in QuestSearchResultsFlowLayoutPanel.Controls)
		{
			control.ForeColor = NAStyles.CounterForeColor;
		}
		if (sender == null)
		{
			return;
		}
		if (QuestManager.quest_search_type == "TS" && TimeSpaceManager.ts_started)
		{
			ShowPopUp("You can not do it while being in a Time-Space!");
			return;
		}
		NALabel obj = (NALabel)sender;
		obj.ForeColor = NAStyles.NotActiveCharColor;
		QuestManager.quest_search_selected_item = obj.Text;
		QuestManager.UnselectQuest();
		List<int> path_map_ids = new List<int>();
		Bitmap bitmap = null;
		if (Mapper == null)
		{
			return;
		}
		switch (QuestManager.quest_search_type)
		{
		case "NPC":
		{
			NPCDto nPCDto = NAvigator.world_NPCs.Find((NPCDto x) => x.name == QuestManager.quest_search_selected_item);
			if (nPCDto == null)
			{
				return;
			}
			path_map_ids = NAvigator.FindShortestPath(Mapper.real_map_id, nPCDto.mapId, "Map");
			Image instance_icon2 = (File.Exists($"images/npcs/{nPCDto.npcId}.png") ? Image.FromFile($"images/npcs/{nPCDto.npcId}.png") : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
			bitmap = NAStyles.GenerateSearchQuestInstanceMap(nPCDto.mapId, "NPC", nPCDto.x, nPCDto.y, instance_icon2, 0);
			QuestManager.navigating_instance_map_id = nPCDto.mapId;
			QuestManager.navigating_instance_id = nPCDto.npcId;
			QuestManager.navigating_instance_type = "NPC";
			ShowTimeSpaceMapButton.Hide();
			NAvigator.show_time_space_map = false;
			break;
		}
		case "Map":
		{
			MapDto mapDto = NAvigator.world_maps.Find((MapDto x) => x.name == QuestManager.quest_search_selected_item);
			if (mapDto == null)
			{
				return;
			}
			path_map_ids = NAvigator.FindShortestPath(Mapper.real_map_id, mapDto.mapId, "Map");
			bitmap = NAStyles.GenerateSearchQuestInstanceMap(mapDto.mapId, "Map", 0, 0, null, 0);
			QuestManager.navigating_instance_map_id = mapDto.mapId;
			QuestManager.navigating_instance_id = mapDto.mapId;
			QuestManager.navigating_instance_type = "Map";
			ShowTimeSpaceMapButton.Hide();
			NAvigator.show_time_space_map = false;
			break;
		}
		case "Mob":
		{
			MobDto mobDto = NAvigator.world_monsters.Find((MobDto x) => x.name == QuestManager.quest_search_selected_item);
			if (mobDto == null)
			{
				return;
			}
			path_map_ids = NAvigator.FindShortestPath(Mapper.real_map_id, mobDto.mapId, "Map");
			Image instance_icon3 = (File.Exists($"images/npcs/{mobDto.vnum}.png") ? Image.FromFile($"images/npcs/{mobDto.vnum}.png") : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
			bitmap = NAStyles.GenerateSearchQuestInstanceMap(mobDto.mapId, "Mob", 0, 0, instance_icon3, mobDto.mobCount);
			QuestManager.navigating_instance_map_id = mobDto.mapId;
			QuestManager.navigating_instance_id = mobDto.vnum;
			QuestManager.navigating_instance_type = "Mob";
			NAvigator.show_time_space_map = false;
			ShowTimeSpaceMapButton.Hide();
			break;
		}
		case "TS":
		{
			TimeSpaceDto timeSpaceDto = NAvigator.world_time_spaces.Find((TimeSpaceDto x) => x.name == QuestManager.quest_search_selected_item);
			if (timeSpaceDto == null)
			{
				return;
			}
			path_map_ids = NAvigator.FindShortestPath(Mapper.real_map_id, timeSpaceDto.mapId, "Map");
			Image instance_icon = (File.Exists("images/npcs/time_space.png") ? Image.FromFile("images/npcs/time_space.png") : (File.Exists("images/npcs/empty.png") ? Image.FromFile("images/npcs/empty.png") : null));
			bitmap = NAStyles.GenerateSearchQuestInstanceMap(timeSpaceDto.mapId, "TimeSpace", timeSpaceDto.x, timeSpaceDto.y, instance_icon, 0);
			QuestManager.navigating_instance_map_id = timeSpaceDto.mapId;
			QuestManager.navigating_instance_id = timeSpaceDto.timeSpaceId;
			QuestManager.navigating_instance_type = "TimeSpace";
			ShowTimeSpaceMapButton.Show();
			ShowTimeSpaceMapButton.BringToFront();
			break;
		}
		}
		NAvigator.show_time_space_map = false;
		ShowTimeSpaceMapButton.Image = Resources.info;
		if (bitmap != null)
		{
			UpdateQuestPath(path_map_ids);
			QuestManager.ShowQuestSearchInstanceMap = true;
			QuestManager.QuestSearchInstanceMap = bitmap;
			form.QuestsTabMap.Image = bitmap;
		}
	}

	private void ShowTimeSpaceMapButton_Click(object sender, EventArgs e)
	{
		if (QuestManager.navigating_instance_type != "TimeSpace" || QuestManager.navigating_instance_id == -1)
		{
			return;
		}
		NAvigator.show_time_space_map = !NAvigator.show_time_space_map;
		ShowTimeSpaceMapButton.Image = (NAvigator.show_time_space_map ? Resources.back_arrow : Resources.info);
		if (!NAvigator.show_time_space_map)
		{
			form.QuestsTabMap.Image = QuestManager.QuestSearchInstanceMap;
			return;
		}
		string text = $"images\\time_spaces\\{QuestManager.navigating_instance_id}.png";
		if (File.Exists(text))
		{
			NAvigator.time_space_map = new Bitmap(text);
			NAvigator.time_space_map_fresh_map = (Bitmap)NAvigator.time_space_map.Clone();
			form.QuestsTabMap.Image = NAvigator.time_space_map;
		}
	}

	private async void QuestsTabMap_MouseMove(object sender, MouseEventArgs e)
	{
		if (sender is PictureBox && !NAStyles.force_live_map_draw)
		{
			PictureBox map = (PictureBox)sender;
			TimeSpaceManager.HighlightTimeSpace(e.X, e.Y, map);
			if (NAvigator.show_time_space_map && NAvigator.time_space_map_fresh_map != null)
			{
				NAvigator.time_space_map = await NAStyles.DrawTSDataMap((Bitmap)NAvigator.time_space_map_fresh_map.Clone(), QuestManager.navigating_instance_id);
				QuestsTabMap.Image = NAvigator.time_space_map;
			}
		}
	}

	private void map_picture_MouseMove(object sender, MouseEventArgs e)
	{
		if (sender is PictureBox && !NAStyles.force_live_map_draw)
		{
			PictureBox map = (PictureBox)sender;
			TimeSpaceManager.HighlightTimeSpace(e.X, e.Y, map);
		}
	}

	private void StopNavigatingButton_Click(object sender, EventArgs e)
	{
		QuestManager.StopNavigating();
		QuestSearchTextBox.Text = "";
		ShowTimeSpaceMapButton.Hide();
	}

	public static void UpdateSwitchMapModeButtonVisibility()
	{
		Utils.InvokeIfRequired(form.SwitchMapModeButton, delegate
		{
			form.SwitchMapModeButton.Visible = TimeSpaceManager.ts_started;
			form.SwitchMapModeButton.BringToFront();
			form.SwitchMapModeQuestButton.Visible = TimeSpaceManager.ts_started;
			form.SwitchMapModeQuestButton.BringToFront();
		});
	}

	private void SwitchMapModeButton_Click(object sender, EventArgs e)
	{
		NAStyles.force_live_map_draw = !NAStyles.force_live_map_draw;
		ForceMapRefresh();
	}

	private void SwitchMapModeButton_MouseEnter(object sender, EventArgs e)
	{
		SwitchMapModeButton.Image = Resources.switch_icon_hover;
	}

	private void SwitchMapModeButton_MouseLeave(object sender, EventArgs e)
	{
		SwitchMapModeButton.Image = Resources.switch_icon;
	}

	private void ShowTimeSpaceMapButton_MouseEnter(object sender, EventArgs e)
	{
		ShowTimeSpaceMapButton.Image = (NAvigator.show_time_space_map ? Resources.back_arrow_hover : Resources.info_hover);
	}

	private void ShowTimeSpaceMapButton_MouseLeave(object sender, EventArgs e)
	{
		ShowTimeSpaceMapButton.Image = (NAvigator.show_time_space_map ? Resources.back_arrow : Resources.info);
	}

	private void SwitchMapModeQuestButton_Click(object sender, EventArgs e)
	{
		NAStyles.force_live_map_draw = !NAStyles.force_live_map_draw;
		ForceMapRefresh();
	}

	private void SwitchMapModeQuestButton_MouseEnter(object sender, EventArgs e)
	{
		SwitchMapModeQuestButton.Image = Resources.switch_icon_hover;
	}

	private void SwitchMapModeQuestButton_MouseLeave(object sender, EventArgs e)
	{
		SwitchMapModeQuestButton.Image = Resources.switch_icon;
	}

	public static void ForceMapRefresh()
	{
		form.timer_map_tick_Tick(null, null);
	}

	private void OpenMapInNewWindowMapTabButton_Click(object sender, EventArgs e)
	{
		if (RaidModeForm != null)
		{
			RaidModeForm.OpenMiniMapInNewWindow();
		}
	}

	private void OpenMapInNewWindowMapTabButton_MouseEnter(object sender, EventArgs e)
	{
		OpenMapInNewWindowMapTabButton.Image = Resources.open_in_new_window_hover;
	}

	private void OpenMapInNewWindowMapTabButton_MouseLeave(object sender, EventArgs e)
	{
		OpenMapInNewWindowMapTabButton.Image = Resources.open_in_new_window;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NosAssistant2.GUI));
		this.packetListener = new System.ComponentModel.BackgroundWorker();
		this.connectionsUpdater = new System.ComponentModel.BackgroundWorker();
		this.packetHandler = new System.ComponentModel.BackgroundWorker();
		this.timer_map_tick = new System.Windows.Forms.Timer(this.components);
		this.SideMenu = new System.Windows.Forms.Panel();
		this.JoinDiscordButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.ExitButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.SwitchToSettingsButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.SwitchToPacketLoggerButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.SwitchToQuestsButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.SwitchToAnalyticsButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.SwitchToMLButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.SwitchToCounterButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.SwitchToRaidsButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.SwitchToMapButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.SwitchToControlPanelButton = new NosAssistant2.GUIElements.NAMenuButton();
		this.LogoPanel = new System.Windows.Forms.Panel();
		this.CountDownPanel = new System.Windows.Forms.Panel();
		this.CountDownLabel = new NosAssistant2.GUIElements.NALabel();
		this.VersionLabel = new System.Windows.Forms.Label();
		this.accessLabel = new System.Windows.Forms.Label();
		this.WelcomeUserLabel = new System.Windows.Forms.Label();
		this.LogoPicture = new System.Windows.Forms.PictureBox();
		this.MapBottomPanel = new System.Windows.Forms.Panel();
		this.MapMobFilterButton = new NosAssistant2.GUIElements.NAButton();
		this.ServerLabel = new System.Windows.Forms.Label();
		this.ChannelLabel = new System.Windows.Forms.Label();
		this.MapShowEntitiesLabel = new NosAssistant2.GUIElements.NALabel();
		this.MapShowEntitiesCheckBox = new NosAssistant2.GUIElements.NACheckbox();
		this.MapShowMobsLabel = new NosAssistant2.GUIElements.NALabel();
		this.MapShowMobsCheckBox = new NosAssistant2.GUIElements.NACheckbox();
		this.MapShowPlayersLabel = new NosAssistant2.GUIElements.NALabel();
		this.MapShowPlayersCheckBox = new NosAssistant2.GUIElements.NACheckbox();
		this.MapShowPetsLabel = new NosAssistant2.GUIElements.NALabel();
		this.MapShowPetsCheckBox = new NosAssistant2.GUIElements.NACheckbox();
		this.MapShowAltsLabel = new NosAssistant2.GUIElements.NALabel();
		this.MapShowAltsCheckBox = new NosAssistant2.GUIElements.NACheckbox();
		this.MapShowMapperLabel = new NosAssistant2.GUIElements.NALabel();
		this.MapShowMapperCheckBox = new NosAssistant2.GUIElements.NACheckbox();
		this.RandomRangeTextBox = new System.Windows.Forms.TextBox();
		this.RandomRangeLabel = new System.Windows.Forms.Label();
		this.MapPanelMapperLabel = new System.Windows.Forms.Label();
		this.MapLabel = new System.Windows.Forms.Label();
		this.MapYLabel = new System.Windows.Forms.Label();
		this.MapXLabel = new System.Windows.Forms.Label();
		this.MainControlPanelLabel = new System.Windows.Forms.Label();
		this.MainMapPanelLabel = new System.Windows.Forms.Label();
		this.MainRaidsPanelLabel = new System.Windows.Forms.Label();
		this.MainMinilandPanelLabel = new System.Windows.Forms.Label();
		this.MainPacketLoggerPanelLabel = new System.Windows.Forms.Label();
		this.MainSettingsPanelLabel = new System.Windows.Forms.Label();
		this.ControlPanelPanel = new NosAssistant2.GUIElements.DoubleBufferedPanel();
		this.DisableHeaderLabel = new NosAssistant2.GUIElements.NALabel();
		this.OtherHeaderLabel = new NosAssistant2.GUIElements.NALabel();
		this.BufferHeaderLabel = new NosAssistant2.GUIElements.NALabel();
		this.AutofullHeaderLabel = new NosAssistant2.GUIElements.NALabel();
		this.MoverHeaderLabel = new NosAssistant2.GUIElements.NALabel();
		this.RaiderHeaderLabel = new NosAssistant2.GUIElements.NALabel();
		this.AttackerHeaderLabel = new NosAssistant2.GUIElements.NALabel();
		this.NicknameHeaderLabel = new NosAssistant2.GUIElements.NALabel();
		this.ControlPanelScrollbar = new NosAssistant2.GUIElements.NAScrollBar();
		this.flowLayoutCharactersPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.InjectDLLButton = new NosAssistant2.GUIElements.NAButton();
		this.change_map_label = new System.Windows.Forms.Label();
		this.map_picture = new System.Windows.Forms.PictureBox();
		this.MapPanel = new System.Windows.Forms.Panel();
		this.OpenMapInNewWindowMapTabButton = new System.Windows.Forms.PictureBox();
		this.SwitchMapModeButton = new System.Windows.Forms.PictureBox();
		this.RaidsPanel = new System.Windows.Forms.Panel();
		this.RaidsNotificationsButton = new NosAssistant2.GUIElements.NAButton();
		this.OpenBoxesLabel = new System.Windows.Forms.Label();
		this.OpenBoxesButton = new NosAssistant2.GUIElements.NAButton();
		this.LimitFPSButton = new NosAssistant2.GUIElements.NAButton();
		this.FPSLabel = new System.Windows.Forms.Label();
		this.FPSTextBox = new System.Windows.Forms.TextBox();
		this.ResizeWidthLabel = new System.Windows.Forms.Label();
		this.ResizeHeightLabel = new System.Windows.Forms.Label();
		this.ResizeHeightTextBox = new System.Windows.Forms.TextBox();
		this.ResizeWidthTextBox = new System.Windows.Forms.TextBox();
		this.ResizeWindowsButton = new NosAssistant2.GUIElements.NAButton();
		this.SaveWindowsButton = new NosAssistant2.GUIElements.NAButton();
		this.LoadWindowsButton = new NosAssistant2.GUIElements.NAButton();
		this.useDebuffsButton = new NosAssistant2.GUIElements.NAButton();
		this.MimicKeyboardLabel = new System.Windows.Forms.Label();
		this.MimicMouseLabel = new System.Windows.Forms.Label();
		this.MimicMouseButton = new NosAssistant2.GUIElements.NAButton();
		this.MimicKeyboardButton = new NosAssistant2.GUIElements.NAButton();
		this.UseSelfBuffsRaidsButton = new NosAssistant2.GUIElements.NAButton();
		this.UseBuffsRaidsButton = new NosAssistant2.GUIElements.NAButton();
		this.RaidersPanelScrollbar = new NosAssistant2.GUIElements.NAScrollBar();
		this.BuffsetsButton = new NosAssistant2.GUIElements.NAButton();
		this.leaveRaidButton = new NosAssistant2.GUIElements.NAButton();
		this.stackWindowsButton = new NosAssistant2.GUIElements.NAButton();
		this.waterfallButton = new NosAssistant2.GUIElements.NAButton();
		this.changeSPButton = new NosAssistant2.GUIElements.NAButton();
		this.RaidersHPStatusPanel = new NosAssistant2.GUIElements.DoubleBufferedPanel();
		this.RefreshRaidsBarsLabel = new NosAssistant2.GUIElements.NALabel();
		this.SwitchRaidersTabPanel = new System.Windows.Forms.PictureBox();
		this.RaidsBarsStatusFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.flowLayoutRaidsPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.RaidHostInfoLabel = new System.Windows.Forms.Label();
		this.AutoFullThresholdLabel = new System.Windows.Forms.Label();
		this.AutofullThresholdTrackbar = new System.Windows.Forms.TrackBar();
		this.HostNameTextBox = new System.Windows.Forms.TextBox();
		this.AutoJoinToggleLabel = new System.Windows.Forms.Label();
		this.AutoJoinToggleButton = new NosAssistant2.GUIElements.NAButton();
		this.JoinListButton = new NosAssistant2.GUIElements.NAButton();
		this.AutoFullToggleLabel = new System.Windows.Forms.Label();
		this.AutoFullToggleButton = new NosAssistant2.GUIElements.NAButton();
		this.MinilandPanel = new System.Windows.Forms.Panel();
		this.PetTrainerFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.panel1 = new System.Windows.Forms.Panel();
		this.PetTrainerIcon1 = new System.Windows.Forms.PictureBox();
		this.PetTrainerTimerLabel1 = new NosAssistant2.GUIElements.NALabel();
		this.panel3 = new System.Windows.Forms.Panel();
		this.PetTrainerIcon2 = new System.Windows.Forms.PictureBox();
		this.PetTrainerTimerLabel2 = new NosAssistant2.GUIElements.NALabel();
		this.panel4 = new System.Windows.Forms.Panel();
		this.PetTrainerIcon3 = new System.Windows.Forms.PictureBox();
		this.PetTrainerTimerLabel3 = new NosAssistant2.GUIElements.NALabel();
		this.panel5 = new System.Windows.Forms.Panel();
		this.TrainedPetIcon1 = new System.Windows.Forms.PictureBox();
		this.TrainedPetInfoLabel1 = new NosAssistant2.GUIElements.NALabel();
		this.panel6 = new System.Windows.Forms.Panel();
		this.TrainedPetIcon2 = new System.Windows.Forms.PictureBox();
		this.TrainedPetInfoLabel2 = new NosAssistant2.GUIElements.NALabel();
		this.panel7 = new System.Windows.Forms.Panel();
		this.TrainedPetIcon3 = new System.Windows.Forms.PictureBox();
		this.TrainedPetInfoLabel3 = new NosAssistant2.GUIElements.NALabel();
		this.AutoRespawnLabel = new System.Windows.Forms.Label();
		this.AutoRespawnButton = new NosAssistant2.GUIElements.NAButton();
		this.OpenDmgContributionWindow = new NosAssistant2.GUIElements.NAButton();
		this.NewNicknameToInviteListTextBox = new NosAssistant2.GUIElements.NATextBox();
		this.RemoveFromInviteListButton = new NosAssistant2.GUIElements.NAButton();
		this.AddToInviteListButton = new NosAssistant2.GUIElements.NAButton();
		this.InviteListPanel = new System.Windows.Forms.Panel();
		this.InviteListDataGrid = new NosAssistant2.GUIElements.CounterDataGrid();
		this.nickname = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.inviteListBindingSource = new System.Windows.Forms.BindingSource(this.components);
		this.useSelfBuffsButton = new NosAssistant2.GUIElements.NAButton();
		this.autoConfirmLabel = new System.Windows.Forms.Label();
		this.autoConfirmToggleButton = new NosAssistant2.GUIElements.NAButton();
		this.prepareMLButton = new NosAssistant2.GUIElements.NAButton();
		this.inviteToMLButton = new NosAssistant2.GUIElements.NAButton();
		this.MLTabInviterLabel = new System.Windows.Forms.Label();
		this.useBuffsButton = new NosAssistant2.GUIElements.NAButton();
		this.SettingsPanel = new System.Windows.Forms.Panel();
		this.SettingsWaypointsMenuButton = new NosAssistant2.GUIElements.NAButton();
		this.BuyLicenseButton = new NosAssistant2.GUIElements.NAButton();
		this.ChangeNicknameButton = new NosAssistant2.GUIElements.NAButton();
		this.ResetArcaneWisdomButton = new System.Windows.Forms.PictureBox();
		this.editArcaneWisdomControlPictureBox = new System.Windows.Forms.PictureBox();
		this.arcaneWisdomLabel = new System.Windows.Forms.Label();
		this.arcaneWisdomControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.SettingsSoundsMenuButton = new NosAssistant2.GUIElements.NAButton();
		this.ResetUseDebuffsButton = new System.Windows.Forms.PictureBox();
		this.editUseDebuffsControlPictureBox = new System.Windows.Forms.PictureBox();
		this.useDebuffsSettingsLabel = new System.Windows.Forms.Label();
		this.useDebuffsControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.SettingsTooltipsButton = new System.Windows.Forms.Button();
		this.TooltipsSettingsLabel = new System.Windows.Forms.Label();
		this.SettingsLowSpecButton = new System.Windows.Forms.Button();
		this.LowSpecSettingsLabel = new System.Windows.Forms.Label();
		this.ResetUseBuffset3Button = new System.Windows.Forms.PictureBox();
		this.editUseBuffset3ControlPictureBox = new System.Windows.Forms.PictureBox();
		this.useBuffset3SettingsLabel = new System.Windows.Forms.Label();
		this.useBuffset3ControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.ResetUseBuffset2Button = new System.Windows.Forms.PictureBox();
		this.editUseBuffset2ControlPictureBox = new System.Windows.Forms.PictureBox();
		this.useBuffset2SettingsLabel = new System.Windows.Forms.Label();
		this.useBuffset2ControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.ResetUseBuffset1Button = new System.Windows.Forms.PictureBox();
		this.editUseBuffset1ControlPictureBox = new System.Windows.Forms.PictureBox();
		this.useBuffset1SettingsLabel = new System.Windows.Forms.Label();
		this.useBuffset1ControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.SettingsDelayItemsTextBox = new System.Windows.Forms.TextBox();
		this.SettingsDelayItemsLabel = new System.Windows.Forms.Label();
		this.ResetUseSelfBuffsButton = new System.Windows.Forms.PictureBox();
		this.ResetWearSPButton = new System.Windows.Forms.PictureBox();
		this.ResetMassHealButton = new System.Windows.Forms.PictureBox();
		this.ResetExitRaidButton = new System.Windows.Forms.PictureBox();
		this.ResetJoinListButton = new System.Windows.Forms.PictureBox();
		this.ResetInviteButton = new System.Windows.Forms.PictureBox();
		this.ResetUseBuffsButton = new System.Windows.Forms.PictureBox();
		this.NetworkDeviceCombobox = new NosAssistant2.GUIElements.NAComboBox();
		this.NetworkDeviceLabel = new System.Windows.Forms.Label();
		this.SettingsSoundsButton = new System.Windows.Forms.Button();
		this.SoundsSettingsLabel = new System.Windows.Forms.Label();
		this.WindowSizeComboBox = new NosAssistant2.GUIElements.NAComboBox();
		this.WindowSizeLabel = new System.Windows.Forms.Label();
		this.DefaultSettingsButton = new NosAssistant2.GUIElements.NAButton();
		this.editExitRaidControlPictureBox = new System.Windows.Forms.PictureBox();
		this.ExitRaidSettingsLabel = new System.Windows.Forms.Label();
		this.ExitRaidControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.SettingsHotkeysButton = new System.Windows.Forms.Button();
		this.HotkeysSettingLabel = new System.Windows.Forms.Label();
		this.editJoinListControlPictureBox = new System.Windows.Forms.PictureBox();
		this.JoinListSettingsLabel = new System.Windows.Forms.Label();
		this.JoinListControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.editUseSelfBuffsControlPictureBox = new System.Windows.Forms.PictureBox();
		this.useSelfBuffsSettingsLabel = new System.Windows.Forms.Label();
		this.editWearSPControlPictureBox = new System.Windows.Forms.PictureBox();
		this.wearSPSettingsLabel = new System.Windows.Forms.Label();
		this.editMassHealControlPictureBox = new System.Windows.Forms.PictureBox();
		this.MassHealSettingsLabel = new System.Windows.Forms.Label();
		this.editInviteControlPictureBox = new System.Windows.Forms.PictureBox();
		this.InviteSettingsLabel = new System.Windows.Forms.Label();
		this.editUseBufssControlPictureBox = new System.Windows.Forms.PictureBox();
		this.InviteControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.MassHealControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.WearSPControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.useSelfBuffsControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.useBuffsControlLabel = new NosAssistant2.GUIElements.NALabel();
		this.useBuffsSettingsLabel = new System.Windows.Forms.Label();
		this.SettingsDelayInviteTextBox = new System.Windows.Forms.TextBox();
		this.SettingsDelayInviteLabel = new System.Windows.Forms.Label();
		this.SettingsDelayMoveTextBox = new System.Windows.Forms.TextBox();
		this.SettingsDelayMoveLabel = new System.Windows.Forms.Label();
		this.SettingsDelayRaidTextBox = new System.Windows.Forms.TextBox();
		this.SettingsDelayRaidLabel = new System.Windows.Forms.Label();
		this.SettingsRenameButton = new System.Windows.Forms.Button();
		this.RenameSettingLabel = new System.Windows.Forms.Label();
		this.SettingsDelayBuffTextBox = new System.Windows.Forms.TextBox();
		this.AddLicenseButton = new NosAssistant2.GUIElements.NAButton();
		this.SettingsDelayBuffLabel = new System.Windows.Forms.Label();
		this.BrowseDLLLabel = new System.Windows.Forms.Label();
		this.DLLFileLabel = new System.Windows.Forms.Label();
		this.InviterButton = new NosAssistant2.GUIElements.NAButton();
		this.MapperButton = new NosAssistant2.GUIElements.NAButton();
		this.InviterLabel = new System.Windows.Forms.Label();
		this.MapperLabel = new System.Windows.Forms.Label();
		this.ControlPanelBottomPanel = new System.Windows.Forms.Panel();
		this.AccounstCountLabel = new NosAssistant2.GUIElements.NALabel();
		this.SaveConfigButton = new NosAssistant2.GUIElements.NAButton();
		this.LoadConfigButton = new NosAssistant2.GUIElements.NAButton();
		this.closeAltsButton = new NosAssistant2.GUIElements.NAButton();
		this.resetTitlesButton = new NosAssistant2.GUIElements.NAButton();
		this.CloseButton = new System.Windows.Forms.Button();
		this.MinimizeButton = new System.Windows.Forms.Button();
		this.PacketLoggerPanel = new System.Windows.Forms.Panel();
		this.PacketLoggerScrollbar = new NosAssistant2.GUIElements.NAScrollBar();
		this.PacketsConsole = new NosAssistant2.GUIElements.ConsoleWindow();
		this.PacketLoggerBottomPanel = new System.Windows.Forms.Panel();
		this.OpenPacketFiltersButton = new NosAssistant2.GUIElements.NAButton();
		this.PacketLoggerPrintSentButton = new NosAssistant2.GUIElements.NAButton();
		this.PacketLogerPrintSentStatusLabel = new System.Windows.Forms.Label();
		this.ClearPacketLoggerButton = new NosAssistant2.GUIElements.NAButton();
		this.PacketLoggerPrintRecvButton = new NosAssistant2.GUIElements.NAButton();
		this.PacketLogerPrintRecvStatusLabel = new System.Windows.Forms.Label();
		this.MainControlPanel = new System.Windows.Forms.Panel();
		this.MainMapPanel = new System.Windows.Forms.Panel();
		this.MainRaidsPanel = new System.Windows.Forms.Panel();
		this.MainMinilandPanel = new System.Windows.Forms.Panel();
		this.MainPacketLoggerPanel = new System.Windows.Forms.Panel();
		this.MainSettingsPanel = new System.Windows.Forms.Panel();
		this.LicesneExpirationDateLabel = new NosAssistant2.GUIElements.NALabel();
		this.MainNoAccessPanel = new System.Windows.Forms.Panel();
		this.NoAccessPanelMainLabel = new System.Windows.Forms.Label();
		this.NoAccessPanel = new System.Windows.Forms.Panel();
		this.ChangeNicknameLabel = new NosAssistant2.GUIElements.NALabel();
		this.BuyLicenseLabel = new NosAssistant2.GUIElements.NALabel();
		this.ForgotLicenseLabel = new NosAssistant2.GUIElements.NALabel();
		this.NoAccessLicenseConfirmButton = new NosAssistant2.GUIElements.NAButton();
		this.NoAccessInfoLabel = new System.Windows.Forms.Label();
		this.NoAccessLicenseTextBox = new System.Windows.Forms.TextBox();
		this.popUpPanel = new NosAssistant2.GUIElements.PopUpPanel();
		this.LoadingScreenPanel = new System.Windows.Forms.Panel();
		this.panel2 = new System.Windows.Forms.Panel();
		this.LoadingScreenBar = new NosAssistant2.GUIElements.NAProgressBar();
		this.LoadingLabel = new NosAssistant2.GUIElements.NALabel();
		this.HideMenuButton = new System.Windows.Forms.PictureBox();
		this.RaidsHistoryPanel = new System.Windows.Forms.Panel();
		this.AnalyticsPlayersTab = new System.Windows.Forms.Panel();
		this.PlayerRaidsStatisticsPanel = new System.Windows.Forms.Panel();
		this.PlayersRaidsStatisticsLoadingLabel = new NosAssistant2.GUIElements.NALabel();
		this.PlayerRaidsStatisticsBossNameLabel = new NosAssistant2.GUIElements.NALabel();
		this.PlayerRaidsStatisticsBossIcon = new System.Windows.Forms.PictureBox();
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel = new NosAssistant2.GUIElements.NALabel();
		this.PlayerRaidsStatisticsBestTimeRankLabel = new NosAssistant2.GUIElements.NALabel();
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel = new NosAssistant2.GUIElements.NALabel();
		this.PlayerRaidsStatisticsTotalMaxHitLabel = new NosAssistant2.GUIElements.NALabel();
		this.PlayerRaidsStatisticsBestTimeLabel = new NosAssistant2.GUIElements.NALabel();
		this.PlayerRaidsStatisticsRaidsFinishedLabel = new NosAssistant2.GUIElements.NALabel();
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel = new System.Windows.Forms.TableLayoutPanel();
		this.PlayerEquipementButton = new NosAssistant2.GUIElements.NAButton();
		this.PlayerRaidsSelectionFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.SPDetailsBorderPanel = new System.Windows.Forms.Panel();
		this.SPDetailsPanel = new System.Windows.Forms.Panel();
		this.CloseSPDetailsButton = new System.Windows.Forms.PictureBox();
		this.SPDetailsShadowLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsShadowImage = new System.Windows.Forms.PictureBox();
		this.SPDetailsLightLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsLightImage = new System.Windows.Forms.PictureBox();
		this.SPDetailsWaterLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsWaterImage = new System.Windows.Forms.PictureBox();
		this.SPDetailsFireLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsFireImage = new System.Windows.Forms.PictureBox();
		this.SPDetailsEnergyLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsEnergyImage = new System.Windows.Forms.PictureBox();
		this.SPDetailsPropertyLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsPropertyImage = new System.Windows.Forms.PictureBox();
		this.SPDetailsDefenceLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsDefenceImage = new System.Windows.Forms.PictureBox();
		this.SPDetailsAttackLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsAttackImage = new System.Windows.Forms.PictureBox();
		this.SPDetailsPerfectionLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsJobLabel = new NosAssistant2.GUIElements.NALabel();
		this.SPDetailsAvatar = new System.Windows.Forms.PictureBox();
		this.Tattoo2UpgradeLabel = new NosAssistant2.GUIElements.NALabel();
		this.Tattoo2Icon = new System.Windows.Forms.PictureBox();
		this.Tattoo1UpgradeLabel = new NosAssistant2.GUIElements.NALabel();
		this.Tattoo1Icon = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerSPsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.ShellInfoMainPanel = new System.Windows.Forms.Panel();
		this.RuneLevelLabel = new NosAssistant2.GUIElements.NALabel();
		this.ShellEffectsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.SwitchToShellButton = new System.Windows.Forms.PictureBox();
		this.SwitchToRuneButton = new System.Windows.Forms.PictureBox();
		this.SwitchShellTypeButton = new System.Windows.Forms.PictureBox();
		this.ShellItemTypeLabel = new NosAssistant2.GUIElements.NALabel();
		this.SearchedPlayerTitle = new NosAssistant2.GUIElements.NALabel();
		this.SearchedPlayerFamilyRole = new NosAssistant2.GUIElements.NALabel();
		this.SearchedPlayerLastUpdateDateLabel = new NosAssistant2.GUIElements.NALabel();
		this.SearchedPlayerLastUpdateLabel = new NosAssistant2.GUIElements.NALabel();
		this.SearchedPlayerFairiesFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.Wings = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerWingsLabel = new NosAssistant2.GUIElements.NALabel();
		this.WeaponSkin = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerWeaponSkinLabel = new NosAssistant2.GUIElements.NALabel();
		this.CostumeHat = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerCostumeHatLabel = new NosAssistant2.GUIElements.NALabel();
		this.Costume = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerCostumeLabel = new NosAssistant2.GUIElements.NALabel();
		this.FlyingPet = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerFlyingPetLabel = new NosAssistant2.GUIElements.NALabel();
		this.Mask = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerMaskLabel = new NosAssistant2.GUIElements.NALabel();
		this.Hat = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerHatLabel = new NosAssistant2.GUIElements.NALabel();
		this.ArmorUpgrade = new NosAssistant2.GUIElements.NALabel();
		this.Armor = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerArmorLabel = new NosAssistant2.GUIElements.NALabel();
		this.SecondaryWeaponUpgrade = new NosAssistant2.GUIElements.NALabel();
		this.SecondaryWeapon = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerSecondWeaponLabel = new NosAssistant2.GUIElements.NALabel();
		this.MainWeaponUpgrade = new NosAssistant2.GUIElements.NALabel();
		this.MainWeapon = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerMainWeaponLabel = new NosAssistant2.GUIElements.NALabel();
		this.Reputation = new System.Windows.Forms.PictureBox();
		this.SearchedPlayerLVLCLVL = new NosAssistant2.GUIElements.NALabel();
		this.SearchedPlayerClassSex = new NosAssistant2.GUIElements.NALabel();
		this.SearchedPlayerFamily = new NosAssistant2.GUIElements.NALabel();
		this.SearchedPlayerNickname = new NosAssistant2.GUIElements.NALabel();
		this.SearchedPlayerAvatar = new System.Windows.Forms.PictureBox();
		this.SearchPlayerButton = new NosAssistant2.GUIElements.NAButton();
		this.SearchServerComboBox = new NosAssistant2.GUIElements.NAComboBox();
		this.SearchNicknameTextBox = new NosAssistant2.GUIElements.NATextBox();
		this.MainFairyDetailsPanel = new System.Windows.Forms.Panel();
		this.FairyDetailsPanel = new System.Windows.Forms.Panel();
		this.FairyUpgradePercentLabel = new NosAssistant2.GUIElements.NALabel();
		this.FairyEffectsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.FairyDetailsIcon = new System.Windows.Forms.PictureBox();
		this.CloseFairyDetailsButton = new System.Windows.Forms.PictureBox();
		this.FairyDetailsLabel = new NosAssistant2.GUIElements.NALabel();
		this.PlayerRaidsStatisticsButton = new NosAssistant2.GUIElements.NAButton();
		this.RaidsHistoryBestTimeLabel = new NosAssistant2.GUIElements.NALabel();
		this.RaidsHistoryAverageTimeLabel = new NosAssistant2.GUIElements.NALabel();
		this.RankingTabLabel = new NosAssistant2.GUIElements.NALabel();
		this.ShowMarathonTotalButton = new NosAssistant2.GUIElements.NAButton();
		this.AnalyticsBackArrow = new System.Windows.Forms.PictureBox();
		this.PlayersTabLabel = new NosAssistant2.GUIElements.NALabel();
		this.FamRecordsNextPageButton = new System.Windows.Forms.PictureBox();
		this.FamRecordsPreviousPageButton = new System.Windows.Forms.PictureBox();
		this.FamRecordsTabLabel = new NosAssistant2.GUIElements.NALabel();
		this.RaidsHistoryTabLabel = new NosAssistant2.GUIElements.NALabel();
		this.BackArrowRaidsHistory = new System.Windows.Forms.PictureBox();
		this.RaidsHistoryFilterTextBox = new NosAssistant2.GUIElements.NATextBox();
		this.RaidsHistoryPageLabel = new NosAssistant2.GUIElements.NALabel();
		this.RaidsHistoryNextPageButton = new System.Windows.Forms.PictureBox();
		this.RaidsHistoryPreviousPageButton = new System.Windows.Forms.PictureBox();
		this.RaidsHistoryDoubleBufferedPanel = new NosAssistant2.GUIElements.DoubleBufferedPanel();
		this.RaidsHistoryDetailsPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.RaidsHistoryListViewBorder = new System.Windows.Forms.Panel();
		this.RaidsHistoryDetailsGridView = new NosAssistant2.GUIElements.CounterDataGrid();
		this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.CharacterID = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Lp = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.PlayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.CLvl = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Family = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.MaxHit = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.MaxHitIcon = new System.Windows.Forms.DataGridViewImageColumn();
		this.Pets = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Special = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.MobDmg = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.OnyxDmg = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.All = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Gold = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Average = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Hit = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Miss = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Crit = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Bon = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.BonCrit = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Dbf = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Dead = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.MBHit = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.AllHits = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.AllMiss = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.RaidsHistoryFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.RaidsHistoryLabel = new NosAssistant2.GUIElements.NALabel();
		this.RaidsHistoryBackButton = new NosAssistant2.GUIElements.NAButton();
		this.RaidsHistorySelectedRaidPlayersFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.RankingTabPanel = new System.Windows.Forms.Panel();
		this.RankingRaidsDoneLabel = new NosAssistant2.GUIElements.NALabel();
		this.RankingBestTimesLabel = new NosAssistant2.GUIElements.NALabel();
		this.RankingMaxHitsLabel = new NosAssistant2.GUIElements.NALabel();
		this.RankingAverageDMGLabel = new NosAssistant2.GUIElements.NALabel();
		this.RankingDoubleBufferedPanel = new NosAssistant2.GUIElements.DoubleBufferedPanel();
		this.RankingFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.RankingPreviousPageButton = new System.Windows.Forms.PictureBox();
		this.RankingNextPageButton = new System.Windows.Forms.PictureBox();
		this.RankingSearchButton = new NosAssistant2.GUIElements.NAButton();
		this.RankingModeLabel = new NosAssistant2.GUIElements.NALabel();
		this.RankingModeButton = new NosAssistant2.GUIElements.NAButton();
		this.ClearRankingFiltersButton = new NosAssistant2.GUIElements.NAButton();
		this.RankingSPFilterFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.RankingRaidTypeFilterFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.RankingPageLabel = new NosAssistant2.GUIElements.NALabel();
		this.FamRecordsPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.MainQuestsPanel = new System.Windows.Forms.Panel();
		this.QuestPathLabel = new NosAssistant2.GUIElements.NALabel();
		this.QuestsPanel = new System.Windows.Forms.Panel();
		this.SwitchMapModeQuestButton = new System.Windows.Forms.PictureBox();
		this.StopNavigatingButton = new NosAssistant2.GUIElements.NAButton();
		this.ShowTimeSpaceMapButton = new System.Windows.Forms.PictureBox();
		this.QuestSearchResultsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.QuestNavigateButton = new NosAssistant2.GUIElements.NAButton();
		this.QuestSearchTypesPanel = new System.Windows.Forms.Panel();
		this.QuestTSTypeLabel = new NosAssistant2.GUIElements.NALabel();
		this.QuestMapTypeLabel = new NosAssistant2.GUIElements.NALabel();
		this.QuestMobTypeLabel = new NosAssistant2.GUIElements.NALabel();
		this.QuestNPCTypeLabel = new NosAssistant2.GUIElements.NALabel();
		this.QuestSearchTextBox = new NosAssistant2.GUIElements.NATextBox();
		this.QuestObjectiveIconsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.QuestObjectiveLabel = new NosAssistant2.GUIElements.NALabel();
		this.QuestsTabMap = new System.Windows.Forms.PictureBox();
		this.QuestsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.QuestsLabel = new System.Windows.Forms.Label();
		this.SideMenu.SuspendLayout();
		this.LogoPanel.SuspendLayout();
		this.CountDownPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.LogoPicture).BeginInit();
		this.MapBottomPanel.SuspendLayout();
		this.ControlPanelPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.map_picture).BeginInit();
		this.MapPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.OpenMapInNewWindowMapTabButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SwitchMapModeButton).BeginInit();
		this.RaidsPanel.SuspendLayout();
		this.RaidersHPStatusPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.SwitchRaidersTabPanel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AutofullThresholdTrackbar).BeginInit();
		this.MinilandPanel.SuspendLayout();
		this.PetTrainerFlowLayoutPanel.SuspendLayout();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.PetTrainerIcon1).BeginInit();
		this.panel3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.PetTrainerIcon2).BeginInit();
		this.panel4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.PetTrainerIcon3).BeginInit();
		this.panel5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.TrainedPetIcon1).BeginInit();
		this.panel6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.TrainedPetIcon2).BeginInit();
		this.panel7.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.TrainedPetIcon3).BeginInit();
		this.InviteListPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.InviteListDataGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inviteListBindingSource).BeginInit();
		this.SettingsPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ResetArcaneWisdomButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editArcaneWisdomControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseDebuffsButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editUseDebuffsControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseBuffset3Button).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editUseBuffset3ControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseBuffset2Button).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editUseBuffset2ControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseBuffset1Button).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editUseBuffset1ControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseSelfBuffsButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetWearSPButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetMassHealButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetExitRaidButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetJoinListButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetInviteButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseBuffsButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editExitRaidControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editJoinListControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editUseSelfBuffsControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editWearSPControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editMassHealControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editInviteControlPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.editUseBufssControlPictureBox).BeginInit();
		this.ControlPanelBottomPanel.SuspendLayout();
		this.PacketLoggerPanel.SuspendLayout();
		this.PacketLoggerBottomPanel.SuspendLayout();
		this.MainControlPanel.SuspendLayout();
		this.MainMapPanel.SuspendLayout();
		this.MainRaidsPanel.SuspendLayout();
		this.MainMinilandPanel.SuspendLayout();
		this.MainPacketLoggerPanel.SuspendLayout();
		this.MainSettingsPanel.SuspendLayout();
		this.MainNoAccessPanel.SuspendLayout();
		this.NoAccessPanel.SuspendLayout();
		this.LoadingScreenPanel.SuspendLayout();
		this.panel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.HideMenuButton).BeginInit();
		this.RaidsHistoryPanel.SuspendLayout();
		this.AnalyticsPlayersTab.SuspendLayout();
		this.PlayerRaidsStatisticsPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.PlayerRaidsStatisticsBossIcon).BeginInit();
		this.SPDetailsBorderPanel.SuspendLayout();
		this.SPDetailsPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.CloseSPDetailsButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsShadowImage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsLightImage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsWaterImage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsFireImage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsEnergyImage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsPropertyImage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsDefenceImage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsAttackImage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsAvatar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Tattoo2Icon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Tattoo1Icon).BeginInit();
		this.ShellInfoMainPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.SwitchToShellButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SwitchToRuneButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SwitchShellTypeButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Wings).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.WeaponSkin).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CostumeHat).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Costume).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.FlyingPet).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Mask).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Hat).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Armor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SecondaryWeapon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MainWeapon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Reputation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SearchedPlayerAvatar).BeginInit();
		this.MainFairyDetailsPanel.SuspendLayout();
		this.FairyDetailsPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.FairyDetailsIcon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CloseFairyDetailsButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AnalyticsBackArrow).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.FamRecordsNextPageButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.FamRecordsPreviousPageButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BackArrowRaidsHistory).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.RaidsHistoryNextPageButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.RaidsHistoryPreviousPageButton).BeginInit();
		this.RaidsHistoryDoubleBufferedPanel.SuspendLayout();
		this.RaidsHistoryDetailsPanel.SuspendLayout();
		this.RaidsHistoryListViewBorder.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.RaidsHistoryDetailsGridView).BeginInit();
		this.RankingTabPanel.SuspendLayout();
		this.RankingDoubleBufferedPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.RankingPreviousPageButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.RankingNextPageButton).BeginInit();
		this.MainQuestsPanel.SuspendLayout();
		this.QuestsPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.SwitchMapModeQuestButton).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ShowTimeSpaceMapButton).BeginInit();
		this.QuestSearchTypesPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.QuestsTabMap).BeginInit();
		base.SuspendLayout();
		this.packetListener.WorkerSupportsCancellation = true;
		this.packetListener.DoWork += new System.ComponentModel.DoWorkEventHandler(packetListener_DoWork);
		this.connectionsUpdater.DoWork += new System.ComponentModel.DoWorkEventHandler(connectionsUpdater_DoWork);
		this.packetHandler.DoWork += new System.ComponentModel.DoWorkEventHandler(packetHandler_DoWork);
		this.timer_map_tick.Enabled = true;
		this.timer_map_tick.Interval = 500;
		this.timer_map_tick.Tick += new System.EventHandler(timer_map_tick_Tick);
		this.SideMenu.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.SideMenu.Controls.Add(this.JoinDiscordButton);
		this.SideMenu.Controls.Add(this.ExitButton);
		this.SideMenu.Controls.Add(this.SwitchToSettingsButton);
		this.SideMenu.Controls.Add(this.SwitchToPacketLoggerButton);
		this.SideMenu.Controls.Add(this.SwitchToQuestsButton);
		this.SideMenu.Controls.Add(this.SwitchToAnalyticsButton);
		this.SideMenu.Controls.Add(this.SwitchToMLButton);
		this.SideMenu.Controls.Add(this.SwitchToCounterButton);
		this.SideMenu.Controls.Add(this.SwitchToRaidsButton);
		this.SideMenu.Controls.Add(this.SwitchToMapButton);
		this.SideMenu.Controls.Add(this.SwitchToControlPanelButton);
		this.SideMenu.Controls.Add(this.LogoPanel);
		this.SideMenu.Dock = System.Windows.Forms.DockStyle.Left;
		this.SideMenu.Location = new System.Drawing.Point(0, 0);
		this.SideMenu.Name = "SideMenu";
		this.SideMenu.Size = new System.Drawing.Size(250, 800);
		this.SideMenu.TabIndex = 0;
		this.SideMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.JoinDiscordButton.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.JoinDiscordButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.JoinDiscordButton.FlatAppearance.BorderSize = 0;
		this.JoinDiscordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.JoinDiscordButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.JoinDiscordButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.JoinDiscordButton.Image = NosAssistant2.Properties.Resources.discord_icon;
		this.JoinDiscordButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.JoinDiscordButton.Location = new System.Drawing.Point(0, 680);
		this.JoinDiscordButton.Name = "JoinDiscordButton";
		this.JoinDiscordButton.Padding = new System.Windows.Forms.Padding(80, 0, 30, 0);
		this.JoinDiscordButton.Size = new System.Drawing.Size(250, 60);
		this.JoinDiscordButton.TabIndex = 11;
		this.JoinDiscordButton.Text = "Join Discord";
		this.JoinDiscordButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.JoinDiscordButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.JoinDiscordButton.UseVisualStyleBackColor = true;
		this.JoinDiscordButton.Click += new System.EventHandler(JoinDiscordButton_Click);
		this.ExitButton.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.ExitButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.ExitButton.FlatAppearance.BorderSize = 0;
		this.ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ExitButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.ExitButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ExitButton.Image = NosAssistant2.Properties.Resources.exit_icon;
		this.ExitButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.ExitButton.Location = new System.Drawing.Point(0, 740);
		this.ExitButton.Name = "ExitButton";
		this.ExitButton.Padding = new System.Windows.Forms.Padding(106, 0, 30, 0);
		this.ExitButton.Size = new System.Drawing.Size(250, 60);
		this.ExitButton.TabIndex = 6;
		this.ExitButton.Text = "Exit";
		this.ExitButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.ExitButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.ExitButton.UseVisualStyleBackColor = true;
		this.ExitButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.SwitchToSettingsButton.Dock = System.Windows.Forms.DockStyle.Top;
		this.SwitchToSettingsButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.SwitchToSettingsButton.FlatAppearance.BorderSize = 0;
		this.SwitchToSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SwitchToSettingsButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.SwitchToSettingsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SwitchToSettingsButton.Image = NosAssistant2.Properties.Resources.settings_icon;
		this.SwitchToSettingsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SwitchToSettingsButton.Location = new System.Drawing.Point(0, 576);
		this.SwitchToSettingsButton.Name = "SwitchToSettingsButton";
		this.SwitchToSettingsButton.Padding = new System.Windows.Forms.Padding(92, 0, 30, 0);
		this.SwitchToSettingsButton.Size = new System.Drawing.Size(250, 50);
		this.SwitchToSettingsButton.TabIndex = 5;
		this.SwitchToSettingsButton.Text = "Settings";
		this.SwitchToSettingsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SwitchToSettingsButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.SwitchToSettingsButton.UseVisualStyleBackColor = true;
		this.SwitchToSettingsButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.SwitchToPacketLoggerButton.Dock = System.Windows.Forms.DockStyle.Top;
		this.SwitchToPacketLoggerButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.SwitchToPacketLoggerButton.FlatAppearance.BorderSize = 0;
		this.SwitchToPacketLoggerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SwitchToPacketLoggerButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.SwitchToPacketLoggerButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SwitchToPacketLoggerButton.Image = NosAssistant2.Properties.Resources.packet_logger_icon;
		this.SwitchToPacketLoggerButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SwitchToPacketLoggerButton.Location = new System.Drawing.Point(0, 526);
		this.SwitchToPacketLoggerButton.Name = "SwitchToPacketLoggerButton";
		this.SwitchToPacketLoggerButton.Padding = new System.Windows.Forms.Padding(76, 0, 30, 0);
		this.SwitchToPacketLoggerButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.SwitchToPacketLoggerButton.Size = new System.Drawing.Size(250, 50);
		this.SwitchToPacketLoggerButton.TabIndex = 8;
		this.SwitchToPacketLoggerButton.Text = "PacketLogger";
		this.SwitchToPacketLoggerButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SwitchToPacketLoggerButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.SwitchToPacketLoggerButton.UseVisualStyleBackColor = true;
		this.SwitchToPacketLoggerButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.SwitchToQuestsButton.Dock = System.Windows.Forms.DockStyle.Top;
		this.SwitchToQuestsButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.SwitchToQuestsButton.FlatAppearance.BorderSize = 0;
		this.SwitchToQuestsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SwitchToQuestsButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.SwitchToQuestsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SwitchToQuestsButton.Image = NosAssistant2.Properties.Resources.quests_icon;
		this.SwitchToQuestsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SwitchToQuestsButton.Location = new System.Drawing.Point(0, 476);
		this.SwitchToQuestsButton.Name = "SwitchToQuestsButton";
		this.SwitchToQuestsButton.Padding = new System.Windows.Forms.Padding(95, 0, 30, 0);
		this.SwitchToQuestsButton.Size = new System.Drawing.Size(250, 50);
		this.SwitchToQuestsButton.TabIndex = 12;
		this.SwitchToQuestsButton.Text = "Quests";
		this.SwitchToQuestsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SwitchToQuestsButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.SwitchToQuestsButton.UseVisualStyleBackColor = true;
		this.SwitchToQuestsButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.SwitchToAnalyticsButton.Dock = System.Windows.Forms.DockStyle.Top;
		this.SwitchToAnalyticsButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.SwitchToAnalyticsButton.FlatAppearance.BorderSize = 0;
		this.SwitchToAnalyticsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SwitchToAnalyticsButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.SwitchToAnalyticsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SwitchToAnalyticsButton.Image = NosAssistant2.Properties.Resources.analytics;
		this.SwitchToAnalyticsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SwitchToAnalyticsButton.Location = new System.Drawing.Point(0, 426);
		this.SwitchToAnalyticsButton.Name = "SwitchToAnalyticsButton";
		this.SwitchToAnalyticsButton.Padding = new System.Windows.Forms.Padding(92, 0, 30, 0);
		this.SwitchToAnalyticsButton.Size = new System.Drawing.Size(250, 50);
		this.SwitchToAnalyticsButton.TabIndex = 9;
		this.SwitchToAnalyticsButton.Text = "Analytics";
		this.SwitchToAnalyticsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SwitchToAnalyticsButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.SwitchToAnalyticsButton.UseVisualStyleBackColor = true;
		this.SwitchToAnalyticsButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.SwitchToMLButton.Dock = System.Windows.Forms.DockStyle.Top;
		this.SwitchToMLButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.SwitchToMLButton.FlatAppearance.BorderSize = 0;
		this.SwitchToMLButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SwitchToMLButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.SwitchToMLButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SwitchToMLButton.Image = NosAssistant2.Properties.Resources.miniland_icon;
		this.SwitchToMLButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SwitchToMLButton.Location = new System.Drawing.Point(0, 376);
		this.SwitchToMLButton.Name = "SwitchToMLButton";
		this.SwitchToMLButton.Padding = new System.Windows.Forms.Padding(90, 0, 30, 0);
		this.SwitchToMLButton.Size = new System.Drawing.Size(250, 50);
		this.SwitchToMLButton.TabIndex = 4;
		this.SwitchToMLButton.Text = "Miniland";
		this.SwitchToMLButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SwitchToMLButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.SwitchToMLButton.UseVisualStyleBackColor = true;
		this.SwitchToMLButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.SwitchToCounterButton.Dock = System.Windows.Forms.DockStyle.Top;
		this.SwitchToCounterButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.SwitchToCounterButton.FlatAppearance.BorderSize = 0;
		this.SwitchToCounterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SwitchToCounterButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.SwitchToCounterButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SwitchToCounterButton.Image = NosAssistant2.Properties.Resources.counter;
		this.SwitchToCounterButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SwitchToCounterButton.Location = new System.Drawing.Point(0, 326);
		this.SwitchToCounterButton.Name = "SwitchToCounterButton";
		this.SwitchToCounterButton.Padding = new System.Windows.Forms.Padding(92, 0, 30, 0);
		this.SwitchToCounterButton.Size = new System.Drawing.Size(250, 50);
		this.SwitchToCounterButton.TabIndex = 10;
		this.SwitchToCounterButton.Text = "Counter";
		this.SwitchToCounterButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SwitchToCounterButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.SwitchToCounterButton.UseVisualStyleBackColor = true;
		this.SwitchToCounterButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.SwitchToRaidsButton.Dock = System.Windows.Forms.DockStyle.Top;
		this.SwitchToRaidsButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.SwitchToRaidsButton.FlatAppearance.BorderSize = 0;
		this.SwitchToRaidsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SwitchToRaidsButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.SwitchToRaidsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SwitchToRaidsButton.Image = NosAssistant2.Properties.Resources.raids_icon;
		this.SwitchToRaidsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SwitchToRaidsButton.Location = new System.Drawing.Point(0, 276);
		this.SwitchToRaidsButton.Name = "SwitchToRaidsButton";
		this.SwitchToRaidsButton.Padding = new System.Windows.Forms.Padding(100, 0, 30, 0);
		this.SwitchToRaidsButton.Size = new System.Drawing.Size(250, 50);
		this.SwitchToRaidsButton.TabIndex = 3;
		this.SwitchToRaidsButton.Text = "Raids";
		this.SwitchToRaidsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SwitchToRaidsButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.SwitchToRaidsButton.UseVisualStyleBackColor = true;
		this.SwitchToRaidsButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.SwitchToMapButton.Dock = System.Windows.Forms.DockStyle.Top;
		this.SwitchToMapButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.SwitchToMapButton.FlatAppearance.BorderSize = 0;
		this.SwitchToMapButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SwitchToMapButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.SwitchToMapButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SwitchToMapButton.Image = NosAssistant2.Properties.Resources.map_icon;
		this.SwitchToMapButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SwitchToMapButton.Location = new System.Drawing.Point(0, 226);
		this.SwitchToMapButton.Name = "SwitchToMapButton";
		this.SwitchToMapButton.Padding = new System.Windows.Forms.Padding(104, 0, 30, 0);
		this.SwitchToMapButton.Size = new System.Drawing.Size(250, 50);
		this.SwitchToMapButton.TabIndex = 7;
		this.SwitchToMapButton.Text = "Map";
		this.SwitchToMapButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SwitchToMapButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.SwitchToMapButton.UseVisualStyleBackColor = true;
		this.SwitchToMapButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.SwitchToControlPanelButton.Dock = System.Windows.Forms.DockStyle.Top;
		this.SwitchToControlPanelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(24, 30, 54);
		this.SwitchToControlPanelButton.FlatAppearance.BorderSize = 0;
		this.SwitchToControlPanelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SwitchToControlPanelButton.Font = new System.Drawing.Font("Nirmala UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.SwitchToControlPanelButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SwitchToControlPanelButton.Image = NosAssistant2.Properties.Resources.control_panel_icon;
		this.SwitchToControlPanelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.SwitchToControlPanelButton.Location = new System.Drawing.Point(0, 176);
		this.SwitchToControlPanelButton.Name = "SwitchToControlPanelButton";
		this.SwitchToControlPanelButton.Padding = new System.Windows.Forms.Padding(75, 0, 30, 0);
		this.SwitchToControlPanelButton.Size = new System.Drawing.Size(250, 50);
		this.SwitchToControlPanelButton.TabIndex = 2;
		this.SwitchToControlPanelButton.Text = "Control Panel";
		this.SwitchToControlPanelButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SwitchToControlPanelButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
		this.SwitchToControlPanelButton.UseVisualStyleBackColor = true;
		this.SwitchToControlPanelButton.Click += new System.EventHandler(SidePanelButton_Click);
		this.LogoPanel.Controls.Add(this.CountDownPanel);
		this.LogoPanel.Controls.Add(this.VersionLabel);
		this.LogoPanel.Controls.Add(this.accessLabel);
		this.LogoPanel.Controls.Add(this.WelcomeUserLabel);
		this.LogoPanel.Controls.Add(this.LogoPicture);
		this.LogoPanel.Dock = System.Windows.Forms.DockStyle.Top;
		this.LogoPanel.Location = new System.Drawing.Point(0, 0);
		this.LogoPanel.Name = "LogoPanel";
		this.LogoPanel.Size = new System.Drawing.Size(250, 176);
		this.LogoPanel.TabIndex = 0;
		this.LogoPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.CountDownPanel.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.CountDownPanel.Controls.Add(this.CountDownLabel);
		this.CountDownPanel.Location = new System.Drawing.Point(0, 0);
		this.CountDownPanel.Margin = new System.Windows.Forms.Padding(0);
		this.CountDownPanel.Name = "CountDownPanel";
		this.CountDownPanel.Size = new System.Drawing.Size(250, 50);
		this.CountDownPanel.TabIndex = 4;
		this.CountDownPanel.Visible = false;
		this.CountDownLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.CountDownLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.CountDownLabel.Location = new System.Drawing.Point(0, 0);
		this.CountDownLabel.Name = "CountDownLabel";
		this.CountDownLabel.Size = new System.Drawing.Size(250, 50);
		this.CountDownLabel.TabIndex = 0;
		this.CountDownLabel.Text = "New Version is Avaiable: NA2 will close in: 1:59:59";
		this.CountDownLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.VersionLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.VersionLabel.Location = new System.Drawing.Point(0, 107);
		this.VersionLabel.Name = "VersionLabel";
		this.VersionLabel.Size = new System.Drawing.Size(250, 15);
		this.VersionLabel.TabIndex = 3;
		this.VersionLabel.Text = "version beta";
		this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.accessLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.accessLabel.ForeColor = System.Drawing.Color.Red;
		this.accessLabel.Location = new System.Drawing.Point(0, 155);
		this.accessLabel.Name = "accessLabel";
		this.accessLabel.Size = new System.Drawing.Size(250, 18);
		this.accessLabel.TabIndex = 2;
		this.accessLabel.Text = "No access";
		this.accessLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.accessLabel.Click += new System.EventHandler(accessLabel_Click);
		this.WelcomeUserLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.WelcomeUserLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.WelcomeUserLabel.Location = new System.Drawing.Point(0, 127);
		this.WelcomeUserLabel.Name = "WelcomeUserLabel";
		this.WelcomeUserLabel.Size = new System.Drawing.Size(250, 18);
		this.WelcomeUserLabel.TabIndex = 1;
		this.WelcomeUserLabel.Text = "Login char with license";
		this.WelcomeUserLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.WelcomeUserLabel.Click += new System.EventHandler(WelcomeUserLabel_Click);
		this.LogoPicture.Image = NosAssistant2.Properties.Resources.NA_logo;
		this.LogoPicture.Location = new System.Drawing.Point(40, 25);
		this.LogoPicture.Name = "LogoPicture";
		this.LogoPicture.Size = new System.Drawing.Size(170, 90);
		this.LogoPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.LogoPicture.TabIndex = 0;
		this.LogoPicture.TabStop = false;
		this.LogoPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MapBottomPanel.Controls.Add(this.MapMobFilterButton);
		this.MapBottomPanel.Controls.Add(this.ServerLabel);
		this.MapBottomPanel.Controls.Add(this.ChannelLabel);
		this.MapBottomPanel.Controls.Add(this.MapShowEntitiesLabel);
		this.MapBottomPanel.Controls.Add(this.MapShowEntitiesCheckBox);
		this.MapBottomPanel.Controls.Add(this.MapShowMobsLabel);
		this.MapBottomPanel.Controls.Add(this.MapShowMobsCheckBox);
		this.MapBottomPanel.Controls.Add(this.MapShowPlayersLabel);
		this.MapBottomPanel.Controls.Add(this.MapShowPlayersCheckBox);
		this.MapBottomPanel.Controls.Add(this.MapShowPetsLabel);
		this.MapBottomPanel.Controls.Add(this.MapShowPetsCheckBox);
		this.MapBottomPanel.Controls.Add(this.MapShowAltsLabel);
		this.MapBottomPanel.Controls.Add(this.MapShowAltsCheckBox);
		this.MapBottomPanel.Controls.Add(this.MapShowMapperLabel);
		this.MapBottomPanel.Controls.Add(this.MapShowMapperCheckBox);
		this.MapBottomPanel.Controls.Add(this.RandomRangeTextBox);
		this.MapBottomPanel.Controls.Add(this.RandomRangeLabel);
		this.MapBottomPanel.Controls.Add(this.MapPanelMapperLabel);
		this.MapBottomPanel.Controls.Add(this.MapLabel);
		this.MapBottomPanel.Controls.Add(this.MapYLabel);
		this.MapBottomPanel.Controls.Add(this.MapXLabel);
		this.MapBottomPanel.Location = new System.Drawing.Point(25, 670);
		this.MapBottomPanel.Name = "MapBottomPanel";
		this.MapBottomPanel.Size = new System.Drawing.Size(900, 129);
		this.MapBottomPanel.TabIndex = 18;
		this.MapBottomPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MapMobFilterButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.MapMobFilterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MapMobFilterButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapMobFilterButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapMobFilterButton.Location = new System.Drawing.Point(390, 60);
		this.MapMobFilterButton.Name = "MapMobFilterButton";
		this.MapMobFilterButton.Size = new System.Drawing.Size(100, 45);
		this.MapMobFilterButton.TabIndex = 38;
		this.MapMobFilterButton.Text = "Mob Filter";
		this.MapMobFilterButton.UseVisualStyleBackColor = false;
		this.MapMobFilterButton.Click += new System.EventHandler(MapMobFilterButton_Click);
		this.ServerLabel.AutoSize = true;
		this.ServerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ServerLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ServerLabel.Location = new System.Drawing.Point(75, 85);
		this.ServerLabel.Name = "ServerLabel";
		this.ServerLabel.Size = new System.Drawing.Size(66, 20);
		this.ServerLabel.TabIndex = 37;
		this.ServerLabel.Text = "Server:";
		this.ServerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ChannelLabel.AutoSize = true;
		this.ChannelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ChannelLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ChannelLabel.Location = new System.Drawing.Point(0, 85);
		this.ChannelLabel.Name = "ChannelLabel";
		this.ChannelLabel.Size = new System.Drawing.Size(39, 20);
		this.ChannelLabel.TabIndex = 36;
		this.ChannelLabel.Text = "CH:";
		this.ChannelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapShowEntitiesLabel.AutoSize = true;
		this.MapShowEntitiesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapShowEntitiesLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapShowEntitiesLabel.Location = new System.Drawing.Point(635, 85);
		this.MapShowEntitiesLabel.Name = "MapShowEntitiesLabel";
		this.MapShowEntitiesLabel.Size = new System.Drawing.Size(44, 20);
		this.MapShowEntitiesLabel.TabIndex = 35;
		this.MapShowEntitiesLabel.Text = "NPC";
		this.MapShowEntitiesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapShowEntitiesCheckBox.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.MapShowEntitiesCheckBox.FlatAppearance.BorderSize = 0;
		this.MapShowEntitiesCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MapShowEntitiesCheckBox.Location = new System.Drawing.Point(710, 83);
		this.MapShowEntitiesCheckBox.Name = "MapShowEntitiesCheckBox";
		this.MapShowEntitiesCheckBox.Size = new System.Drawing.Size(24, 24);
		this.MapShowEntitiesCheckBox.TabIndex = 34;
		this.MapShowEntitiesCheckBox.UseVisualStyleBackColor = false;
		this.MapShowEntitiesCheckBox.Click += new System.EventHandler(MapShowEntitiesCheckBox_Click);
		this.MapShowMobsLabel.AutoSize = true;
		this.MapShowMobsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapShowMobsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapShowMobsLabel.Location = new System.Drawing.Point(500, 50);
		this.MapShowMobsLabel.Name = "MapShowMobsLabel";
		this.MapShowMobsLabel.Size = new System.Drawing.Size(52, 20);
		this.MapShowMobsLabel.TabIndex = 33;
		this.MapShowMobsLabel.Text = "Mobs";
		this.MapShowMobsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapShowMobsCheckBox.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.MapShowMobsCheckBox.FlatAppearance.BorderSize = 0;
		this.MapShowMobsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MapShowMobsCheckBox.Location = new System.Drawing.Point(575, 48);
		this.MapShowMobsCheckBox.Name = "MapShowMobsCheckBox";
		this.MapShowMobsCheckBox.Size = new System.Drawing.Size(24, 24);
		this.MapShowMobsCheckBox.TabIndex = 32;
		this.MapShowMobsCheckBox.UseVisualStyleBackColor = false;
		this.MapShowMobsCheckBox.Click += new System.EventHandler(MapShowMobsCheckBox_Click);
		this.MapShowPlayersLabel.AutoSize = true;
		this.MapShowPlayersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapShowPlayersLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapShowPlayersLabel.Location = new System.Drawing.Point(500, 15);
		this.MapShowPlayersLabel.Name = "MapShowPlayersLabel";
		this.MapShowPlayersLabel.Size = new System.Drawing.Size(67, 20);
		this.MapShowPlayersLabel.TabIndex = 31;
		this.MapShowPlayersLabel.Text = "Players";
		this.MapShowPlayersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapShowPlayersCheckBox.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.MapShowPlayersCheckBox.FlatAppearance.BorderSize = 0;
		this.MapShowPlayersCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MapShowPlayersCheckBox.Location = new System.Drawing.Point(575, 13);
		this.MapShowPlayersCheckBox.Name = "MapShowPlayersCheckBox";
		this.MapShowPlayersCheckBox.Size = new System.Drawing.Size(24, 24);
		this.MapShowPlayersCheckBox.TabIndex = 30;
		this.MapShowPlayersCheckBox.UseVisualStyleBackColor = false;
		this.MapShowPlayersCheckBox.Click += new System.EventHandler(MapShowPlayersCheckBox_Click);
		this.MapShowPetsLabel.AutoSize = true;
		this.MapShowPetsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapShowPetsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapShowPetsLabel.Location = new System.Drawing.Point(500, 85);
		this.MapShowPetsLabel.Name = "MapShowPetsLabel";
		this.MapShowPetsLabel.Size = new System.Drawing.Size(45, 20);
		this.MapShowPetsLabel.TabIndex = 29;
		this.MapShowPetsLabel.Text = "Pets";
		this.MapShowPetsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapShowPetsCheckBox.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.MapShowPetsCheckBox.FlatAppearance.BorderSize = 0;
		this.MapShowPetsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MapShowPetsCheckBox.Location = new System.Drawing.Point(575, 83);
		this.MapShowPetsCheckBox.Name = "MapShowPetsCheckBox";
		this.MapShowPetsCheckBox.Size = new System.Drawing.Size(24, 24);
		this.MapShowPetsCheckBox.TabIndex = 28;
		this.MapShowPetsCheckBox.UseVisualStyleBackColor = false;
		this.MapShowPetsCheckBox.Click += new System.EventHandler(MapShowPetsCheckBox_Click);
		this.MapShowAltsLabel.AutoSize = true;
		this.MapShowAltsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapShowAltsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapShowAltsLabel.Location = new System.Drawing.Point(635, 50);
		this.MapShowAltsLabel.Name = "MapShowAltsLabel";
		this.MapShowAltsLabel.Size = new System.Drawing.Size(40, 20);
		this.MapShowAltsLabel.TabIndex = 27;
		this.MapShowAltsLabel.Text = "Alts";
		this.MapShowAltsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapShowAltsCheckBox.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.MapShowAltsCheckBox.FlatAppearance.BorderSize = 0;
		this.MapShowAltsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MapShowAltsCheckBox.Location = new System.Drawing.Point(710, 48);
		this.MapShowAltsCheckBox.Name = "MapShowAltsCheckBox";
		this.MapShowAltsCheckBox.Size = new System.Drawing.Size(24, 24);
		this.MapShowAltsCheckBox.TabIndex = 26;
		this.MapShowAltsCheckBox.UseVisualStyleBackColor = false;
		this.MapShowAltsCheckBox.Click += new System.EventHandler(MapShowAltsCheckBox_Click);
		this.MapShowMapperLabel.AutoSize = true;
		this.MapShowMapperLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapShowMapperLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapShowMapperLabel.Location = new System.Drawing.Point(635, 15);
		this.MapShowMapperLabel.Name = "MapShowMapperLabel";
		this.MapShowMapperLabel.Size = new System.Drawing.Size(69, 20);
		this.MapShowMapperLabel.TabIndex = 25;
		this.MapShowMapperLabel.Text = "Mapper";
		this.MapShowMapperLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapShowMapperCheckBox.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.MapShowMapperCheckBox.FlatAppearance.BorderSize = 0;
		this.MapShowMapperCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MapShowMapperCheckBox.Location = new System.Drawing.Point(710, 13);
		this.MapShowMapperCheckBox.Name = "MapShowMapperCheckBox";
		this.MapShowMapperCheckBox.Size = new System.Drawing.Size(24, 24);
		this.MapShowMapperCheckBox.TabIndex = 24;
		this.MapShowMapperCheckBox.UseVisualStyleBackColor = false;
		this.MapShowMapperCheckBox.Click += new System.EventHandler(MapShowMapperCheckBox_Click);
		this.RandomRangeTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.RandomRangeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.RandomRangeTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.RandomRangeTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RandomRangeTextBox.Location = new System.Drawing.Point(802, 43);
		this.RandomRangeTextBox.Name = "RandomRangeTextBox";
		this.RandomRangeTextBox.Size = new System.Drawing.Size(51, 26);
		this.RandomRangeTextBox.TabIndex = 23;
		this.RandomRangeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.RandomRangeTextBox.TextChanged += new System.EventHandler(RandomRangeTextBox_TextChanged);
		this.RandomRangeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(RandomRangeTextBox_KeyPress);
		this.RandomRangeLabel.AutoSize = true;
		this.RandomRangeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RandomRangeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RandomRangeLabel.Location = new System.Drawing.Point(772, 10);
		this.RandomRangeLabel.Name = "RandomRangeLabel";
		this.RandomRangeLabel.Size = new System.Drawing.Size(110, 20);
		this.RandomRangeLabel.TabIndex = 22;
		this.RandomRangeLabel.Text = "Rand Range";
		this.RandomRangeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapPanelMapperLabel.AutoSize = true;
		this.MapPanelMapperLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapPanelMapperLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapPanelMapperLabel.Location = new System.Drawing.Point(75, 50);
		this.MapPanelMapperLabel.Name = "MapPanelMapperLabel";
		this.MapPanelMapperLabel.Size = new System.Drawing.Size(74, 20);
		this.MapPanelMapperLabel.TabIndex = 18;
		this.MapPanelMapperLabel.Text = "Mapper:";
		this.MapPanelMapperLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapLabel.AutoSize = true;
		this.MapLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapLabel.Location = new System.Drawing.Point(75, 15);
		this.MapLabel.Name = "MapLabel";
		this.MapLabel.Size = new System.Drawing.Size(48, 20);
		this.MapLabel.TabIndex = 17;
		this.MapLabel.Text = "Map:";
		this.MapLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapYLabel.AutoSize = true;
		this.MapYLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapYLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapYLabel.Location = new System.Drawing.Point(0, 50);
		this.MapYLabel.Name = "MapYLabel";
		this.MapYLabel.Size = new System.Drawing.Size(26, 20);
		this.MapYLabel.TabIndex = 16;
		this.MapYLabel.Text = "Y:";
		this.MapYLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapXLabel.AutoSize = true;
		this.MapXLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapXLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapXLabel.Location = new System.Drawing.Point(0, 15);
		this.MapXLabel.Name = "MapXLabel";
		this.MapXLabel.Size = new System.Drawing.Size(26, 20);
		this.MapXLabel.TabIndex = 15;
		this.MapXLabel.Text = "X:";
		this.MapXLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MainControlPanelLabel.AutoSize = true;
		this.MainControlPanelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MainControlPanelLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MainControlPanelLabel.Location = new System.Drawing.Point(25, 9);
		this.MainControlPanelLabel.Name = "MainControlPanelLabel";
		this.MainControlPanelLabel.Size = new System.Drawing.Size(239, 39);
		this.MainControlPanelLabel.TabIndex = 1;
		this.MainControlPanelLabel.Text = "Control Panel";
		this.MainControlPanelLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainMapPanelLabel.AutoSize = true;
		this.MainMapPanelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MainMapPanelLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MainMapPanelLabel.Location = new System.Drawing.Point(25, 9);
		this.MainMapPanelLabel.Name = "MainMapPanelLabel";
		this.MainMapPanelLabel.Size = new System.Drawing.Size(87, 39);
		this.MainMapPanelLabel.TabIndex = 1;
		this.MainMapPanelLabel.Text = "Map";
		this.MainMapPanelLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainRaidsPanelLabel.AutoSize = true;
		this.MainRaidsPanelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MainRaidsPanelLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MainRaidsPanelLabel.Location = new System.Drawing.Point(25, 9);
		this.MainRaidsPanelLabel.Name = "MainRaidsPanelLabel";
		this.MainRaidsPanelLabel.Size = new System.Drawing.Size(111, 39);
		this.MainRaidsPanelLabel.TabIndex = 1;
		this.MainRaidsPanelLabel.Text = "Raids";
		this.MainRaidsPanelLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainMinilandPanelLabel.AutoSize = true;
		this.MainMinilandPanelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MainMinilandPanelLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MainMinilandPanelLabel.Location = new System.Drawing.Point(25, 9);
		this.MainMinilandPanelLabel.Name = "MainMinilandPanelLabel";
		this.MainMinilandPanelLabel.Size = new System.Drawing.Size(154, 39);
		this.MainMinilandPanelLabel.TabIndex = 1;
		this.MainMinilandPanelLabel.Text = "Miniland";
		this.MainMinilandPanelLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainPacketLoggerPanelLabel.AutoSize = true;
		this.MainPacketLoggerPanelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MainPacketLoggerPanelLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MainPacketLoggerPanelLabel.Location = new System.Drawing.Point(25, 9);
		this.MainPacketLoggerPanelLabel.Name = "MainPacketLoggerPanelLabel";
		this.MainPacketLoggerPanelLabel.Size = new System.Drawing.Size(253, 39);
		this.MainPacketLoggerPanelLabel.TabIndex = 1;
		this.MainPacketLoggerPanelLabel.Text = "Packet Logger";
		this.MainPacketLoggerPanelLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainSettingsPanelLabel.AutoSize = true;
		this.MainSettingsPanelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MainSettingsPanelLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MainSettingsPanelLabel.Location = new System.Drawing.Point(25, 9);
		this.MainSettingsPanelLabel.Name = "MainSettingsPanelLabel";
		this.MainSettingsPanelLabel.Size = new System.Drawing.Size(151, 39);
		this.MainSettingsPanelLabel.TabIndex = 1;
		this.MainSettingsPanelLabel.Text = "Settings";
		this.MainSettingsPanelLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.ControlPanelPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.ControlPanelPanel.Controls.Add(this.DisableHeaderLabel);
		this.ControlPanelPanel.Controls.Add(this.OtherHeaderLabel);
		this.ControlPanelPanel.Controls.Add(this.BufferHeaderLabel);
		this.ControlPanelPanel.Controls.Add(this.AutofullHeaderLabel);
		this.ControlPanelPanel.Controls.Add(this.MoverHeaderLabel);
		this.ControlPanelPanel.Controls.Add(this.RaiderHeaderLabel);
		this.ControlPanelPanel.Controls.Add(this.AttackerHeaderLabel);
		this.ControlPanelPanel.Controls.Add(this.NicknameHeaderLabel);
		this.ControlPanelPanel.Controls.Add(this.ControlPanelScrollbar);
		this.ControlPanelPanel.Controls.Add(this.flowLayoutCharactersPanel);
		this.ControlPanelPanel.Location = new System.Drawing.Point(25, 70);
		this.ControlPanelPanel.Name = "ControlPanelPanel";
		this.ControlPanelPanel.Size = new System.Drawing.Size(900, 600);
		this.ControlPanelPanel.TabIndex = 2;
		this.DisableHeaderLabel.AutoSize = true;
		this.DisableHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.DisableHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.DisableHeaderLabel.Location = new System.Drawing.Point(644, 10);
		this.DisableHeaderLabel.Name = "DisableHeaderLabel";
		this.DisableHeaderLabel.Size = new System.Drawing.Size(69, 20);
		this.DisableHeaderLabel.TabIndex = 6;
		this.DisableHeaderLabel.Text = "Disable";
		this.DisableHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.OtherHeaderLabel.AutoSize = true;
		this.OtherHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.OtherHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.OtherHeaderLabel.Location = new System.Drawing.Point(736, 10);
		this.OtherHeaderLabel.Name = "OtherHeaderLabel";
		this.OtherHeaderLabel.Size = new System.Drawing.Size(54, 20);
		this.OtherHeaderLabel.TabIndex = 5;
		this.OtherHeaderLabel.Text = "Other";
		this.OtherHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.BufferHeaderLabel.AutoSize = true;
		this.BufferHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.BufferHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.BufferHeaderLabel.Location = new System.Drawing.Point(479, 10);
		this.BufferHeaderLabel.Name = "BufferHeaderLabel";
		this.BufferHeaderLabel.Size = new System.Drawing.Size(59, 20);
		this.BufferHeaderLabel.TabIndex = 5;
		this.BufferHeaderLabel.Text = "Buffer";
		this.BufferHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.AutofullHeaderLabel.AutoSize = true;
		this.AutofullHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AutofullHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AutofullHeaderLabel.Location = new System.Drawing.Point(558, 10);
		this.AutofullHeaderLabel.Name = "AutofullHeaderLabel";
		this.AutofullHeaderLabel.Size = new System.Drawing.Size(71, 20);
		this.AutofullHeaderLabel.TabIndex = 4;
		this.AutofullHeaderLabel.Text = "Autofull";
		this.AutofullHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MoverHeaderLabel.AutoSize = true;
		this.MoverHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MoverHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MoverHeaderLabel.Location = new System.Drawing.Point(394, 10);
		this.MoverHeaderLabel.Name = "MoverHeaderLabel";
		this.MoverHeaderLabel.Size = new System.Drawing.Size(57, 20);
		this.MoverHeaderLabel.TabIndex = 4;
		this.MoverHeaderLabel.Text = "Mover";
		this.MoverHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RaiderHeaderLabel.AutoSize = true;
		this.RaiderHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RaiderHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaiderHeaderLabel.Location = new System.Drawing.Point(308, 10);
		this.RaiderHeaderLabel.Name = "RaiderHeaderLabel";
		this.RaiderHeaderLabel.Size = new System.Drawing.Size(62, 20);
		this.RaiderHeaderLabel.TabIndex = 3;
		this.RaiderHeaderLabel.Text = "Raider";
		this.RaiderHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.AttackerHeaderLabel.AutoSize = true;
		this.AttackerHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AttackerHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AttackerHeaderLabel.Location = new System.Drawing.Point(215, 10);
		this.AttackerHeaderLabel.Name = "AttackerHeaderLabel";
		this.AttackerHeaderLabel.Size = new System.Drawing.Size(77, 20);
		this.AttackerHeaderLabel.TabIndex = 2;
		this.AttackerHeaderLabel.Text = "Attacker";
		this.AttackerHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.NicknameHeaderLabel.AutoSize = true;
		this.NicknameHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.NicknameHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.NicknameHeaderLabel.Location = new System.Drawing.Point(93, 10);
		this.NicknameHeaderLabel.Name = "NicknameHeaderLabel";
		this.NicknameHeaderLabel.Size = new System.Drawing.Size(87, 20);
		this.NicknameHeaderLabel.TabIndex = 1;
		this.NicknameHeaderLabel.Text = "Nickname";
		this.NicknameHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ControlPanelScrollbar.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.ControlPanelScrollbar.Location = new System.Drawing.Point(888, 40);
		this.ControlPanelScrollbar.Name = "ControlPanelScrollbar";
		this.ControlPanelScrollbar.Size = new System.Drawing.Size(12, 530);
		this.ControlPanelScrollbar.TabIndex = 0;
		this.ControlPanelScrollbar.targetPanel = null;
		this.flowLayoutCharactersPanel.AutoScroll = true;
		this.flowLayoutCharactersPanel.Location = new System.Drawing.Point(0, 40);
		this.flowLayoutCharactersPanel.Margin = new System.Windows.Forms.Padding(0);
		this.flowLayoutCharactersPanel.Name = "flowLayoutCharactersPanel";
		this.flowLayoutCharactersPanel.Size = new System.Drawing.Size(900, 530);
		this.flowLayoutCharactersPanel.TabIndex = 0;
		this.InjectDLLButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.InjectDLLButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.InjectDLLButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.InjectDLLButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.InjectDLLButton.Location = new System.Drawing.Point(800, 15);
		this.InjectDLLButton.Name = "InjectDLLButton";
		this.InjectDLLButton.Size = new System.Drawing.Size(100, 45);
		this.InjectDLLButton.TabIndex = 0;
		this.InjectDLLButton.Text = "Inject";
		this.InjectDLLButton.UseVisualStyleBackColor = false;
		this.InjectDLLButton.MouseDown += new System.Windows.Forms.MouseEventHandler(InjectDLLButton_MouseDown);
		this.change_map_label.AutoSize = true;
		this.change_map_label.Font = new System.Drawing.Font("Nirmala UI", 30f, System.Drawing.FontStyle.Bold);
		this.change_map_label.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.change_map_label.Location = new System.Drawing.Point(63, 226);
		this.change_map_label.Name = "change_map_label";
		this.change_map_label.Size = new System.Drawing.Size(796, 54);
		this.change_map_label.TabIndex = 0;
		this.change_map_label.Text = "Please Change Map with Main To Display";
		this.map_picture.BackColor = System.Drawing.Color.Transparent;
		this.map_picture.Location = new System.Drawing.Point(0, 0);
		this.map_picture.Name = "map_picture";
		this.map_picture.Size = new System.Drawing.Size(100, 44);
		this.map_picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.map_picture.TabIndex = 10;
		this.map_picture.TabStop = false;
		this.map_picture.MouseClick += new System.Windows.Forms.MouseEventHandler(map_picture_MouseClick);
		this.map_picture.MouseMove += new System.Windows.Forms.MouseEventHandler(map_picture_MouseMove);
		this.MapPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.MapPanel.Controls.Add(this.OpenMapInNewWindowMapTabButton);
		this.MapPanel.Controls.Add(this.SwitchMapModeButton);
		this.MapPanel.Controls.Add(this.map_picture);
		this.MapPanel.Controls.Add(this.change_map_label);
		this.MapPanel.Location = new System.Drawing.Point(25, 70);
		this.MapPanel.Name = "MapPanel";
		this.MapPanel.Size = new System.Drawing.Size(900, 600);
		this.MapPanel.TabIndex = 3;
		this.OpenMapInNewWindowMapTabButton.BackColor = System.Drawing.Color.FromArgb(58, 12, 163);
		this.OpenMapInNewWindowMapTabButton.Image = NosAssistant2.Properties.Resources.open_in_new_window;
		this.OpenMapInNewWindowMapTabButton.Location = new System.Drawing.Point(868, 2);
		this.OpenMapInNewWindowMapTabButton.Name = "OpenMapInNewWindowMapTabButton";
		this.OpenMapInNewWindowMapTabButton.Size = new System.Drawing.Size(30, 30);
		this.OpenMapInNewWindowMapTabButton.TabIndex = 12;
		this.OpenMapInNewWindowMapTabButton.TabStop = false;
		this.OpenMapInNewWindowMapTabButton.Click += new System.EventHandler(OpenMapInNewWindowMapTabButton_Click);
		this.OpenMapInNewWindowMapTabButton.MouseEnter += new System.EventHandler(OpenMapInNewWindowMapTabButton_MouseEnter);
		this.OpenMapInNewWindowMapTabButton.MouseLeave += new System.EventHandler(OpenMapInNewWindowMapTabButton_MouseLeave);
		this.SwitchMapModeButton.BackColor = System.Drawing.Color.FromArgb(58, 12, 163);
		this.SwitchMapModeButton.Image = NosAssistant2.Properties.Resources.switch_icon;
		this.SwitchMapModeButton.Location = new System.Drawing.Point(838, 2);
		this.SwitchMapModeButton.Name = "SwitchMapModeButton";
		this.SwitchMapModeButton.Size = new System.Drawing.Size(30, 30);
		this.SwitchMapModeButton.TabIndex = 11;
		this.SwitchMapModeButton.TabStop = false;
		this.SwitchMapModeButton.Visible = false;
		this.SwitchMapModeButton.Click += new System.EventHandler(SwitchMapModeButton_Click);
		this.SwitchMapModeButton.MouseEnter += new System.EventHandler(SwitchMapModeButton_MouseEnter);
		this.SwitchMapModeButton.MouseLeave += new System.EventHandler(SwitchMapModeButton_MouseLeave);
		this.RaidsPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsPanel.Controls.Add(this.RaidsNotificationsButton);
		this.RaidsPanel.Controls.Add(this.OpenBoxesLabel);
		this.RaidsPanel.Controls.Add(this.OpenBoxesButton);
		this.RaidsPanel.Controls.Add(this.LimitFPSButton);
		this.RaidsPanel.Controls.Add(this.FPSLabel);
		this.RaidsPanel.Controls.Add(this.FPSTextBox);
		this.RaidsPanel.Controls.Add(this.ResizeWidthLabel);
		this.RaidsPanel.Controls.Add(this.ResizeHeightLabel);
		this.RaidsPanel.Controls.Add(this.ResizeHeightTextBox);
		this.RaidsPanel.Controls.Add(this.ResizeWidthTextBox);
		this.RaidsPanel.Controls.Add(this.ResizeWindowsButton);
		this.RaidsPanel.Controls.Add(this.SaveWindowsButton);
		this.RaidsPanel.Controls.Add(this.LoadWindowsButton);
		this.RaidsPanel.Controls.Add(this.useDebuffsButton);
		this.RaidsPanel.Controls.Add(this.MimicKeyboardLabel);
		this.RaidsPanel.Controls.Add(this.MimicMouseLabel);
		this.RaidsPanel.Controls.Add(this.MimicMouseButton);
		this.RaidsPanel.Controls.Add(this.MimicKeyboardButton);
		this.RaidsPanel.Controls.Add(this.UseSelfBuffsRaidsButton);
		this.RaidsPanel.Controls.Add(this.UseBuffsRaidsButton);
		this.RaidsPanel.Controls.Add(this.RaidersPanelScrollbar);
		this.RaidsPanel.Controls.Add(this.BuffsetsButton);
		this.RaidsPanel.Controls.Add(this.leaveRaidButton);
		this.RaidsPanel.Controls.Add(this.stackWindowsButton);
		this.RaidsPanel.Controls.Add(this.waterfallButton);
		this.RaidsPanel.Controls.Add(this.changeSPButton);
		this.RaidsPanel.Controls.Add(this.RaidersHPStatusPanel);
		this.RaidsPanel.Controls.Add(this.RaidHostInfoLabel);
		this.RaidsPanel.Controls.Add(this.AutoFullThresholdLabel);
		this.RaidsPanel.Controls.Add(this.AutofullThresholdTrackbar);
		this.RaidsPanel.Controls.Add(this.HostNameTextBox);
		this.RaidsPanel.Controls.Add(this.AutoJoinToggleLabel);
		this.RaidsPanel.Controls.Add(this.AutoJoinToggleButton);
		this.RaidsPanel.Controls.Add(this.JoinListButton);
		this.RaidsPanel.Controls.Add(this.AutoFullToggleLabel);
		this.RaidsPanel.Controls.Add(this.AutoFullToggleButton);
		this.RaidsPanel.Location = new System.Drawing.Point(25, 70);
		this.RaidsPanel.Name = "RaidsPanel";
		this.RaidsPanel.Size = new System.Drawing.Size(900, 600);
		this.RaidsPanel.TabIndex = 4;
		this.RaidsNotificationsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsNotificationsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.RaidsNotificationsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RaidsNotificationsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaidsNotificationsButton.Location = new System.Drawing.Point(260, 180);
		this.RaidsNotificationsButton.Name = "RaidsNotificationsButton";
		this.RaidsNotificationsButton.Size = new System.Drawing.Size(100, 45);
		this.RaidsNotificationsButton.TabIndex = 51;
		this.RaidsNotificationsButton.Text = "Raids Notification";
		this.RaidsNotificationsButton.UseVisualStyleBackColor = false;
		this.RaidsNotificationsButton.Click += new System.EventHandler(RaidsNotificationsButton_Click);
		this.OpenBoxesLabel.AutoSize = true;
		this.OpenBoxesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.OpenBoxesLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.OpenBoxesLabel.Location = new System.Drawing.Point(125, 302);
		this.OpenBoxesLabel.Name = "OpenBoxesLabel";
		this.OpenBoxesLabel.Size = new System.Drawing.Size(34, 20);
		this.OpenBoxesLabel.TabIndex = 50;
		this.OpenBoxesLabel.Text = "Off";
		this.OpenBoxesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.OpenBoxesButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.OpenBoxesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OpenBoxesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.OpenBoxesButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.OpenBoxesButton.Location = new System.Drawing.Point(15, 290);
		this.OpenBoxesButton.Name = "OpenBoxesButton";
		this.OpenBoxesButton.Size = new System.Drawing.Size(100, 45);
		this.OpenBoxesButton.TabIndex = 49;
		this.OpenBoxesButton.Text = "Open Boxes";
		this.OpenBoxesButton.UseVisualStyleBackColor = false;
		this.OpenBoxesButton.Click += new System.EventHandler(OpenBoxesButton_Click);
		this.LimitFPSButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.LimitFPSButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.LimitFPSButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.LimitFPSButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.LimitFPSButton.Location = new System.Drawing.Point(15, 410);
		this.LimitFPSButton.Name = "LimitFPSButton";
		this.LimitFPSButton.Size = new System.Drawing.Size(100, 45);
		this.LimitFPSButton.TabIndex = 48;
		this.LimitFPSButton.Text = "Limit FPS";
		this.LimitFPSButton.UseVisualStyleBackColor = false;
		this.LimitFPSButton.Click += new System.EventHandler(LimitFPSButton_Click);
		this.FPSLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.FPSLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.FPSLabel.Location = new System.Drawing.Point(15, 356);
		this.FPSLabel.Name = "FPSLabel";
		this.FPSLabel.Size = new System.Drawing.Size(100, 20);
		this.FPSLabel.TabIndex = 47;
		this.FPSLabel.Text = "FPS";
		this.FPSLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.FPSTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.FPSTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.FPSTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.FPSTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.FPSTextBox.Location = new System.Drawing.Point(35, 378);
		this.FPSTextBox.Name = "FPSTextBox";
		this.FPSTextBox.Size = new System.Drawing.Size(60, 26);
		this.FPSTextBox.TabIndex = 46;
		this.FPSTextBox.Text = "60";
		this.FPSTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.FPSTextBox.TextChanged += new System.EventHandler(FPSTextBox_TextChanged);
		this.ResizeWidthLabel.AutoSize = true;
		this.ResizeWidthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ResizeWidthLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ResizeWidthLabel.Location = new System.Drawing.Point(251, 409);
		this.ResizeWidthLabel.Name = "ResizeWidthLabel";
		this.ResizeWidthLabel.Size = new System.Drawing.Size(60, 20);
		this.ResizeWidthLabel.TabIndex = 45;
		this.ResizeWidthLabel.Text = "Width:";
		this.ResizeWidthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ResizeHeightLabel.AutoSize = true;
		this.ResizeHeightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ResizeHeightLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ResizeHeightLabel.Location = new System.Drawing.Point(250, 436);
		this.ResizeHeightLabel.Name = "ResizeHeightLabel";
		this.ResizeHeightLabel.Size = new System.Drawing.Size(67, 20);
		this.ResizeHeightLabel.TabIndex = 45;
		this.ResizeHeightLabel.Text = "Height:";
		this.ResizeHeightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ResizeHeightTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.ResizeHeightTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.ResizeHeightTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.ResizeHeightTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ResizeHeightTextBox.Location = new System.Drawing.Point(320, 434);
		this.ResizeHeightTextBox.Name = "ResizeHeightTextBox";
		this.ResizeHeightTextBox.Size = new System.Drawing.Size(60, 26);
		this.ResizeHeightTextBox.TabIndex = 44;
		this.ResizeHeightTextBox.Text = "768";
		this.ResizeHeightTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.ResizeHeightTextBox.TextChanged += new System.EventHandler(ResizeHeightTextBox_TextChanged);
		this.ResizeWidthTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.ResizeWidthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.ResizeWidthTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.ResizeWidthTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ResizeWidthTextBox.Location = new System.Drawing.Point(320, 404);
		this.ResizeWidthTextBox.Name = "ResizeWidthTextBox";
		this.ResizeWidthTextBox.Size = new System.Drawing.Size(60, 26);
		this.ResizeWidthTextBox.TabIndex = 43;
		this.ResizeWidthTextBox.Text = "1024";
		this.ResizeWidthTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.ResizeWidthTextBox.TextChanged += new System.EventHandler(ResizeWidthTextBox_TextChanged);
		this.ResizeWindowsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.ResizeWindowsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ResizeWindowsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ResizeWindowsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ResizeWindowsButton.Location = new System.Drawing.Point(390, 410);
		this.ResizeWindowsButton.Name = "ResizeWindowsButton";
		this.ResizeWindowsButton.Size = new System.Drawing.Size(100, 45);
		this.ResizeWindowsButton.TabIndex = 42;
		this.ResizeWindowsButton.Text = "Resize Windows";
		this.ResizeWindowsButton.UseVisualStyleBackColor = false;
		this.ResizeWindowsButton.Click += new System.EventHandler(ResizeWindowsButton_Click);
		this.SaveWindowsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.SaveWindowsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SaveWindowsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SaveWindowsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SaveWindowsButton.Location = new System.Drawing.Point(375, 265);
		this.SaveWindowsButton.Name = "SaveWindowsButton";
		this.SaveWindowsButton.Size = new System.Drawing.Size(100, 45);
		this.SaveWindowsButton.TabIndex = 41;
		this.SaveWindowsButton.Text = "Save Windows";
		this.SaveWindowsButton.UseVisualStyleBackColor = false;
		this.SaveWindowsButton.Click += new System.EventHandler(SaveWindowsButton_Click);
		this.LoadWindowsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.LoadWindowsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.LoadWindowsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.LoadWindowsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.LoadWindowsButton.Location = new System.Drawing.Point(375, 320);
		this.LoadWindowsButton.Name = "LoadWindowsButton";
		this.LoadWindowsButton.Size = new System.Drawing.Size(100, 45);
		this.LoadWindowsButton.TabIndex = 40;
		this.LoadWindowsButton.Text = "Load Windows";
		this.LoadWindowsButton.UseVisualStyleBackColor = false;
		this.LoadWindowsButton.Click += new System.EventHandler(LoadWindowsButton_Click);
		this.useDebuffsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.useDebuffsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.useDebuffsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useDebuffsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useDebuffsButton.Location = new System.Drawing.Point(375, 180);
		this.useDebuffsButton.Name = "useDebuffsButton";
		this.useDebuffsButton.Size = new System.Drawing.Size(100, 45);
		this.useDebuffsButton.TabIndex = 39;
		this.useDebuffsButton.Text = "Debuffs";
		this.useDebuffsButton.UseVisualStyleBackColor = false;
		this.useDebuffsButton.Click += new System.EventHandler(useDebuffsButton_Click);
		this.MimicKeyboardLabel.AutoSize = true;
		this.MimicKeyboardLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MimicKeyboardLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MimicKeyboardLabel.Location = new System.Drawing.Point(126, 552);
		this.MimicKeyboardLabel.Name = "MimicKeyboardLabel";
		this.MimicKeyboardLabel.Size = new System.Drawing.Size(34, 20);
		this.MimicKeyboardLabel.TabIndex = 38;
		this.MimicKeyboardLabel.Text = "Off";
		this.MimicKeyboardLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MimicMouseLabel.AutoSize = true;
		this.MimicMouseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MimicMouseLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MimicMouseLabel.Location = new System.Drawing.Point(125, 497);
		this.MimicMouseLabel.Name = "MimicMouseLabel";
		this.MimicMouseLabel.Size = new System.Drawing.Size(34, 20);
		this.MimicMouseLabel.TabIndex = 37;
		this.MimicMouseLabel.Text = "Off";
		this.MimicMouseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MimicMouseButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.MimicMouseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MimicMouseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MimicMouseButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MimicMouseButton.Location = new System.Drawing.Point(15, 485);
		this.MimicMouseButton.Name = "MimicMouseButton";
		this.MimicMouseButton.Size = new System.Drawing.Size(100, 45);
		this.MimicMouseButton.TabIndex = 36;
		this.MimicMouseButton.Text = "Mimic Mouse";
		this.MimicMouseButton.UseVisualStyleBackColor = false;
		this.MimicMouseButton.Click += new System.EventHandler(MimicMouseButton_Click);
		this.MimicKeyboardButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.MimicKeyboardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MimicKeyboardButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MimicKeyboardButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MimicKeyboardButton.Location = new System.Drawing.Point(15, 540);
		this.MimicKeyboardButton.Name = "MimicKeyboardButton";
		this.MimicKeyboardButton.Size = new System.Drawing.Size(100, 45);
		this.MimicKeyboardButton.TabIndex = 35;
		this.MimicKeyboardButton.Text = "Mimic Keyboard";
		this.MimicKeyboardButton.UseVisualStyleBackColor = false;
		this.MimicKeyboardButton.Click += new System.EventHandler(MimicKeyboardButton_Click);
		this.UseSelfBuffsRaidsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.UseSelfBuffsRaidsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.UseSelfBuffsRaidsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.UseSelfBuffsRaidsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.UseSelfBuffsRaidsButton.Location = new System.Drawing.Point(375, 125);
		this.UseSelfBuffsRaidsButton.Name = "UseSelfBuffsRaidsButton";
		this.UseSelfBuffsRaidsButton.Size = new System.Drawing.Size(100, 45);
		this.UseSelfBuffsRaidsButton.TabIndex = 34;
		this.UseSelfBuffsRaidsButton.Text = "Self Buffs";
		this.UseSelfBuffsRaidsButton.UseVisualStyleBackColor = false;
		this.UseSelfBuffsRaidsButton.Click += new System.EventHandler(UseSelfBuffsRaidsButton_Click);
		this.UseBuffsRaidsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.UseBuffsRaidsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.UseBuffsRaidsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.UseBuffsRaidsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.UseBuffsRaidsButton.Location = new System.Drawing.Point(375, 70);
		this.UseBuffsRaidsButton.Name = "UseBuffsRaidsButton";
		this.UseBuffsRaidsButton.Size = new System.Drawing.Size(100, 45);
		this.UseBuffsRaidsButton.TabIndex = 33;
		this.UseBuffsRaidsButton.Text = "Buffs";
		this.UseBuffsRaidsButton.UseVisualStyleBackColor = false;
		this.UseBuffsRaidsButton.Click += new System.EventHandler(UseBuffsRaidsButton_Click);
		this.RaidersPanelScrollbar.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.RaidersPanelScrollbar.Location = new System.Drawing.Point(888, 44);
		this.RaidersPanelScrollbar.Name = "RaidersPanelScrollbar";
		this.RaidersPanelScrollbar.Size = new System.Drawing.Size(12, 520);
		this.RaidersPanelScrollbar.TabIndex = 32;
		this.RaidersPanelScrollbar.targetPanel = null;
		this.BuffsetsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.BuffsetsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.BuffsetsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.BuffsetsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.BuffsetsButton.Location = new System.Drawing.Point(375, 15);
		this.BuffsetsButton.Name = "BuffsetsButton";
		this.BuffsetsButton.Size = new System.Drawing.Size(100, 45);
		this.BuffsetsButton.TabIndex = 31;
		this.BuffsetsButton.Text = "Buff Sets";
		this.BuffsetsButton.UseVisualStyleBackColor = false;
		this.BuffsetsButton.Click += new System.EventHandler(BuffsetsButton_Click);
		this.leaveRaidButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.leaveRaidButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.leaveRaidButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.leaveRaidButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.leaveRaidButton.Location = new System.Drawing.Point(15, 180);
		this.leaveRaidButton.Name = "leaveRaidButton";
		this.leaveRaidButton.Size = new System.Drawing.Size(100, 45);
		this.leaveRaidButton.TabIndex = 29;
		this.leaveRaidButton.Text = "Leave";
		this.leaveRaidButton.UseVisualStyleBackColor = false;
		this.leaveRaidButton.Click += new System.EventHandler(leaveRaidButton_Click);
		this.stackWindowsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.stackWindowsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.stackWindowsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.stackWindowsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.stackWindowsButton.Location = new System.Drawing.Point(260, 265);
		this.stackWindowsButton.Name = "stackWindowsButton";
		this.stackWindowsButton.Size = new System.Drawing.Size(100, 45);
		this.stackWindowsButton.TabIndex = 28;
		this.stackWindowsButton.Text = "Stack";
		this.stackWindowsButton.UseVisualStyleBackColor = false;
		this.stackWindowsButton.Click += new System.EventHandler(stackWindowsButton_Click);
		this.waterfallButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.waterfallButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.waterfallButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.waterfallButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.waterfallButton.Location = new System.Drawing.Point(260, 320);
		this.waterfallButton.Name = "waterfallButton";
		this.waterfallButton.Size = new System.Drawing.Size(100, 45);
		this.waterfallButton.TabIndex = 27;
		this.waterfallButton.Text = "Waterfall";
		this.waterfallButton.UseVisualStyleBackColor = false;
		this.waterfallButton.Click += new System.EventHandler(waterfallButton_Click);
		this.changeSPButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.changeSPButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.changeSPButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.changeSPButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.changeSPButton.Location = new System.Drawing.Point(15, 235);
		this.changeSPButton.Name = "changeSPButton";
		this.changeSPButton.Size = new System.Drawing.Size(100, 45);
		this.changeSPButton.TabIndex = 25;
		this.changeSPButton.Text = "SP";
		this.changeSPButton.UseVisualStyleBackColor = false;
		this.changeSPButton.Click += new System.EventHandler(changeSPButton_Click);
		this.RaidersHPStatusPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidersHPStatusPanel.Controls.Add(this.RefreshRaidsBarsLabel);
		this.RaidersHPStatusPanel.Controls.Add(this.SwitchRaidersTabPanel);
		this.RaidersHPStatusPanel.Controls.Add(this.RaidsBarsStatusFlowLayoutPanel);
		this.RaidersHPStatusPanel.Controls.Add(this.flowLayoutRaidsPanel);
		this.RaidersHPStatusPanel.Location = new System.Drawing.Point(517, 14);
		this.RaidersHPStatusPanel.Name = "RaidersHPStatusPanel";
		this.RaidersHPStatusPanel.Size = new System.Drawing.Size(380, 560);
		this.RaidersHPStatusPanel.TabIndex = 24;
		this.RefreshRaidsBarsLabel.Cursor = System.Windows.Forms.Cursors.Hand;
		this.RefreshRaidsBarsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RefreshRaidsBarsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RefreshRaidsBarsLabel.Location = new System.Drawing.Point(10, 0);
		this.RefreshRaidsBarsLabel.Name = "RefreshRaidsBarsLabel";
		this.RefreshRaidsBarsLabel.Size = new System.Drawing.Size(100, 30);
		this.RefreshRaidsBarsLabel.TabIndex = 3;
		this.RefreshRaidsBarsLabel.Text = "Refresh";
		this.RefreshRaidsBarsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RefreshRaidsBarsLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(RefreshRaidsBarsLabel_MouseClick);
		this.RefreshRaidsBarsLabel.MouseEnter += new System.EventHandler(RefreshRaidsBarsLabel_MouseEnter);
		this.RefreshRaidsBarsLabel.MouseLeave += new System.EventHandler(RefreshRaidsBarsLabel_MouseLeave);
		this.SwitchRaidersTabPanel.Image = NosAssistant2.Properties.Resources.switch_icon;
		this.SwitchRaidersTabPanel.Location = new System.Drawing.Point(340, 0);
		this.SwitchRaidersTabPanel.Name = "SwitchRaidersTabPanel";
		this.SwitchRaidersTabPanel.Size = new System.Drawing.Size(30, 30);
		this.SwitchRaidersTabPanel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SwitchRaidersTabPanel.TabIndex = 1;
		this.SwitchRaidersTabPanel.TabStop = false;
		this.SwitchRaidersTabPanel.Click += new System.EventHandler(SwitchRaidersTabPanel_Click);
		this.SwitchRaidersTabPanel.MouseEnter += new System.EventHandler(SwitchRaidersTabPanel_MouseEnter);
		this.SwitchRaidersTabPanel.MouseLeave += new System.EventHandler(SwitchRaidersTabPanel_MouseLeave);
		this.RaidsBarsStatusFlowLayoutPanel.Location = new System.Drawing.Point(10, 30);
		this.RaidsBarsStatusFlowLayoutPanel.Name = "RaidsBarsStatusFlowLayoutPanel";
		this.RaidsBarsStatusFlowLayoutPanel.Size = new System.Drawing.Size(360, 520);
		this.RaidsBarsStatusFlowLayoutPanel.TabIndex = 2;
		this.flowLayoutRaidsPanel.AllowDrop = true;
		this.flowLayoutRaidsPanel.AutoScroll = true;
		this.flowLayoutRaidsPanel.Location = new System.Drawing.Point(10, 30);
		this.flowLayoutRaidsPanel.Name = "flowLayoutRaidsPanel";
		this.flowLayoutRaidsPanel.Size = new System.Drawing.Size(360, 520);
		this.flowLayoutRaidsPanel.TabIndex = 0;
		this.flowLayoutRaidsPanel.Visible = false;
		this.flowLayoutRaidsPanel.DragDrop += new System.Windows.Forms.DragEventHandler(flowLayoutRaidsPanel_DragDrop);
		this.flowLayoutRaidsPanel.DragEnter += new System.Windows.Forms.DragEventHandler(flowLayoutRaidsPanel_DragEnter);
		this.flowLayoutRaidsPanel.DragOver += new System.Windows.Forms.DragEventHandler(flowLayoutRaidsPanel_DragOver);
		this.flowLayoutRaidsPanel.DragLeave += new System.EventHandler(flowLayoutRaidsPanel_DragLeave);
		this.RaidHostInfoLabel.AutoSize = true;
		this.RaidHostInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RaidHostInfoLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaidHostInfoLabel.Location = new System.Drawing.Point(308, 495);
		this.RaidHostInfoLabel.Name = "RaidHostInfoLabel";
		this.RaidHostInfoLabel.Size = new System.Drawing.Size(128, 20);
		this.RaidHostInfoLabel.TabIndex = 23;
		this.RaidHostInfoLabel.Text = "Raid Host Nick";
		this.RaidHostInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.AutoFullThresholdLabel.AutoSize = true;
		this.AutoFullThresholdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AutoFullThresholdLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AutoFullThresholdLabel.Location = new System.Drawing.Point(270, 27);
		this.AutoFullThresholdLabel.Name = "AutoFullThresholdLabel";
		this.AutoFullThresholdLabel.Size = new System.Drawing.Size(34, 20);
		this.AutoFullThresholdLabel.TabIndex = 22;
		this.AutoFullThresholdLabel.Text = "0%";
		this.AutoFullThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.AutoFullThresholdLabel.Visible = false;
		this.AutofullThresholdTrackbar.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.AutofullThresholdTrackbar.Location = new System.Drawing.Point(160, 27);
		this.AutofullThresholdTrackbar.Maximum = 100;
		this.AutofullThresholdTrackbar.Name = "AutofullThresholdTrackbar";
		this.AutofullThresholdTrackbar.Size = new System.Drawing.Size(104, 45);
		this.AutofullThresholdTrackbar.TabIndex = 21;
		this.AutofullThresholdTrackbar.TickFrequency = 0;
		this.AutofullThresholdTrackbar.TickStyle = System.Windows.Forms.TickStyle.None;
		this.AutofullThresholdTrackbar.Visible = false;
		this.AutofullThresholdTrackbar.Scroll += new System.EventHandler(AutofullThresholdTrackbar_Scroll);
		this.HostNameTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.HostNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.HostNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.HostNameTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.HostNameTextBox.Location = new System.Drawing.Point(297, 534);
		this.HostNameTextBox.Name = "HostNameTextBox";
		this.HostNameTextBox.Size = new System.Drawing.Size(150, 26);
		this.HostNameTextBox.TabIndex = 20;
		this.HostNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.HostNameTextBox.TextChanged += new System.EventHandler(HostNameTextBox_TextChanged);
		this.AutoJoinToggleLabel.AutoSize = true;
		this.AutoJoinToggleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AutoJoinToggleLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AutoJoinToggleLabel.Location = new System.Drawing.Point(125, 82);
		this.AutoJoinToggleLabel.Name = "AutoJoinToggleLabel";
		this.AutoJoinToggleLabel.Size = new System.Drawing.Size(34, 20);
		this.AutoJoinToggleLabel.TabIndex = 19;
		this.AutoJoinToggleLabel.Text = "Off";
		this.AutoJoinToggleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.AutoJoinToggleButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.AutoJoinToggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.AutoJoinToggleButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AutoJoinToggleButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AutoJoinToggleButton.Location = new System.Drawing.Point(15, 70);
		this.AutoJoinToggleButton.Name = "AutoJoinToggleButton";
		this.AutoJoinToggleButton.Size = new System.Drawing.Size(100, 45);
		this.AutoJoinToggleButton.TabIndex = 18;
		this.AutoJoinToggleButton.Text = "Auto Join";
		this.AutoJoinToggleButton.UseVisualStyleBackColor = false;
		this.AutoJoinToggleButton.Click += new System.EventHandler(AutoJoinToggleButton_Click);
		this.JoinListButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.JoinListButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.JoinListButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.JoinListButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.JoinListButton.Location = new System.Drawing.Point(15, 125);
		this.JoinListButton.Name = "JoinListButton";
		this.JoinListButton.Size = new System.Drawing.Size(100, 45);
		this.JoinListButton.TabIndex = 17;
		this.JoinListButton.Text = "Join List";
		this.JoinListButton.UseVisualStyleBackColor = false;
		this.JoinListButton.Click += new System.EventHandler(JoinListButton_Click);
		this.AutoFullToggleLabel.AutoSize = true;
		this.AutoFullToggleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AutoFullToggleLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AutoFullToggleLabel.Location = new System.Drawing.Point(124, 27);
		this.AutoFullToggleLabel.Name = "AutoFullToggleLabel";
		this.AutoFullToggleLabel.Size = new System.Drawing.Size(34, 20);
		this.AutoFullToggleLabel.TabIndex = 16;
		this.AutoFullToggleLabel.Text = "Off";
		this.AutoFullToggleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.AutoFullToggleButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.AutoFullToggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.AutoFullToggleButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AutoFullToggleButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AutoFullToggleButton.Location = new System.Drawing.Point(15, 15);
		this.AutoFullToggleButton.Name = "AutoFullToggleButton";
		this.AutoFullToggleButton.Size = new System.Drawing.Size(100, 45);
		this.AutoFullToggleButton.TabIndex = 1;
		this.AutoFullToggleButton.Text = "Autofull";
		this.AutoFullToggleButton.UseVisualStyleBackColor = false;
		this.AutoFullToggleButton.Click += new System.EventHandler(AutoFullToggleButton_Click);
		this.MinilandPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.MinilandPanel.Controls.Add(this.PetTrainerFlowLayoutPanel);
		this.MinilandPanel.Controls.Add(this.AutoRespawnLabel);
		this.MinilandPanel.Controls.Add(this.AutoRespawnButton);
		this.MinilandPanel.Controls.Add(this.OpenDmgContributionWindow);
		this.MinilandPanel.Controls.Add(this.NewNicknameToInviteListTextBox);
		this.MinilandPanel.Controls.Add(this.RemoveFromInviteListButton);
		this.MinilandPanel.Controls.Add(this.AddToInviteListButton);
		this.MinilandPanel.Controls.Add(this.InviteListPanel);
		this.MinilandPanel.Controls.Add(this.useSelfBuffsButton);
		this.MinilandPanel.Controls.Add(this.autoConfirmLabel);
		this.MinilandPanel.Controls.Add(this.autoConfirmToggleButton);
		this.MinilandPanel.Controls.Add(this.prepareMLButton);
		this.MinilandPanel.Controls.Add(this.inviteToMLButton);
		this.MinilandPanel.Controls.Add(this.MLTabInviterLabel);
		this.MinilandPanel.Controls.Add(this.useBuffsButton);
		this.MinilandPanel.Location = new System.Drawing.Point(25, 70);
		this.MinilandPanel.Name = "MinilandPanel";
		this.MinilandPanel.Size = new System.Drawing.Size(900, 600);
		this.MinilandPanel.TabIndex = 5;
		this.PetTrainerFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.PetTrainerFlowLayoutPanel.Controls.Add(this.panel1);
		this.PetTrainerFlowLayoutPanel.Controls.Add(this.panel3);
		this.PetTrainerFlowLayoutPanel.Controls.Add(this.panel4);
		this.PetTrainerFlowLayoutPanel.Controls.Add(this.panel5);
		this.PetTrainerFlowLayoutPanel.Controls.Add(this.panel6);
		this.PetTrainerFlowLayoutPanel.Controls.Add(this.panel7);
		this.PetTrainerFlowLayoutPanel.Location = new System.Drawing.Point(360, 25);
		this.PetTrainerFlowLayoutPanel.Name = "PetTrainerFlowLayoutPanel";
		this.PetTrainerFlowLayoutPanel.Size = new System.Drawing.Size(270, 210);
		this.PetTrainerFlowLayoutPanel.TabIndex = 34;
		this.panel1.Controls.Add(this.PetTrainerIcon1);
		this.panel1.Controls.Add(this.PetTrainerTimerLabel1);
		this.panel1.Location = new System.Drawing.Point(0, 0);
		this.panel1.Margin = new System.Windows.Forms.Padding(0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(270, 30);
		this.panel1.TabIndex = 6;
		this.PetTrainerIcon1.Location = new System.Drawing.Point(10, 0);
		this.PetTrainerIcon1.Name = "PetTrainerIcon1";
		this.PetTrainerIcon1.Size = new System.Drawing.Size(30, 30);
		this.PetTrainerIcon1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.PetTrainerIcon1.TabIndex = 1;
		this.PetTrainerIcon1.TabStop = false;
		this.PetTrainerTimerLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PetTrainerTimerLabel1.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PetTrainerTimerLabel1.Location = new System.Drawing.Point(40, 0);
		this.PetTrainerTimerLabel1.Margin = new System.Windows.Forms.Padding(0);
		this.PetTrainerTimerLabel1.Name = "PetTrainerTimerLabel1";
		this.PetTrainerTimerLabel1.Size = new System.Drawing.Size(230, 30);
		this.PetTrainerTimerLabel1.TabIndex = 0;
		this.PetTrainerTimerLabel1.Text = "Trainer #1: 00:00";
		this.PetTrainerTimerLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.panel3.Controls.Add(this.PetTrainerIcon2);
		this.panel3.Controls.Add(this.PetTrainerTimerLabel2);
		this.panel3.Location = new System.Drawing.Point(0, 30);
		this.panel3.Margin = new System.Windows.Forms.Padding(0);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(270, 30);
		this.panel3.TabIndex = 7;
		this.PetTrainerIcon2.Location = new System.Drawing.Point(10, 0);
		this.PetTrainerIcon2.Name = "PetTrainerIcon2";
		this.PetTrainerIcon2.Size = new System.Drawing.Size(30, 30);
		this.PetTrainerIcon2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.PetTrainerIcon2.TabIndex = 1;
		this.PetTrainerIcon2.TabStop = false;
		this.PetTrainerTimerLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PetTrainerTimerLabel2.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PetTrainerTimerLabel2.Location = new System.Drawing.Point(40, 0);
		this.PetTrainerTimerLabel2.Margin = new System.Windows.Forms.Padding(0);
		this.PetTrainerTimerLabel2.Name = "PetTrainerTimerLabel2";
		this.PetTrainerTimerLabel2.Size = new System.Drawing.Size(230, 30);
		this.PetTrainerTimerLabel2.TabIndex = 0;
		this.PetTrainerTimerLabel2.Text = "Trainer #2: 00:00";
		this.PetTrainerTimerLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.panel4.Controls.Add(this.PetTrainerIcon3);
		this.panel4.Controls.Add(this.PetTrainerTimerLabel3);
		this.panel4.Location = new System.Drawing.Point(0, 60);
		this.panel4.Margin = new System.Windows.Forms.Padding(0);
		this.panel4.Name = "panel4";
		this.panel4.Size = new System.Drawing.Size(270, 30);
		this.panel4.TabIndex = 8;
		this.PetTrainerIcon3.Location = new System.Drawing.Point(10, 0);
		this.PetTrainerIcon3.Name = "PetTrainerIcon3";
		this.PetTrainerIcon3.Size = new System.Drawing.Size(30, 30);
		this.PetTrainerIcon3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.PetTrainerIcon3.TabIndex = 1;
		this.PetTrainerIcon3.TabStop = false;
		this.PetTrainerTimerLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PetTrainerTimerLabel3.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PetTrainerTimerLabel3.Location = new System.Drawing.Point(40, 0);
		this.PetTrainerTimerLabel3.Margin = new System.Windows.Forms.Padding(0);
		this.PetTrainerTimerLabel3.Name = "PetTrainerTimerLabel3";
		this.PetTrainerTimerLabel3.Size = new System.Drawing.Size(230, 30);
		this.PetTrainerTimerLabel3.TabIndex = 0;
		this.PetTrainerTimerLabel3.Text = "Trainer #3: 00:00";
		this.PetTrainerTimerLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.panel5.Controls.Add(this.TrainedPetIcon1);
		this.panel5.Controls.Add(this.TrainedPetInfoLabel1);
		this.panel5.Location = new System.Drawing.Point(0, 120);
		this.panel5.Margin = new System.Windows.Forms.Padding(0, 30, 0, 0);
		this.panel5.Name = "panel5";
		this.panel5.Size = new System.Drawing.Size(270, 30);
		this.panel5.TabIndex = 9;
		this.TrainedPetIcon1.Location = new System.Drawing.Point(10, 0);
		this.TrainedPetIcon1.Name = "TrainedPetIcon1";
		this.TrainedPetIcon1.Size = new System.Drawing.Size(30, 30);
		this.TrainedPetIcon1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.TrainedPetIcon1.TabIndex = 1;
		this.TrainedPetIcon1.TabStop = false;
		this.TrainedPetInfoLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.TrainedPetInfoLabel1.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.TrainedPetInfoLabel1.Location = new System.Drawing.Point(40, 0);
		this.TrainedPetInfoLabel1.Margin = new System.Windows.Forms.Padding(0);
		this.TrainedPetInfoLabel1.Name = "TrainedPetInfoLabel1";
		this.TrainedPetInfoLabel1.Size = new System.Drawing.Size(230, 30);
		this.TrainedPetInfoLabel1.TabIndex = 0;
		this.TrainedPetInfoLabel1.Text = "Pet #1: -";
		this.TrainedPetInfoLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.panel6.Controls.Add(this.TrainedPetIcon2);
		this.panel6.Controls.Add(this.TrainedPetInfoLabel2);
		this.panel6.Location = new System.Drawing.Point(0, 150);
		this.panel6.Margin = new System.Windows.Forms.Padding(0);
		this.panel6.Name = "panel6";
		this.panel6.Size = new System.Drawing.Size(270, 30);
		this.panel6.TabIndex = 10;
		this.TrainedPetIcon2.Location = new System.Drawing.Point(10, 0);
		this.TrainedPetIcon2.Name = "TrainedPetIcon2";
		this.TrainedPetIcon2.Size = new System.Drawing.Size(30, 30);
		this.TrainedPetIcon2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.TrainedPetIcon2.TabIndex = 1;
		this.TrainedPetIcon2.TabStop = false;
		this.TrainedPetInfoLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.TrainedPetInfoLabel2.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.TrainedPetInfoLabel2.Location = new System.Drawing.Point(40, 0);
		this.TrainedPetInfoLabel2.Margin = new System.Windows.Forms.Padding(0);
		this.TrainedPetInfoLabel2.Name = "TrainedPetInfoLabel2";
		this.TrainedPetInfoLabel2.Size = new System.Drawing.Size(230, 30);
		this.TrainedPetInfoLabel2.TabIndex = 0;
		this.TrainedPetInfoLabel2.Text = "Pet #2: -";
		this.TrainedPetInfoLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.panel7.Controls.Add(this.TrainedPetIcon3);
		this.panel7.Controls.Add(this.TrainedPetInfoLabel3);
		this.panel7.Location = new System.Drawing.Point(0, 180);
		this.panel7.Margin = new System.Windows.Forms.Padding(0);
		this.panel7.Name = "panel7";
		this.panel7.Size = new System.Drawing.Size(270, 30);
		this.panel7.TabIndex = 11;
		this.TrainedPetIcon3.Location = new System.Drawing.Point(10, 0);
		this.TrainedPetIcon3.Name = "TrainedPetIcon3";
		this.TrainedPetIcon3.Size = new System.Drawing.Size(30, 30);
		this.TrainedPetIcon3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.TrainedPetIcon3.TabIndex = 1;
		this.TrainedPetIcon3.TabStop = false;
		this.TrainedPetInfoLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.TrainedPetInfoLabel3.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.TrainedPetInfoLabel3.Location = new System.Drawing.Point(40, 0);
		this.TrainedPetInfoLabel3.Margin = new System.Windows.Forms.Padding(0);
		this.TrainedPetInfoLabel3.Name = "TrainedPetInfoLabel3";
		this.TrainedPetInfoLabel3.Size = new System.Drawing.Size(230, 30);
		this.TrainedPetInfoLabel3.TabIndex = 0;
		this.TrainedPetInfoLabel3.Text = "Pet #3: -";
		this.TrainedPetInfoLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.AutoRespawnLabel.AutoSize = true;
		this.AutoRespawnLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AutoRespawnLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AutoRespawnLabel.Location = new System.Drawing.Point(120, 427);
		this.AutoRespawnLabel.Name = "AutoRespawnLabel";
		this.AutoRespawnLabel.Size = new System.Drawing.Size(34, 20);
		this.AutoRespawnLabel.TabIndex = 33;
		this.AutoRespawnLabel.Text = "Off";
		this.AutoRespawnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.AutoRespawnButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.AutoRespawnButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.AutoRespawnButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AutoRespawnButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AutoRespawnButton.Location = new System.Drawing.Point(15, 415);
		this.AutoRespawnButton.Name = "AutoRespawnButton";
		this.AutoRespawnButton.Size = new System.Drawing.Size(100, 45);
		this.AutoRespawnButton.TabIndex = 32;
		this.AutoRespawnButton.Text = "Auto Respawn";
		this.AutoRespawnButton.UseVisualStyleBackColor = false;
		this.AutoRespawnButton.Click += new System.EventHandler(AutoRespawnButton_Click);
		this.OpenDmgContributionWindow.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.OpenDmgContributionWindow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OpenDmgContributionWindow.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.OpenDmgContributionWindow.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.OpenDmgContributionWindow.Location = new System.Drawing.Point(125, 15);
		this.OpenDmgContributionWindow.Name = "OpenDmgContributionWindow";
		this.OpenDmgContributionWindow.Size = new System.Drawing.Size(100, 45);
		this.OpenDmgContributionWindow.TabIndex = 31;
		this.OpenDmgContributionWindow.Text = "XP Party";
		this.OpenDmgContributionWindow.UseVisualStyleBackColor = false;
		this.OpenDmgContributionWindow.Click += new System.EventHandler(OpenDmgContributionWindow_Click);
		this.NewNicknameToInviteListTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.NewNicknameToInviteListTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.NewNicknameToInviteListTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.NewNicknameToInviteListTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.NewNicknameToInviteListTextBox.Location = new System.Drawing.Point(772, 553);
		this.NewNicknameToInviteListTextBox.Name = "NewNicknameToInviteListTextBox";
		this.NewNicknameToInviteListTextBox.Size = new System.Drawing.Size(100, 26);
		this.NewNicknameToInviteListTextBox.TabIndex = 30;
		this.NewNicknameToInviteListTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.RemoveFromInviteListButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RemoveFromInviteListButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.RemoveFromInviteListButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RemoveFromInviteListButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RemoveFromInviteListButton.Location = new System.Drawing.Point(650, 495);
		this.RemoveFromInviteListButton.Name = "RemoveFromInviteListButton";
		this.RemoveFromInviteListButton.Size = new System.Drawing.Size(100, 45);
		this.RemoveFromInviteListButton.TabIndex = 29;
		this.RemoveFromInviteListButton.Text = "Remove";
		this.RemoveFromInviteListButton.UseVisualStyleBackColor = false;
		this.RemoveFromInviteListButton.Click += new System.EventHandler(RemoveFromInviteListButton_Click);
		this.AddToInviteListButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.AddToInviteListButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.AddToInviteListButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AddToInviteListButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AddToInviteListButton.Location = new System.Drawing.Point(772, 495);
		this.AddToInviteListButton.Name = "AddToInviteListButton";
		this.AddToInviteListButton.Size = new System.Drawing.Size(100, 45);
		this.AddToInviteListButton.TabIndex = 28;
		this.AddToInviteListButton.Text = "Add";
		this.AddToInviteListButton.UseVisualStyleBackColor = false;
		this.AddToInviteListButton.Click += new System.EventHandler(AddToInviteListButton_Click);
		this.InviteListPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.InviteListPanel.Controls.Add(this.InviteListDataGrid);
		this.InviteListPanel.Location = new System.Drawing.Point(650, 25);
		this.InviteListPanel.Name = "InviteListPanel";
		this.InviteListPanel.Size = new System.Drawing.Size(220, 460);
		this.InviteListPanel.TabIndex = 27;
		this.InviteListDataGrid.AllowUserToAddRows = false;
		this.InviteListDataGrid.AllowUserToDeleteRows = false;
		this.InviteListDataGrid.AllowUserToResizeColumns = false;
		this.InviteListDataGrid.AllowUserToResizeRows = false;
		this.InviteListDataGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.InviteListDataGrid.AutoGenerateColumns = false;
		this.InviteListDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.InviteListDataGrid.BackgroundColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.InviteListDataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.InviteListDataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		dataGridViewCellStyle.Font = new System.Drawing.Font("Segoe UI", 9f);
		dataGridViewCellStyle.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.InviteListDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.InviteListDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
		this.InviteListDataGrid.Columns.AddRange(this.nickname);
		this.InviteListDataGrid.DataSource = this.inviteListBindingSource;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(72, 149, 239);
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(72, 149, 239);
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.InviteListDataGrid.DefaultCellStyle = dataGridViewCellStyle2;
		this.InviteListDataGrid.EnableHeadersVisualStyles = false;
		this.InviteListDataGrid.GridColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.InviteListDataGrid.Location = new System.Drawing.Point(20, 20);
		this.InviteListDataGrid.MultiSelect = false;
		this.InviteListDataGrid.Name = "InviteListDataGrid";
		this.InviteListDataGrid.ReadOnly = true;
		this.InviteListDataGrid.RowHeadersVisible = false;
		this.InviteListDataGrid.RowHeadersWidth = 30;
		this.InviteListDataGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.InviteListDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
		this.InviteListDataGrid.Size = new System.Drawing.Size(180, 420);
		this.InviteListDataGrid.TabIndex = 31;
		this.InviteListDataGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(InviteListDataGrid_CellMouseClick);
		this.nickname.DataPropertyName = "nickname";
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.nickname.DefaultCellStyle = dataGridViewCellStyle3;
		this.nickname.HeaderText = "Invite List";
		this.nickname.Name = "nickname";
		this.nickname.ReadOnly = true;
		this.nickname.Resizable = System.Windows.Forms.DataGridViewTriState.False;
		this.inviteListBindingSource.DataSource = typeof(NosAssistant2.GameObjects.InviteItem);
		this.useSelfBuffsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.useSelfBuffsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.useSelfBuffsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useSelfBuffsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useSelfBuffsButton.Location = new System.Drawing.Point(15, 180);
		this.useSelfBuffsButton.Name = "useSelfBuffsButton";
		this.useSelfBuffsButton.Size = new System.Drawing.Size(100, 45);
		this.useSelfBuffsButton.TabIndex = 26;
		this.useSelfBuffsButton.Text = "Self Buffs";
		this.useSelfBuffsButton.UseVisualStyleBackColor = false;
		this.useSelfBuffsButton.Click += new System.EventHandler(useSelfBuffsButton_Click);
		this.autoConfirmLabel.AutoSize = true;
		this.autoConfirmLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.autoConfirmLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.autoConfirmLabel.Location = new System.Drawing.Point(120, 482);
		this.autoConfirmLabel.Name = "autoConfirmLabel";
		this.autoConfirmLabel.Size = new System.Drawing.Size(34, 20);
		this.autoConfirmLabel.TabIndex = 25;
		this.autoConfirmLabel.Text = "Off";
		this.autoConfirmLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.autoConfirmToggleButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.autoConfirmToggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.autoConfirmToggleButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.autoConfirmToggleButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.autoConfirmToggleButton.Location = new System.Drawing.Point(15, 470);
		this.autoConfirmToggleButton.Name = "autoConfirmToggleButton";
		this.autoConfirmToggleButton.Size = new System.Drawing.Size(100, 45);
		this.autoConfirmToggleButton.TabIndex = 24;
		this.autoConfirmToggleButton.Text = "Auto Confirm";
		this.autoConfirmToggleButton.UseVisualStyleBackColor = false;
		this.autoConfirmToggleButton.Click += new System.EventHandler(autoConfirmToggleButton_Click);
		this.prepareMLButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.prepareMLButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.prepareMLButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.prepareMLButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.prepareMLButton.Location = new System.Drawing.Point(15, 125);
		this.prepareMLButton.Name = "prepareMLButton";
		this.prepareMLButton.Size = new System.Drawing.Size(100, 45);
		this.prepareMLButton.TabIndex = 23;
		this.prepareMLButton.Text = "Prep ML";
		this.prepareMLButton.UseVisualStyleBackColor = false;
		this.prepareMLButton.Click += new System.EventHandler(prepareMLButton_Click);
		this.inviteToMLButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.inviteToMLButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.inviteToMLButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.inviteToMLButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.inviteToMLButton.Location = new System.Drawing.Point(15, 70);
		this.inviteToMLButton.Name = "inviteToMLButton";
		this.inviteToMLButton.Size = new System.Drawing.Size(100, 45);
		this.inviteToMLButton.TabIndex = 22;
		this.inviteToMLButton.Text = "Invite";
		this.inviteToMLButton.UseVisualStyleBackColor = false;
		this.inviteToMLButton.Click += new System.EventHandler(inviteToMLButton_Click);
		this.MLTabInviterLabel.AutoSize = true;
		this.MLTabInviterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MLTabInviterLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MLTabInviterLabel.Location = new System.Drawing.Point(15, 565);
		this.MLTabInviterLabel.Name = "MLTabInviterLabel";
		this.MLTabInviterLabel.Size = new System.Drawing.Size(111, 20);
		this.MLTabInviterLabel.TabIndex = 21;
		this.MLTabInviterLabel.Text = "Inviter: None";
		this.MLTabInviterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.useBuffsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.useBuffsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.useBuffsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useBuffsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useBuffsButton.Location = new System.Drawing.Point(15, 15);
		this.useBuffsButton.Name = "useBuffsButton";
		this.useBuffsButton.Size = new System.Drawing.Size(100, 45);
		this.useBuffsButton.TabIndex = 20;
		this.useBuffsButton.Text = "Buffs";
		this.useBuffsButton.UseVisualStyleBackColor = false;
		this.useBuffsButton.Click += new System.EventHandler(useBuffsButton_Click);
		this.SettingsPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.SettingsPanel.Controls.Add(this.SettingsWaypointsMenuButton);
		this.SettingsPanel.Controls.Add(this.BuyLicenseButton);
		this.SettingsPanel.Controls.Add(this.ChangeNicknameButton);
		this.SettingsPanel.Controls.Add(this.ResetArcaneWisdomButton);
		this.SettingsPanel.Controls.Add(this.editArcaneWisdomControlPictureBox);
		this.SettingsPanel.Controls.Add(this.arcaneWisdomLabel);
		this.SettingsPanel.Controls.Add(this.arcaneWisdomControlLabel);
		this.SettingsPanel.Controls.Add(this.SettingsSoundsMenuButton);
		this.SettingsPanel.Controls.Add(this.ResetUseDebuffsButton);
		this.SettingsPanel.Controls.Add(this.editUseDebuffsControlPictureBox);
		this.SettingsPanel.Controls.Add(this.useDebuffsSettingsLabel);
		this.SettingsPanel.Controls.Add(this.useDebuffsControlLabel);
		this.SettingsPanel.Controls.Add(this.SettingsTooltipsButton);
		this.SettingsPanel.Controls.Add(this.TooltipsSettingsLabel);
		this.SettingsPanel.Controls.Add(this.SettingsLowSpecButton);
		this.SettingsPanel.Controls.Add(this.LowSpecSettingsLabel);
		this.SettingsPanel.Controls.Add(this.ResetUseBuffset3Button);
		this.SettingsPanel.Controls.Add(this.editUseBuffset3ControlPictureBox);
		this.SettingsPanel.Controls.Add(this.useBuffset3SettingsLabel);
		this.SettingsPanel.Controls.Add(this.useBuffset3ControlLabel);
		this.SettingsPanel.Controls.Add(this.ResetUseBuffset2Button);
		this.SettingsPanel.Controls.Add(this.editUseBuffset2ControlPictureBox);
		this.SettingsPanel.Controls.Add(this.useBuffset2SettingsLabel);
		this.SettingsPanel.Controls.Add(this.useBuffset2ControlLabel);
		this.SettingsPanel.Controls.Add(this.ResetUseBuffset1Button);
		this.SettingsPanel.Controls.Add(this.editUseBuffset1ControlPictureBox);
		this.SettingsPanel.Controls.Add(this.useBuffset1SettingsLabel);
		this.SettingsPanel.Controls.Add(this.useBuffset1ControlLabel);
		this.SettingsPanel.Controls.Add(this.SettingsDelayItemsTextBox);
		this.SettingsPanel.Controls.Add(this.SettingsDelayItemsLabel);
		this.SettingsPanel.Controls.Add(this.ResetUseSelfBuffsButton);
		this.SettingsPanel.Controls.Add(this.ResetWearSPButton);
		this.SettingsPanel.Controls.Add(this.ResetMassHealButton);
		this.SettingsPanel.Controls.Add(this.ResetExitRaidButton);
		this.SettingsPanel.Controls.Add(this.ResetJoinListButton);
		this.SettingsPanel.Controls.Add(this.ResetInviteButton);
		this.SettingsPanel.Controls.Add(this.ResetUseBuffsButton);
		this.SettingsPanel.Controls.Add(this.NetworkDeviceCombobox);
		this.SettingsPanel.Controls.Add(this.NetworkDeviceLabel);
		this.SettingsPanel.Controls.Add(this.SettingsSoundsButton);
		this.SettingsPanel.Controls.Add(this.SoundsSettingsLabel);
		this.SettingsPanel.Controls.Add(this.WindowSizeComboBox);
		this.SettingsPanel.Controls.Add(this.WindowSizeLabel);
		this.SettingsPanel.Controls.Add(this.DefaultSettingsButton);
		this.SettingsPanel.Controls.Add(this.editExitRaidControlPictureBox);
		this.SettingsPanel.Controls.Add(this.ExitRaidSettingsLabel);
		this.SettingsPanel.Controls.Add(this.ExitRaidControlLabel);
		this.SettingsPanel.Controls.Add(this.SettingsHotkeysButton);
		this.SettingsPanel.Controls.Add(this.HotkeysSettingLabel);
		this.SettingsPanel.Controls.Add(this.editJoinListControlPictureBox);
		this.SettingsPanel.Controls.Add(this.JoinListSettingsLabel);
		this.SettingsPanel.Controls.Add(this.JoinListControlLabel);
		this.SettingsPanel.Controls.Add(this.editUseSelfBuffsControlPictureBox);
		this.SettingsPanel.Controls.Add(this.useSelfBuffsSettingsLabel);
		this.SettingsPanel.Controls.Add(this.editWearSPControlPictureBox);
		this.SettingsPanel.Controls.Add(this.wearSPSettingsLabel);
		this.SettingsPanel.Controls.Add(this.editMassHealControlPictureBox);
		this.SettingsPanel.Controls.Add(this.MassHealSettingsLabel);
		this.SettingsPanel.Controls.Add(this.editInviteControlPictureBox);
		this.SettingsPanel.Controls.Add(this.InviteSettingsLabel);
		this.SettingsPanel.Controls.Add(this.editUseBufssControlPictureBox);
		this.SettingsPanel.Controls.Add(this.InviteControlLabel);
		this.SettingsPanel.Controls.Add(this.MassHealControlLabel);
		this.SettingsPanel.Controls.Add(this.WearSPControlLabel);
		this.SettingsPanel.Controls.Add(this.useSelfBuffsControlLabel);
		this.SettingsPanel.Controls.Add(this.useBuffsControlLabel);
		this.SettingsPanel.Controls.Add(this.useBuffsSettingsLabel);
		this.SettingsPanel.Controls.Add(this.SettingsDelayInviteTextBox);
		this.SettingsPanel.Controls.Add(this.SettingsDelayInviteLabel);
		this.SettingsPanel.Controls.Add(this.SettingsDelayMoveTextBox);
		this.SettingsPanel.Controls.Add(this.SettingsDelayMoveLabel);
		this.SettingsPanel.Controls.Add(this.SettingsDelayRaidTextBox);
		this.SettingsPanel.Controls.Add(this.SettingsDelayRaidLabel);
		this.SettingsPanel.Controls.Add(this.SettingsRenameButton);
		this.SettingsPanel.Controls.Add(this.RenameSettingLabel);
		this.SettingsPanel.Controls.Add(this.SettingsDelayBuffTextBox);
		this.SettingsPanel.Controls.Add(this.AddLicenseButton);
		this.SettingsPanel.Controls.Add(this.SettingsDelayBuffLabel);
		this.SettingsPanel.Location = new System.Drawing.Point(25, 70);
		this.SettingsPanel.Name = "SettingsPanel";
		this.SettingsPanel.Size = new System.Drawing.Size(900, 600);
		this.SettingsPanel.TabIndex = 6;
		this.SettingsPanel.Click += new System.EventHandler(SettingsPanel_Click);
		this.SettingsWaypointsMenuButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.SettingsWaypointsMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SettingsWaypointsMenuButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SettingsWaypointsMenuButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsWaypointsMenuButton.Location = new System.Drawing.Point(700, 265);
		this.SettingsWaypointsMenuButton.Name = "SettingsWaypointsMenuButton";
		this.SettingsWaypointsMenuButton.Size = new System.Drawing.Size(150, 45);
		this.SettingsWaypointsMenuButton.TabIndex = 106;
		this.SettingsWaypointsMenuButton.Text = "Waypoints";
		this.SettingsWaypointsMenuButton.UseVisualStyleBackColor = false;
		this.SettingsWaypointsMenuButton.Click += new System.EventHandler(SettingsWaypointsMenuButton_Click);
		this.BuyLicenseButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.BuyLicenseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.BuyLicenseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.BuyLicenseButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.BuyLicenseButton.Location = new System.Drawing.Point(25, 475);
		this.BuyLicenseButton.Name = "BuyLicenseButton";
		this.BuyLicenseButton.Size = new System.Drawing.Size(130, 45);
		this.BuyLicenseButton.TabIndex = 105;
		this.BuyLicenseButton.Text = "Buy License";
		this.BuyLicenseButton.UseVisualStyleBackColor = false;
		this.BuyLicenseButton.Click += new System.EventHandler(BuyLicenseButton_Click);
		this.ChangeNicknameButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.ChangeNicknameButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ChangeNicknameButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ChangeNicknameButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ChangeNicknameButton.Location = new System.Drawing.Point(25, 415);
		this.ChangeNicknameButton.Name = "ChangeNicknameButton";
		this.ChangeNicknameButton.Size = new System.Drawing.Size(130, 45);
		this.ChangeNicknameButton.TabIndex = 104;
		this.ChangeNicknameButton.Text = "Change Nickname";
		this.ChangeNicknameButton.UseVisualStyleBackColor = false;
		this.ChangeNicknameButton.Click += new System.EventHandler(ChangeNicknameButton_Click);
		this.ResetArcaneWisdomButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetArcaneWisdomButton.Location = new System.Drawing.Point(586, 405);
		this.ResetArcaneWisdomButton.Name = "ResetArcaneWisdomButton";
		this.ResetArcaneWisdomButton.Size = new System.Drawing.Size(30, 30);
		this.ResetArcaneWisdomButton.TabIndex = 101;
		this.ResetArcaneWisdomButton.TabStop = false;
		this.ResetArcaneWisdomButton.Click += new System.EventHandler(ResetArcaneWisdomButton_Click);
		this.editArcaneWisdomControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editArcaneWisdomControlPictureBox.Location = new System.Drawing.Point(550, 405);
		this.editArcaneWisdomControlPictureBox.Name = "editArcaneWisdomControlPictureBox";
		this.editArcaneWisdomControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editArcaneWisdomControlPictureBox.TabIndex = 99;
		this.editArcaneWisdomControlPictureBox.TabStop = false;
		this.editArcaneWisdomControlPictureBox.Click += new System.EventHandler(editArcaneWisdomControlPictureBox_Click);
		this.arcaneWisdomLabel.AutoSize = true;
		this.arcaneWisdomLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.arcaneWisdomLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.arcaneWisdomLabel.Location = new System.Drawing.Point(340, 410);
		this.arcaneWisdomLabel.Name = "arcaneWisdomLabel";
		this.arcaneWisdomLabel.Size = new System.Drawing.Size(114, 20);
		this.arcaneWisdomLabel.TabIndex = 98;
		this.arcaneWisdomLabel.Text = "Arc. Wisdom:";
		this.arcaneWisdomLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.arcaneWisdomControlLabel.AutoSize = true;
		this.arcaneWisdomControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.arcaneWisdomControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.arcaneWisdomControlLabel.Location = new System.Drawing.Point(465, 410);
		this.arcaneWisdomControlLabel.Name = "arcaneWisdomControlLabel";
		this.arcaneWisdomControlLabel.Size = new System.Drawing.Size(51, 20);
		this.arcaneWisdomControlLabel.TabIndex = 100;
		this.arcaneWisdomControlLabel.Text = "None";
		this.arcaneWisdomControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsSoundsMenuButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.SettingsSoundsMenuButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SettingsSoundsMenuButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SettingsSoundsMenuButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsSoundsMenuButton.Location = new System.Drawing.Point(700, 205);
		this.SettingsSoundsMenuButton.Name = "SettingsSoundsMenuButton";
		this.SettingsSoundsMenuButton.Size = new System.Drawing.Size(150, 45);
		this.SettingsSoundsMenuButton.TabIndex = 97;
		this.SettingsSoundsMenuButton.Text = "Sounds";
		this.SettingsSoundsMenuButton.UseVisualStyleBackColor = false;
		this.SettingsSoundsMenuButton.Click += new System.EventHandler(SettingsSoundsMenuButton_Click);
		this.ResetUseDebuffsButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetUseDebuffsButton.Location = new System.Drawing.Point(586, 370);
		this.ResetUseDebuffsButton.Name = "ResetUseDebuffsButton";
		this.ResetUseDebuffsButton.Size = new System.Drawing.Size(30, 30);
		this.ResetUseDebuffsButton.TabIndex = 96;
		this.ResetUseDebuffsButton.TabStop = false;
		this.ResetUseDebuffsButton.Click += new System.EventHandler(ResetUseDebuffsButton_Click);
		this.editUseDebuffsControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editUseDebuffsControlPictureBox.Location = new System.Drawing.Point(550, 370);
		this.editUseDebuffsControlPictureBox.Name = "editUseDebuffsControlPictureBox";
		this.editUseDebuffsControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editUseDebuffsControlPictureBox.TabIndex = 94;
		this.editUseDebuffsControlPictureBox.TabStop = false;
		this.editUseDebuffsControlPictureBox.Click += new System.EventHandler(editUseDebuffsControlPictureBox_Click);
		this.useDebuffsSettingsLabel.AutoSize = true;
		this.useDebuffsSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useDebuffsSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useDebuffsSettingsLabel.Location = new System.Drawing.Point(340, 375);
		this.useDebuffsSettingsLabel.Name = "useDebuffsSettingsLabel";
		this.useDebuffsSettingsLabel.Size = new System.Drawing.Size(115, 20);
		this.useDebuffsSettingsLabel.TabIndex = 93;
		this.useDebuffsSettingsLabel.Text = "Use Debuffs:";
		this.useDebuffsSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.useDebuffsControlLabel.AutoSize = true;
		this.useDebuffsControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useDebuffsControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useDebuffsControlLabel.Location = new System.Drawing.Point(465, 375);
		this.useDebuffsControlLabel.Name = "useDebuffsControlLabel";
		this.useDebuffsControlLabel.Size = new System.Drawing.Size(51, 20);
		this.useDebuffsControlLabel.TabIndex = 95;
		this.useDebuffsControlLabel.Text = "None";
		this.useDebuffsControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsTooltipsButton.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.SettingsTooltipsButton.FlatAppearance.BorderSize = 0;
		this.SettingsTooltipsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SettingsTooltipsButton.Location = new System.Drawing.Point(824, 163);
		this.SettingsTooltipsButton.Name = "SettingsTooltipsButton";
		this.SettingsTooltipsButton.Size = new System.Drawing.Size(24, 24);
		this.SettingsTooltipsButton.TabIndex = 92;
		this.SettingsTooltipsButton.UseVisualStyleBackColor = false;
		this.SettingsTooltipsButton.Click += new System.EventHandler(SettingsTooltipsButton_Click);
		this.TooltipsSettingsLabel.AutoSize = true;
		this.TooltipsSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.TooltipsSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.TooltipsSettingsLabel.Location = new System.Drawing.Point(700, 165);
		this.TooltipsSettingsLabel.Name = "TooltipsSettingsLabel";
		this.TooltipsSettingsLabel.Size = new System.Drawing.Size(72, 20);
		this.TooltipsSettingsLabel.TabIndex = 91;
		this.TooltipsSettingsLabel.Text = "Tooltips";
		this.TooltipsSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsLowSpecButton.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.SettingsLowSpecButton.FlatAppearance.BorderSize = 0;
		this.SettingsLowSpecButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SettingsLowSpecButton.Location = new System.Drawing.Point(824, 93);
		this.SettingsLowSpecButton.Name = "SettingsLowSpecButton";
		this.SettingsLowSpecButton.Size = new System.Drawing.Size(24, 24);
		this.SettingsLowSpecButton.TabIndex = 90;
		this.SettingsLowSpecButton.UseVisualStyleBackColor = false;
		this.SettingsLowSpecButton.Click += new System.EventHandler(LowSpecSoundsButton_Click);
		this.LowSpecSettingsLabel.AutoSize = true;
		this.LowSpecSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.LowSpecSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.LowSpecSettingsLabel.Location = new System.Drawing.Point(700, 95);
		this.LowSpecSettingsLabel.Name = "LowSpecSettingsLabel";
		this.LowSpecSettingsLabel.Size = new System.Drawing.Size(87, 20);
		this.LowSpecSettingsLabel.TabIndex = 89;
		this.LowSpecSettingsLabel.Text = "Low Spec";
		this.LowSpecSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ResetUseBuffset3Button.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetUseBuffset3Button.Location = new System.Drawing.Point(586, 335);
		this.ResetUseBuffset3Button.Name = "ResetUseBuffset3Button";
		this.ResetUseBuffset3Button.Size = new System.Drawing.Size(30, 30);
		this.ResetUseBuffset3Button.TabIndex = 88;
		this.ResetUseBuffset3Button.TabStop = false;
		this.ResetUseBuffset3Button.Click += new System.EventHandler(ResetUseBuffset3Button_Click);
		this.editUseBuffset3ControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editUseBuffset3ControlPictureBox.Location = new System.Drawing.Point(550, 335);
		this.editUseBuffset3ControlPictureBox.Name = "editUseBuffset3ControlPictureBox";
		this.editUseBuffset3ControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editUseBuffset3ControlPictureBox.TabIndex = 86;
		this.editUseBuffset3ControlPictureBox.TabStop = false;
		this.editUseBuffset3ControlPictureBox.Click += new System.EventHandler(editUseBuffset3ControlPictureBox_Click);
		this.useBuffset3SettingsLabel.AutoSize = true;
		this.useBuffset3SettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useBuffset3SettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useBuffset3SettingsLabel.Location = new System.Drawing.Point(340, 340);
		this.useBuffset3SettingsLabel.Name = "useBuffset3SettingsLabel";
		this.useBuffset3SettingsLabel.Size = new System.Drawing.Size(106, 20);
		this.useBuffset3SettingsLabel.TabIndex = 85;
		this.useBuffset3SettingsLabel.Text = "Buff Set #3:";
		this.useBuffset3SettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.useBuffset3ControlLabel.AutoSize = true;
		this.useBuffset3ControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useBuffset3ControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useBuffset3ControlLabel.Location = new System.Drawing.Point(465, 340);
		this.useBuffset3ControlLabel.Name = "useBuffset3ControlLabel";
		this.useBuffset3ControlLabel.Size = new System.Drawing.Size(51, 20);
		this.useBuffset3ControlLabel.TabIndex = 87;
		this.useBuffset3ControlLabel.Text = "None";
		this.useBuffset3ControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ResetUseBuffset2Button.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetUseBuffset2Button.Location = new System.Drawing.Point(586, 300);
		this.ResetUseBuffset2Button.Name = "ResetUseBuffset2Button";
		this.ResetUseBuffset2Button.Size = new System.Drawing.Size(30, 30);
		this.ResetUseBuffset2Button.TabIndex = 84;
		this.ResetUseBuffset2Button.TabStop = false;
		this.ResetUseBuffset2Button.Click += new System.EventHandler(ResetUseBuffset2Button_Click);
		this.editUseBuffset2ControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editUseBuffset2ControlPictureBox.Location = new System.Drawing.Point(550, 300);
		this.editUseBuffset2ControlPictureBox.Name = "editUseBuffset2ControlPictureBox";
		this.editUseBuffset2ControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editUseBuffset2ControlPictureBox.TabIndex = 82;
		this.editUseBuffset2ControlPictureBox.TabStop = false;
		this.editUseBuffset2ControlPictureBox.Click += new System.EventHandler(editUseBuffset2ControlPictureBox_Click);
		this.useBuffset2SettingsLabel.AutoSize = true;
		this.useBuffset2SettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useBuffset2SettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useBuffset2SettingsLabel.Location = new System.Drawing.Point(340, 305);
		this.useBuffset2SettingsLabel.Name = "useBuffset2SettingsLabel";
		this.useBuffset2SettingsLabel.Size = new System.Drawing.Size(106, 20);
		this.useBuffset2SettingsLabel.TabIndex = 81;
		this.useBuffset2SettingsLabel.Text = "Buff Set #2:";
		this.useBuffset2SettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.useBuffset2ControlLabel.AutoSize = true;
		this.useBuffset2ControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useBuffset2ControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useBuffset2ControlLabel.Location = new System.Drawing.Point(465, 305);
		this.useBuffset2ControlLabel.Name = "useBuffset2ControlLabel";
		this.useBuffset2ControlLabel.Size = new System.Drawing.Size(51, 20);
		this.useBuffset2ControlLabel.TabIndex = 83;
		this.useBuffset2ControlLabel.Text = "None";
		this.useBuffset2ControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ResetUseBuffset1Button.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetUseBuffset1Button.Location = new System.Drawing.Point(586, 265);
		this.ResetUseBuffset1Button.Name = "ResetUseBuffset1Button";
		this.ResetUseBuffset1Button.Size = new System.Drawing.Size(30, 30);
		this.ResetUseBuffset1Button.TabIndex = 80;
		this.ResetUseBuffset1Button.TabStop = false;
		this.ResetUseBuffset1Button.Click += new System.EventHandler(ResetUseBuffset1Button_Click);
		this.editUseBuffset1ControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editUseBuffset1ControlPictureBox.Location = new System.Drawing.Point(550, 265);
		this.editUseBuffset1ControlPictureBox.Name = "editUseBuffset1ControlPictureBox";
		this.editUseBuffset1ControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editUseBuffset1ControlPictureBox.TabIndex = 78;
		this.editUseBuffset1ControlPictureBox.TabStop = false;
		this.editUseBuffset1ControlPictureBox.Click += new System.EventHandler(editUseBuffset1ControlPictureBox_Click);
		this.useBuffset1SettingsLabel.AutoSize = true;
		this.useBuffset1SettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useBuffset1SettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useBuffset1SettingsLabel.Location = new System.Drawing.Point(340, 270);
		this.useBuffset1SettingsLabel.Name = "useBuffset1SettingsLabel";
		this.useBuffset1SettingsLabel.Size = new System.Drawing.Size(106, 20);
		this.useBuffset1SettingsLabel.TabIndex = 77;
		this.useBuffset1SettingsLabel.Text = "Buff Set #1:";
		this.useBuffset1SettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.useBuffset1ControlLabel.AutoSize = true;
		this.useBuffset1ControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useBuffset1ControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useBuffset1ControlLabel.Location = new System.Drawing.Point(465, 270);
		this.useBuffset1ControlLabel.Name = "useBuffset1ControlLabel";
		this.useBuffset1ControlLabel.Size = new System.Drawing.Size(51, 20);
		this.useBuffset1ControlLabel.TabIndex = 79;
		this.useBuffset1ControlLabel.Text = "None";
		this.useBuffset1ControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsDelayItemsTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.SettingsDelayItemsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.SettingsDelayItemsTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.SettingsDelayItemsTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayItemsTextBox.Location = new System.Drawing.Point(130, 180);
		this.SettingsDelayItemsTextBox.Name = "SettingsDelayItemsTextBox";
		this.SettingsDelayItemsTextBox.Size = new System.Drawing.Size(64, 26);
		this.SettingsDelayItemsTextBox.TabIndex = 76;
		this.SettingsDelayItemsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SettingsDelayItemsTextBox.Leave += new System.EventHandler(SettingsDelayItemsTextBox_Leave);
		this.SettingsDelayItemsLabel.AutoSize = true;
		this.SettingsDelayItemsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SettingsDelayItemsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayItemsLabel.Location = new System.Drawing.Point(25, 185);
		this.SettingsDelayItemsLabel.Name = "SettingsDelayItemsLabel";
		this.SettingsDelayItemsLabel.Size = new System.Drawing.Size(104, 20);
		this.SettingsDelayItemsLabel.TabIndex = 75;
		this.SettingsDelayItemsLabel.Text = "Delay Items";
		this.SettingsDelayItemsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ResetUseSelfBuffsButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetUseSelfBuffsButton.Location = new System.Drawing.Point(586, 230);
		this.ResetUseSelfBuffsButton.Name = "ResetUseSelfBuffsButton";
		this.ResetUseSelfBuffsButton.Size = new System.Drawing.Size(30, 30);
		this.ResetUseSelfBuffsButton.TabIndex = 74;
		this.ResetUseSelfBuffsButton.TabStop = false;
		this.ResetUseSelfBuffsButton.Click += new System.EventHandler(ResetUseSelfBuffsButton_Click);
		this.ResetWearSPButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetWearSPButton.Location = new System.Drawing.Point(586, 195);
		this.ResetWearSPButton.Name = "ResetWearSPButton";
		this.ResetWearSPButton.Size = new System.Drawing.Size(30, 30);
		this.ResetWearSPButton.TabIndex = 72;
		this.ResetWearSPButton.TabStop = false;
		this.ResetWearSPButton.Click += new System.EventHandler(ResetWearSPButton_Click);
		this.ResetMassHealButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetMassHealButton.Location = new System.Drawing.Point(586, 160);
		this.ResetMassHealButton.Name = "ResetMassHealButton";
		this.ResetMassHealButton.Size = new System.Drawing.Size(30, 30);
		this.ResetMassHealButton.TabIndex = 71;
		this.ResetMassHealButton.TabStop = false;
		this.ResetMassHealButton.Click += new System.EventHandler(ResetMassHealButton_Click);
		this.ResetExitRaidButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetExitRaidButton.Location = new System.Drawing.Point(586, 125);
		this.ResetExitRaidButton.Name = "ResetExitRaidButton";
		this.ResetExitRaidButton.Size = new System.Drawing.Size(30, 30);
		this.ResetExitRaidButton.TabIndex = 70;
		this.ResetExitRaidButton.TabStop = false;
		this.ResetExitRaidButton.Click += new System.EventHandler(ResetExitRaidButton_Click);
		this.ResetJoinListButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetJoinListButton.Location = new System.Drawing.Point(586, 90);
		this.ResetJoinListButton.Name = "ResetJoinListButton";
		this.ResetJoinListButton.Size = new System.Drawing.Size(30, 30);
		this.ResetJoinListButton.TabIndex = 69;
		this.ResetJoinListButton.TabStop = false;
		this.ResetJoinListButton.Click += new System.EventHandler(ResetJoinListButton_Click);
		this.ResetInviteButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetInviteButton.Location = new System.Drawing.Point(586, 55);
		this.ResetInviteButton.Name = "ResetInviteButton";
		this.ResetInviteButton.Size = new System.Drawing.Size(30, 30);
		this.ResetInviteButton.TabIndex = 68;
		this.ResetInviteButton.TabStop = false;
		this.ResetInviteButton.Click += new System.EventHandler(ResetInviteButton_Click);
		this.ResetUseBuffsButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.ResetUseBuffsButton.Location = new System.Drawing.Point(586, 20);
		this.ResetUseBuffsButton.Name = "ResetUseBuffsButton";
		this.ResetUseBuffsButton.Size = new System.Drawing.Size(30, 30);
		this.ResetUseBuffsButton.TabIndex = 67;
		this.ResetUseBuffsButton.TabStop = false;
		this.ResetUseBuffsButton.Click += new System.EventHandler(ResetUseBuffsButton_Click);
		this.NetworkDeviceCombobox.BackColor = System.Drawing.Color.Transparent;
		this.NetworkDeviceCombobox.Location = new System.Drawing.Point(129, 314);
		this.NetworkDeviceCombobox.Name = "NetworkDeviceCombobox";
		this.NetworkDeviceCombobox.Size = new System.Drawing.Size(152, 32);
		this.NetworkDeviceCombobox.TabIndex = 66;
		this.NetworkDeviceLabel.AutoSize = true;
		this.NetworkDeviceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.NetworkDeviceLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.NetworkDeviceLabel.Location = new System.Drawing.Point(25, 316);
		this.NetworkDeviceLabel.Name = "NetworkDeviceLabel";
		this.NetworkDeviceLabel.Size = new System.Drawing.Size(96, 20);
		this.NetworkDeviceLabel.TabIndex = 65;
		this.NetworkDeviceLabel.Text = "Net Device";
		this.NetworkDeviceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsSoundsButton.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.SettingsSoundsButton.FlatAppearance.BorderSize = 0;
		this.SettingsSoundsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SettingsSoundsButton.Location = new System.Drawing.Point(824, 128);
		this.SettingsSoundsButton.Name = "SettingsSoundsButton";
		this.SettingsSoundsButton.Size = new System.Drawing.Size(24, 24);
		this.SettingsSoundsButton.TabIndex = 60;
		this.SettingsSoundsButton.UseVisualStyleBackColor = false;
		this.SettingsSoundsButton.Click += new System.EventHandler(SettingsSoundsButton_Click);
		this.SoundsSettingsLabel.AutoSize = true;
		this.SoundsSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SoundsSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SoundsSettingsLabel.Location = new System.Drawing.Point(700, 130);
		this.SoundsSettingsLabel.Name = "SoundsSettingsLabel";
		this.SoundsSettingsLabel.Size = new System.Drawing.Size(70, 20);
		this.SoundsSettingsLabel.TabIndex = 59;
		this.SoundsSettingsLabel.Text = "Sounds";
		this.SoundsSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.WindowSizeComboBox.BackColor = System.Drawing.Color.Transparent;
		this.WindowSizeComboBox.Location = new System.Drawing.Point(129, 255);
		this.WindowSizeComboBox.Name = "WindowSizeComboBox";
		this.WindowSizeComboBox.Size = new System.Drawing.Size(152, 35);
		this.WindowSizeComboBox.TabIndex = 58;
		this.WindowSizeLabel.AutoSize = true;
		this.WindowSizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.WindowSizeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.WindowSizeLabel.Location = new System.Drawing.Point(25, 257);
		this.WindowSizeLabel.Name = "WindowSizeLabel";
		this.WindowSizeLabel.Size = new System.Drawing.Size(81, 20);
		this.WindowSizeLabel.TabIndex = 57;
		this.WindowSizeLabel.Text = "App Size";
		this.WindowSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.DefaultSettingsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.DefaultSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.DefaultSettingsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.DefaultSettingsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.DefaultSettingsButton.Location = new System.Drawing.Point(775, 524);
		this.DefaultSettingsButton.Name = "DefaultSettingsButton";
		this.DefaultSettingsButton.Size = new System.Drawing.Size(100, 45);
		this.DefaultSettingsButton.TabIndex = 56;
		this.DefaultSettingsButton.Text = "Default";
		this.DefaultSettingsButton.UseVisualStyleBackColor = false;
		this.DefaultSettingsButton.Click += new System.EventHandler(DefaultSettingsButton_Click);
		this.editExitRaidControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editExitRaidControlPictureBox.Location = new System.Drawing.Point(550, 125);
		this.editExitRaidControlPictureBox.Name = "editExitRaidControlPictureBox";
		this.editExitRaidControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editExitRaidControlPictureBox.TabIndex = 54;
		this.editExitRaidControlPictureBox.TabStop = false;
		this.editExitRaidControlPictureBox.Click += new System.EventHandler(editExitRaidControlPictureBox_Click);
		this.editExitRaidControlPictureBox.MouseEnter += new System.EventHandler(editControlPictureBox_MouseEnter);
		this.editExitRaidControlPictureBox.MouseLeave += new System.EventHandler(editControlPictureBox_MouseLeave);
		this.ExitRaidSettingsLabel.AutoSize = true;
		this.ExitRaidSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ExitRaidSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ExitRaidSettingsLabel.Location = new System.Drawing.Point(340, 130);
		this.ExitRaidSettingsLabel.Name = "ExitRaidSettingsLabel";
		this.ExitRaidSettingsLabel.Size = new System.Drawing.Size(81, 20);
		this.ExitRaidSettingsLabel.TabIndex = 53;
		this.ExitRaidSettingsLabel.Text = "Exit Raid";
		this.ExitRaidSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ExitRaidControlLabel.AutoSize = true;
		this.ExitRaidControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ExitRaidControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ExitRaidControlLabel.Location = new System.Drawing.Point(465, 130);
		this.ExitRaidControlLabel.Name = "ExitRaidControlLabel";
		this.ExitRaidControlLabel.Size = new System.Drawing.Size(51, 20);
		this.ExitRaidControlLabel.TabIndex = 55;
		this.ExitRaidControlLabel.Text = "None";
		this.ExitRaidControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsHotkeysButton.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.SettingsHotkeysButton.FlatAppearance.BorderSize = 0;
		this.SettingsHotkeysButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SettingsHotkeysButton.Location = new System.Drawing.Point(824, 58);
		this.SettingsHotkeysButton.Name = "SettingsHotkeysButton";
		this.SettingsHotkeysButton.Size = new System.Drawing.Size(24, 24);
		this.SettingsHotkeysButton.TabIndex = 52;
		this.SettingsHotkeysButton.UseVisualStyleBackColor = false;
		this.SettingsHotkeysButton.Click += new System.EventHandler(SettingsHotkeysButton_Click);
		this.HotkeysSettingLabel.AutoSize = true;
		this.HotkeysSettingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.HotkeysSettingLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.HotkeysSettingLabel.Location = new System.Drawing.Point(700, 60);
		this.HotkeysSettingLabel.Name = "HotkeysSettingLabel";
		this.HotkeysSettingLabel.Size = new System.Drawing.Size(74, 20);
		this.HotkeysSettingLabel.TabIndex = 51;
		this.HotkeysSettingLabel.Text = "Hotkeys";
		this.HotkeysSettingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.editJoinListControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editJoinListControlPictureBox.Location = new System.Drawing.Point(550, 90);
		this.editJoinListControlPictureBox.Name = "editJoinListControlPictureBox";
		this.editJoinListControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editJoinListControlPictureBox.TabIndex = 49;
		this.editJoinListControlPictureBox.TabStop = false;
		this.editJoinListControlPictureBox.Click += new System.EventHandler(editJoinListControlPictureBox_Click);
		this.editJoinListControlPictureBox.MouseEnter += new System.EventHandler(editControlPictureBox_MouseEnter);
		this.editJoinListControlPictureBox.MouseLeave += new System.EventHandler(editControlPictureBox_MouseLeave);
		this.JoinListSettingsLabel.AutoSize = true;
		this.JoinListSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.JoinListSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.JoinListSettingsLabel.Location = new System.Drawing.Point(340, 95);
		this.JoinListSettingsLabel.Name = "JoinListSettingsLabel";
		this.JoinListSettingsLabel.Size = new System.Drawing.Size(76, 20);
		this.JoinListSettingsLabel.TabIndex = 48;
		this.JoinListSettingsLabel.Text = "Join List";
		this.JoinListSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.JoinListControlLabel.AutoSize = true;
		this.JoinListControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.JoinListControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.JoinListControlLabel.Location = new System.Drawing.Point(465, 95);
		this.JoinListControlLabel.Name = "JoinListControlLabel";
		this.JoinListControlLabel.Size = new System.Drawing.Size(51, 20);
		this.JoinListControlLabel.TabIndex = 50;
		this.JoinListControlLabel.Text = "None";
		this.JoinListControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.editUseSelfBuffsControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editUseSelfBuffsControlPictureBox.Location = new System.Drawing.Point(550, 230);
		this.editUseSelfBuffsControlPictureBox.Name = "editUseSelfBuffsControlPictureBox";
		this.editUseSelfBuffsControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editUseSelfBuffsControlPictureBox.TabIndex = 41;
		this.editUseSelfBuffsControlPictureBox.TabStop = false;
		this.editUseSelfBuffsControlPictureBox.Click += new System.EventHandler(editUseSelfBuffsControl_Click);
		this.editUseSelfBuffsControlPictureBox.MouseEnter += new System.EventHandler(editControlPictureBox_MouseEnter);
		this.editUseSelfBuffsControlPictureBox.MouseLeave += new System.EventHandler(editControlPictureBox_MouseLeave);
		this.useSelfBuffsSettingsLabel.AutoSize = true;
		this.useSelfBuffsSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useSelfBuffsSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useSelfBuffsSettingsLabel.Location = new System.Drawing.Point(340, 235);
		this.useSelfBuffsSettingsLabel.Name = "useSelfBuffsSettingsLabel";
		this.useSelfBuffsSettingsLabel.Size = new System.Drawing.Size(94, 20);
		this.useSelfBuffsSettingsLabel.TabIndex = 40;
		this.useSelfBuffsSettingsLabel.Text = "Self Buffs:";
		this.useSelfBuffsSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.editWearSPControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editWearSPControlPictureBox.Location = new System.Drawing.Point(550, 195);
		this.editWearSPControlPictureBox.Name = "editWearSPControlPictureBox";
		this.editWearSPControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editWearSPControlPictureBox.TabIndex = 37;
		this.editWearSPControlPictureBox.TabStop = false;
		this.editWearSPControlPictureBox.Click += new System.EventHandler(editWearSPControl_Click);
		this.editWearSPControlPictureBox.MouseEnter += new System.EventHandler(editControlPictureBox_MouseEnter);
		this.editWearSPControlPictureBox.MouseLeave += new System.EventHandler(editControlPictureBox_MouseLeave);
		this.wearSPSettingsLabel.AutoSize = true;
		this.wearSPSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.wearSPSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.wearSPSettingsLabel.Location = new System.Drawing.Point(340, 200);
		this.wearSPSettingsLabel.Name = "wearSPSettingsLabel";
		this.wearSPSettingsLabel.Size = new System.Drawing.Size(84, 20);
		this.wearSPSettingsLabel.TabIndex = 36;
		this.wearSPSettingsLabel.Text = "Wear SP:";
		this.wearSPSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.editMassHealControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editMassHealControlPictureBox.Location = new System.Drawing.Point(550, 160);
		this.editMassHealControlPictureBox.Name = "editMassHealControlPictureBox";
		this.editMassHealControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editMassHealControlPictureBox.TabIndex = 35;
		this.editMassHealControlPictureBox.TabStop = false;
		this.editMassHealControlPictureBox.Click += new System.EventHandler(editMoveControl_Click);
		this.editMassHealControlPictureBox.MouseEnter += new System.EventHandler(editControlPictureBox_MouseEnter);
		this.editMassHealControlPictureBox.MouseLeave += new System.EventHandler(editControlPictureBox_MouseLeave);
		this.MassHealSettingsLabel.AutoSize = true;
		this.MassHealSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MassHealSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MassHealSettingsLabel.Location = new System.Drawing.Point(340, 165);
		this.MassHealSettingsLabel.Name = "MassHealSettingsLabel";
		this.MassHealSettingsLabel.Size = new System.Drawing.Size(98, 20);
		this.MassHealSettingsLabel.TabIndex = 34;
		this.MassHealSettingsLabel.Text = "Mass Heal:";
		this.MassHealSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.editInviteControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editInviteControlPictureBox.Location = new System.Drawing.Point(550, 55);
		this.editInviteControlPictureBox.Name = "editInviteControlPictureBox";
		this.editInviteControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editInviteControlPictureBox.TabIndex = 33;
		this.editInviteControlPictureBox.TabStop = false;
		this.editInviteControlPictureBox.Click += new System.EventHandler(editInviteControl_Click);
		this.editInviteControlPictureBox.MouseEnter += new System.EventHandler(editControlPictureBox_MouseEnter);
		this.editInviteControlPictureBox.MouseLeave += new System.EventHandler(editControlPictureBox_MouseLeave);
		this.InviteSettingsLabel.AutoSize = true;
		this.InviteSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.InviteSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.InviteSettingsLabel.Location = new System.Drawing.Point(340, 60);
		this.InviteSettingsLabel.Name = "InviteSettingsLabel";
		this.InviteSettingsLabel.Size = new System.Drawing.Size(63, 20);
		this.InviteSettingsLabel.TabIndex = 32;
		this.InviteSettingsLabel.Text = "Invite: ";
		this.InviteSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.editUseBufssControlPictureBox.Image = NosAssistant2.Properties.Resources.edit_icon;
		this.editUseBufssControlPictureBox.Location = new System.Drawing.Point(550, 20);
		this.editUseBufssControlPictureBox.Name = "editUseBufssControlPictureBox";
		this.editUseBufssControlPictureBox.Size = new System.Drawing.Size(30, 30);
		this.editUseBufssControlPictureBox.TabIndex = 31;
		this.editUseBufssControlPictureBox.TabStop = false;
		this.editUseBufssControlPictureBox.Click += new System.EventHandler(editUseBufssControl_Click);
		this.editUseBufssControlPictureBox.MouseEnter += new System.EventHandler(editControlPictureBox_MouseEnter);
		this.editUseBufssControlPictureBox.MouseLeave += new System.EventHandler(editControlPictureBox_MouseLeave);
		this.InviteControlLabel.AutoSize = true;
		this.InviteControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.InviteControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.InviteControlLabel.Location = new System.Drawing.Point(465, 60);
		this.InviteControlLabel.Name = "InviteControlLabel";
		this.InviteControlLabel.Size = new System.Drawing.Size(51, 20);
		this.InviteControlLabel.TabIndex = 47;
		this.InviteControlLabel.Text = "None";
		this.InviteControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MassHealControlLabel.AutoSize = true;
		this.MassHealControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MassHealControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MassHealControlLabel.Location = new System.Drawing.Point(465, 165);
		this.MassHealControlLabel.Name = "MassHealControlLabel";
		this.MassHealControlLabel.Size = new System.Drawing.Size(51, 20);
		this.MassHealControlLabel.TabIndex = 46;
		this.MassHealControlLabel.Text = "None";
		this.MassHealControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.WearSPControlLabel.AutoSize = true;
		this.WearSPControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.WearSPControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.WearSPControlLabel.Location = new System.Drawing.Point(465, 200);
		this.WearSPControlLabel.Name = "WearSPControlLabel";
		this.WearSPControlLabel.Size = new System.Drawing.Size(51, 20);
		this.WearSPControlLabel.TabIndex = 45;
		this.WearSPControlLabel.Text = "None";
		this.WearSPControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.useSelfBuffsControlLabel.AutoSize = true;
		this.useSelfBuffsControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useSelfBuffsControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useSelfBuffsControlLabel.Location = new System.Drawing.Point(465, 235);
		this.useSelfBuffsControlLabel.Name = "useSelfBuffsControlLabel";
		this.useSelfBuffsControlLabel.Size = new System.Drawing.Size(51, 20);
		this.useSelfBuffsControlLabel.TabIndex = 43;
		this.useSelfBuffsControlLabel.Text = "None";
		this.useSelfBuffsControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.useBuffsControlLabel.AutoSize = true;
		this.useBuffsControlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useBuffsControlLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useBuffsControlLabel.Location = new System.Drawing.Point(465, 25);
		this.useBuffsControlLabel.Name = "useBuffsControlLabel";
		this.useBuffsControlLabel.Size = new System.Drawing.Size(51, 20);
		this.useBuffsControlLabel.TabIndex = 42;
		this.useBuffsControlLabel.Text = "None";
		this.useBuffsControlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.useBuffsSettingsLabel.AutoSize = true;
		this.useBuffsSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.useBuffsSettingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.useBuffsSettingsLabel.Location = new System.Drawing.Point(340, 25);
		this.useBuffsSettingsLabel.Name = "useBuffsSettingsLabel";
		this.useBuffsSettingsLabel.Size = new System.Drawing.Size(99, 20);
		this.useBuffsSettingsLabel.TabIndex = 30;
		this.useBuffsSettingsLabel.Text = "Use Buffs: ";
		this.useBuffsSettingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsDelayInviteTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.SettingsDelayInviteTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.SettingsDelayInviteTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.SettingsDelayInviteTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayInviteTextBox.Location = new System.Drawing.Point(130, 140);
		this.SettingsDelayInviteTextBox.Name = "SettingsDelayInviteTextBox";
		this.SettingsDelayInviteTextBox.Size = new System.Drawing.Size(64, 26);
		this.SettingsDelayInviteTextBox.TabIndex = 29;
		this.SettingsDelayInviteTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SettingsDelayInviteTextBox.Leave += new System.EventHandler(SettingsDelayInviteTextBox_Leave);
		this.SettingsDelayInviteLabel.AutoSize = true;
		this.SettingsDelayInviteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SettingsDelayInviteLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayInviteLabel.Location = new System.Drawing.Point(25, 145);
		this.SettingsDelayInviteLabel.Name = "SettingsDelayInviteLabel";
		this.SettingsDelayInviteLabel.Size = new System.Drawing.Size(103, 20);
		this.SettingsDelayInviteLabel.TabIndex = 28;
		this.SettingsDelayInviteLabel.Text = "Delay Invite";
		this.SettingsDelayInviteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsDelayMoveTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.SettingsDelayMoveTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.SettingsDelayMoveTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.SettingsDelayMoveTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayMoveTextBox.Location = new System.Drawing.Point(130, 100);
		this.SettingsDelayMoveTextBox.Name = "SettingsDelayMoveTextBox";
		this.SettingsDelayMoveTextBox.Size = new System.Drawing.Size(64, 26);
		this.SettingsDelayMoveTextBox.TabIndex = 27;
		this.SettingsDelayMoveTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SettingsDelayMoveTextBox.Leave += new System.EventHandler(SettingsDelayMoveTextBox_Leave);
		this.SettingsDelayMoveLabel.AutoSize = true;
		this.SettingsDelayMoveLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SettingsDelayMoveLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayMoveLabel.Location = new System.Drawing.Point(25, 105);
		this.SettingsDelayMoveLabel.Name = "SettingsDelayMoveLabel";
		this.SettingsDelayMoveLabel.Size = new System.Drawing.Size(101, 20);
		this.SettingsDelayMoveLabel.TabIndex = 26;
		this.SettingsDelayMoveLabel.Text = "Delay Move";
		this.SettingsDelayMoveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsDelayRaidTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.SettingsDelayRaidTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.SettingsDelayRaidTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.SettingsDelayRaidTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayRaidTextBox.Location = new System.Drawing.Point(130, 60);
		this.SettingsDelayRaidTextBox.Name = "SettingsDelayRaidTextBox";
		this.SettingsDelayRaidTextBox.Size = new System.Drawing.Size(64, 26);
		this.SettingsDelayRaidTextBox.TabIndex = 25;
		this.SettingsDelayRaidTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SettingsDelayRaidTextBox.Leave += new System.EventHandler(SettingsDelayRaidTextBox_Leave);
		this.SettingsDelayRaidLabel.AutoSize = true;
		this.SettingsDelayRaidLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SettingsDelayRaidLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayRaidLabel.Location = new System.Drawing.Point(25, 65);
		this.SettingsDelayRaidLabel.Name = "SettingsDelayRaidLabel";
		this.SettingsDelayRaidLabel.Size = new System.Drawing.Size(96, 20);
		this.SettingsDelayRaidLabel.TabIndex = 24;
		this.SettingsDelayRaidLabel.Text = "Delay Raid";
		this.SettingsDelayRaidLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsRenameButton.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.SettingsRenameButton.FlatAppearance.BorderSize = 0;
		this.SettingsRenameButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SettingsRenameButton.Location = new System.Drawing.Point(824, 23);
		this.SettingsRenameButton.Name = "SettingsRenameButton";
		this.SettingsRenameButton.Size = new System.Drawing.Size(24, 24);
		this.SettingsRenameButton.TabIndex = 23;
		this.SettingsRenameButton.UseVisualStyleBackColor = false;
		this.SettingsRenameButton.MouseClick += new System.Windows.Forms.MouseEventHandler(SettingsRenameButton_MouseClick);
		this.RenameSettingLabel.AutoSize = true;
		this.RenameSettingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RenameSettingLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RenameSettingLabel.Location = new System.Drawing.Point(700, 25);
		this.RenameSettingLabel.Name = "RenameSettingLabel";
		this.RenameSettingLabel.Size = new System.Drawing.Size(76, 20);
		this.RenameSettingLabel.TabIndex = 22;
		this.RenameSettingLabel.Text = "Rename";
		this.RenameSettingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SettingsDelayBuffTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.SettingsDelayBuffTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.SettingsDelayBuffTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.SettingsDelayBuffTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayBuffTextBox.Location = new System.Drawing.Point(130, 20);
		this.SettingsDelayBuffTextBox.Name = "SettingsDelayBuffTextBox";
		this.SettingsDelayBuffTextBox.Size = new System.Drawing.Size(64, 26);
		this.SettingsDelayBuffTextBox.TabIndex = 21;
		this.SettingsDelayBuffTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SettingsDelayBuffTextBox.Leave += new System.EventHandler(SettingsDelayBuffTextBox_Leave);
		this.AddLicenseButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.AddLicenseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.AddLicenseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AddLicenseButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AddLicenseButton.Location = new System.Drawing.Point(25, 535);
		this.AddLicenseButton.Name = "AddLicenseButton";
		this.AddLicenseButton.Size = new System.Drawing.Size(130, 45);
		this.AddLicenseButton.TabIndex = 18;
		this.AddLicenseButton.Text = "Add License";
		this.AddLicenseButton.UseVisualStyleBackColor = false;
		this.AddLicenseButton.Click += new System.EventHandler(AddLicenseButton_Click);
		this.SettingsDelayBuffLabel.AutoSize = true;
		this.SettingsDelayBuffLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SettingsDelayBuffLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SettingsDelayBuffLabel.Location = new System.Drawing.Point(25, 25);
		this.SettingsDelayBuffLabel.Name = "SettingsDelayBuffLabel";
		this.SettingsDelayBuffLabel.Size = new System.Drawing.Size(93, 20);
		this.SettingsDelayBuffLabel.TabIndex = 19;
		this.SettingsDelayBuffLabel.Text = "Delay Buff";
		this.SettingsDelayBuffLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.BrowseDLLLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.BrowseDLLLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.BrowseDLLLabel.Location = new System.Drawing.Point(800, 69);
		this.BrowseDLLLabel.Name = "BrowseDLLLabel";
		this.BrowseDLLLabel.Size = new System.Drawing.Size(101, 20);
		this.BrowseDLLLabel.TabIndex = 11;
		this.BrowseDLLLabel.Text = "BrowseDLL";
		this.BrowseDLLLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(BrowseDLLLabel_MouseDown);
		this.BrowseDLLLabel.MouseEnter += new System.EventHandler(BrowseDLLLabel_MouseEnter);
		this.BrowseDLLLabel.MouseLeave += new System.EventHandler(BrowseDLLLabel_MouseLeave);
		this.DLLFileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.DLLFileLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.DLLFileLabel.Location = new System.Drawing.Point(775, 94);
		this.DLLFileLabel.Name = "DLLFileLabel";
		this.DLLFileLabel.Size = new System.Drawing.Size(150, 20);
		this.DLLFileLabel.TabIndex = 12;
		this.DLLFileLabel.Text = "No FIle Chosen";
		this.DLLFileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.InviterButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.InviterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.InviterButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.InviterButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.InviterButton.Location = new System.Drawing.Point(0, 15);
		this.InviterButton.Name = "InviterButton";
		this.InviterButton.Size = new System.Drawing.Size(100, 45);
		this.InviterButton.TabIndex = 13;
		this.InviterButton.Text = "Inviter";
		this.InviterButton.UseVisualStyleBackColor = false;
		this.InviterButton.Click += new System.EventHandler(InviterButton_Click);
		this.MapperButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.MapperButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MapperButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapperButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapperButton.Location = new System.Drawing.Point(0, 70);
		this.MapperButton.Name = "MapperButton";
		this.MapperButton.Size = new System.Drawing.Size(100, 45);
		this.MapperButton.TabIndex = 14;
		this.MapperButton.Text = "Main";
		this.MapperButton.UseVisualStyleBackColor = false;
		this.MapperButton.Click += new System.EventHandler(MapperButton_Click);
		this.InviterLabel.AutoSize = true;
		this.InviterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.InviterLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.InviterLabel.Location = new System.Drawing.Point(111, 27);
		this.InviterLabel.Name = "InviterLabel";
		this.InviterLabel.Size = new System.Drawing.Size(51, 20);
		this.InviterLabel.TabIndex = 15;
		this.InviterLabel.Text = "None";
		this.InviterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MapperLabel.AutoSize = true;
		this.MapperLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MapperLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MapperLabel.Location = new System.Drawing.Point(111, 82);
		this.MapperLabel.Name = "MapperLabel";
		this.MapperLabel.Size = new System.Drawing.Size(51, 20);
		this.MapperLabel.TabIndex = 16;
		this.MapperLabel.Text = "None";
		this.MapperLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ControlPanelBottomPanel.Controls.Add(this.AccounstCountLabel);
		this.ControlPanelBottomPanel.Controls.Add(this.SaveConfigButton);
		this.ControlPanelBottomPanel.Controls.Add(this.LoadConfigButton);
		this.ControlPanelBottomPanel.Controls.Add(this.closeAltsButton);
		this.ControlPanelBottomPanel.Controls.Add(this.resetTitlesButton);
		this.ControlPanelBottomPanel.Controls.Add(this.MapperLabel);
		this.ControlPanelBottomPanel.Controls.Add(this.InjectDLLButton);
		this.ControlPanelBottomPanel.Controls.Add(this.InviterButton);
		this.ControlPanelBottomPanel.Controls.Add(this.DLLFileLabel);
		this.ControlPanelBottomPanel.Controls.Add(this.InviterLabel);
		this.ControlPanelBottomPanel.Controls.Add(this.BrowseDLLLabel);
		this.ControlPanelBottomPanel.Controls.Add(this.MapperButton);
		this.ControlPanelBottomPanel.Location = new System.Drawing.Point(25, 670);
		this.ControlPanelBottomPanel.Name = "ControlPanelBottomPanel";
		this.ControlPanelBottomPanel.Size = new System.Drawing.Size(925, 130);
		this.ControlPanelBottomPanel.TabIndex = 17;
		this.ControlPanelBottomPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.AccounstCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.AccounstCountLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.AccounstCountLabel.Location = new System.Drawing.Point(400, 27);
		this.AccounstCountLabel.Name = "AccounstCountLabel";
		this.AccounstCountLabel.Size = new System.Drawing.Size(133, 23);
		this.AccounstCountLabel.TabIndex = 101;
		this.AccounstCountLabel.Text = "Accounts: 1";
		this.AccounstCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SaveConfigButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.SaveConfigButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SaveConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SaveConfigButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SaveConfigButton.Location = new System.Drawing.Point(535, 70);
		this.SaveConfigButton.Name = "SaveConfigButton";
		this.SaveConfigButton.Size = new System.Drawing.Size(100, 45);
		this.SaveConfigButton.TabIndex = 100;
		this.SaveConfigButton.Text = "Save Config";
		this.SaveConfigButton.UseVisualStyleBackColor = false;
		this.SaveConfigButton.Click += new System.EventHandler(SaveConfigButton_Click);
		this.LoadConfigButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.LoadConfigButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.LoadConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.LoadConfigButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.LoadConfigButton.Location = new System.Drawing.Point(645, 70);
		this.LoadConfigButton.Name = "LoadConfigButton";
		this.LoadConfigButton.Size = new System.Drawing.Size(100, 45);
		this.LoadConfigButton.TabIndex = 99;
		this.LoadConfigButton.Text = "Load Config";
		this.LoadConfigButton.UseVisualStyleBackColor = false;
		this.LoadConfigButton.Click += new System.EventHandler(LoadConfigButton_Click);
		this.closeAltsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.closeAltsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.closeAltsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.closeAltsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.closeAltsButton.Location = new System.Drawing.Point(535, 15);
		this.closeAltsButton.Name = "closeAltsButton";
		this.closeAltsButton.Size = new System.Drawing.Size(100, 45);
		this.closeAltsButton.TabIndex = 18;
		this.closeAltsButton.Text = "Close Alts";
		this.closeAltsButton.UseVisualStyleBackColor = false;
		this.closeAltsButton.Click += new System.EventHandler(closeAltsButton_Click);
		this.resetTitlesButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.resetTitlesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.resetTitlesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.resetTitlesButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.resetTitlesButton.Location = new System.Drawing.Point(645, 15);
		this.resetTitlesButton.Name = "resetTitlesButton";
		this.resetTitlesButton.Size = new System.Drawing.Size(100, 45);
		this.resetTitlesButton.TabIndex = 17;
		this.resetTitlesButton.Text = "Titles";
		this.resetTitlesButton.UseVisualStyleBackColor = false;
		this.resetTitlesButton.Click += new System.EventHandler(resetTitlesButton_Click);
		this.CloseButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.CloseButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.CloseButton.Location = new System.Drawing.Point(1158, 12);
		this.CloseButton.Name = "CloseButton";
		this.CloseButton.Size = new System.Drawing.Size(30, 30);
		this.CloseButton.TabIndex = 19;
		this.CloseButton.UseVisualStyleBackColor = false;
		this.CloseButton.Click += new System.EventHandler(CloseButton_Click);
		this.MinimizeButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.MinimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.MinimizeButton.Image = NosAssistant2.Properties.Resources.minimize_small;
		this.MinimizeButton.Location = new System.Drawing.Point(1117, 12);
		this.MinimizeButton.Name = "MinimizeButton";
		this.MinimizeButton.Size = new System.Drawing.Size(30, 30);
		this.MinimizeButton.TabIndex = 20;
		this.MinimizeButton.UseVisualStyleBackColor = false;
		this.MinimizeButton.Click += new System.EventHandler(MinimizeButton_Click);
		this.PacketLoggerPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.PacketLoggerPanel.Controls.Add(this.PacketLoggerScrollbar);
		this.PacketLoggerPanel.Controls.Add(this.PacketsConsole);
		this.PacketLoggerPanel.Location = new System.Drawing.Point(25, 70);
		this.PacketLoggerPanel.Name = "PacketLoggerPanel";
		this.PacketLoggerPanel.Size = new System.Drawing.Size(900, 600);
		this.PacketLoggerPanel.TabIndex = 3;
		this.PacketLoggerScrollbar.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.PacketLoggerScrollbar.Location = new System.Drawing.Point(881, 1);
		this.PacketLoggerScrollbar.Name = "PacketLoggerScrollbar";
		this.PacketLoggerScrollbar.Size = new System.Drawing.Size(19, 598);
		this.PacketLoggerScrollbar.TabIndex = 1;
		this.PacketLoggerScrollbar.targetPanel = null;
		this.PacketLoggerScrollbar.Visible = false;
		this.PacketsConsole.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.PacketsConsole.Font = new System.Drawing.Font("Consolas", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.PacketsConsole.ForeColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.PacketsConsole.FullRowSelect = true;
		this.PacketsConsole.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
		this.PacketsConsole.Location = new System.Drawing.Point(203, 37);
		this.PacketsConsole.MultiSelect = false;
		this.PacketsConsole.Name = "PacketsConsole";
		this.PacketsConsole.Size = new System.Drawing.Size(500, 500);
		this.PacketsConsole.TabIndex = 0;
		this.PacketsConsole.UseCompatibleStateImageBehavior = false;
		this.PacketsConsole.View = System.Windows.Forms.View.Details;
		this.PacketLoggerBottomPanel.Controls.Add(this.OpenPacketFiltersButton);
		this.PacketLoggerBottomPanel.Controls.Add(this.PacketLoggerPrintSentButton);
		this.PacketLoggerBottomPanel.Controls.Add(this.PacketLogerPrintSentStatusLabel);
		this.PacketLoggerBottomPanel.Controls.Add(this.ClearPacketLoggerButton);
		this.PacketLoggerBottomPanel.Controls.Add(this.PacketLoggerPrintRecvButton);
		this.PacketLoggerBottomPanel.Controls.Add(this.PacketLogerPrintRecvStatusLabel);
		this.PacketLoggerBottomPanel.Location = new System.Drawing.Point(25, 670);
		this.PacketLoggerBottomPanel.Name = "PacketLoggerBottomPanel";
		this.PacketLoggerBottomPanel.Size = new System.Drawing.Size(900, 126);
		this.PacketLoggerBottomPanel.TabIndex = 19;
		this.PacketLoggerBottomPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.OpenPacketFiltersButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.OpenPacketFiltersButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.OpenPacketFiltersButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.OpenPacketFiltersButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.OpenPacketFiltersButton.Location = new System.Drawing.Point(797, 65);
		this.OpenPacketFiltersButton.Name = "OpenPacketFiltersButton";
		this.OpenPacketFiltersButton.Size = new System.Drawing.Size(100, 45);
		this.OpenPacketFiltersButton.TabIndex = 21;
		this.OpenPacketFiltersButton.Text = "Filters";
		this.OpenPacketFiltersButton.UseVisualStyleBackColor = false;
		this.OpenPacketFiltersButton.Click += new System.EventHandler(OpenPacketFiltersButton_Click);
		this.PacketLoggerPrintSentButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.PacketLoggerPrintSentButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.PacketLoggerPrintSentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PacketLoggerPrintSentButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PacketLoggerPrintSentButton.Location = new System.Drawing.Point(5, 65);
		this.PacketLoggerPrintSentButton.Name = "PacketLoggerPrintSentButton";
		this.PacketLoggerPrintSentButton.Size = new System.Drawing.Size(100, 45);
		this.PacketLoggerPrintSentButton.TabIndex = 19;
		this.PacketLoggerPrintSentButton.Text = "Sent";
		this.PacketLoggerPrintSentButton.UseVisualStyleBackColor = false;
		this.PacketLoggerPrintSentButton.Click += new System.EventHandler(PacketLoggerPrintSentButton_Click);
		this.PacketLogerPrintSentStatusLabel.AutoSize = true;
		this.PacketLogerPrintSentStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PacketLogerPrintSentStatusLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PacketLogerPrintSentStatusLabel.Location = new System.Drawing.Point(111, 77);
		this.PacketLogerPrintSentStatusLabel.Name = "PacketLogerPrintSentStatusLabel";
		this.PacketLogerPrintSentStatusLabel.Size = new System.Drawing.Size(34, 20);
		this.PacketLogerPrintSentStatusLabel.TabIndex = 20;
		this.PacketLogerPrintSentStatusLabel.Text = "Off";
		this.PacketLogerPrintSentStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ClearPacketLoggerButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.ClearPacketLoggerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ClearPacketLoggerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ClearPacketLoggerButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ClearPacketLoggerButton.Location = new System.Drawing.Point(797, 10);
		this.ClearPacketLoggerButton.Name = "ClearPacketLoggerButton";
		this.ClearPacketLoggerButton.Size = new System.Drawing.Size(100, 45);
		this.ClearPacketLoggerButton.TabIndex = 18;
		this.ClearPacketLoggerButton.Text = "Clear";
		this.ClearPacketLoggerButton.UseVisualStyleBackColor = false;
		this.ClearPacketLoggerButton.Click += new System.EventHandler(ClearPacketLoggerButton_Click);
		this.PacketLoggerPrintRecvButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.PacketLoggerPrintRecvButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.PacketLoggerPrintRecvButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PacketLoggerPrintRecvButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PacketLoggerPrintRecvButton.Location = new System.Drawing.Point(5, 10);
		this.PacketLoggerPrintRecvButton.Name = "PacketLoggerPrintRecvButton";
		this.PacketLoggerPrintRecvButton.Size = new System.Drawing.Size(100, 45);
		this.PacketLoggerPrintRecvButton.TabIndex = 16;
		this.PacketLoggerPrintRecvButton.Text = "Recv";
		this.PacketLoggerPrintRecvButton.UseVisualStyleBackColor = false;
		this.PacketLoggerPrintRecvButton.Click += new System.EventHandler(PacketLoggerPrintButton_Click);
		this.PacketLogerPrintRecvStatusLabel.AutoSize = true;
		this.PacketLogerPrintRecvStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PacketLogerPrintRecvStatusLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PacketLogerPrintRecvStatusLabel.Location = new System.Drawing.Point(111, 22);
		this.PacketLogerPrintRecvStatusLabel.Name = "PacketLogerPrintRecvStatusLabel";
		this.PacketLogerPrintRecvStatusLabel.Size = new System.Drawing.Size(34, 20);
		this.PacketLogerPrintRecvStatusLabel.TabIndex = 17;
		this.PacketLogerPrintRecvStatusLabel.Text = "Off";
		this.PacketLogerPrintRecvStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MainControlPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.MainControlPanel.Controls.Add(this.ControlPanelBottomPanel);
		this.MainControlPanel.Controls.Add(this.MainControlPanelLabel);
		this.MainControlPanel.Controls.Add(this.ControlPanelPanel);
		this.MainControlPanel.Location = new System.Drawing.Point(250, 0);
		this.MainControlPanel.Name = "MainControlPanel";
		this.MainControlPanel.Size = new System.Drawing.Size(950, 800);
		this.MainControlPanel.TabIndex = 3;
		this.MainControlPanel.Visible = false;
		this.MainControlPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainMapPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.MainMapPanel.Controls.Add(this.MapPanel);
		this.MainMapPanel.Controls.Add(this.MapBottomPanel);
		this.MainMapPanel.Controls.Add(this.MainMapPanelLabel);
		this.MainMapPanel.Location = new System.Drawing.Point(250, 0);
		this.MainMapPanel.Name = "MainMapPanel";
		this.MainMapPanel.Size = new System.Drawing.Size(950, 800);
		this.MainMapPanel.TabIndex = 4;
		this.MainMapPanel.Visible = false;
		this.MainMapPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainRaidsPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.MainRaidsPanel.Controls.Add(this.MainRaidsPanelLabel);
		this.MainRaidsPanel.Controls.Add(this.RaidsPanel);
		this.MainRaidsPanel.Location = new System.Drawing.Point(250, 0);
		this.MainRaidsPanel.Name = "MainRaidsPanel";
		this.MainRaidsPanel.Size = new System.Drawing.Size(950, 800);
		this.MainRaidsPanel.TabIndex = 5;
		this.MainRaidsPanel.Visible = false;
		this.MainRaidsPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainMinilandPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.MainMinilandPanel.Controls.Add(this.MinilandPanel);
		this.MainMinilandPanel.Controls.Add(this.MainMinilandPanelLabel);
		this.MainMinilandPanel.Location = new System.Drawing.Point(250, 0);
		this.MainMinilandPanel.Name = "MainMinilandPanel";
		this.MainMinilandPanel.Size = new System.Drawing.Size(950, 800);
		this.MainMinilandPanel.TabIndex = 6;
		this.MainMinilandPanel.Visible = false;
		this.MainMinilandPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainPacketLoggerPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.MainPacketLoggerPanel.Controls.Add(this.PacketLoggerPanel);
		this.MainPacketLoggerPanel.Controls.Add(this.PacketLoggerBottomPanel);
		this.MainPacketLoggerPanel.Controls.Add(this.MainPacketLoggerPanelLabel);
		this.MainPacketLoggerPanel.Location = new System.Drawing.Point(250, 0);
		this.MainPacketLoggerPanel.Name = "MainPacketLoggerPanel";
		this.MainPacketLoggerPanel.Size = new System.Drawing.Size(950, 800);
		this.MainPacketLoggerPanel.TabIndex = 7;
		this.MainPacketLoggerPanel.Visible = false;
		this.MainPacketLoggerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.MainSettingsPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.MainSettingsPanel.Controls.Add(this.LicesneExpirationDateLabel);
		this.MainSettingsPanel.Controls.Add(this.SettingsPanel);
		this.MainSettingsPanel.Controls.Add(this.MainSettingsPanelLabel);
		this.MainSettingsPanel.Location = new System.Drawing.Point(250, 0);
		this.MainSettingsPanel.Name = "MainSettingsPanel";
		this.MainSettingsPanel.Size = new System.Drawing.Size(950, 800);
		this.MainSettingsPanel.TabIndex = 8;
		this.MainSettingsPanel.Visible = false;
		this.MainSettingsPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.LicesneExpirationDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.LicesneExpirationDateLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.LicesneExpirationDateLabel.Location = new System.Drawing.Point(625, 755);
		this.LicesneExpirationDateLabel.Name = "LicesneExpirationDateLabel";
		this.LicesneExpirationDateLabel.Size = new System.Drawing.Size(300, 44);
		this.LicesneExpirationDateLabel.TabIndex = 7;
		this.LicesneExpirationDateLabel.Text = "License Expires: 21.03.2024";
		this.LicesneExpirationDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.MainNoAccessPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.MainNoAccessPanel.Controls.Add(this.NoAccessPanelMainLabel);
		this.MainNoAccessPanel.Controls.Add(this.NoAccessPanel);
		this.MainNoAccessPanel.Location = new System.Drawing.Point(250, 0);
		this.MainNoAccessPanel.Name = "MainNoAccessPanel";
		this.MainNoAccessPanel.Size = new System.Drawing.Size(950, 800);
		this.MainNoAccessPanel.TabIndex = 9;
		this.MainNoAccessPanel.Visible = false;
		this.MainNoAccessPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.NoAccessPanelMainLabel.AutoSize = true;
		this.NoAccessPanelMainLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.NoAccessPanelMainLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.NoAccessPanelMainLabel.Location = new System.Drawing.Point(30, 12);
		this.NoAccessPanelMainLabel.Name = "NoAccessPanelMainLabel";
		this.NoAccessPanelMainLabel.Size = new System.Drawing.Size(193, 39);
		this.NoAccessPanelMainLabel.TabIndex = 23;
		this.NoAccessPanelMainLabel.Text = "No Access";
		this.NoAccessPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.NoAccessPanel.Controls.Add(this.ChangeNicknameLabel);
		this.NoAccessPanel.Controls.Add(this.BuyLicenseLabel);
		this.NoAccessPanel.Controls.Add(this.ForgotLicenseLabel);
		this.NoAccessPanel.Controls.Add(this.NoAccessLicenseConfirmButton);
		this.NoAccessPanel.Controls.Add(this.NoAccessInfoLabel);
		this.NoAccessPanel.Controls.Add(this.NoAccessLicenseTextBox);
		this.NoAccessPanel.Location = new System.Drawing.Point(25, 100);
		this.NoAccessPanel.Name = "NoAccessPanel";
		this.NoAccessPanel.Size = new System.Drawing.Size(900, 600);
		this.NoAccessPanel.TabIndex = 22;
		this.ChangeNicknameLabel.Cursor = System.Windows.Forms.Cursors.Hand;
		this.ChangeNicknameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ChangeNicknameLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ChangeNicknameLabel.Location = new System.Drawing.Point(300, 450);
		this.ChangeNicknameLabel.Name = "ChangeNicknameLabel";
		this.ChangeNicknameLabel.Size = new System.Drawing.Size(300, 25);
		this.ChangeNicknameLabel.TabIndex = 27;
		this.ChangeNicknameLabel.Text = "Change Nickname";
		this.ChangeNicknameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ChangeNicknameLabel.Click += new System.EventHandler(ChangeNicknameLabel_Click);
		this.ChangeNicknameLabel.MouseEnter += new System.EventHandler(ChangeNicknameLabel_MouseEnter);
		this.ChangeNicknameLabel.MouseLeave += new System.EventHandler(ChangeNicknameLabel_MouseLeave);
		this.BuyLicenseLabel.Cursor = System.Windows.Forms.Cursors.Hand;
		this.BuyLicenseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.BuyLicenseLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.BuyLicenseLabel.Location = new System.Drawing.Point(300, 425);
		this.BuyLicenseLabel.Name = "BuyLicenseLabel";
		this.BuyLicenseLabel.Size = new System.Drawing.Size(300, 25);
		this.BuyLicenseLabel.TabIndex = 26;
		this.BuyLicenseLabel.Text = "Buy License";
		this.BuyLicenseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.BuyLicenseLabel.Click += new System.EventHandler(BuyLicenseLabel_Click);
		this.BuyLicenseLabel.MouseEnter += new System.EventHandler(BuyLicenseLabel_MouseEnter);
		this.BuyLicenseLabel.MouseLeave += new System.EventHandler(BuyLicenseLabel_MouseLeave);
		this.ForgotLicenseLabel.Cursor = System.Windows.Forms.Cursors.Hand;
		this.ForgotLicenseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ForgotLicenseLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ForgotLicenseLabel.Location = new System.Drawing.Point(300, 400);
		this.ForgotLicenseLabel.Name = "ForgotLicenseLabel";
		this.ForgotLicenseLabel.Size = new System.Drawing.Size(300, 25);
		this.ForgotLicenseLabel.TabIndex = 25;
		this.ForgotLicenseLabel.Text = "Forgot License";
		this.ForgotLicenseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ForgotLicenseLabel.Click += new System.EventHandler(ForgotLicenseLabel_Click);
		this.ForgotLicenseLabel.MouseEnter += new System.EventHandler(ForgotLicenseLabel_MouseEnter);
		this.ForgotLicenseLabel.MouseLeave += new System.EventHandler(ForgotLicenseLabel_MouseLeave);
		this.NoAccessLicenseConfirmButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.NoAccessLicenseConfirmButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.NoAccessLicenseConfirmButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.NoAccessLicenseConfirmButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.NoAccessLicenseConfirmButton.Location = new System.Drawing.Point(300, 350);
		this.NoAccessLicenseConfirmButton.Name = "NoAccessLicenseConfirmButton";
		this.NoAccessLicenseConfirmButton.Size = new System.Drawing.Size(300, 40);
		this.NoAccessLicenseConfirmButton.TabIndex = 24;
		this.NoAccessLicenseConfirmButton.Text = "Confirm";
		this.NoAccessLicenseConfirmButton.UseVisualStyleBackColor = false;
		this.NoAccessLicenseConfirmButton.Click += new System.EventHandler(NoAccessLicenseConfirmButton_Click);
		this.NoAccessInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.NoAccessInfoLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.NoAccessInfoLabel.Location = new System.Drawing.Point(125, 155);
		this.NoAccessInfoLabel.Name = "NoAccessInfoLabel";
		this.NoAccessInfoLabel.Size = new System.Drawing.Size(650, 40);
		this.NoAccessInfoLabel.TabIndex = 2;
		this.NoAccessInfoLabel.Text = "Insert License Key or Log in your Main";
		this.NoAccessLicenseTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.NoAccessLicenseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.NoAccessLicenseTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20f, System.Drawing.FontStyle.Bold);
		this.NoAccessLicenseTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.NoAccessLicenseTextBox.Location = new System.Drawing.Point(125, 250);
		this.NoAccessLicenseTextBox.Name = "NoAccessLicenseTextBox";
		this.NoAccessLicenseTextBox.Size = new System.Drawing.Size(650, 38);
		this.NoAccessLicenseTextBox.TabIndex = 21;
		this.NoAccessLicenseTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.popUpPanel.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.popUpPanel.Location = new System.Drawing.Point(850, -200);
		this.popUpPanel.Name = "popUpPanel";
		this.popUpPanel.Size = new System.Drawing.Size(200, 100);
		this.popUpPanel.TabIndex = 22;
		this.popUpPanel.Visible = false;
		this.LoadingScreenPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.LoadingScreenPanel.Controls.Add(this.panel2);
		this.LoadingScreenPanel.Location = new System.Drawing.Point(250, 0);
		this.LoadingScreenPanel.Name = "LoadingScreenPanel";
		this.LoadingScreenPanel.Size = new System.Drawing.Size(950, 800);
		this.LoadingScreenPanel.TabIndex = 9;
		this.LoadingScreenPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.panel2.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.panel2.Controls.Add(this.LoadingScreenBar);
		this.panel2.Controls.Add(this.LoadingLabel);
		this.panel2.Location = new System.Drawing.Point(25, 70);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(900, 600);
		this.panel2.TabIndex = 6;
		this.LoadingScreenBar.BackColor = System.Drawing.Color.FromArgb(100, 97, 200);
		this.LoadingScreenBar.Location = new System.Drawing.Point(150, 300);
		this.LoadingScreenBar.Name = "LoadingScreenBar";
		this.LoadingScreenBar.Size = new System.Drawing.Size(600, 25);
		this.LoadingScreenBar.TabIndex = 2;
		this.LoadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.LoadingLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.LoadingLabel.Location = new System.Drawing.Point(150, 190);
		this.LoadingLabel.Name = "LoadingLabel";
		this.LoadingLabel.Size = new System.Drawing.Size(600, 31);
		this.LoadingLabel.TabIndex = 1;
		this.LoadingLabel.Text = "Downloading Missing Files...";
		this.LoadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.HideMenuButton.ErrorImage = null;
		this.HideMenuButton.Image = NosAssistant2.Properties.Resources.triangleRight;
		this.HideMenuButton.Location = new System.Drawing.Point(250, 388);
		this.HideMenuButton.Name = "HideMenuButton";
		this.HideMenuButton.Size = new System.Drawing.Size(25, 25);
		this.HideMenuButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.HideMenuButton.TabIndex = 6;
		this.HideMenuButton.TabStop = false;
		this.HideMenuButton.Click += new System.EventHandler(HideMenuButton_Click);
		this.HideMenuButton.MouseEnter += new System.EventHandler(HideMenuButton_MouseEnter);
		this.HideMenuButton.MouseLeave += new System.EventHandler(HideMenuButton_MouseLeave);
		this.RaidsHistoryPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.RaidsHistoryPanel.Controls.Add(this.AnalyticsPlayersTab);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryBestTimeLabel);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryAverageTimeLabel);
		this.RaidsHistoryPanel.Controls.Add(this.RankingTabLabel);
		this.RaidsHistoryPanel.Controls.Add(this.ShowMarathonTotalButton);
		this.RaidsHistoryPanel.Controls.Add(this.AnalyticsBackArrow);
		this.RaidsHistoryPanel.Controls.Add(this.PlayersTabLabel);
		this.RaidsHistoryPanel.Controls.Add(this.FamRecordsNextPageButton);
		this.RaidsHistoryPanel.Controls.Add(this.FamRecordsPreviousPageButton);
		this.RaidsHistoryPanel.Controls.Add(this.FamRecordsTabLabel);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryTabLabel);
		this.RaidsHistoryPanel.Controls.Add(this.BackArrowRaidsHistory);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryFilterTextBox);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryPageLabel);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryNextPageButton);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryPreviousPageButton);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryDoubleBufferedPanel);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryLabel);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistoryBackButton);
		this.RaidsHistoryPanel.Controls.Add(this.RaidsHistorySelectedRaidPlayersFlowLayoutPanel);
		this.RaidsHistoryPanel.Controls.Add(this.RankingTabPanel);
		this.RaidsHistoryPanel.Controls.Add(this.FamRecordsPanel);
		this.RaidsHistoryPanel.Location = new System.Drawing.Point(0, 0);
		this.RaidsHistoryPanel.Name = "RaidsHistoryPanel";
		this.RaidsHistoryPanel.Size = new System.Drawing.Size(1200, 800);
		this.RaidsHistoryPanel.TabIndex = 10;
		this.RaidsHistoryPanel.Visible = false;
		this.RaidsHistoryPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.AnalyticsPlayersTab.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.AnalyticsPlayersTab.Controls.Add(this.PlayerRaidsStatisticsPanel);
		this.AnalyticsPlayersTab.Controls.Add(this.SPDetailsBorderPanel);
		this.AnalyticsPlayersTab.Controls.Add(this.Tattoo2UpgradeLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.Tattoo2Icon);
		this.AnalyticsPlayersTab.Controls.Add(this.Tattoo1UpgradeLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.Tattoo1Icon);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerSPsFlowLayoutPanel);
		this.AnalyticsPlayersTab.Controls.Add(this.ShellInfoMainPanel);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerTitle);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerFamilyRole);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerLastUpdateDateLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerLastUpdateLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerFairiesFlowLayoutPanel);
		this.AnalyticsPlayersTab.Controls.Add(this.Wings);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerWingsLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.WeaponSkin);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerWeaponSkinLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.CostumeHat);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerCostumeHatLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.Costume);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerCostumeLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.FlyingPet);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerFlyingPetLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.Mask);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerMaskLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.Hat);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerHatLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.ArmorUpgrade);
		this.AnalyticsPlayersTab.Controls.Add(this.Armor);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerArmorLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.SecondaryWeaponUpgrade);
		this.AnalyticsPlayersTab.Controls.Add(this.SecondaryWeapon);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerSecondWeaponLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.MainWeaponUpgrade);
		this.AnalyticsPlayersTab.Controls.Add(this.MainWeapon);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerMainWeaponLabel);
		this.AnalyticsPlayersTab.Controls.Add(this.Reputation);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerLVLCLVL);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerClassSex);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerFamily);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerNickname);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchedPlayerAvatar);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchPlayerButton);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchServerComboBox);
		this.AnalyticsPlayersTab.Controls.Add(this.SearchNicknameTextBox);
		this.AnalyticsPlayersTab.Controls.Add(this.MainFairyDetailsPanel);
		this.AnalyticsPlayersTab.Controls.Add(this.PlayerRaidsStatisticsButton);
		this.AnalyticsPlayersTab.Location = new System.Drawing.Point(25, 75);
		this.AnalyticsPlayersTab.Margin = new System.Windows.Forms.Padding(0);
		this.AnalyticsPlayersTab.Name = "AnalyticsPlayersTab";
		this.AnalyticsPlayersTab.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
		this.AnalyticsPlayersTab.Size = new System.Drawing.Size(1150, 700);
		this.AnalyticsPlayersTab.TabIndex = 4;
		this.AnalyticsPlayersTab.Visible = false;
		this.AnalyticsPlayersTab.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayersRaidsStatisticsLoadingLabel);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsStatisticsBossNameLabel);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsStatisticsBossIcon);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsStatisticsTotalMaxHitRankLabel);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsStatisticsBestTimeRankLabel);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsStatisticsRaidsFinishedRankLabel);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsStatisticsTotalMaxHitLabel);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsStatisticsBestTimeLabel);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsStatisticsRaidsFinishedLabel);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsStatisticsRaidsStatisticsTablePanel);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerEquipementButton);
		this.PlayerRaidsStatisticsPanel.Controls.Add(this.PlayerRaidsSelectionFlowLayoutPanel);
		this.PlayerRaidsStatisticsPanel.Location = new System.Drawing.Point(0, 0);
		this.PlayerRaidsStatisticsPanel.Name = "PlayerRaidsStatisticsPanel";
		this.PlayerRaidsStatisticsPanel.Size = new System.Drawing.Size(1150, 700);
		this.PlayerRaidsStatisticsPanel.TabIndex = 56;
		this.PlayerRaidsStatisticsPanel.Visible = false;
		this.PlayerRaidsStatisticsPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayersRaidsStatisticsLoadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayersRaidsStatisticsLoadingLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayersRaidsStatisticsLoadingLabel.Location = new System.Drawing.Point(367, 86);
		this.PlayersRaidsStatisticsLoadingLabel.Name = "PlayersRaidsStatisticsLoadingLabel";
		this.PlayersRaidsStatisticsLoadingLabel.Size = new System.Drawing.Size(400, 28);
		this.PlayersRaidsStatisticsLoadingLabel.TabIndex = 67;
		this.PlayersRaidsStatisticsLoadingLabel.Text = "Loading...";
		this.PlayersRaidsStatisticsLoadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.PlayersRaidsStatisticsLoadingLabel.Visible = false;
		this.PlayersRaidsStatisticsLoadingLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsBossNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayerRaidsStatisticsBossNameLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayerRaidsStatisticsBossNameLabel.Location = new System.Drawing.Point(757, 660);
		this.PlayerRaidsStatisticsBossNameLabel.Name = "PlayerRaidsStatisticsBossNameLabel";
		this.PlayerRaidsStatisticsBossNameLabel.Size = new System.Drawing.Size(350, 25);
		this.PlayerRaidsStatisticsBossNameLabel.TabIndex = 66;
		this.PlayerRaidsStatisticsBossNameLabel.Text = "Paimon";
		this.PlayerRaidsStatisticsBossNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.PlayerRaidsStatisticsBossNameLabel.Visible = false;
		this.PlayerRaidsStatisticsBossNameLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsBossIcon.Location = new System.Drawing.Point(757, 300);
		this.PlayerRaidsStatisticsBossIcon.Name = "PlayerRaidsStatisticsBossIcon";
		this.PlayerRaidsStatisticsBossIcon.Size = new System.Drawing.Size(350, 350);
		this.PlayerRaidsStatisticsBossIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.PlayerRaidsStatisticsBossIcon.TabIndex = 65;
		this.PlayerRaidsStatisticsBossIcon.TabStop = false;
		this.PlayerRaidsStatisticsBossIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.Location = new System.Drawing.Point(1065, 135);
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.Name = "PlayerRaidsStatisticsTotalMaxHitRankLabel";
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.Size = new System.Drawing.Size(80, 23);
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.TabIndex = 64;
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.Text = "#21";
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.Visible = false;
		this.PlayerRaidsStatisticsTotalMaxHitRankLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsBestTimeRankLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayerRaidsStatisticsBestTimeRankLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayerRaidsStatisticsBestTimeRankLabel.Location = new System.Drawing.Point(1065, 165);
		this.PlayerRaidsStatisticsBestTimeRankLabel.Name = "PlayerRaidsStatisticsBestTimeRankLabel";
		this.PlayerRaidsStatisticsBestTimeRankLabel.Size = new System.Drawing.Size(80, 23);
		this.PlayerRaidsStatisticsBestTimeRankLabel.TabIndex = 63;
		this.PlayerRaidsStatisticsBestTimeRankLabel.Text = "#21";
		this.PlayerRaidsStatisticsBestTimeRankLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.PlayerRaidsStatisticsBestTimeRankLabel.Visible = false;
		this.PlayerRaidsStatisticsBestTimeRankLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.Location = new System.Drawing.Point(1065, 195);
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.Name = "PlayerRaidsStatisticsRaidsFinishedRankLabel";
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.Size = new System.Drawing.Size(80, 23);
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.TabIndex = 62;
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.Text = "#21";
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.Visible = false;
		this.PlayerRaidsStatisticsRaidsFinishedRankLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsTotalMaxHitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayerRaidsStatisticsTotalMaxHitLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayerRaidsStatisticsTotalMaxHitLabel.Location = new System.Drawing.Point(755, 135);
		this.PlayerRaidsStatisticsTotalMaxHitLabel.Name = "PlayerRaidsStatisticsTotalMaxHitLabel";
		this.PlayerRaidsStatisticsTotalMaxHitLabel.Size = new System.Drawing.Size(240, 23);
		this.PlayerRaidsStatisticsTotalMaxHitLabel.TabIndex = 61;
		this.PlayerRaidsStatisticsTotalMaxHitLabel.Text = "Total Max Hit: 2.137.000";
		this.PlayerRaidsStatisticsTotalMaxHitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.PlayerRaidsStatisticsTotalMaxHitLabel.Visible = false;
		this.PlayerRaidsStatisticsTotalMaxHitLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsBestTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayerRaidsStatisticsBestTimeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayerRaidsStatisticsBestTimeLabel.Location = new System.Drawing.Point(755, 165);
		this.PlayerRaidsStatisticsBestTimeLabel.Name = "PlayerRaidsStatisticsBestTimeLabel";
		this.PlayerRaidsStatisticsBestTimeLabel.Size = new System.Drawing.Size(240, 23);
		this.PlayerRaidsStatisticsBestTimeLabel.TabIndex = 60;
		this.PlayerRaidsStatisticsBestTimeLabel.Text = "Best Time 21:37";
		this.PlayerRaidsStatisticsBestTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.PlayerRaidsStatisticsBestTimeLabel.Visible = false;
		this.PlayerRaidsStatisticsBestTimeLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsRaidsFinishedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayerRaidsStatisticsRaidsFinishedLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayerRaidsStatisticsRaidsFinishedLabel.Location = new System.Drawing.Point(755, 195);
		this.PlayerRaidsStatisticsRaidsFinishedLabel.Name = "PlayerRaidsStatisticsRaidsFinishedLabel";
		this.PlayerRaidsStatisticsRaidsFinishedLabel.Size = new System.Drawing.Size(240, 23);
		this.PlayerRaidsStatisticsRaidsFinishedLabel.TabIndex = 59;
		this.PlayerRaidsStatisticsRaidsFinishedLabel.Text = "Raids Finished: 2137";
		this.PlayerRaidsStatisticsRaidsFinishedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.PlayerRaidsStatisticsRaidsFinishedLabel.Visible = false;
		this.PlayerRaidsStatisticsRaidsFinishedLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.ColumnCount = 7;
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.Location = new System.Drawing.Point(15, 135);
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.Name = "PlayerRaidsStatisticsRaidsStatisticsTablePanel";
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowCount = 11;
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.Size = new System.Drawing.Size(700, 550);
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.TabIndex = 58;
		this.PlayerRaidsStatisticsRaidsStatisticsTablePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerEquipementButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.PlayerEquipementButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.PlayerEquipementButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayerEquipementButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayerEquipementButton.Location = new System.Drawing.Point(15, 72);
		this.PlayerEquipementButton.Name = "PlayerEquipementButton";
		this.PlayerEquipementButton.Size = new System.Drawing.Size(152, 45);
		this.PlayerEquipementButton.TabIndex = 57;
		this.PlayerEquipementButton.Text = "Equipment";
		this.PlayerEquipementButton.UseVisualStyleBackColor = false;
		this.PlayerEquipementButton.Click += new System.EventHandler(PlayerEquipementButton_Click);
		this.PlayerRaidsSelectionFlowLayoutPanel.Location = new System.Drawing.Point(25, 15);
		this.PlayerRaidsSelectionFlowLayoutPanel.Name = "PlayerRaidsSelectionFlowLayoutPanel";
		this.PlayerRaidsSelectionFlowLayoutPanel.Size = new System.Drawing.Size(1120, 45);
		this.PlayerRaidsSelectionFlowLayoutPanel.TabIndex = 0;
		this.SPDetailsBorderPanel.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.SPDetailsBorderPanel.Controls.Add(this.SPDetailsPanel);
		this.SPDetailsBorderPanel.Location = new System.Drawing.Point(378, 295);
		this.SPDetailsBorderPanel.Name = "SPDetailsBorderPanel";
		this.SPDetailsBorderPanel.Size = new System.Drawing.Size(520, 210);
		this.SPDetailsBorderPanel.TabIndex = 50;
		this.SPDetailsBorderPanel.Visible = false;
		this.SPDetailsBorderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.SPDetailsPanel.Controls.Add(this.CloseSPDetailsButton);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsShadowLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsShadowImage);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsLightLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsLightImage);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsWaterLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsWaterImage);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsFireLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsFireImage);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsEnergyLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsEnergyImage);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsPropertyLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsPropertyImage);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsDefenceLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsDefenceImage);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsAttackLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsAttackImage);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsPerfectionLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsJobLabel);
		this.SPDetailsPanel.Controls.Add(this.SPDetailsAvatar);
		this.SPDetailsPanel.Location = new System.Drawing.Point(10, 10);
		this.SPDetailsPanel.Name = "SPDetailsPanel";
		this.SPDetailsPanel.Size = new System.Drawing.Size(500, 190);
		this.SPDetailsPanel.TabIndex = 14;
		this.SPDetailsPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.CloseSPDetailsButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.CloseSPDetailsButton.Location = new System.Drawing.Point(470, 0);
		this.CloseSPDetailsButton.Name = "CloseSPDetailsButton";
		this.CloseSPDetailsButton.Size = new System.Drawing.Size(30, 30);
		this.CloseSPDetailsButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.CloseSPDetailsButton.TabIndex = 19;
		this.CloseSPDetailsButton.TabStop = false;
		this.CloseSPDetailsButton.Click += new System.EventHandler(CloseSPDetailsButton_Click);
		this.SPDetailsShadowLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsShadowLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsShadowLabel.Location = new System.Drawing.Point(370, 145);
		this.SPDetailsShadowLabel.Name = "SPDetailsShadowLabel";
		this.SPDetailsShadowLabel.Size = new System.Drawing.Size(90, 30);
		this.SPDetailsShadowLabel.TabIndex = 18;
		this.SPDetailsShadowLabel.Text = "100 (15)";
		this.SPDetailsShadowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsShadowLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsShadowImage.Image = NosAssistant2.Properties.Resources.sp_shadow;
		this.SPDetailsShadowImage.Location = new System.Drawing.Point(330, 145);
		this.SPDetailsShadowImage.Name = "SPDetailsShadowImage";
		this.SPDetailsShadowImage.Size = new System.Drawing.Size(30, 30);
		this.SPDetailsShadowImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SPDetailsShadowImage.TabIndex = 17;
		this.SPDetailsShadowImage.TabStop = false;
		this.SPDetailsShadowImage.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsLightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsLightLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsLightLabel.Location = new System.Drawing.Point(370, 100);
		this.SPDetailsLightLabel.Name = "SPDetailsLightLabel";
		this.SPDetailsLightLabel.Size = new System.Drawing.Size(90, 30);
		this.SPDetailsLightLabel.TabIndex = 16;
		this.SPDetailsLightLabel.Text = "100 (15)";
		this.SPDetailsLightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsLightLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsLightImage.Image = NosAssistant2.Properties.Resources.sp_light;
		this.SPDetailsLightImage.Location = new System.Drawing.Point(330, 100);
		this.SPDetailsLightImage.Name = "SPDetailsLightImage";
		this.SPDetailsLightImage.Size = new System.Drawing.Size(30, 30);
		this.SPDetailsLightImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SPDetailsLightImage.TabIndex = 15;
		this.SPDetailsLightImage.TabStop = false;
		this.SPDetailsLightImage.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsWaterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsWaterLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsWaterLabel.Location = new System.Drawing.Point(370, 55);
		this.SPDetailsWaterLabel.Name = "SPDetailsWaterLabel";
		this.SPDetailsWaterLabel.Size = new System.Drawing.Size(90, 30);
		this.SPDetailsWaterLabel.TabIndex = 14;
		this.SPDetailsWaterLabel.Text = "100 (15)";
		this.SPDetailsWaterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsWaterLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsWaterImage.Image = NosAssistant2.Properties.Resources.sp_water;
		this.SPDetailsWaterImage.Location = new System.Drawing.Point(330, 55);
		this.SPDetailsWaterImage.Name = "SPDetailsWaterImage";
		this.SPDetailsWaterImage.Size = new System.Drawing.Size(30, 30);
		this.SPDetailsWaterImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SPDetailsWaterImage.TabIndex = 13;
		this.SPDetailsWaterImage.TabStop = false;
		this.SPDetailsWaterImage.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsFireLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsFireLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsFireLabel.Location = new System.Drawing.Point(370, 10);
		this.SPDetailsFireLabel.Name = "SPDetailsFireLabel";
		this.SPDetailsFireLabel.Size = new System.Drawing.Size(90, 30);
		this.SPDetailsFireLabel.TabIndex = 12;
		this.SPDetailsFireLabel.Text = "100 (15)";
		this.SPDetailsFireLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsFireLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsFireImage.Image = NosAssistant2.Properties.Resources.sp_fire;
		this.SPDetailsFireImage.Location = new System.Drawing.Point(330, 10);
		this.SPDetailsFireImage.Name = "SPDetailsFireImage";
		this.SPDetailsFireImage.Size = new System.Drawing.Size(30, 30);
		this.SPDetailsFireImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SPDetailsFireImage.TabIndex = 11;
		this.SPDetailsFireImage.TabStop = false;
		this.SPDetailsFireImage.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsEnergyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsEnergyLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsEnergyLabel.Location = new System.Drawing.Point(190, 145);
		this.SPDetailsEnergyLabel.Name = "SPDetailsEnergyLabel";
		this.SPDetailsEnergyLabel.Size = new System.Drawing.Size(90, 30);
		this.SPDetailsEnergyLabel.TabIndex = 10;
		this.SPDetailsEnergyLabel.Text = "100 (15)";
		this.SPDetailsEnergyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsEnergyLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsEnergyImage.Image = NosAssistant2.Properties.Resources.sp_energy;
		this.SPDetailsEnergyImage.Location = new System.Drawing.Point(150, 145);
		this.SPDetailsEnergyImage.Name = "SPDetailsEnergyImage";
		this.SPDetailsEnergyImage.Size = new System.Drawing.Size(30, 30);
		this.SPDetailsEnergyImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SPDetailsEnergyImage.TabIndex = 9;
		this.SPDetailsEnergyImage.TabStop = false;
		this.SPDetailsEnergyImage.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsPropertyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsPropertyLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsPropertyLabel.Location = new System.Drawing.Point(190, 100);
		this.SPDetailsPropertyLabel.Name = "SPDetailsPropertyLabel";
		this.SPDetailsPropertyLabel.Size = new System.Drawing.Size(90, 30);
		this.SPDetailsPropertyLabel.TabIndex = 8;
		this.SPDetailsPropertyLabel.Text = "100 (15)";
		this.SPDetailsPropertyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsPropertyLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsPropertyImage.Image = NosAssistant2.Properties.Resources.sp_property;
		this.SPDetailsPropertyImage.Location = new System.Drawing.Point(150, 100);
		this.SPDetailsPropertyImage.Name = "SPDetailsPropertyImage";
		this.SPDetailsPropertyImage.Size = new System.Drawing.Size(30, 30);
		this.SPDetailsPropertyImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SPDetailsPropertyImage.TabIndex = 7;
		this.SPDetailsPropertyImage.TabStop = false;
		this.SPDetailsPropertyImage.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsDefenceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsDefenceLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsDefenceLabel.Location = new System.Drawing.Point(190, 55);
		this.SPDetailsDefenceLabel.Name = "SPDetailsDefenceLabel";
		this.SPDetailsDefenceLabel.Size = new System.Drawing.Size(90, 30);
		this.SPDetailsDefenceLabel.TabIndex = 6;
		this.SPDetailsDefenceLabel.Text = "100 (15)";
		this.SPDetailsDefenceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsDefenceLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsDefenceImage.Image = NosAssistant2.Properties.Resources.sp_defence;
		this.SPDetailsDefenceImage.Location = new System.Drawing.Point(150, 55);
		this.SPDetailsDefenceImage.Name = "SPDetailsDefenceImage";
		this.SPDetailsDefenceImage.Size = new System.Drawing.Size(30, 30);
		this.SPDetailsDefenceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SPDetailsDefenceImage.TabIndex = 5;
		this.SPDetailsDefenceImage.TabStop = false;
		this.SPDetailsDefenceImage.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsAttackLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsAttackLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsAttackLabel.Location = new System.Drawing.Point(190, 10);
		this.SPDetailsAttackLabel.Name = "SPDetailsAttackLabel";
		this.SPDetailsAttackLabel.Size = new System.Drawing.Size(90, 30);
		this.SPDetailsAttackLabel.TabIndex = 4;
		this.SPDetailsAttackLabel.Text = "100 (15)";
		this.SPDetailsAttackLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsAttackLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsAttackImage.Image = NosAssistant2.Properties.Resources.sp_attack;
		this.SPDetailsAttackImage.Location = new System.Drawing.Point(150, 10);
		this.SPDetailsAttackImage.Name = "SPDetailsAttackImage";
		this.SPDetailsAttackImage.Size = new System.Drawing.Size(30, 30);
		this.SPDetailsAttackImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SPDetailsAttackImage.TabIndex = 3;
		this.SPDetailsAttackImage.TabStop = false;
		this.SPDetailsAttackImage.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsPerfectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsPerfectionLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsPerfectionLabel.Location = new System.Drawing.Point(0, 160);
		this.SPDetailsPerfectionLabel.Name = "SPDetailsPerfectionLabel";
		this.SPDetailsPerfectionLabel.Size = new System.Drawing.Size(140, 20);
		this.SPDetailsPerfectionLabel.TabIndex = 2;
		this.SPDetailsPerfectionLabel.Text = "Perfection: 100";
		this.SPDetailsPerfectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsPerfectionLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsJobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SPDetailsJobLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SPDetailsJobLabel.Location = new System.Drawing.Point(0, 130);
		this.SPDetailsJobLabel.Name = "SPDetailsJobLabel";
		this.SPDetailsJobLabel.Size = new System.Drawing.Size(140, 20);
		this.SPDetailsJobLabel.TabIndex = 1;
		this.SPDetailsJobLabel.Text = "Job: 99";
		this.SPDetailsJobLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SPDetailsJobLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SPDetailsAvatar.Location = new System.Drawing.Point(20, 20);
		this.SPDetailsAvatar.Name = "SPDetailsAvatar";
		this.SPDetailsAvatar.Size = new System.Drawing.Size(100, 100);
		this.SPDetailsAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SPDetailsAvatar.TabIndex = 0;
		this.SPDetailsAvatar.TabStop = false;
		this.SPDetailsAvatar.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.Tattoo2UpgradeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.Tattoo2UpgradeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.Tattoo2UpgradeLabel.Location = new System.Drawing.Point(891, 370);
		this.Tattoo2UpgradeLabel.Name = "Tattoo2UpgradeLabel";
		this.Tattoo2UpgradeLabel.Size = new System.Drawing.Size(29, 40);
		this.Tattoo2UpgradeLabel.TabIndex = 53;
		this.Tattoo2UpgradeLabel.Text = "+0";
		this.Tattoo2UpgradeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.Tattoo2Icon.Location = new System.Drawing.Point(848, 370);
		this.Tattoo2Icon.Name = "Tattoo2Icon";
		this.Tattoo2Icon.Size = new System.Drawing.Size(40, 40);
		this.Tattoo2Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.Tattoo2Icon.TabIndex = 52;
		this.Tattoo2Icon.TabStop = false;
		this.Tattoo1UpgradeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.Tattoo1UpgradeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.Tattoo1UpgradeLabel.Location = new System.Drawing.Point(770, 370);
		this.Tattoo1UpgradeLabel.Name = "Tattoo1UpgradeLabel";
		this.Tattoo1UpgradeLabel.Size = new System.Drawing.Size(29, 40);
		this.Tattoo1UpgradeLabel.TabIndex = 51;
		this.Tattoo1UpgradeLabel.Text = "+0";
		this.Tattoo1UpgradeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.Tattoo1Icon.Location = new System.Drawing.Point(727, 370);
		this.Tattoo1Icon.Name = "Tattoo1Icon";
		this.Tattoo1Icon.Size = new System.Drawing.Size(40, 40);
		this.Tattoo1Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.Tattoo1Icon.TabIndex = 50;
		this.Tattoo1Icon.TabStop = false;
		this.SearchedPlayerSPsFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.SearchedPlayerSPsFlowLayoutPanel.Location = new System.Drawing.Point(125, 435);
		this.SearchedPlayerSPsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
		this.SearchedPlayerSPsFlowLayoutPanel.Name = "SearchedPlayerSPsFlowLayoutPanel";
		this.SearchedPlayerSPsFlowLayoutPanel.Size = new System.Drawing.Size(1000, 240);
		this.SearchedPlayerSPsFlowLayoutPanel.TabIndex = 42;
		this.ShellInfoMainPanel.Controls.Add(this.RuneLevelLabel);
		this.ShellInfoMainPanel.Controls.Add(this.ShellEffectsFlowLayoutPanel);
		this.ShellInfoMainPanel.Controls.Add(this.SwitchToShellButton);
		this.ShellInfoMainPanel.Controls.Add(this.SwitchToRuneButton);
		this.ShellInfoMainPanel.Controls.Add(this.SwitchShellTypeButton);
		this.ShellInfoMainPanel.Controls.Add(this.ShellItemTypeLabel);
		this.ShellInfoMainPanel.Location = new System.Drawing.Point(180, 30);
		this.ShellInfoMainPanel.Name = "ShellInfoMainPanel";
		this.ShellInfoMainPanel.Size = new System.Drawing.Size(300, 375);
		this.ShellInfoMainPanel.TabIndex = 49;
		this.ShellInfoMainPanel.Visible = false;
		this.RuneLevelLabel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.RuneLevelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RuneLevelLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RuneLevelLabel.Location = new System.Drawing.Point(0, 335);
		this.RuneLevelLabel.Name = "RuneLevelLabel";
		this.RuneLevelLabel.Size = new System.Drawing.Size(300, 40);
		this.RuneLevelLabel.TabIndex = 55;
		this.RuneLevelLabel.Text = "0/21";
		this.RuneLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ShellEffectsFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.ShellEffectsFlowLayoutPanel.Location = new System.Drawing.Point(0, 50);
		this.ShellEffectsFlowLayoutPanel.Name = "ShellEffectsFlowLayoutPanel";
		this.ShellEffectsFlowLayoutPanel.Size = new System.Drawing.Size(300, 325);
		this.ShellEffectsFlowLayoutPanel.TabIndex = 54;
		this.SwitchToShellButton.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.SwitchToShellButton.Image = NosAssistant2.Properties.Resources.shell;
		this.SwitchToShellButton.Location = new System.Drawing.Point(10, 10);
		this.SwitchToShellButton.Name = "SwitchToShellButton";
		this.SwitchToShellButton.Size = new System.Drawing.Size(30, 30);
		this.SwitchToShellButton.TabIndex = 53;
		this.SwitchToShellButton.TabStop = false;
		this.SwitchToShellButton.Click += new System.EventHandler(SwitchToShellButton_Click);
		this.SwitchToRuneButton.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.SwitchToRuneButton.Image = NosAssistant2.Properties.Resources.rune;
		this.SwitchToRuneButton.Location = new System.Drawing.Point(50, 10);
		this.SwitchToRuneButton.Name = "SwitchToRuneButton";
		this.SwitchToRuneButton.Size = new System.Drawing.Size(30, 30);
		this.SwitchToRuneButton.TabIndex = 52;
		this.SwitchToRuneButton.TabStop = false;
		this.SwitchToRuneButton.Click += new System.EventHandler(SwitchToRuneButton_Click);
		this.SwitchShellTypeButton.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.SwitchShellTypeButton.Image = NosAssistant2.Properties.Resources.switch_icon;
		this.SwitchShellTypeButton.Location = new System.Drawing.Point(260, 10);
		this.SwitchShellTypeButton.Name = "SwitchShellTypeButton";
		this.SwitchShellTypeButton.Size = new System.Drawing.Size(30, 30);
		this.SwitchShellTypeButton.TabIndex = 50;
		this.SwitchShellTypeButton.TabStop = false;
		this.SwitchShellTypeButton.Click += new System.EventHandler(SwitchShellTypeButton_Click);
		this.SwitchShellTypeButton.MouseEnter += new System.EventHandler(SwitchShellTypeButton_MouseEnter);
		this.SwitchShellTypeButton.MouseLeave += new System.EventHandler(SwitchShellTypeButton_MouseLeave);
		this.ShellItemTypeLabel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.ShellItemTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ShellItemTypeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ShellItemTypeLabel.Location = new System.Drawing.Point(0, 0);
		this.ShellItemTypeLabel.Name = "ShellItemTypeLabel";
		this.ShellItemTypeLabel.Size = new System.Drawing.Size(300, 50);
		this.ShellItemTypeLabel.TabIndex = 51;
		this.ShellItemTypeLabel.Text = "Type";
		this.ShellItemTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SearchedPlayerTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerTitle.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerTitle.Location = new System.Drawing.Point(950, 230);
		this.SearchedPlayerTitle.Name = "SearchedPlayerTitle";
		this.SearchedPlayerTitle.Size = new System.Drawing.Size(200, 25);
		this.SearchedPlayerTitle.TabIndex = 47;
		this.SearchedPlayerTitle.Text = "Title";
		this.SearchedPlayerTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SearchedPlayerFamilyRole.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerFamilyRole.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerFamilyRole.Location = new System.Drawing.Point(950, 290);
		this.SearchedPlayerFamilyRole.Name = "SearchedPlayerFamilyRole";
		this.SearchedPlayerFamilyRole.Size = new System.Drawing.Size(200, 25);
		this.SearchedPlayerFamilyRole.TabIndex = 46;
		this.SearchedPlayerFamilyRole.Text = "Family Role";
		this.SearchedPlayerFamilyRole.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SearchedPlayerLastUpdateDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerLastUpdateDateLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerLastUpdateDateLabel.Location = new System.Drawing.Point(950, 380);
		this.SearchedPlayerLastUpdateDateLabel.Name = "SearchedPlayerLastUpdateDateLabel";
		this.SearchedPlayerLastUpdateDateLabel.Size = new System.Drawing.Size(200, 25);
		this.SearchedPlayerLastUpdateDateLabel.TabIndex = 45;
		this.SearchedPlayerLastUpdateDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SearchedPlayerLastUpdateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerLastUpdateLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerLastUpdateLabel.Location = new System.Drawing.Point(950, 350);
		this.SearchedPlayerLastUpdateLabel.Name = "SearchedPlayerLastUpdateLabel";
		this.SearchedPlayerLastUpdateLabel.Size = new System.Drawing.Size(200, 25);
		this.SearchedPlayerLastUpdateLabel.TabIndex = 44;
		this.SearchedPlayerLastUpdateLabel.Text = "Last Updated";
		this.SearchedPlayerLastUpdateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SearchedPlayerFairiesFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.SearchedPlayerFairiesFlowLayoutPanel.Location = new System.Drawing.Point(50, 435);
		this.SearchedPlayerFairiesFlowLayoutPanel.Name = "SearchedPlayerFairiesFlowLayoutPanel";
		this.SearchedPlayerFairiesFlowLayoutPanel.Size = new System.Drawing.Size(60, 240);
		this.SearchedPlayerFairiesFlowLayoutPanel.TabIndex = 43;
		this.Wings.Location = new System.Drawing.Point(667, 265);
		this.Wings.Name = "Wings";
		this.Wings.Size = new System.Drawing.Size(30, 30);
		this.Wings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.Wings.TabIndex = 41;
		this.Wings.TabStop = false;
		this.SearchedPlayerWingsLabel.AutoSize = true;
		this.SearchedPlayerWingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerWingsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerWingsLabel.Location = new System.Drawing.Point(512, 270);
		this.SearchedPlayerWingsLabel.Name = "SearchedPlayerWingsLabel";
		this.SearchedPlayerWingsLabel.Size = new System.Drawing.Size(63, 20);
		this.SearchedPlayerWingsLabel.TabIndex = 40;
		this.SearchedPlayerWingsLabel.Text = "Wings:";
		this.SearchedPlayerWingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.WeaponSkin.Location = new System.Drawing.Point(667, 225);
		this.WeaponSkin.Name = "WeaponSkin";
		this.WeaponSkin.Size = new System.Drawing.Size(30, 30);
		this.WeaponSkin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.WeaponSkin.TabIndex = 39;
		this.WeaponSkin.TabStop = false;
		this.SearchedPlayerWeaponSkinLabel.AutoSize = true;
		this.SearchedPlayerWeaponSkinLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerWeaponSkinLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerWeaponSkinLabel.Location = new System.Drawing.Point(512, 230);
		this.SearchedPlayerWeaponSkinLabel.Name = "SearchedPlayerWeaponSkinLabel";
		this.SearchedPlayerWeaponSkinLabel.Size = new System.Drawing.Size(120, 20);
		this.SearchedPlayerWeaponSkinLabel.TabIndex = 38;
		this.SearchedPlayerWeaponSkinLabel.Text = "Weapon Skin:";
		this.SearchedPlayerWeaponSkinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.CostumeHat.Location = new System.Drawing.Point(667, 185);
		this.CostumeHat.Name = "CostumeHat";
		this.CostumeHat.Size = new System.Drawing.Size(30, 30);
		this.CostumeHat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.CostumeHat.TabIndex = 37;
		this.CostumeHat.TabStop = false;
		this.SearchedPlayerCostumeHatLabel.AutoSize = true;
		this.SearchedPlayerCostumeHatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerCostumeHatLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerCostumeHatLabel.Location = new System.Drawing.Point(512, 190);
		this.SearchedPlayerCostumeHatLabel.Name = "SearchedPlayerCostumeHatLabel";
		this.SearchedPlayerCostumeHatLabel.Size = new System.Drawing.Size(119, 20);
		this.SearchedPlayerCostumeHatLabel.TabIndex = 36;
		this.SearchedPlayerCostumeHatLabel.Text = "Costume Hat:";
		this.SearchedPlayerCostumeHatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.Costume.Location = new System.Drawing.Point(667, 145);
		this.Costume.Name = "Costume";
		this.Costume.Size = new System.Drawing.Size(30, 30);
		this.Costume.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.Costume.TabIndex = 35;
		this.Costume.TabStop = false;
		this.SearchedPlayerCostumeLabel.AutoSize = true;
		this.SearchedPlayerCostumeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerCostumeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerCostumeLabel.Location = new System.Drawing.Point(512, 150);
		this.SearchedPlayerCostumeLabel.Name = "SearchedPlayerCostumeLabel";
		this.SearchedPlayerCostumeLabel.Size = new System.Drawing.Size(85, 20);
		this.SearchedPlayerCostumeLabel.TabIndex = 34;
		this.SearchedPlayerCostumeLabel.Text = "Costume:";
		this.SearchedPlayerCostumeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.FlyingPet.Location = new System.Drawing.Point(667, 105);
		this.FlyingPet.Name = "FlyingPet";
		this.FlyingPet.Size = new System.Drawing.Size(30, 30);
		this.FlyingPet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.FlyingPet.TabIndex = 33;
		this.FlyingPet.TabStop = false;
		this.SearchedPlayerFlyingPetLabel.AutoSize = true;
		this.SearchedPlayerFlyingPetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerFlyingPetLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerFlyingPetLabel.Location = new System.Drawing.Point(512, 110);
		this.SearchedPlayerFlyingPetLabel.Name = "SearchedPlayerFlyingPetLabel";
		this.SearchedPlayerFlyingPetLabel.Size = new System.Drawing.Size(78, 20);
		this.SearchedPlayerFlyingPetLabel.TabIndex = 32;
		this.SearchedPlayerFlyingPetLabel.Text = "Mini Pet:";
		this.SearchedPlayerFlyingPetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.Mask.Location = new System.Drawing.Point(667, 65);
		this.Mask.Name = "Mask";
		this.Mask.Size = new System.Drawing.Size(30, 30);
		this.Mask.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.Mask.TabIndex = 31;
		this.Mask.TabStop = false;
		this.SearchedPlayerMaskLabel.AutoSize = true;
		this.SearchedPlayerMaskLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerMaskLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerMaskLabel.Location = new System.Drawing.Point(512, 70);
		this.SearchedPlayerMaskLabel.Name = "SearchedPlayerMaskLabel";
		this.SearchedPlayerMaskLabel.Size = new System.Drawing.Size(56, 20);
		this.SearchedPlayerMaskLabel.TabIndex = 30;
		this.SearchedPlayerMaskLabel.Text = "Mask:";
		this.SearchedPlayerMaskLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.Hat.Location = new System.Drawing.Point(667, 25);
		this.Hat.Name = "Hat";
		this.Hat.Size = new System.Drawing.Size(30, 30);
		this.Hat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.Hat.TabIndex = 29;
		this.Hat.TabStop = false;
		this.SearchedPlayerHatLabel.AutoSize = true;
		this.SearchedPlayerHatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerHatLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerHatLabel.Location = new System.Drawing.Point(512, 30);
		this.SearchedPlayerHatLabel.Name = "SearchedPlayerHatLabel";
		this.SearchedPlayerHatLabel.Size = new System.Drawing.Size(43, 20);
		this.SearchedPlayerHatLabel.TabIndex = 28;
		this.SearchedPlayerHatLabel.Text = "Hat:";
		this.SearchedPlayerHatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ArmorUpgrade.AutoSize = true;
		this.ArmorUpgrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ArmorUpgrade.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ArmorUpgrade.Location = new System.Drawing.Point(922, 110);
		this.ArmorUpgrade.Name = "ArmorUpgrade";
		this.ArmorUpgrade.Size = new System.Drawing.Size(0, 20);
		this.ArmorUpgrade.TabIndex = 27;
		this.ArmorUpgrade.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.Armor.Location = new System.Drawing.Point(882, 105);
		this.Armor.Name = "Armor";
		this.Armor.Size = new System.Drawing.Size(30, 30);
		this.Armor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.Armor.TabIndex = 16;
		this.Armor.TabStop = false;
		this.SearchedPlayerArmorLabel.AutoSize = true;
		this.SearchedPlayerArmorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerArmorLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerArmorLabel.Location = new System.Drawing.Point(727, 110);
		this.SearchedPlayerArmorLabel.Name = "SearchedPlayerArmorLabel";
		this.SearchedPlayerArmorLabel.Size = new System.Drawing.Size(62, 20);
		this.SearchedPlayerArmorLabel.TabIndex = 15;
		this.SearchedPlayerArmorLabel.Text = "Armor:";
		this.SearchedPlayerArmorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.SecondaryWeaponUpgrade.AutoSize = true;
		this.SecondaryWeaponUpgrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SecondaryWeaponUpgrade.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SecondaryWeaponUpgrade.Location = new System.Drawing.Point(922, 70);
		this.SecondaryWeaponUpgrade.Name = "SecondaryWeaponUpgrade";
		this.SecondaryWeaponUpgrade.Size = new System.Drawing.Size(0, 20);
		this.SecondaryWeaponUpgrade.TabIndex = 14;
		this.SecondaryWeaponUpgrade.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SecondaryWeapon.Location = new System.Drawing.Point(882, 65);
		this.SecondaryWeapon.Name = "SecondaryWeapon";
		this.SecondaryWeapon.Size = new System.Drawing.Size(30, 30);
		this.SecondaryWeapon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SecondaryWeapon.TabIndex = 13;
		this.SecondaryWeapon.TabStop = false;
		this.SearchedPlayerSecondWeaponLabel.AutoSize = true;
		this.SearchedPlayerSecondWeaponLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerSecondWeaponLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerSecondWeaponLabel.Location = new System.Drawing.Point(727, 70);
		this.SearchedPlayerSecondWeaponLabel.Name = "SearchedPlayerSecondWeaponLabel";
		this.SearchedPlayerSecondWeaponLabel.Size = new System.Drawing.Size(151, 20);
		this.SearchedPlayerSecondWeaponLabel.TabIndex = 12;
		this.SearchedPlayerSecondWeaponLabel.Text = "Second. Weapon:";
		this.SearchedPlayerSecondWeaponLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MainWeaponUpgrade.AutoSize = true;
		this.MainWeaponUpgrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.MainWeaponUpgrade.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.MainWeaponUpgrade.Location = new System.Drawing.Point(922, 30);
		this.MainWeaponUpgrade.Name = "MainWeaponUpgrade";
		this.MainWeaponUpgrade.Size = new System.Drawing.Size(0, 20);
		this.MainWeaponUpgrade.TabIndex = 11;
		this.MainWeaponUpgrade.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.MainWeapon.Location = new System.Drawing.Point(882, 25);
		this.MainWeapon.Name = "MainWeapon";
		this.MainWeapon.Size = new System.Drawing.Size(30, 30);
		this.MainWeapon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.MainWeapon.TabIndex = 10;
		this.MainWeapon.TabStop = false;
		this.SearchedPlayerMainWeaponLabel.AutoSize = true;
		this.SearchedPlayerMainWeaponLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerMainWeaponLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerMainWeaponLabel.Location = new System.Drawing.Point(727, 30);
		this.SearchedPlayerMainWeaponLabel.Name = "SearchedPlayerMainWeaponLabel";
		this.SearchedPlayerMainWeaponLabel.Size = new System.Drawing.Size(123, 20);
		this.SearchedPlayerMainWeaponLabel.TabIndex = 9;
		this.SearchedPlayerMainWeaponLabel.Text = "Main Weapon:";
		this.SearchedPlayerMainWeaponLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.Reputation.Location = new System.Drawing.Point(1090, 172);
		this.Reputation.Name = "Reputation";
		this.Reputation.Size = new System.Drawing.Size(21, 21);
		this.Reputation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.Reputation.TabIndex = 8;
		this.Reputation.TabStop = false;
		this.SearchedPlayerLVLCLVL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerLVLCLVL.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerLVLCLVL.Location = new System.Drawing.Point(1010, 170);
		this.SearchedPlayerLVLCLVL.Name = "SearchedPlayerLVLCLVL";
		this.SearchedPlayerLVLCLVL.Size = new System.Drawing.Size(80, 25);
		this.SearchedPlayerLVLCLVL.TabIndex = 7;
		this.SearchedPlayerLVLCLVL.Text = "LVL:CLV";
		this.SearchedPlayerLVLCLVL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SearchedPlayerClassSex.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerClassSex.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerClassSex.Location = new System.Drawing.Point(950, 200);
		this.SearchedPlayerClassSex.Name = "SearchedPlayerClassSex";
		this.SearchedPlayerClassSex.Size = new System.Drawing.Size(200, 25);
		this.SearchedPlayerClassSex.TabIndex = 6;
		this.SearchedPlayerClassSex.Text = "Class:Gender";
		this.SearchedPlayerClassSex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SearchedPlayerFamily.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerFamily.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerFamily.Location = new System.Drawing.Point(950, 260);
		this.SearchedPlayerFamily.Name = "SearchedPlayerFamily";
		this.SearchedPlayerFamily.Size = new System.Drawing.Size(200, 25);
		this.SearchedPlayerFamily.TabIndex = 5;
		this.SearchedPlayerFamily.Text = "Family [lvl]";
		this.SearchedPlayerFamily.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SearchedPlayerNickname.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchedPlayerNickname.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchedPlayerNickname.Location = new System.Drawing.Point(950, 140);
		this.SearchedPlayerNickname.Name = "SearchedPlayerNickname";
		this.SearchedPlayerNickname.Size = new System.Drawing.Size(200, 25);
		this.SearchedPlayerNickname.TabIndex = 4;
		this.SearchedPlayerNickname.Text = "Nickname";
		this.SearchedPlayerNickname.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.SearchedPlayerAvatar.Location = new System.Drawing.Point(1000, 25);
		this.SearchedPlayerAvatar.Name = "SearchedPlayerAvatar";
		this.SearchedPlayerAvatar.Size = new System.Drawing.Size(100, 100);
		this.SearchedPlayerAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.SearchedPlayerAvatar.TabIndex = 3;
		this.SearchedPlayerAvatar.TabStop = false;
		this.SearchPlayerButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.SearchPlayerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.SearchPlayerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.SearchPlayerButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchPlayerButton.Location = new System.Drawing.Point(15, 14);
		this.SearchPlayerButton.Name = "SearchPlayerButton";
		this.SearchPlayerButton.Size = new System.Drawing.Size(152, 46);
		this.SearchPlayerButton.TabIndex = 2;
		this.SearchPlayerButton.Text = "Search";
		this.SearchPlayerButton.UseVisualStyleBackColor = false;
		this.SearchPlayerButton.Click += new System.EventHandler(SearchPlayerButton_Click);
		this.SearchServerComboBox.BackColor = System.Drawing.Color.White;
		this.SearchServerComboBox.Location = new System.Drawing.Point(15, 111);
		this.SearchServerComboBox.Name = "SearchServerComboBox";
		this.SearchServerComboBox.Size = new System.Drawing.Size(152, 32);
		this.SearchServerComboBox.TabIndex = 1;
		this.SearchNicknameTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.SearchNicknameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.SearchNicknameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.SearchNicknameTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.SearchNicknameTextBox.Location = new System.Drawing.Point(15, 70);
		this.SearchNicknameTextBox.Name = "SearchNicknameTextBox";
		this.SearchNicknameTextBox.Size = new System.Drawing.Size(152, 26);
		this.SearchNicknameTextBox.TabIndex = 0;
		this.SearchNicknameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.SearchNicknameTextBox.TextChanged += new System.EventHandler(SearchNicknameTextBox_TextChanged);
		this.SearchNicknameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(SearchNicknameTextBox_KeyPress);
		this.MainFairyDetailsPanel.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.MainFairyDetailsPanel.Controls.Add(this.FairyDetailsPanel);
		this.MainFairyDetailsPanel.Location = new System.Drawing.Point(440, 153);
		this.MainFairyDetailsPanel.Name = "MainFairyDetailsPanel";
		this.MainFairyDetailsPanel.Size = new System.Drawing.Size(320, 395);
		this.MainFairyDetailsPanel.TabIndex = 55;
		this.MainFairyDetailsPanel.Visible = false;
		this.MainFairyDetailsPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.FairyDetailsPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.FairyDetailsPanel.Controls.Add(this.FairyUpgradePercentLabel);
		this.FairyDetailsPanel.Controls.Add(this.FairyEffectsFlowLayoutPanel);
		this.FairyDetailsPanel.Controls.Add(this.FairyDetailsIcon);
		this.FairyDetailsPanel.Controls.Add(this.CloseFairyDetailsButton);
		this.FairyDetailsPanel.Controls.Add(this.FairyDetailsLabel);
		this.FairyDetailsPanel.Location = new System.Drawing.Point(10, 10);
		this.FairyDetailsPanel.Name = "FairyDetailsPanel";
		this.FairyDetailsPanel.Size = new System.Drawing.Size(300, 375);
		this.FairyDetailsPanel.TabIndex = 54;
		this.FairyUpgradePercentLabel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.FairyUpgradePercentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.FairyUpgradePercentLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.FairyUpgradePercentLabel.Location = new System.Drawing.Point(0, 335);
		this.FairyUpgradePercentLabel.Name = "FairyUpgradePercentLabel";
		this.FairyUpgradePercentLabel.Size = new System.Drawing.Size(300, 40);
		this.FairyUpgradePercentLabel.TabIndex = 56;
		this.FairyUpgradePercentLabel.Text = "0/9 (80%)";
		this.FairyUpgradePercentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.FairyEffectsFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.FairyEffectsFlowLayoutPanel.Location = new System.Drawing.Point(0, 50);
		this.FairyEffectsFlowLayoutPanel.Name = "FairyEffectsFlowLayoutPanel";
		this.FairyEffectsFlowLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
		this.FairyEffectsFlowLayoutPanel.Size = new System.Drawing.Size(300, 285);
		this.FairyEffectsFlowLayoutPanel.TabIndex = 54;
		this.FairyEffectsFlowLayoutPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.FairyDetailsIcon.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.FairyDetailsIcon.Location = new System.Drawing.Point(10, 10);
		this.FairyDetailsIcon.Name = "FairyDetailsIcon";
		this.FairyDetailsIcon.Size = new System.Drawing.Size(30, 30);
		this.FairyDetailsIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.FairyDetailsIcon.TabIndex = 53;
		this.FairyDetailsIcon.TabStop = false;
		this.FairyDetailsIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.CloseFairyDetailsButton.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.CloseFairyDetailsButton.Image = NosAssistant2.Properties.Resources.close_small;
		this.CloseFairyDetailsButton.Location = new System.Drawing.Point(260, 10);
		this.CloseFairyDetailsButton.Name = "CloseFairyDetailsButton";
		this.CloseFairyDetailsButton.Size = new System.Drawing.Size(30, 30);
		this.CloseFairyDetailsButton.TabIndex = 50;
		this.CloseFairyDetailsButton.TabStop = false;
		this.CloseFairyDetailsButton.Click += new System.EventHandler(CloseFairyDetailsButton_Click);
		this.FairyDetailsLabel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.FairyDetailsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.FairyDetailsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.FairyDetailsLabel.Location = new System.Drawing.Point(0, 0);
		this.FairyDetailsLabel.Name = "FairyDetailsLabel";
		this.FairyDetailsLabel.Size = new System.Drawing.Size(300, 50);
		this.FairyDetailsLabel.TabIndex = 51;
		this.FairyDetailsLabel.Text = "Ladine+0";
		this.FairyDetailsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.FairyDetailsLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.PlayerRaidsStatisticsButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.PlayerRaidsStatisticsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.PlayerRaidsStatisticsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayerRaidsStatisticsButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayerRaidsStatisticsButton.Location = new System.Drawing.Point(15, 153);
		this.PlayerRaidsStatisticsButton.Name = "PlayerRaidsStatisticsButton";
		this.PlayerRaidsStatisticsButton.Size = new System.Drawing.Size(152, 45);
		this.PlayerRaidsStatisticsButton.TabIndex = 1;
		this.PlayerRaidsStatisticsButton.Text = "Raids";
		this.PlayerRaidsStatisticsButton.UseVisualStyleBackColor = false;
		this.PlayerRaidsStatisticsButton.Click += new System.EventHandler(PlayerRaidsStatisticsButton_Click);
		this.RaidsHistoryBestTimeLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsHistoryBestTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RaidsHistoryBestTimeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaidsHistoryBestTimeLabel.Location = new System.Drawing.Point(800, 670);
		this.RaidsHistoryBestTimeLabel.Name = "RaidsHistoryBestTimeLabel";
		this.RaidsHistoryBestTimeLabel.Size = new System.Drawing.Size(370, 23);
		this.RaidsHistoryBestTimeLabel.TabIndex = 18;
		this.RaidsHistoryBestTimeLabel.Text = "Best Time: 00:00";
		this.RaidsHistoryBestTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RaidsHistoryBestTimeLabel.Visible = false;
		this.RaidsHistoryAverageTimeLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsHistoryAverageTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RaidsHistoryAverageTimeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaidsHistoryAverageTimeLabel.Location = new System.Drawing.Point(800, 695);
		this.RaidsHistoryAverageTimeLabel.Name = "RaidsHistoryAverageTimeLabel";
		this.RaidsHistoryAverageTimeLabel.Size = new System.Drawing.Size(370, 23);
		this.RaidsHistoryAverageTimeLabel.TabIndex = 17;
		this.RaidsHistoryAverageTimeLabel.Text = "Average Time: 00:00";
		this.RaidsHistoryAverageTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RaidsHistoryAverageTimeLabel.Visible = false;
		this.RankingTabLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 90);
		this.RankingTabLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RankingTabLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RankingTabLabel.Location = new System.Drawing.Point(325, 55);
		this.RankingTabLabel.Name = "RankingTabLabel";
		this.RankingTabLabel.Size = new System.Drawing.Size(100, 20);
		this.RankingTabLabel.TabIndex = 14;
		this.RankingTabLabel.Text = "Ranking";
		this.RankingTabLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RankingTabLabel.Click += new System.EventHandler(RankingTabLabel_Click);
		this.ShowMarathonTotalButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.ShowMarathonTotalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ShowMarathonTotalButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ShowMarathonTotalButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ShowMarathonTotalButton.Location = new System.Drawing.Point(940, 735);
		this.ShowMarathonTotalButton.Name = "ShowMarathonTotalButton";
		this.ShowMarathonTotalButton.Size = new System.Drawing.Size(100, 45);
		this.ShowMarathonTotalButton.TabIndex = 13;
		this.ShowMarathonTotalButton.Text = "Total";
		this.ShowMarathonTotalButton.UseVisualStyleBackColor = false;
		this.ShowMarathonTotalButton.Visible = false;
		this.ShowMarathonTotalButton.Click += new System.EventHandler(ShowMarathonTotalButton_Click);
		this.AnalyticsBackArrow.Image = NosAssistant2.Properties.Resources.back_arrow;
		this.AnalyticsBackArrow.Location = new System.Drawing.Point(25, 20);
		this.AnalyticsBackArrow.Name = "AnalyticsBackArrow";
		this.AnalyticsBackArrow.Size = new System.Drawing.Size(30, 30);
		this.AnalyticsBackArrow.TabIndex = 12;
		this.AnalyticsBackArrow.TabStop = false;
		this.AnalyticsBackArrow.Visible = false;
		this.AnalyticsBackArrow.Click += new System.EventHandler(AnalyticsBackArrow_Click);
		this.AnalyticsBackArrow.MouseEnter += new System.EventHandler(AnalyticsBackArrow_MouseEnter);
		this.AnalyticsBackArrow.MouseLeave += new System.EventHandler(AnalyticsBackArrow_MouseLeave);
		this.PlayersTabLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 90);
		this.PlayersTabLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.PlayersTabLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.PlayersTabLabel.Location = new System.Drawing.Point(225, 55);
		this.PlayersTabLabel.Name = "PlayersTabLabel";
		this.PlayersTabLabel.Size = new System.Drawing.Size(100, 20);
		this.PlayersTabLabel.TabIndex = 11;
		this.PlayersTabLabel.Text = "Players";
		this.PlayersTabLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.PlayersTabLabel.Click += new System.EventHandler(PlayersTabLabel_Click);
		this.FamRecordsNextPageButton.Image = NosAssistant2.Properties.Resources.triangleRight;
		this.FamRecordsNextPageButton.Location = new System.Drawing.Point(1175, 363);
		this.FamRecordsNextPageButton.Name = "FamRecordsNextPageButton";
		this.FamRecordsNextPageButton.Size = new System.Drawing.Size(25, 25);
		this.FamRecordsNextPageButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.FamRecordsNextPageButton.TabIndex = 10;
		this.FamRecordsNextPageButton.TabStop = false;
		this.FamRecordsNextPageButton.Visible = false;
		this.FamRecordsNextPageButton.Click += new System.EventHandler(FamRecordsNextPageButton_Click);
		this.FamRecordsNextPageButton.MouseEnter += new System.EventHandler(FamRecordsNextPageButton_MouseEnter);
		this.FamRecordsNextPageButton.MouseLeave += new System.EventHandler(FamRecordsNextPageButton_MouseLeave);
		this.FamRecordsPreviousPageButton.Image = NosAssistant2.Properties.Resources.triangleLeft;
		this.FamRecordsPreviousPageButton.Location = new System.Drawing.Point(0, 363);
		this.FamRecordsPreviousPageButton.Name = "FamRecordsPreviousPageButton";
		this.FamRecordsPreviousPageButton.Size = new System.Drawing.Size(25, 25);
		this.FamRecordsPreviousPageButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.FamRecordsPreviousPageButton.TabIndex = 9;
		this.FamRecordsPreviousPageButton.TabStop = false;
		this.FamRecordsPreviousPageButton.Visible = false;
		this.FamRecordsPreviousPageButton.Click += new System.EventHandler(FamRecordsPreviousPageButton_Click);
		this.FamRecordsPreviousPageButton.MouseEnter += new System.EventHandler(FamRecordsPreviousPageButton_MouseEnter);
		this.FamRecordsPreviousPageButton.MouseLeave += new System.EventHandler(FamRecordsPreviousPageButton_MouseLeave);
		this.FamRecordsTabLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 90);
		this.FamRecordsTabLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.FamRecordsTabLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.FamRecordsTabLabel.Location = new System.Drawing.Point(125, 55);
		this.FamRecordsTabLabel.Name = "FamRecordsTabLabel";
		this.FamRecordsTabLabel.Size = new System.Drawing.Size(100, 20);
		this.FamRecordsTabLabel.TabIndex = 8;
		this.FamRecordsTabLabel.Text = "Fam Records";
		this.FamRecordsTabLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.FamRecordsTabLabel.Click += new System.EventHandler(FamRecordsLabel_Click);
		this.RaidsHistoryTabLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsHistoryTabLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RaidsHistoryTabLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaidsHistoryTabLabel.Location = new System.Drawing.Point(25, 55);
		this.RaidsHistoryTabLabel.Name = "RaidsHistoryTabLabel";
		this.RaidsHistoryTabLabel.Size = new System.Drawing.Size(100, 20);
		this.RaidsHistoryTabLabel.TabIndex = 7;
		this.RaidsHistoryTabLabel.Text = "Raids History";
		this.RaidsHistoryTabLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RaidsHistoryTabLabel.Click += new System.EventHandler(RaidsHistoryTabLabel_Click);
		this.BackArrowRaidsHistory.Image = NosAssistant2.Properties.Resources.back_arrow;
		this.BackArrowRaidsHistory.Location = new System.Drawing.Point(25, 20);
		this.BackArrowRaidsHistory.Name = "BackArrowRaidsHistory";
		this.BackArrowRaidsHistory.Size = new System.Drawing.Size(30, 30);
		this.BackArrowRaidsHistory.TabIndex = 6;
		this.BackArrowRaidsHistory.TabStop = false;
		this.BackArrowRaidsHistory.Visible = false;
		this.BackArrowRaidsHistory.Click += new System.EventHandler(BackArrowRaidsHistory_Click);
		this.BackArrowRaidsHistory.MouseEnter += new System.EventHandler(BackArrowRaidsHistory_MouseEnter);
		this.BackArrowRaidsHistory.MouseLeave += new System.EventHandler(BackArrowRaidsHistory_MouseLeave);
		this.RaidsHistoryFilterTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.RaidsHistoryFilterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.RaidsHistoryFilterTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.RaidsHistoryFilterTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaidsHistoryFilterTextBox.Location = new System.Drawing.Point(620, 760);
		this.RaidsHistoryFilterTextBox.Name = "RaidsHistoryFilterTextBox";
		this.RaidsHistoryFilterTextBox.Size = new System.Drawing.Size(130, 26);
		this.RaidsHistoryFilterTextBox.TabIndex = 16;
		this.RaidsHistoryFilterTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.RaidsHistoryFilterTextBox.TextChanged += new System.EventHandler(RaidsHistoryFilterTextBox_TextChanged);
		this.RaidsHistoryPageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RaidsHistoryPageLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaidsHistoryPageLabel.Location = new System.Drawing.Point(50, 761);
		this.RaidsHistoryPageLabel.Name = "RaidsHistoryPageLabel";
		this.RaidsHistoryPageLabel.Size = new System.Drawing.Size(700, 23);
		this.RaidsHistoryPageLabel.TabIndex = 5;
		this.RaidsHistoryPageLabel.Text = "Page: 1";
		this.RaidsHistoryPageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RaidsHistoryPageLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.RaidsHistoryNextPageButton.Image = NosAssistant2.Properties.Resources.triangleRight;
		this.RaidsHistoryNextPageButton.Location = new System.Drawing.Point(750, 760);
		this.RaidsHistoryNextPageButton.Name = "RaidsHistoryNextPageButton";
		this.RaidsHistoryNextPageButton.Size = new System.Drawing.Size(25, 25);
		this.RaidsHistoryNextPageButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.RaidsHistoryNextPageButton.TabIndex = 4;
		this.RaidsHistoryNextPageButton.TabStop = false;
		this.RaidsHistoryNextPageButton.Click += new System.EventHandler(RaidsHistoryNextPageButton_Click);
		this.RaidsHistoryNextPageButton.MouseEnter += new System.EventHandler(RaidsHistoryNextPageButton_MouseEnter);
		this.RaidsHistoryNextPageButton.MouseLeave += new System.EventHandler(RaidsHistoryNextPageButton_MouseLeave);
		this.RaidsHistoryPreviousPageButton.Image = NosAssistant2.Properties.Resources.triangleLeft;
		this.RaidsHistoryPreviousPageButton.Location = new System.Drawing.Point(25, 760);
		this.RaidsHistoryPreviousPageButton.Name = "RaidsHistoryPreviousPageButton";
		this.RaidsHistoryPreviousPageButton.Size = new System.Drawing.Size(25, 25);
		this.RaidsHistoryPreviousPageButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.RaidsHistoryPreviousPageButton.TabIndex = 3;
		this.RaidsHistoryPreviousPageButton.TabStop = false;
		this.RaidsHistoryPreviousPageButton.Click += new System.EventHandler(RaidsHistoryPreviousPageButton_Click);
		this.RaidsHistoryPreviousPageButton.MouseEnter += new System.EventHandler(RaidsHistoryPreviousPageButton_MouseEnter);
		this.RaidsHistoryPreviousPageButton.MouseLeave += new System.EventHandler(RaidsHistoryPreviousPageButton_MouseLeave);
		this.RaidsHistoryDoubleBufferedPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsHistoryDoubleBufferedPanel.Controls.Add(this.RaidsHistoryDetailsPanel);
		this.RaidsHistoryDoubleBufferedPanel.Controls.Add(this.RaidsHistoryFlowLayoutPanel);
		this.RaidsHistoryDoubleBufferedPanel.Location = new System.Drawing.Point(25, 75);
		this.RaidsHistoryDoubleBufferedPanel.Name = "RaidsHistoryDoubleBufferedPanel";
		this.RaidsHistoryDoubleBufferedPanel.Size = new System.Drawing.Size(750, 675);
		this.RaidsHistoryDoubleBufferedPanel.TabIndex = 0;
		this.RaidsHistoryDetailsPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsHistoryDetailsPanel.Controls.Add(this.RaidsHistoryListViewBorder);
		this.RaidsHistoryDetailsPanel.Location = new System.Drawing.Point(0, 0);
		this.RaidsHistoryDetailsPanel.Margin = new System.Windows.Forms.Padding(0);
		this.RaidsHistoryDetailsPanel.Name = "RaidsHistoryDetailsPanel";
		this.RaidsHistoryDetailsPanel.Size = new System.Drawing.Size(750, 675);
		this.RaidsHistoryDetailsPanel.TabIndex = 2;
		this.RaidsHistoryDetailsPanel.Visible = false;
		this.RaidsHistoryListViewBorder.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.RaidsHistoryListViewBorder.Controls.Add(this.RaidsHistoryDetailsGridView);
		this.RaidsHistoryListViewBorder.Location = new System.Drawing.Point(0, 0);
		this.RaidsHistoryListViewBorder.Margin = new System.Windows.Forms.Padding(0);
		this.RaidsHistoryListViewBorder.Name = "RaidsHistoryListViewBorder";
		this.RaidsHistoryListViewBorder.Size = new System.Drawing.Size(750, 675);
		this.RaidsHistoryListViewBorder.TabIndex = 1;
		this.RaidsHistoryDetailsGridView.AllowUserToAddRows = false;
		this.RaidsHistoryDetailsGridView.AllowUserToDeleteRows = false;
		this.RaidsHistoryDetailsGridView.AllowUserToOrderColumns = true;
		this.RaidsHistoryDetailsGridView.AllowUserToResizeColumns = false;
		this.RaidsHistoryDetailsGridView.AllowUserToResizeRows = false;
		this.RaidsHistoryDetailsGridView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.RaidsHistoryDetailsGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.RaidsHistoryDetailsGridView.BackgroundColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsHistoryDetailsGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.RaidsHistoryDetailsGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
		dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(72, 149, 239);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9f);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.RaidsHistoryDetailsGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
		this.RaidsHistoryDetailsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
		this.RaidsHistoryDetailsGridView.Columns.AddRange(this.ID, this.CharacterID, this.Lp, this.PlayerName, this.CLvl, this.Family, this.Total, this.MaxHit, this.MaxHitIcon, this.Pets, this.Special, this.MobDmg, this.OnyxDmg, this.All, this.Gold, this.Average, this.Hit, this.Miss, this.Crit, this.Bon, this.BonCrit, this.Dbf, this.Dead, this.MBHit, this.AllHits, this.AllMiss);
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9f);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.DeepSkyBlue;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(72, 149, 239);
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.RaidsHistoryDetailsGridView.DefaultCellStyle = dataGridViewCellStyle5;
		this.RaidsHistoryDetailsGridView.EnableHeadersVisualStyles = false;
		this.RaidsHistoryDetailsGridView.GridColor = System.Drawing.Color.FromArgb(72, 149, 239);
		this.RaidsHistoryDetailsGridView.Location = new System.Drawing.Point(1, 1);
		this.RaidsHistoryDetailsGridView.MultiSelect = false;
		this.RaidsHistoryDetailsGridView.Name = "RaidsHistoryDetailsGridView";
		this.RaidsHistoryDetailsGridView.ReadOnly = true;
		this.RaidsHistoryDetailsGridView.RowHeadersVisible = false;
		this.RaidsHistoryDetailsGridView.RowHeadersWidth = 30;
		this.RaidsHistoryDetailsGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.RaidsHistoryDetailsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
		this.RaidsHistoryDetailsGridView.Size = new System.Drawing.Size(748, 673);
		this.RaidsHistoryDetailsGridView.TabIndex = 0;
		this.RaidsHistoryDetailsGridView.Paint += new System.Windows.Forms.PaintEventHandler(DataGridView_Paint);
		this.ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.ID.DataPropertyName = "ID";
		this.ID.HeaderText = "";
		this.ID.Name = "ID";
		this.ID.ReadOnly = true;
		this.ID.Visible = false;
		this.ID.Width = 5;
		this.CharacterID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.CharacterID.DataPropertyName = "CharacterID";
		this.CharacterID.HeaderText = "";
		this.CharacterID.Name = "CharacterID";
		this.CharacterID.ReadOnly = true;
		this.CharacterID.Visible = false;
		this.CharacterID.Width = 5;
		this.Lp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Lp.DataPropertyName = "Lp";
		this.Lp.HeaderText = "Nr";
		this.Lp.Name = "Lp";
		this.Lp.ReadOnly = true;
		this.Lp.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Lp.Width = 25;
		this.PlayerName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.PlayerName.DataPropertyName = "Nickname";
		this.PlayerName.FillWeight = 101f;
		this.PlayerName.HeaderText = "Nickname";
		this.PlayerName.Name = "PlayerName";
		this.PlayerName.ReadOnly = true;
		this.PlayerName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
		this.PlayerName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.CLvl.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.CLvl.DataPropertyName = "CLvl";
		this.CLvl.HeaderText = "Lvl";
		this.CLvl.Name = "CLvl";
		this.CLvl.ReadOnly = true;
		this.CLvl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.CLvl.Width = 50;
		this.Family.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Family.DataPropertyName = "Family";
		this.Family.FillWeight = 101f;
		this.Family.HeaderText = "Family";
		this.Family.Name = "Family";
		this.Family.ReadOnly = true;
		this.Family.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Family.Visible = false;
		this.Total.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Total.DataPropertyName = "Total";
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle6.Format = "N0";
		dataGridViewCellStyle6.NullValue = null;
		this.Total.DefaultCellStyle = dataGridViewCellStyle6;
		this.Total.HeaderText = "Boss Dmg";
		this.Total.Name = "Total";
		this.Total.ReadOnly = true;
		this.Total.Resizable = System.Windows.Forms.DataGridViewTriState.True;
		this.Total.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Total.Width = 80;
		this.MaxHit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.MaxHit.DataPropertyName = "MaxHit";
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle7.Format = "N0";
		dataGridViewCellStyle7.NullValue = null;
		this.MaxHit.DefaultCellStyle = dataGridViewCellStyle7;
		this.MaxHit.FillWeight = 80f;
		this.MaxHit.HeaderText = "Max";
		this.MaxHit.Name = "MaxHit";
		this.MaxHit.ReadOnly = true;
		this.MaxHit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.MaxHit.Width = 60;
		this.MaxHitIcon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.MaxHitIcon.DataPropertyName = "MaxHitIcon";
		this.MaxHitIcon.HeaderText = "MI";
		this.MaxHitIcon.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
		this.MaxHitIcon.Name = "MaxHitIcon";
		this.MaxHitIcon.ReadOnly = true;
		this.MaxHitIcon.Width = 25;
		this.Pets.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Pets.DataPropertyName = "Pets";
		dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle8.Format = "N0";
		dataGridViewCellStyle8.NullValue = null;
		this.Pets.DefaultCellStyle = dataGridViewCellStyle8;
		this.Pets.HeaderText = "Pets";
		this.Pets.Name = "Pets";
		this.Pets.ReadOnly = true;
		this.Pets.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Pets.Visible = false;
		this.Pets.Width = 50;
		this.Special.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Special.DataPropertyName = "Special";
		dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle9.Format = "N0";
		dataGridViewCellStyle9.NullValue = null;
		this.Special.DefaultCellStyle = dataGridViewCellStyle9;
		this.Special.HeaderText = "MBossDmg";
		this.Special.Name = "Special";
		this.Special.ReadOnly = true;
		this.Special.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Special.Width = 80;
		this.MobDmg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.MobDmg.DataPropertyName = "MobDmg";
		dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle10.Format = "N0";
		this.MobDmg.DefaultCellStyle = dataGridViewCellStyle10;
		this.MobDmg.HeaderText = "MobDmg";
		this.MobDmg.Name = "MobDmg";
		this.MobDmg.ReadOnly = true;
		this.MobDmg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.MobDmg.Width = 80;
		this.OnyxDmg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.OnyxDmg.DataPropertyName = "OnyxDmg";
		dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle11.Format = "N0";
		this.OnyxDmg.DefaultCellStyle = dataGridViewCellStyle11;
		this.OnyxDmg.HeaderText = "Onyx";
		this.OnyxDmg.Name = "OnyxDmg";
		this.OnyxDmg.ReadOnly = true;
		this.OnyxDmg.Resizable = System.Windows.Forms.DataGridViewTriState.False;
		this.OnyxDmg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.OnyxDmg.Width = 60;
		this.All.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.All.DataPropertyName = "All";
		dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle12.Format = "N0";
		dataGridViewCellStyle12.NullValue = null;
		this.All.DefaultCellStyle = dataGridViewCellStyle12;
		this.All.HeaderText = "AllDmg";
		this.All.Name = "All";
		this.All.ReadOnly = true;
		this.All.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.All.Visible = false;
		this.All.Width = 80;
		this.Gold.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Gold.DataPropertyName = "Gold";
		dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle13.Format = "N0";
		dataGridViewCellStyle13.NullValue = null;
		this.Gold.DefaultCellStyle = dataGridViewCellStyle13;
		this.Gold.HeaderText = "Gold";
		this.Gold.Name = "Gold";
		this.Gold.ReadOnly = true;
		this.Gold.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Gold.Width = 65;
		this.Average.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Average.DataPropertyName = "Average";
		dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle14.Format = "N0";
		this.Average.DefaultCellStyle = dataGridViewCellStyle14;
		this.Average.HeaderText = "Average";
		this.Average.Name = "Average";
		this.Average.ReadOnly = true;
		this.Average.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Average.Visible = false;
		this.Average.Width = 60;
		this.Hit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Hit.DataPropertyName = "Hit";
		dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		this.Hit.DefaultCellStyle = dataGridViewCellStyle15;
		this.Hit.HeaderText = "Hit";
		this.Hit.Name = "Hit";
		this.Hit.ReadOnly = true;
		this.Hit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Hit.Width = 40;
		this.Miss.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Miss.DataPropertyName = "Miss";
		dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		this.Miss.DefaultCellStyle = dataGridViewCellStyle16;
		this.Miss.HeaderText = "Miss";
		this.Miss.Name = "Miss";
		this.Miss.ReadOnly = true;
		this.Miss.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Miss.Visible = false;
		this.Miss.Width = 40;
		this.Crit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Crit.DataPropertyName = "Crit";
		dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		this.Crit.DefaultCellStyle = dataGridViewCellStyle17;
		this.Crit.HeaderText = "Crit";
		this.Crit.Name = "Crit";
		this.Crit.ReadOnly = true;
		this.Crit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Crit.Visible = false;
		this.Crit.Width = 40;
		this.Bon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Bon.DataPropertyName = "Bon";
		dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		this.Bon.DefaultCellStyle = dataGridViewCellStyle18;
		this.Bon.HeaderText = "Bon";
		this.Bon.Name = "Bon";
		this.Bon.ReadOnly = true;
		this.Bon.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Bon.Visible = false;
		this.Bon.Width = 40;
		this.BonCrit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.BonCrit.DataPropertyName = "BonCrit";
		dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		this.BonCrit.DefaultCellStyle = dataGridViewCellStyle19;
		this.BonCrit.HeaderText = "BC";
		this.BonCrit.Name = "BonCrit";
		this.BonCrit.ReadOnly = true;
		this.BonCrit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
		this.BonCrit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.BonCrit.Visible = false;
		this.BonCrit.Width = 40;
		this.Dbf.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Dbf.DataPropertyName = "Dbf";
		dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		this.Dbf.DefaultCellStyle = dataGridViewCellStyle20;
		this.Dbf.HeaderText = "Dbf";
		this.Dbf.Name = "Dbf";
		this.Dbf.ReadOnly = true;
		this.Dbf.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Dbf.Width = 40;
		this.Dead.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Dead.DataPropertyName = "Dead";
		dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		this.Dead.DefaultCellStyle = dataGridViewCellStyle21;
		this.Dead.HeaderText = "Dead";
		this.Dead.Name = "Dead";
		this.Dead.ReadOnly = true;
		this.Dead.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.Dead.Width = 40;
		this.MBHit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.MBHit.DataPropertyName = "MBHit";
		dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		this.MBHit.DefaultCellStyle = dataGridViewCellStyle22;
		this.MBHit.HeaderText = "MBHit";
		this.MBHit.Name = "MBHit";
		this.MBHit.ReadOnly = true;
		this.MBHit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.MBHit.Visible = false;
		this.MBHit.Width = 40;
		this.AllHits.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.AllHits.DataPropertyName = "AllHits";
		dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle23.NullValue = null;
		this.AllHits.DefaultCellStyle = dataGridViewCellStyle23;
		this.AllHits.HeaderText = "All.H";
		this.AllHits.Name = "AllHits";
		this.AllHits.ReadOnly = true;
		this.AllHits.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.AllHits.Visible = false;
		this.AllHits.Width = 40;
		this.AllMiss.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.AllMiss.DataPropertyName = "AllMiss";
		dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
		dataGridViewCellStyle24.NullValue = null;
		this.AllMiss.DefaultCellStyle = dataGridViewCellStyle24;
		this.AllMiss.HeaderText = "All.M";
		this.AllMiss.Name = "AllMiss";
		this.AllMiss.ReadOnly = true;
		this.AllMiss.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.AllMiss.Visible = false;
		this.AllMiss.Width = 40;
		this.RaidsHistoryFlowLayoutPanel.AutoScroll = true;
		this.RaidsHistoryFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsHistoryFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
		this.RaidsHistoryFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
		this.RaidsHistoryFlowLayoutPanel.Name = "RaidsHistoryFlowLayoutPanel";
		this.RaidsHistoryFlowLayoutPanel.Size = new System.Drawing.Size(750, 675);
		this.RaidsHistoryFlowLayoutPanel.TabIndex = 1;
		this.RaidsHistoryFlowLayoutPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.RaidsHistoryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RaidsHistoryLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaidsHistoryLabel.Location = new System.Drawing.Point(0, 15);
		this.RaidsHistoryLabel.Name = "RaidsHistoryLabel";
		this.RaidsHistoryLabel.Size = new System.Drawing.Size(1200, 45);
		this.RaidsHistoryLabel.TabIndex = 2;
		this.RaidsHistoryLabel.Text = "Raids History";
		this.RaidsHistoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RaidsHistoryLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.RaidsHistoryBackButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsHistoryBackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.RaidsHistoryBackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RaidsHistoryBackButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RaidsHistoryBackButton.Location = new System.Drawing.Point(1060, 735);
		this.RaidsHistoryBackButton.Name = "RaidsHistoryBackButton";
		this.RaidsHistoryBackButton.Size = new System.Drawing.Size(100, 45);
		this.RaidsHistoryBackButton.TabIndex = 0;
		this.RaidsHistoryBackButton.Text = "Back";
		this.RaidsHistoryBackButton.UseVisualStyleBackColor = false;
		this.RaidsHistoryBackButton.Click += new System.EventHandler(RaidsHistoryBackButton_Click);
		this.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Location = new System.Drawing.Point(800, 75);
		this.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
		this.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Name = "RaidsHistorySelectedRaidPlayersFlowLayoutPanel";
		this.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Padding = new System.Windows.Forms.Padding(20, 20, 0, 0);
		this.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.Size = new System.Drawing.Size(370, 645);
		this.RaidsHistorySelectedRaidPlayersFlowLayoutPanel.TabIndex = 2;
		this.RankingTabPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RankingTabPanel.Controls.Add(this.RankingRaidsDoneLabel);
		this.RankingTabPanel.Controls.Add(this.RankingBestTimesLabel);
		this.RankingTabPanel.Controls.Add(this.RankingMaxHitsLabel);
		this.RankingTabPanel.Controls.Add(this.RankingAverageDMGLabel);
		this.RankingTabPanel.Controls.Add(this.RankingDoubleBufferedPanel);
		this.RankingTabPanel.Controls.Add(this.RankingPreviousPageButton);
		this.RankingTabPanel.Controls.Add(this.RankingNextPageButton);
		this.RankingTabPanel.Controls.Add(this.RankingSearchButton);
		this.RankingTabPanel.Controls.Add(this.RankingModeLabel);
		this.RankingTabPanel.Controls.Add(this.RankingModeButton);
		this.RankingTabPanel.Controls.Add(this.ClearRankingFiltersButton);
		this.RankingTabPanel.Controls.Add(this.RankingSPFilterFlowLayoutPanel);
		this.RankingTabPanel.Controls.Add(this.RankingRaidTypeFilterFlowLayoutPanel);
		this.RankingTabPanel.Controls.Add(this.RankingPageLabel);
		this.RankingTabPanel.Location = new System.Drawing.Point(25, 75);
		this.RankingTabPanel.Name = "RankingTabPanel";
		this.RankingTabPanel.Size = new System.Drawing.Size(1150, 700);
		this.RankingTabPanel.TabIndex = 15;
		this.RankingRaidsDoneLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 90);
		this.RankingRaidsDoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RankingRaidsDoneLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RankingRaidsDoneLabel.Location = new System.Drawing.Point(930, 0);
		this.RankingRaidsDoneLabel.Name = "RankingRaidsDoneLabel";
		this.RankingRaidsDoneLabel.Size = new System.Drawing.Size(100, 20);
		this.RankingRaidsDoneLabel.TabIndex = 21;
		this.RankingRaidsDoneLabel.Text = "Raids Done";
		this.RankingRaidsDoneLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RankingRaidsDoneLabel.Click += new System.EventHandler(RankingRaidsDoneLabel_Click);
		this.RankingBestTimesLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 90);
		this.RankingBestTimesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RankingBestTimesLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RankingBestTimesLabel.Location = new System.Drawing.Point(830, 0);
		this.RankingBestTimesLabel.Name = "RankingBestTimesLabel";
		this.RankingBestTimesLabel.Size = new System.Drawing.Size(100, 20);
		this.RankingBestTimesLabel.TabIndex = 20;
		this.RankingBestTimesLabel.Text = "Best Times";
		this.RankingBestTimesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RankingBestTimesLabel.Click += new System.EventHandler(RankingBestTimesLabel_Click);
		this.RankingMaxHitsLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 90);
		this.RankingMaxHitsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RankingMaxHitsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RankingMaxHitsLabel.Location = new System.Drawing.Point(730, 0);
		this.RankingMaxHitsLabel.Name = "RankingMaxHitsLabel";
		this.RankingMaxHitsLabel.Size = new System.Drawing.Size(100, 20);
		this.RankingMaxHitsLabel.TabIndex = 19;
		this.RankingMaxHitsLabel.Text = "Max Hits";
		this.RankingMaxHitsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RankingMaxHitsLabel.Click += new System.EventHandler(RankingMaxHitsLabel_Click);
		this.RankingAverageDMGLabel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RankingAverageDMGLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RankingAverageDMGLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RankingAverageDMGLabel.Location = new System.Drawing.Point(630, 0);
		this.RankingAverageDMGLabel.Name = "RankingAverageDMGLabel";
		this.RankingAverageDMGLabel.Size = new System.Drawing.Size(100, 20);
		this.RankingAverageDMGLabel.TabIndex = 19;
		this.RankingAverageDMGLabel.Text = "Average DMG";
		this.RankingAverageDMGLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RankingAverageDMGLabel.Click += new System.EventHandler(RankingAverageDMGLabel_Click);
		this.RankingDoubleBufferedPanel.Controls.Add(this.RankingFlowLayoutPanel);
		this.RankingDoubleBufferedPanel.Location = new System.Drawing.Point(630, 20);
		this.RankingDoubleBufferedPanel.Name = "RankingDoubleBufferedPanel";
		this.RankingDoubleBufferedPanel.Size = new System.Drawing.Size(500, 660);
		this.RankingDoubleBufferedPanel.TabIndex = 18;
		this.RankingFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.RankingFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
		this.RankingFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
		this.RankingFlowLayoutPanel.Name = "RankingFlowLayoutPanel";
		this.RankingFlowLayoutPanel.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
		this.RankingFlowLayoutPanel.Size = new System.Drawing.Size(500, 660);
		this.RankingFlowLayoutPanel.TabIndex = 0;
		this.RankingPreviousPageButton.Image = NosAssistant2.Properties.Resources.triangleLeft;
		this.RankingPreviousPageButton.Location = new System.Drawing.Point(630, 680);
		this.RankingPreviousPageButton.Name = "RankingPreviousPageButton";
		this.RankingPreviousPageButton.Size = new System.Drawing.Size(20, 20);
		this.RankingPreviousPageButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.RankingPreviousPageButton.TabIndex = 16;
		this.RankingPreviousPageButton.TabStop = false;
		this.RankingPreviousPageButton.Visible = false;
		this.RankingPreviousPageButton.Click += new System.EventHandler(RankingPreviousPageButton_Click);
		this.RankingPreviousPageButton.MouseEnter += new System.EventHandler(RankingPreviousPageButton_MouseEnter);
		this.RankingPreviousPageButton.MouseLeave += new System.EventHandler(RankingPreviousPageButton_MouseLeave);
		this.RankingNextPageButton.Image = NosAssistant2.Properties.Resources.triangleRight;
		this.RankingNextPageButton.Location = new System.Drawing.Point(1110, 680);
		this.RankingNextPageButton.Name = "RankingNextPageButton";
		this.RankingNextPageButton.Size = new System.Drawing.Size(20, 20);
		this.RankingNextPageButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.RankingNextPageButton.TabIndex = 11;
		this.RankingNextPageButton.TabStop = false;
		this.RankingNextPageButton.Visible = false;
		this.RankingNextPageButton.Click += new System.EventHandler(RankingNextPageButton_Click);
		this.RankingNextPageButton.MouseEnter += new System.EventHandler(RankingNextPageButton_MouseEnter);
		this.RankingNextPageButton.MouseLeave += new System.EventHandler(RankingNextPageButton_MouseLeave);
		this.RankingSearchButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RankingSearchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.RankingSearchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RankingSearchButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RankingSearchButton.Location = new System.Drawing.Point(510, 480);
		this.RankingSearchButton.Name = "RankingSearchButton";
		this.RankingSearchButton.Size = new System.Drawing.Size(100, 45);
		this.RankingSearchButton.TabIndex = 6;
		this.RankingSearchButton.Text = "Search";
		this.RankingSearchButton.UseVisualStyleBackColor = false;
		this.RankingSearchButton.Click += new System.EventHandler(RankingSearchButton_Click);
		this.RankingModeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RankingModeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RankingModeLabel.Location = new System.Drawing.Point(0, 660);
		this.RankingModeLabel.Name = "RankingModeLabel";
		this.RankingModeLabel.Size = new System.Drawing.Size(630, 40);
		this.RankingModeLabel.TabIndex = 5;
		this.RankingModeLabel.Text = "Global - No Raid Selected";
		this.RankingModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RankingModeButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.RankingModeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.RankingModeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RankingModeButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RankingModeButton.Location = new System.Drawing.Point(20, 535);
		this.RankingModeButton.Name = "RankingModeButton";
		this.RankingModeButton.Size = new System.Drawing.Size(100, 45);
		this.RankingModeButton.TabIndex = 4;
		this.RankingModeButton.Text = "Mode";
		this.RankingModeButton.UseVisualStyleBackColor = false;
		this.RankingModeButton.Click += new System.EventHandler(RankingModeButton_Click);
		this.ClearRankingFiltersButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.ClearRankingFiltersButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ClearRankingFiltersButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.ClearRankingFiltersButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.ClearRankingFiltersButton.Location = new System.Drawing.Point(20, 480);
		this.ClearRankingFiltersButton.Name = "ClearRankingFiltersButton";
		this.ClearRankingFiltersButton.Size = new System.Drawing.Size(100, 45);
		this.ClearRankingFiltersButton.TabIndex = 3;
		this.ClearRankingFiltersButton.Text = "Clear Fitlers";
		this.ClearRankingFiltersButton.UseVisualStyleBackColor = false;
		this.ClearRankingFiltersButton.Click += new System.EventHandler(ClearRankingFiltersButton_Click);
		this.RankingSPFilterFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.RankingSPFilterFlowLayoutPanel.Location = new System.Drawing.Point(20, 220);
		this.RankingSPFilterFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
		this.RankingSPFilterFlowLayoutPanel.Name = "RankingSPFilterFlowLayoutPanel";
		this.RankingSPFilterFlowLayoutPanel.Size = new System.Drawing.Size(590, 210);
		this.RankingSPFilterFlowLayoutPanel.TabIndex = 2;
		this.RankingRaidTypeFilterFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.RankingRaidTypeFilterFlowLayoutPanel.Location = new System.Drawing.Point(20, 20);
		this.RankingRaidTypeFilterFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
		this.RankingRaidTypeFilterFlowLayoutPanel.Name = "RankingRaidTypeFilterFlowLayoutPanel";
		this.RankingRaidTypeFilterFlowLayoutPanel.Size = new System.Drawing.Size(590, 180);
		this.RankingRaidTypeFilterFlowLayoutPanel.TabIndex = 1;
		this.RankingPageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.RankingPageLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.RankingPageLabel.Location = new System.Drawing.Point(650, 680);
		this.RankingPageLabel.Name = "RankingPageLabel";
		this.RankingPageLabel.Size = new System.Drawing.Size(460, 20);
		this.RankingPageLabel.TabIndex = 17;
		this.RankingPageLabel.Text = "1/1";
		this.RankingPageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.RankingPageLabel.Visible = false;
		this.FamRecordsPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.FamRecordsPanel.Location = new System.Drawing.Point(25, 75);
		this.FamRecordsPanel.Margin = new System.Windows.Forms.Padding(0);
		this.FamRecordsPanel.Name = "FamRecordsPanel";
		this.FamRecordsPanel.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
		this.FamRecordsPanel.Size = new System.Drawing.Size(1150, 700);
		this.FamRecordsPanel.TabIndex = 3;
		this.FamRecordsPanel.Visible = false;
		this.MainQuestsPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.MainQuestsPanel.Controls.Add(this.QuestPathLabel);
		this.MainQuestsPanel.Controls.Add(this.QuestsPanel);
		this.MainQuestsPanel.Controls.Add(this.QuestsLabel);
		this.MainQuestsPanel.Location = new System.Drawing.Point(250, 0);
		this.MainQuestsPanel.Name = "MainQuestsPanel";
		this.MainQuestsPanel.Size = new System.Drawing.Size(950, 800);
		this.MainQuestsPanel.TabIndex = 19;
		this.MainQuestsPanel.Visible = false;
		this.MainQuestsPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.QuestPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.QuestPathLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.QuestPathLabel.Location = new System.Drawing.Point(25, 712);
		this.QuestPathLabel.Name = "QuestPathLabel";
		this.QuestPathLabel.Size = new System.Drawing.Size(900, 75);
		this.QuestPathLabel.TabIndex = 5;
		this.QuestPathLabel.Text = "Path: -";
		this.QuestPathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.QuestsPanel.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.QuestsPanel.Controls.Add(this.StopNavigatingButton);
		this.QuestsPanel.Controls.Add(this.ShowTimeSpaceMapButton);
		this.QuestsPanel.Controls.Add(this.QuestSearchResultsFlowLayoutPanel);
		this.QuestsPanel.Controls.Add(this.QuestNavigateButton);
		this.QuestsPanel.Controls.Add(this.QuestSearchTypesPanel);
		this.QuestsPanel.Controls.Add(this.QuestSearchTextBox);
		this.QuestsPanel.Controls.Add(this.QuestObjectiveIconsFlowLayoutPanel);
		this.QuestsPanel.Controls.Add(this.QuestObjectiveLabel);
		this.QuestsPanel.Controls.Add(this.QuestsTabMap);
		this.QuestsPanel.Controls.Add(this.QuestsFlowLayoutPanel);
		this.QuestsPanel.Controls.Add(this.SwitchMapModeQuestButton);
		this.QuestsPanel.Location = new System.Drawing.Point(25, 100);
		this.QuestsPanel.Name = "QuestsPanel";
		this.QuestsPanel.Size = new System.Drawing.Size(900, 600);
		this.QuestsPanel.TabIndex = 4;
		this.QuestsPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SwitchMapModeQuestButton.BackColor = System.Drawing.Color.FromArgb(58, 12, 163);
		this.SwitchMapModeQuestButton.Image = NosAssistant2.Properties.Resources.switch_icon;
		this.SwitchMapModeQuestButton.Location = new System.Drawing.Point(545, 25);
		this.SwitchMapModeQuestButton.Name = "SwitchMapModeQuestButton";
		this.SwitchMapModeQuestButton.Size = new System.Drawing.Size(30, 30);
		this.SwitchMapModeQuestButton.TabIndex = 11;
		this.SwitchMapModeQuestButton.TabStop = false;
		this.SwitchMapModeQuestButton.Visible = false;
		this.SwitchMapModeQuestButton.Click += new System.EventHandler(SwitchMapModeQuestButton_Click);
		this.SwitchMapModeQuestButton.MouseEnter += new System.EventHandler(SwitchMapModeQuestButton_MouseEnter);
		this.SwitchMapModeQuestButton.MouseLeave += new System.EventHandler(SwitchMapModeQuestButton_MouseLeave);
		this.StopNavigatingButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.StopNavigatingButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.StopNavigatingButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.StopNavigatingButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.StopNavigatingButton.Location = new System.Drawing.Point(742, 555);
		this.StopNavigatingButton.Name = "StopNavigatingButton";
		this.StopNavigatingButton.Size = new System.Drawing.Size(132, 30);
		this.StopNavigatingButton.TabIndex = 10;
		this.StopNavigatingButton.Text = "Stop Nav.";
		this.StopNavigatingButton.UseVisualStyleBackColor = false;
		this.StopNavigatingButton.Click += new System.EventHandler(StopNavigatingButton_Click);
		this.ShowTimeSpaceMapButton.BackColor = System.Drawing.Color.FromArgb(58, 12, 163);
		this.ShowTimeSpaceMapButton.Image = NosAssistant2.Properties.Resources.info;
		this.ShowTimeSpaceMapButton.Location = new System.Drawing.Point(545, 25);
		this.ShowTimeSpaceMapButton.Name = "ShowTimeSpaceMapButton";
		this.ShowTimeSpaceMapButton.Size = new System.Drawing.Size(30, 30);
		this.ShowTimeSpaceMapButton.TabIndex = 9;
		this.ShowTimeSpaceMapButton.TabStop = false;
		this.ShowTimeSpaceMapButton.Visible = false;
		this.ShowTimeSpaceMapButton.Click += new System.EventHandler(ShowTimeSpaceMapButton_Click);
		this.ShowTimeSpaceMapButton.MouseEnter += new System.EventHandler(ShowTimeSpaceMapButton_MouseEnter);
		this.ShowTimeSpaceMapButton.MouseLeave += new System.EventHandler(ShowTimeSpaceMapButton_MouseLeave);
		this.QuestSearchResultsFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.QuestSearchResultsFlowLayoutPanel.Location = new System.Drawing.Point(600, 90);
		this.QuestSearchResultsFlowLayoutPanel.Name = "QuestSearchResultsFlowLayoutPanel";
		this.QuestSearchResultsFlowLayoutPanel.Size = new System.Drawing.Size(275, 450);
		this.QuestSearchResultsFlowLayoutPanel.TabIndex = 8;
		this.QuestNavigateButton.BackColor = System.Drawing.Color.FromArgb(20, 9, 67);
		this.QuestNavigateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.QuestNavigateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.QuestNavigateButton.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.QuestNavigateButton.Location = new System.Drawing.Point(600, 555);
		this.QuestNavigateButton.Name = "QuestNavigateButton";
		this.QuestNavigateButton.Size = new System.Drawing.Size(132, 30);
		this.QuestNavigateButton.TabIndex = 7;
		this.QuestNavigateButton.Text = "Navigate";
		this.QuestNavigateButton.UseVisualStyleBackColor = false;
		this.QuestNavigateButton.Click += new System.EventHandler(QuestNavigateButton_Click);
		this.QuestSearchTypesPanel.Controls.Add(this.QuestTSTypeLabel);
		this.QuestSearchTypesPanel.Controls.Add(this.QuestMapTypeLabel);
		this.QuestSearchTypesPanel.Controls.Add(this.QuestMobTypeLabel);
		this.QuestSearchTypesPanel.Controls.Add(this.QuestNPCTypeLabel);
		this.QuestSearchTypesPanel.Location = new System.Drawing.Point(600, 58);
		this.QuestSearchTypesPanel.Name = "QuestSearchTypesPanel";
		this.QuestSearchTypesPanel.Size = new System.Drawing.Size(275, 26);
		this.QuestSearchTypesPanel.TabIndex = 6;
		this.QuestTSTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.QuestTSTypeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.QuestTSTypeLabel.Location = new System.Drawing.Point(207, 1);
		this.QuestTSTypeLabel.Name = "QuestTSTypeLabel";
		this.QuestTSTypeLabel.Size = new System.Drawing.Size(68, 26);
		this.QuestTSTypeLabel.TabIndex = 8;
		this.QuestTSTypeLabel.Text = "TS";
		this.QuestTSTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.QuestTSTypeLabel.Click += new System.EventHandler(SetQuestSearchType);
		this.QuestTSTypeLabel.MouseEnter += new System.EventHandler(QuestSearchTypeLabel_MouseEnter);
		this.QuestTSTypeLabel.MouseLeave += new System.EventHandler(QuestSearchTypeLabel_MouseLeave);
		this.QuestMapTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.QuestMapTypeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.QuestMapTypeLabel.Location = new System.Drawing.Point(138, 0);
		this.QuestMapTypeLabel.Name = "QuestMapTypeLabel";
		this.QuestMapTypeLabel.Size = new System.Drawing.Size(69, 26);
		this.QuestMapTypeLabel.TabIndex = 7;
		this.QuestMapTypeLabel.Text = "Map";
		this.QuestMapTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.QuestMapTypeLabel.Click += new System.EventHandler(SetQuestSearchType);
		this.QuestMapTypeLabel.MouseEnter += new System.EventHandler(QuestSearchTypeLabel_MouseEnter);
		this.QuestMapTypeLabel.MouseLeave += new System.EventHandler(QuestSearchTypeLabel_MouseLeave);
		this.QuestMobTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.QuestMobTypeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.QuestMobTypeLabel.Location = new System.Drawing.Point(69, 0);
		this.QuestMobTypeLabel.Name = "QuestMobTypeLabel";
		this.QuestMobTypeLabel.Size = new System.Drawing.Size(69, 26);
		this.QuestMobTypeLabel.TabIndex = 6;
		this.QuestMobTypeLabel.Text = "Mob";
		this.QuestMobTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.QuestMobTypeLabel.Click += new System.EventHandler(SetQuestSearchType);
		this.QuestMobTypeLabel.MouseEnter += new System.EventHandler(QuestSearchTypeLabel_MouseEnter);
		this.QuestMobTypeLabel.MouseLeave += new System.EventHandler(QuestSearchTypeLabel_MouseLeave);
		this.QuestNPCTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.QuestNPCTypeLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.QuestNPCTypeLabel.Location = new System.Drawing.Point(0, 0);
		this.QuestNPCTypeLabel.Name = "QuestNPCTypeLabel";
		this.QuestNPCTypeLabel.Size = new System.Drawing.Size(69, 26);
		this.QuestNPCTypeLabel.TabIndex = 5;
		this.QuestNPCTypeLabel.Text = "NPC";
		this.QuestNPCTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.QuestNPCTypeLabel.Click += new System.EventHandler(SetQuestSearchType);
		this.QuestNPCTypeLabel.MouseEnter += new System.EventHandler(QuestSearchTypeLabel_MouseEnter);
		this.QuestNPCTypeLabel.MouseLeave += new System.EventHandler(QuestSearchTypeLabel_MouseLeave);
		this.QuestSearchTextBox.BackColor = System.Drawing.Color.FromArgb(63, 55, 201);
		this.QuestSearchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.QuestSearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold);
		this.QuestSearchTextBox.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.QuestSearchTextBox.Location = new System.Drawing.Point(600, 25);
		this.QuestSearchTextBox.Name = "QuestSearchTextBox";
		this.QuestSearchTextBox.Size = new System.Drawing.Size(275, 26);
		this.QuestSearchTextBox.TabIndex = 4;
		this.QuestSearchTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.QuestSearchTextBox.TextChanged += new System.EventHandler(QuestSearchTextBox_TextChanged);
		this.QuestObjectiveIconsFlowLayoutPanel.Location = new System.Drawing.Point(25, 475);
		this.QuestObjectiveIconsFlowLayoutPanel.Name = "QuestObjectiveIconsFlowLayoutPanel";
		this.QuestObjectiveIconsFlowLayoutPanel.Size = new System.Drawing.Size(400, 100);
		this.QuestObjectiveIconsFlowLayoutPanel.TabIndex = 3;
		this.QuestObjectiveLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.QuestObjectiveLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.QuestObjectiveLabel.Location = new System.Drawing.Point(25, 400);
		this.QuestObjectiveLabel.Name = "QuestObjectiveLabel";
		this.QuestObjectiveLabel.Size = new System.Drawing.Size(400, 50);
		this.QuestObjectiveLabel.TabIndex = 2;
		this.QuestObjectiveLabel.Text = "Talk With:";
		this.QuestObjectiveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.QuestObjectiveLabel.Visible = false;
		this.QuestsTabMap.Location = new System.Drawing.Point(25, 25);
		this.QuestsTabMap.Name = "QuestsTabMap";
		this.QuestsTabMap.Size = new System.Drawing.Size(550, 365);
		this.QuestsTabMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.QuestsTabMap.TabIndex = 1;
		this.QuestsTabMap.TabStop = false;
		this.QuestsTabMap.MouseClick += new System.Windows.Forms.MouseEventHandler(map_picture_MouseClick);
		this.QuestsTabMap.MouseMove += new System.Windows.Forms.MouseEventHandler(QuestsTabMap_MouseMove);
		this.QuestsFlowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		this.QuestsFlowLayoutPanel.Location = new System.Drawing.Point(425, 405);
		this.QuestsFlowLayoutPanel.Name = "QuestsFlowLayoutPanel";
		this.QuestsFlowLayoutPanel.Size = new System.Drawing.Size(150, 180);
		this.QuestsFlowLayoutPanel.TabIndex = 0;
		this.QuestsFlowLayoutPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.QuestsLabel.AutoSize = true;
		this.QuestsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.QuestsLabel.ForeColor = System.Drawing.Color.FromArgb(233, 138, 232);
		this.QuestsLabel.Location = new System.Drawing.Point(25, 9);
		this.QuestsLabel.Name = "QuestsLabel";
		this.QuestsLabel.Size = new System.Drawing.Size(134, 39);
		this.QuestsLabel.TabIndex = 1;
		this.QuestsLabel.Text = "Quests";
		this.QuestsLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		this.BackColor = System.Drawing.Color.FromArgb(32, 15, 85);
		base.ClientSize = new System.Drawing.Size(1200, 800);
		base.Controls.Add(this.popUpPanel);
		base.Controls.Add(this.CloseButton);
		base.Controls.Add(this.MinimizeButton);
		base.Controls.Add(this.HideMenuButton);
		base.Controls.Add(this.SideMenu);
		base.Controls.Add(this.MainQuestsPanel);
		base.Controls.Add(this.MainRaidsPanel);
		base.Controls.Add(this.MainMinilandPanel);
		base.Controls.Add(this.MainPacketLoggerPanel);
		base.Controls.Add(this.RaidsHistoryPanel);
		base.Controls.Add(this.MainSettingsPanel);
		base.Controls.Add(this.LoadingScreenPanel);
		base.Controls.Add(this.MainNoAccessPanel);
		base.Controls.Add(this.MainControlPanel);
		base.Controls.Add(this.MainMapPanel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "GUI";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "NosAssistant2";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(GUI_FormClosing);
		base.Load += new System.EventHandler(GUI_Load);
		base.MouseDown += new System.Windows.Forms.MouseEventHandler(GUI_MouseDown);
		this.SideMenu.ResumeLayout(false);
		this.LogoPanel.ResumeLayout(false);
		this.CountDownPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.LogoPicture).EndInit();
		this.MapBottomPanel.ResumeLayout(false);
		this.MapBottomPanel.PerformLayout();
		this.ControlPanelPanel.ResumeLayout(false);
		this.ControlPanelPanel.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.map_picture).EndInit();
		this.MapPanel.ResumeLayout(false);
		this.MapPanel.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.OpenMapInNewWindowMapTabButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SwitchMapModeButton).EndInit();
		this.RaidsPanel.ResumeLayout(false);
		this.RaidsPanel.PerformLayout();
		this.RaidersHPStatusPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.SwitchRaidersTabPanel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AutofullThresholdTrackbar).EndInit();
		this.MinilandPanel.ResumeLayout(false);
		this.MinilandPanel.PerformLayout();
		this.PetTrainerFlowLayoutPanel.ResumeLayout(false);
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.PetTrainerIcon1).EndInit();
		this.panel3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.PetTrainerIcon2).EndInit();
		this.panel4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.PetTrainerIcon3).EndInit();
		this.panel5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.TrainedPetIcon1).EndInit();
		this.panel6.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.TrainedPetIcon2).EndInit();
		this.panel7.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.TrainedPetIcon3).EndInit();
		this.InviteListPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.InviteListDataGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inviteListBindingSource).EndInit();
		this.SettingsPanel.ResumeLayout(false);
		this.SettingsPanel.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.ResetArcaneWisdomButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editArcaneWisdomControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseDebuffsButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editUseDebuffsControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseBuffset3Button).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editUseBuffset3ControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseBuffset2Button).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editUseBuffset2ControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseBuffset1Button).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editUseBuffset1ControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseSelfBuffsButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetWearSPButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetMassHealButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetExitRaidButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetJoinListButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetInviteButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ResetUseBuffsButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editExitRaidControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editJoinListControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editUseSelfBuffsControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editWearSPControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editMassHealControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editInviteControlPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.editUseBufssControlPictureBox).EndInit();
		this.ControlPanelBottomPanel.ResumeLayout(false);
		this.ControlPanelBottomPanel.PerformLayout();
		this.PacketLoggerPanel.ResumeLayout(false);
		this.PacketLoggerBottomPanel.ResumeLayout(false);
		this.PacketLoggerBottomPanel.PerformLayout();
		this.MainControlPanel.ResumeLayout(false);
		this.MainControlPanel.PerformLayout();
		this.MainMapPanel.ResumeLayout(false);
		this.MainMapPanel.PerformLayout();
		this.MainRaidsPanel.ResumeLayout(false);
		this.MainRaidsPanel.PerformLayout();
		this.MainMinilandPanel.ResumeLayout(false);
		this.MainMinilandPanel.PerformLayout();
		this.MainPacketLoggerPanel.ResumeLayout(false);
		this.MainPacketLoggerPanel.PerformLayout();
		this.MainSettingsPanel.ResumeLayout(false);
		this.MainSettingsPanel.PerformLayout();
		this.MainNoAccessPanel.ResumeLayout(false);
		this.MainNoAccessPanel.PerformLayout();
		this.NoAccessPanel.ResumeLayout(false);
		this.NoAccessPanel.PerformLayout();
		this.LoadingScreenPanel.ResumeLayout(false);
		this.panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.HideMenuButton).EndInit();
		this.RaidsHistoryPanel.ResumeLayout(false);
		this.RaidsHistoryPanel.PerformLayout();
		this.AnalyticsPlayersTab.ResumeLayout(false);
		this.AnalyticsPlayersTab.PerformLayout();
		this.PlayerRaidsStatisticsPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.PlayerRaidsStatisticsBossIcon).EndInit();
		this.SPDetailsBorderPanel.ResumeLayout(false);
		this.SPDetailsPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.CloseSPDetailsButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsShadowImage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsLightImage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsWaterImage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsFireImage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsEnergyImage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsPropertyImage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsDefenceImage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsAttackImage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SPDetailsAvatar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Tattoo2Icon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Tattoo1Icon).EndInit();
		this.ShellInfoMainPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.SwitchToShellButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SwitchToRuneButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SwitchShellTypeButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Wings).EndInit();
		((System.ComponentModel.ISupportInitialize)this.WeaponSkin).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CostumeHat).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Costume).EndInit();
		((System.ComponentModel.ISupportInitialize)this.FlyingPet).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Mask).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Hat).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Armor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SecondaryWeapon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MainWeapon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Reputation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SearchedPlayerAvatar).EndInit();
		this.MainFairyDetailsPanel.ResumeLayout(false);
		this.FairyDetailsPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.FairyDetailsIcon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CloseFairyDetailsButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AnalyticsBackArrow).EndInit();
		((System.ComponentModel.ISupportInitialize)this.FamRecordsNextPageButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.FamRecordsPreviousPageButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BackArrowRaidsHistory).EndInit();
		((System.ComponentModel.ISupportInitialize)this.RaidsHistoryNextPageButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.RaidsHistoryPreviousPageButton).EndInit();
		this.RaidsHistoryDoubleBufferedPanel.ResumeLayout(false);
		this.RaidsHistoryDetailsPanel.ResumeLayout(false);
		this.RaidsHistoryListViewBorder.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.RaidsHistoryDetailsGridView).EndInit();
		this.RankingTabPanel.ResumeLayout(false);
		this.RankingDoubleBufferedPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.RankingPreviousPageButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.RankingNextPageButton).EndInit();
		this.MainQuestsPanel.ResumeLayout(false);
		this.MainQuestsPanel.PerformLayout();
		this.QuestsPanel.ResumeLayout(false);
		this.QuestsPanel.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.SwitchMapModeQuestButton).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ShowTimeSpaceMapButton).EndInit();
		this.QuestSearchTypesPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.QuestsTabMap).EndInit();
		base.ResumeLayout(false);
	}
}
