using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NosAssistant2.GameObjects;
using NosAssistant2.GUIElements;

namespace NosAssistant2.Helpers;

public static class ICManager
{
	public static int ICStarted { get; set; } = 0;


	public static int WaveIC { get; set; } = -1;


	public static void InstantCombatStarted()
	{
		RaidManager.raidStarted = true;
		RaidManager.bossDead = false;
		RaidManager.lastFinishedRaid = -2;
		RaidManager.ResetCounter();
		RaidManager.stopwatch = Stopwatch.StartNew();
		WaveIC = 0;
		if (GUI.RaidModeForm != null)
		{
			GUI.RaidModeForm.DisableRaidModeButton();
			GUI.RaidModeForm.showICRelatedColumns();
			Task.Run(async delegate
			{
				await Task.Delay(7000);
				GUI.RaidModeForm.sortICOrder();
			});
		}
	}

	public static void IncreaseRound()
	{
		WaveIC++;
		foreach (RaidDamage item in RaidManager.singleRaid)
		{
			item.Clear();
			RaidForm.updateCharacterInList(item);
		}
		RaidForm.clearCounterColors();
		RaidManager.mobsKilled = 0;
		GUI.RaidModeForm?.UpdateMobKilled();
	}

	public static void InstantCombatFinished()
	{
		ICStarted = 0;
		WaveIC = -1;
		RaidManager.raidStarted = false;
		RaidManager.bossDead = false;
		GUI.RaidModeForm.EnableRaidModeButton();
		RaidManager.stopwatch.Stop();
		RaidManager.raidsFinished++;
		GUI.RaidModeForm?.setRaidsFinishedCount(RaidManager.raidsFinished);
		TimeSpan elapsed = RaidManager.stopwatch.Elapsed;
		RaidManager.raidsDuration.Add(elapsed);
		GUI.RaidModeForm?.setCurrentTime(elapsed);
		GUI.RaidModeForm?.setBestTime(RaidManager.raidsDuration.Min());
		GUI.RaidModeForm?.setAverageTime((RaidManager.raidsDuration.Count != 0) ? TimeSpan.FromTicks((long)RaidManager.raidsDuration.Average((TimeSpan d) => d.Ticks)) : TimeSpan.Zero);
		GUI.RaidModeForm.updateColumnsVisibility();
	}

	public static int GetRequiredDMG(int player_lvl)
	{
		if (WaveIC < 0)
		{
			return -1;
		}
		if (ICStarted == 1)
		{
			return player_lvl * WaveIC switch
			{
				0 => (player_lvl < 40) ? 25 : ((player_lvl < 60) ? 50 : ((player_lvl < 80) ? 75 : 100)), 
				1 => (player_lvl < 40) ? 75 : ((player_lvl < 60) ? 150 : ((player_lvl < 80) ? 225 : 300)), 
				2 => (player_lvl < 40) ? 150 : ((player_lvl < 60) ? 300 : ((player_lvl < 80) ? 500 : 600)), 
				3 => (player_lvl < 40) ? 250 : ((player_lvl < 60) ? 500 : ((player_lvl < 80) ? 750 : 1000)), 
				_ => 500000, 
			} * 3;
		}
		if (ICStarted == 2)
		{
			return WaveIC switch
			{
				0 => 200000, 
				1 => 350000, 
				2 => 500000, 
				3 => 600000, 
				_ => 0, 
			};
		}
		return -1;
	}
}
