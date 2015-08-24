﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IEMod.Helpers;
using IEMod.Mods.ObjectBrowser;
using IEMod.Mods.Options;
using IEMod.Mods.UICustomization;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IEMod.Mods.ConsoleMod {
	[ModifiesType()]
	public class mod_CommandLine : CommandLine
	{
		[ModifiesType()]
		public class Mod5_GameState : GameState {
			[ModifiesAccessibility]
			public new bool s_playerCharacter;
		}
		[ModifiesType()]
		public class Mod5_Loot : Loot {

			[ModifiesAccessibility]
			public new bool GetInventoryComponent() {
				return false;
			}
		}
		[ModifiesType()]
		public class Mod5_Health : Health {
			public new bool m_isAnimalCompanion {
				[ModifiesAccessibility()] get;
				[ModifiesAccessibility()] set; 
			}

			public new bool CanDie
			{
				[ModifiesAccessibility()]
				get;
				set;
			}
		}
		[ModifiesType]
		public class Mod5_QuestManager : QuestManager {

			[ModifiesAccessibility("get_ActiveQuests")]
			public void ActiveGetter() {
				
			}
		}
		[ModifiesType("StringTableManager")]
		public static class Mod5_StringTableManager {
			[ModifiesAccessibility]
			public static bool StringTables;
			[ModifiesAccessibility]
			public static bool StringTableLookup;
		}

		[ModifiesType]
		public class Mod5_UIOptionsManager : UIOptionsManager {
			[ModifiesAccessibility]
			public new bool m_GameMode;
		}
		[ModifiesType]
		public class Mod5_CharacterStats : global::CharacterStats {
			[ModifiesAccessibility]
			public new bool m_stronghold;
		}

		[NewMember]
		public  static GameObject modelViewerBackground;
		[NewMember]
		public  static UITexture GameBrowserBackground;

		[NewMember]
		public  static void ExtractMemorials()
		{
			string xmlPath = PathHelper.Combine(Application.dataPath, "Managed/iemod", "MemorialEntries.xml");
			string xml = Resources.Load("Data/UI/BackerMemorials").ToString();

			File.WriteAllText(xmlPath, xml);
			Console.AddMessage("Extraction: done.");
		}

		[NewMember]
		public  static void CC()
		{
			GameState.s_playerCharacter.GetComponent<Mover>().UseWalkSpeed();
		}

		[NewMember]
		public  static void SetDefaultZoom(float value)
		{
			IEModOptions.DefaultZoom = value;
			global::Console.AddMessage("Default zoom set to: " + value + ". Reminder: game's vanilla value is 1.");
		}

		[NewMember]
		public  static void DisableBackerDialogues(bool state)
		{
			if (state) {
				IEModOptions.DisableBackerDialogs = true;
				global::Console.AddMessage("If you're using the \"Rename backers\" mod, backer dialogues will now be DISABLED as soon as you transition to another area or reload a save.");
			}
			else {
				IEModOptions.DisableBackerDialogs = false;
				global::Console.AddMessage("Backer dialogues will now be ENABLED as soon as you transition to another area or reload a save.");
			}
		}

		[NewMember]
		public  static void TS()
		{
			Vector3 test = new Vector3() { x = 20f };
			Vector3 second = new Vector3() { x = 30f };
			//test.x = 20;
			Debug.DrawLine(test, second);
		}

		[NewMember]
		public  static void DD()
		{
			QualitySettings.IncreaseLevel();
		}

		[NewMember]
		public  static void CheckAchievements()
		{
			if (AchievementTracker.Instance.DisableAchievements == true)
			{
				global::Console.AddMessage("Your achievements were previously disabled for this playthrough.", Color.red);
				global::Console.AddMessage("To reactivate them, type: ReenableAchievements");
			}
			else
				global::Console.AddMessage("Your achievements are doing fine.", Color.green);
		}

		[NewMember]
		public  static void AssignClericalGod(string charname, string godname)
		{
			charname = charname.Replace("_", " ");

			GameObject npc = null;

			foreach (var partymember in PartyMemberAI.PartyMembers)
			{
				if (partymember != null && RemoveDiacritics(partymember.gameObject.GetComponent<CharacterStats>().Name()).Contains(charname))
					npc = partymember.gameObject;
			}

			if (npc != null)
			{
				bool goOn = false;
				try
				{
					if (Enum.Parse(typeof(global::Religion.Deity), godname) != null)
						goOn = true;
				}
				catch
				{
					global::Console.AddMessage(godname + " - not found as a Deity.");
				}
				if (goOn)
				{
					object newclassobj = Enum.Parse(typeof(global::Religion.Deity), godname);
					int newGodId = Convert.ToInt32(newclassobj);

					npc.GetComponent<CharacterStats>().Deity = (global::Religion.Deity)newGodId;

					global::Console.AddMessage("Deity assigned.", Color.green);
				}
			}
		}

		[NewMember]
		public  static void AssignPaladinOrder(string charname, string ordername)
		{
			charname = charname.Replace("_", " ");

			GameObject npc = null;

			foreach (var partymember in PartyMemberAI.PartyMembers)
			{
				if (partymember != null && RemoveDiacritics(partymember.gameObject.GetComponent<CharacterStats>().Name()).Contains(charname))
					npc = partymember.gameObject;
			}

			if (npc != null)
			{
				bool goOn = false;
				try
				{
					if (Enum.Parse(typeof(global::Religion.PaladinOrder), ordername) != null)
						goOn = true;
				}
				catch
				{
					global::Console.AddMessage(ordername + " - not found as a Palandin Order.");
				}
				if (goOn)
				{
					object newclassobj = Enum.Parse(typeof(global::Religion.PaladinOrder), ordername);
					int newOrderId = Convert.ToInt32(newclassobj);

					npc.GetComponent<CharacterStats>().PaladinOrder = (global::Religion.PaladinOrder)newOrderId;

					global::Console.AddMessage("Paladin order assigned.", Color.green);
				}
			}
		}

		//code by Michael Kaplan
		[NewMember]
		static string RemoveDiacritics(string text)
		{
			var normalizedString = text.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

		[NewMember]
		public  static void IERemove(string charname, string abilname)
		{

			charname = charname.Replace("_", " ");

			GameObject npc = null;

			foreach (var partymember in PartyMemberAI.PartyMembers)
			{
				if (partymember != null && RemoveDiacritics(partymember.gameObject.GetComponent<CharacterStats>().Name()).Contains(charname))
					npc = partymember.gameObject;
			}

			if (npc != null)
			{
				bool removedSomething = false;

				CharacterStats stats = npc.GetComponent<CharacterStats>();
				for (int i = stats.ActiveTalents.Count - 1; i > -1; i--)
				{
					if (stats.ActiveTalents[i].gameObject.name.Contains(abilname))
					{
						global::Console.AddMessage("Removed active talent: " + stats.ActiveTalents[i].gameObject.name);
						AbilityProgressionTable.RemoveAbilityFromCharacter(stats.ActiveTalents[i].gameObject, stats);
						removedSomething = true;
						break;
					}
				}

				for (int i = stats.Talents.Count - 1; i > -1; i--)
				{
					if (stats.Talents[i].gameObject.name.Contains(abilname))
					{
						global::Console.AddMessage("Removed talent: " + stats.Talents[i].gameObject.name);
						AbilityProgressionTable.RemoveAbilityFromCharacter(stats.Talents[i].gameObject, stats);
						removedSomething = true;
						break;
					}
				}

				for (int i = stats.ActiveAbilities.Count - 1; i > -1; i--)
				{
					if (stats.ActiveAbilities[i].gameObject.name.Contains(abilname))
					{
						global::Console.AddMessage("Removed active ability: " + stats.ActiveAbilities[i].gameObject.name);
						stats.ActiveAbilities[i].ForceDeactivate(npc);
						AbilityProgressionTable.RemoveAbilityFromCharacter(stats.ActiveAbilities[i].gameObject, stats);
						removedSomething = true;
						break;
					}
				}

				for (int i = stats.Abilities.Count - 1; i > -1; i--)
				{
					if (stats.Abilities[i].gameObject.name.Contains(abilname))
					{
						global::Console.AddMessage("Removed ability: " + stats.Abilities[i].gameObject.name);
						stats.Abilities[i].ForceDeactivate(npc);
						AbilityProgressionTable.RemoveAbilityFromCharacter(stats.Abilities[i].gameObject, stats);
						removedSomething = true;
						break;
					}
				}

				if (!removedSomething)
				{
					global::Console.AddMessage("Nothing was removed. Talent wasn't found.");
				}

			}
			else
				global::Console.AddMessage("Party memeber not found.");
		}

		[NewMember]
		public  static void SwitchPOTD()
		{
			if (GameState.Instance.Difficulty != GameDifficulty.PathOfTheDamned)
			{
				GameState.Instance.Difficulty = GameDifficulty.PathOfTheDamned;
				global::Console.AddMessage("The difficulty is now: Path of the Damned.");
			}
			else
			{
				GameState.Instance.Difficulty = GameDifficulty.Hard;
				global::Console.AddMessage("The difficulty is now: Hard.");
			}
		}

		[NewMember]
		public  static void ReenableAchievements()
		{
			if (AchievementTracker.Instance.DisableAchievements == true)
			{
				AchievementTracker.Instance.DisableAchievements = false;
				global::Console.AddMessage("Achievements have been reenabled for this playthrough.", Color.green);
			}
			else
				global::Console.AddMessage("Nothing was done, since achievements were already active.");
		}

		[NewMember]
		public  static void FixSagani(string guid)
		{
			GameObject sagani = UnityEngine.GameObject.Find(guid);
			if (sagani != null)
			{
				for (int i = sagani.GetComponent<CharacterStats>().ActiveAbilities.Count - 1; i > -1; i--)
				{
					if (sagani.GetComponent<CharacterStats>().ActiveAbilities[i].gameObject.name.Contains("SummonCompanion") && !sagani.GetComponent<CharacterStats>().ActiveAbilities[i].gameObject.name.Contains("ArcticFox"))
					{
						sagani.GetComponent<CharacterStats>().ActiveAbilities[i].ForceDeactivate(sagani);
						AbilityProgressionTable.RemoveAbilityFromCharacter(sagani.GetComponent<CharacterStats>().ActiveAbilities[i].gameObject, sagani.GetComponent<CharacterStats>());
					}
				}
			}
			else
				global::Console.AddMessage("Character not found.");
		}

		[NewMember]
		public static void ClearAllPreferences(bool confirmation) {
			if (!confirmation) {
				Console.AddMessage("You need to supply a 'true' argument if you're sure you want to clear all preferences.");
				return;
			}
			PlayerPrefs.DeleteAll();
			Console.AddMessage("All preferences cleared. Please restart the game so that no errors occur.");
		}

		[NewMember]
		public  static void ChangeClass(string guid, string charclass)
		{
			GameObject npc = null;
			string charname = guid.Replace("_", " ");
			foreach (var partymember in PartyMemberAI.PartyMembers)
			{
				if (partymember != null && RemoveDiacritics(partymember.gameObject.GetComponent<CharacterStats>().Name()).Contains(charname))
					npc = partymember.gameObject;
			}
			if (npc == null)
				npc = UnityEngine.GameObject.Find(guid);
			if (npc != null)
			{
				bool goOn = false;
				try
				{
					if (Enum.Parse(typeof(CharacterStats.Class), charclass) != null)
						goOn = true;
				}
				catch
				{
					global::Console.AddMessage(charclass + " - not found as a class.");
				}
				if (goOn)
				{
					object newclassobj = Enum.Parse(typeof(CharacterStats.Class), charclass);
					int newclassId = Convert.ToInt32(newclassobj);

					List<string> Innates = new List<string>();

					//Put all innate non-racial talents and abilities here (case insensitive):
					Innates.Add("crucible_of_the_soul");
					Innates.Add("armed_to_the_teeth");
					Innates.Add("speaker_to_the_restless");
					Innates.Add("dominion_of_the_sleepers");
					Innates.Add("steps_to_the_wheel");
					Innates.Add("Beraths_Boon");
					Innates.Add("Hyleas_Boon");
					Innates.Add("Waels_Boon");
					Innates.Add("Galawains_Boon");
					Innates.Add("Rymrgands_Boon");
					Innates.Add("Skaens_Boon");
					Innates.Add("Second_Skin");
					Innates.Add("The_Merciless_Hand");
					Innates.Add("Mob_Justice");
					Innates.Add("Mob Justice"); //the ability has a space in place of an underscore...
					Innates.Add("Blooded_Hunter");
					Innates.Add("Song_of_the_Heavens");
					Innates.Add("Wild_Running");
					Innates.Add("Dungeon_Delver");
					Innates.Add("Scale-Breaker");
					Innates.Add("Gift_from_the_Machine");
					Innates.Add("Effigys_Resentment"); //should work for all types

					if (npc.GetComponent<CharacterStats>().name.Contains("Sagani"))
					{
						Innates.Add("SummonCompanionArcticFox");
					}

					//==========================================================================
					//REMOVE TALENTS
					//==========================================================================
					List<GenericTalent> talentRemoveList = new List<GenericTalent>();
					foreach (GenericTalent activeTalent in npc.GetComponent<CharacterStats>().ActiveTalents)
					{
						bool saveMe = false;
						foreach (string innate in Innates)
						{
							if (activeTalent.gameObject.name.IndexOf(innate, StringComparison.OrdinalIgnoreCase) >= 0) //look for substring
							{
								saveMe = true;
								break;
							}
						}
						if (!saveMe)
							talentRemoveList.Add(activeTalent);
					}
					foreach (GenericTalent talentToRemove in talentRemoveList)
						AbilityProgressionTable.RemoveAbilityFromCharacter(talentToRemove.gameObject, npc.GetComponent<CharacterStats>());

					talentRemoveList.Clear();
					foreach (GenericTalent talent in npc.GetComponent<CharacterStats>().Talents)
					{
						bool saveMe = false;
						foreach (string innate in Innates)
						{
							if (talent.gameObject.name.IndexOf(innate, StringComparison.OrdinalIgnoreCase) >= 0) //look for substring
							{
								saveMe = true;
								break;
							}
						}
						if (!saveMe)
							talentRemoveList.Add(talent);
					}
					foreach (GenericTalent talentToRemove in talentRemoveList)
						AbilityProgressionTable.RemoveAbilityFromCharacter(talentToRemove.gameObject, npc.GetComponent<CharacterStats>());
					//==========================================================================

					//==========================================================================
					//REMOVE ABILITIES
					//==========================================================================
					List<GenericAbility> abilRemoveList = new List<GenericAbility>();
					foreach (GenericAbility activeAbility in npc.GetComponent<CharacterStats>().ActiveAbilities)
					{
						if (activeAbility.EffectType == GenericAbility.AbilityType.Racial)
							continue;
						bool saveMe = false;
						foreach (string innate in Innates)
						{
							if (activeAbility.gameObject.name.IndexOf(innate, StringComparison.OrdinalIgnoreCase) >= 0) //look for substring
							{
								saveMe = true;
								break;
							}
						}
						if (!saveMe)
							abilRemoveList.Add(activeAbility);
					}
					foreach (GenericAbility abilToRemove in abilRemoveList)
					{
						abilToRemove.ForceDeactivate(npc);
						AbilityProgressionTable.RemoveAbilityFromCharacter(abilToRemove.gameObject, npc.GetComponent<CharacterStats>());
					}
					abilRemoveList.Clear();
					foreach (GenericAbility ability in npc.GetComponent<CharacterStats>().Abilities)
					{
						if (ability.EffectType == GenericAbility.AbilityType.Racial)
							continue;
						bool saveMe = false;
						foreach (string innate in Innates)
						{
							if (ability.gameObject.name.IndexOf(innate, StringComparison.OrdinalIgnoreCase) >= 0) //look for substring
							{
								saveMe = true;
								break;
							}
						}
						if (!saveMe)
							abilRemoveList.Add(ability);
					}
					foreach (GenericAbility abilToRemove in abilRemoveList)
					{
						abilToRemove.ForceDeactivate(npc);
						AbilityProgressionTable.RemoveAbilityFromCharacter(abilToRemove.gameObject, npc.GetComponent<CharacterStats>());
					}
					//==========================================================================

					// remove ranger's pet
					if (npc.GetComponent<CharacterStats>().CharacterClass == CharacterStats.Class.Ranger && !npc.GetComponent<CharacterStats>().name.Contains("Sagani"))
					{
						foreach (var cre in npc.GetComponent<AIController>().SummonedCreatureList)
						{
							if (GameUtilities.IsAnimalCompanion(cre.gameObject))
							{
								PartyMemberAI.RemoveFromActiveParty(cre.GetComponent<PartyMemberAI>(), true);
								cre.GetComponent<Persistence>().UnloadsBetweenLevels = true;
								cre.GetComponent<Health>().m_isAnimalCompanion = false;
								cre.GetComponent<Health>().ApplyDamageDirectly(1000);
								cre.GetComponent<Health>().ApplyDamageDirectly(1000);
								global::Console.AddMessage(cre.GetComponent<CharacterStats>().Name() + " is free from its bonds and returns to the wilds to be with its own kind.", Color.green);
								cre.SetActive(false);
							}
						}
						//npc.GetComponent<AIController> ().SummonedCreatureList.Clear ();
					}

					// remove or give grimoire
					if (npc.GetComponent<CharacterStats>().CharacterClass != (CharacterStats.Class)newclassId)
					{
						if (npc.GetComponent<CharacterStats>().CharacterClass == CharacterStats.Class.Wizard)
						{
							npc.GetComponent<Equipment>().UnEquip(Equippable.EquipmentSlot.Grimoire);
						}

						npc.GetComponent<CharacterStats>().CharacterClass = (CharacterStats.Class)newclassId;

						if (npc.GetComponent<CharacterStats>().CharacterClass == CharacterStats.Class.Wizard)
						{
							// equip an empty grimoire...?
							Equippable grim = GameResources.LoadPrefab<Equippable>("empty_grimoire_01", true);
							if (grim != null)
							{
								grim.GetComponent<Grimoire>().PrimaryOwnerName = npc.GetComponent<CharacterStats>().Name();
								npc.GetComponent<Equipment>().Equip(grim);
							}
						}
					}

					//BaseDeflection,BaseFortitude,BaseReflexes,BaseWill,MeleeAccuracyBonus,RangedAccuracyBonus,MaxHealth,MaxStamina,HealthStaminaPerLevel,ClassHealthMultiplier
					object comp = (object)npc.GetComponent<CharacterStats>();
					DataManager.AdjustFromData(ref comp);

					npc.GetComponent<CharacterStats>().Level = 0;

					npc.GetComponent<CharacterStats>().StealthSkill = 0;
					npc.GetComponent<CharacterStats>().StealthBonus = 0;
					npc.GetComponent<CharacterStats>().AthleticsSkill = 0;
					npc.GetComponent<CharacterStats>().AthleticsBonus = 0;
					npc.GetComponent<CharacterStats>().LoreSkill = 0;
					npc.GetComponent<CharacterStats>().LoreBonus = 0;
					npc.GetComponent<CharacterStats>().MechanicsSkill = 0;
					npc.GetComponent<CharacterStats>().MechanicsBonus = 0;
					npc.GetComponent<CharacterStats>().SurvivalSkill = 0;
					npc.GetComponent<CharacterStats>().SurvivalBonus = 0;

					npc.GetComponent<CharacterStats>().RemainingSkillPoints = 0;

					string HeOrShe = npc.GetComponent<CharacterStats>().Gender.ToString();
					global::Console.AddMessage(npc.GetComponent<CharacterStats>().Name() + " has reformed into a " + charclass + ". " + (HeOrShe == "Male" ? "He" : "She") + " lost all " + (HeOrShe == "Male" ? "his" : "her") + " previous abilities and talents.", Color.green);
				}
			}
			else
				global::Console.AddMessage("Couldn't find: " + guid, Color.yellow);
		}

		[ModifiesMember("Skill")]
		public  static void SkillNew(string name, string skillScore, string value)
		{
			int num;
			if (!int.TryParse(value, out num))
			{
				Debug.Log("Skill: ERROR - value not in integer format.");
			}
			else
			{
				CharacterStats component = null;
				if (name.ToLower() == "player")
				{
					if (GameState.s_playerCharacter == null)
					{
						Debug.Log("Skill: Error - player character not found.");
						return;
					}
					component = GameState.s_playerCharacter.GetComponent<CharacterStats>();
				}
				else
				{
					CharacterStats[] statsArray = UnityEngine.Object.FindObjectsOfType<CharacterStats>();
					for (int i = 0; i < statsArray.Length; i++)
					{
						if (statsArray[i].gameObject.name.ToLower().Contains(name.ToLower()))
						{
							component = statsArray[i];
							break;
						}
					}
				}
				if (component == null)
				{
					Debug.Log("Skill: Error - stats component not found for " + name);
				}
				else
				{
					skillScore = skillScore.ToUpper();
					int length = Enum.GetNames(typeof(CharacterStats.SkillType)).Length;
					for (int j = 0; j < length; j++)
					{
						if (!(skillScore == Enum.GetNames(typeof(CharacterStats.SkillType))[j].ToUpper()))
						{
							continue;
						}
						switch (((CharacterStats.SkillType)j))
						{
							case CharacterStats.SkillType.Stealth:
								component.StealthSkill = num;
								break;

							case CharacterStats.SkillType.Athletics:
								component.AthleticsSkill = num;
								break;

							case CharacterStats.SkillType.Lore:
								component.LoreSkill = num;
								break;

							case CharacterStats.SkillType.Mechanics:
								component.MechanicsSkill = num;
								break;

							case CharacterStats.SkillType.Survival:
								component.SurvivalSkill = num;
								break;

							case CharacterStats.SkillType.Crafting:
								component.CraftingSkill = num;
								break;
						}
						global::Console.AddMessage(name + "'s " + skillScore + " is now " + value.ToString());
						return;
					}
					Debug.Log("Skill Score: Error - could not find skill " + skillScore);
				}
			}
		}

		[NewMember]
		public  static void RenameCreature(string guid, string newname)
		{
			GameObject npc = UnityEngine.GameObject.Find(guid);
			if (npc != null && npc.GetComponent<CharacterStats>() != null)
				npc.GetComponent<CharacterStats>().OverrideName = newname;
		}


		[NewMember]
		[Cheat]
		public static void ShowMouseDebug() {
			GameCursor.ShowDebug = !GameCursor.ShowDebug;
		}

		// makes it possible to use some console commands without having to type in "IRoll20s" first
		[ModifiesMember("RunCommand")]
		public  static void RunCommandNew(string command)
		{
			if (!string.IsNullOrEmpty(command) && (command.ToLower() != "runcommand"))
			{
				List<string> list = new List<string>();
				char[] separator = new char[] { ' ' };
				list.AddRange(command.Split(separator));

				var methods = typeof(Scripts).GetMethods().Concat<System.Reflection.MethodInfo>(typeof(CommandLine).GetMethods());
				
				IEnumerator<MethodInfo> enumerator = methods.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						System.Reflection.MethodInfo current = enumerator.Current;
						bool alwaysCheat = true; // added this line and put it in the if below
						if (((current.GetCustomAttributes(typeof(CheatAttribute), true).Length <= 0) || alwaysCheat) && (string.Compare(current.Name, list[0], true) == 0))
						{
							ParameterInfo[] parameters = current.GetParameters();
							if (parameters.Length == (list.Count - 1))
							{
								object[] objArray = new object[parameters.Length];
								for (int i = 0; i < parameters.Length; i++)
								{
									Type attributeType = (Type)s_ScriptAttributeTypes[i];
									object[] customAttributes = current.GetCustomAttributes(attributeType, false);
									Scripts.BrowserType none = Scripts.BrowserType.None;
									if (customAttributes.Length > 0)
									{
										none = (Scripts.BrowserType)((int)attributeType.GetProperty("Browser").GetValue(customAttributes[0], null));
									}
									ParameterInfo info2 = parameters[i];
									if (info2.ParameterType.IsEnum)
									{
										try
										{
											objArray[i] = Enum.Parse(info2.ParameterType, list[i + 1]);
										}
										catch
										{
											objArray[i] = Enum.GetValues(info2.ParameterType).GetValue(0);
										}
									}
									else if ((none == Scripts.BrowserType.ObjectGuid) || (info2.ParameterType == typeof(Guid)))
									{
										try
										{
											objArray[i] = new Guid(list[i + 1]);
										}
										catch
										{
											GameObject obj2 = GameObject.Find(list[i + 1]);
											if (obj2 != null)
											{
												InstanceID component = obj2.GetComponent<InstanceID>();
												if (component != null)
												{
													objArray[i] = component.Guid;
												}
												else
												{
													objArray[i] = Guid.Empty;
												}
											}
											else
											{
												objArray[i] = Guid.Empty;
											}
										}
									}
									else if (none == Scripts.BrowserType.Quest)
									{
										List<string> list2 = QuestManager.Instance.FindLoadedQuests(list[i + 1]);
										if (list2.Count != 0)
										{
											if (list2.Count != 1)
											{
												global::Console.AddMessage("Multiple quests matched search '" + list[i + 1] + "':", Color.yellow);
												foreach (string str in list2)
												{
													global::Console.AddMessage(Path.GetFileNameWithoutExtension(str), Color.yellow);
												}
												return;
											}
											objArray[i] = list2[0];
										}
										else
										{
											objArray[i] = list[i + 1];
										}
									}
									else if (none == Scripts.BrowserType.Conversation)
									{
										List<string> list3 = ConversationManager.Instance.FindConversations(list[i + 1]);
										if (list3.Count != 0)
										{
											if (list3.Count != 1)
											{
												global::Console.AddMessage("Multiple conversations matched search '" + list[i + 1] + "':", Color.yellow);
												foreach (string str2 in list3)
												{
													global::Console.AddMessage(Path.GetFileNameWithoutExtension(str2), Color.yellow);
												}
												return;
											}
											objArray[i] = list3[0];
										}
										else
										{
											objArray[i] = list[i + 1];
										}
									}
									else if (info2.ParameterType.IsValueType)
									{
										try
										{
											objArray[i] = Convert.ChangeType(list[i + 1], info2.ParameterType, CultureInfo.InvariantCulture);
										}
										catch (Exception)
										{
											global::Console.AddMessage(string.Concat(new object[] { "Script error: could not convert parameter ", i + 1, " ('", list[i], "') into type ", info2.ParameterType.Name, ". " }), Color.yellow);
											return;
										}
									}
									else
									{
										objArray[i] = list[i + 1];
									}
								}
								current.Invoke(null, objArray);
								return;
							}
						}
					}
				}
				finally
				{
					if (enumerator == null)
					{
					}
					else
						enumerator.Dispose(); // seems there was an "else" lacking here
				}
				global::Console.AddMessage(string.Concat(new object[] { "No command or script '", list[0], "' accepting ", list.Count - 1, " parameters exists." }), Color.yellow);
			}
		}



		[NewMember]
		public  static void ListActiveQuests()
		{
			foreach (var quest in QuestManager.Instance.ActiveQuests)
				global::Console.AddMessage(quest.Key);
		}

		[NewMember]
		public  static void ForceAdvanceQuest(string name)
		{
			QuestManager.Instance.AdvanceQuest(name, true);
		}

		[NewMember]
		public  static void OpenContainer(string objectGuid)
		{
			GameObject container = GameObject.Find(objectGuid);
			if (container != null)
			{
				Container chest = container.GetComponent<Container>();
				if (chest != null)
					chest.Open(GameState.s_playerCharacter.gameObject, true);
				else
				{
					global::Console.AddMessage("Object is not a container.");
				}
				//oCLComponent.SealOpen ();
			}
			else
			{
				global::Console.AddMessage("Container not found");
			}
		}

		[NewMember]
		public  static void ShowAIState()
		{
			if (GameCursor.CharacterUnderCursor)
			{
				var ai = GameCursor.CharacterUnderCursor.GetComponent<AIController>();
				if (ai)
				{
					var stateManager = ai.StateManager;
					var str = new System.Text.StringBuilder();
					stateManager.BuildDebugText(str);
					global::Console.AddMessage(str.ToString());
				}
				else
				{
					global::Console.AddMessage("no AI available");
				}
			}
			else
			{
				global::Console.AddMessage("no one under the cursor");
			}
		}

		[NewMember]
		public  static void Jump()
		{
			if (GameState.s_playerCharacter.IsMouseOnWalkMesh())
			{
				foreach (var partymember in PartyMemberAI.GetSelectedPartyMembers())
				{
					partymember.transform.position = GameInput.WorldMousePosition;
				}
			}
			else
			{
				global::Console.AddMessage("Mouse is not on navmesh.");
			}
		}

		

		// spawns a creature... param1 is the creature, param2 is a bool: 0 for friendly, 1 for hostile
		// for instance: BSC cre_druid_cat01 1
		static void BSC(string param1, string param2)
		{
			if (GameState.s_playerCharacter.IsMouseOnWalkMesh())
			{
				if (param2 == "1")
					global::Console.AddMessage("Spawning a hostile: " + param1, UnityEngine.Color.green);
				else
					global::Console.AddMessage("Spawning a friendly: " + param1, UnityEngine.Color.green);
				UnityEngine.GameObject besterCreature;
				besterCreature = GameResources.LoadPrefab<UnityEngine.GameObject>(param1, true);
				if (besterCreature != null)
				{
					besterCreature.transform.position = GameInput.WorldMousePosition;
					besterCreature.transform.rotation = GameState.s_playerCharacter.transform.rotation;

					bool isHostile = false;
					if (param2 == "1")
						isHostile = true;

					if (isHostile == false)
					{
						if (besterCreature.GetComponent<Faction>() != null)
						{
							besterCreature.GetComponent<Faction>().RelationshipToPlayer = Faction.Relationship.Neutral;
							besterCreature.GetComponent<Faction>().CurrentTeamInstance = Team.GetTeamByTag("player");
							besterCreature.GetComponent<Faction>().UnitHostileToPlayer = false;
						}
						if (besterCreature.GetComponent<AIPackageController>() != null)
						{
							// some monsters might come without AI, which would make them always stand idly, so we give them DefaultAI
							besterCreature.GetComponent<AIPackageController>().ChangeBehavior(AIPackageController.PackageType.DefaultAI);
							besterCreature.GetComponent<AIPackageController>().InitAI();
						}
					}
					else
					{
						if (besterCreature.GetComponent<Faction>() != null)
						{
							besterCreature.GetComponent<Faction>().CurrentTeamInstance = Team.GetTeamByTag("monster");
							besterCreature.GetComponent<Faction>().RelationshipToPlayer = Faction.Relationship.Hostile;

						}
						if (besterCreature.GetComponent<AIPackageController>() != null)
						{
							besterCreature.GetComponent<AIPackageController>().ChangeBehavior(AIPackageController.PackageType.DefaultAI);
							besterCreature.GetComponent<AIPackageController>().InitAI();
						}
					}
					global::CameraControl.Instance.FocusOnPoint(besterCreature.transform.position);
				}
				else
					global::Console.AddMessage("Failed to spawn " + param1 + " - probably bad naming.", UnityEngine.Color.red);
			}
			else
				global::Console.AddMessage("Mouse is not on navmesh, move mouse elsewhere and try again.", UnityEngine.Color.red);
		}
	
		

		// this method gives your maincharacter all existing mage spells... it was just to test something, but someone might want to use some bits of it
		[NewMember]
		public  static void AdAb()
		{
			CharacterStats firstparam = GameState.s_playerCharacter.GetComponent<CharacterStats>();
			AbilityProgressionTable wizardsProgressionTable = AbilityProgressionTable.LoadAbilityProgressionTable("Wizard");
			global::Console.AddMessage("Wizard abilities in game: " + wizardsProgressionTable.AbilityUnlocks.Length);
			global::Console.AddMessage("This wizard has abilities: " + GameState.s_playerCharacter.GetComponent<CharacterStats>().GetCopyOfCoreData().KnownSkills.Count());
			foreach (var abil in wizardsProgressionTable.AbilityUnlocks)
			{
				bool hasSpell = false;

				foreach (var spell in firstparam.GetCopyOfCoreData().KnownSkills)
					if (abil.Ability.name == spell.name.Replace("(Clone)", ""))
						hasSpell = true;

				if (hasSpell)
					global::Console.AddMessage("The wizard already knows: " + abil.Ability.name);
				else
					CommandLine.AddAbility(firstparam, abil.Ability.name);
			}
		}
		[ModifiesMember(".cctor")]
		private void ConstructorNew()
		{
			// added code
			modelViewerBackground = null;
			GameBrowserBackground = null;
			// end of added code
			s_ScriptAttributeTypes = new object[] { typeof(ScriptParam0Attribute), typeof(ScriptParam1Attribute), typeof(ScriptParam2Attribute), typeof(ScriptParam3Attribute), typeof(ScriptParam4Attribute), typeof(ScriptParam5Attribute), typeof(ScriptParam6Attribute), typeof(ScriptParam7Attribute) };
		}

		[NewMember]
		public static void DeleteIEModSettings(bool areYouSure) {
			if (!areYouSure) {
				Console.AddMessage("You need to pass 'true' if you really want to delete all settings.", Color.red);
				return;
			}
			IEModOptions.DeleteAllSettings();
			Console.AddMessage("All settings have been deleted.", Color.green);
		}


		[NewMember]
		public  static void SelectCircles(float width)
		{
			global::Console.AddMessage("Setting selection circle width to: " + width, Color.green);
			InGameHUD.Instance.SelectionCircleWidth = width;
			InGameHUD.Instance.EngagedCircleWidth = width;
			IEModOptions.SelectionCircleWidth = width;
		}

		[NewMember]
		public static void BB() {
			UICustomizer.ShowInterface(true);
		}

		[NewMember]
		public  static void TT()
		{
			if (((Mod_OnGUI_Player)GameState.s_playerCharacter).showGameObjectBrowser == false)
			{
				if (((Mod_OnGUI_Player)GameState.s_playerCharacter).inspecting == null)
					((Mod_OnGUI_Player)GameState.s_playerCharacter).inspecting = UICustomizer.UiCamera.transform;

				if (GameBrowserBackground == null)
				{
					UIMultiSpriteImageButton portraitLast = null;

					foreach (UIMultiSpriteImageButton btn in UnityEngine.Object.FindObjectsOfType<UIMultiSpriteImageButton>())
						if (btn.ToString() == "PartyPortrait(Clone) (UIMultiSpriteImageButton)" && portraitLast == null)
							portraitLast = btn;

					GameBrowserBackground = NGUITools.AddWidget<UITexture>(portraitLast.transform.parent.parent.gameObject);
					GameBrowserBackground.mainTexture = new Texture2D(500, 500);

					GameBrowserBackground.transform.localScale = new Vector3(1100f, 600f, 1f); // those values are for 1920*1080...
					//GameBrowserBackground.transform.localScale = new Vector3 (1600f, 800f, 1f); // those values are for 1280*1024

					// rescaler
					if (Screen.width != 1920)
					{
						float addwidth = (1920 - Screen.width) * 0.78f;
						float addheight = (1080 - Screen.height) * 0.55f;
						GameBrowserBackground.transform.localScale += new Vector3(addwidth, addheight, 0f);
					}

					BoxCollider boxColl = NGUITools.AddWidgetCollider(GameBrowserBackground.gameObject); // adding a box collider, it's required for the UINoClick component
					boxColl.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
					UIAnchor ank = GameBrowserBackground.gameObject.AddComponent<UIAnchor>();
					ank.side = UIAnchor.Side.Center;

				}
				else
					GameBrowserBackground.gameObject.SetActive(true);

				((Mod_OnGUI_Player)GameState.s_playerCharacter).showGameObjectBrowser = true; // temporarily turned off
			}
			else
			{
				GameBrowserBackground.gameObject.SetActive(false);
				((Mod_OnGUI_Player)GameState.s_playerCharacter).showGameObjectBrowser = false;
			}
		}

		[NewMember]
		public  static void ShowModelViewer()
		{
			if (((Mod_OnGUI_Player)GameState.s_playerCharacter).showModelViewer)
			{
				global::Console.AddMessage("Closing ModelViewer.");
				((Mod_OnGUI_Player)GameState.s_playerCharacter).showModelViewer = false;
				modelViewerBackground.SetActive(false);
			}
			else
			{
				global::Console.AddMessage("Opening ModelViewer. Type the same command to hide it.");
				if (modelViewerBackground == null)
				{
					////////////////////// finding various buttons and textures
					UISprite closeButton = null;

					UISprite[] mdo = Resources.FindObjectsOfTypeAll(typeof(UISprite)) as UISprite[];
					foreach (UISprite brr in mdo)
					{
						if (brr.ToString() == "CloseButton (UISprite)" && closeButton == null)
						{
							closeButton = brr;
							//Console.AddMessage ("Assigning CloseButton.");
						}
					}

					UIMultiSpriteImageButton portraitLast = null;

					foreach (UIMultiSpriteImageButton btn in UnityEngine.Object.FindObjectsOfType<UIMultiSpriteImageButton>())
						if (btn.ToString() == "PartyPortrait(Clone) (UIMultiSpriteImageButton)" && portraitLast == null)
						{
							portraitLast = btn;
							//Console.AddMessage ("Assigning optioButton.");
						}
					///////////////////////////////

					modelViewerBackground = new GameObject("ModelViewer");
					modelViewerBackground.transform.parent = portraitLast.transform.parent;
					modelViewerBackground.layer = portraitLast.gameObject.layer;
					modelViewerBackground.transform.localScale = new Vector3(1f, 1f, 1f);

					UISprite spr = NGUITools.AddSprite(modelViewerBackground, closeButton.atlas, "BUT_genericBGMid");
					spr.transform.localScale = new Vector3(Screen.width / 4, Screen.height * 1.6f, 1f);
					spr.type = UISprite.Type.Sliced;
					spr.color = new Color(0f, 0f, 0f, 0.3f);

					BoxCollider cold = spr.gameObject.AddComponent<BoxCollider>();
					cold.size = new Vector3(1f, 1f, 1f);

					spr.gameObject.AddComponent<UINoClick>().BlockClicking = true;

					modelViewerBackground.transform.localPosition = new Vector3(Screen.width / 10, Screen.height * 0.8f, 0);
				}
				else
					modelViewerBackground.SetActive(true);

				((Mod_OnGUI_Player)GameState.s_playerCharacter).showModelViewer = true;
			}
		}


	}

}