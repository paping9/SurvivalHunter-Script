#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Utils.Extension
{
	public static class AddressableEx
	{
		public static void SetAddressableAsset(string assetPath, string addrPath, string groupName,
			string label = "", bool bAddLabel = false)
		{
			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			if (string.IsNullOrEmpty(guid))
			{
				Debug.LogError($"Path ERROR : {assetPath}");
				return;
			}

			var addrSetting = AddressableAssetSettingsDefaultObject.Settings;
			AddressableAssetGroup group = null;
			if (string.IsNullOrEmpty(groupName))
				group = addrSetting.DefaultGroup;
			else
				group = addrSetting.FindGroup(groupName);

			if (group is null)
			{
				var defaultGroup = addrSetting.DefaultGroup;
				group = addrSetting.CreateGroup(groupName, false, false, true,
					defaultGroup.Schemas, defaultGroup.SchemaTypes.ToArray());
			}

			if (group is null)
			{
				Debug.LogError($"Adressable Group [{groupName}] is invalid.");
				return;
			}

			var entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
				AssetDatabase.AssetPathToGUID(assetPath), group, false, true);

			if (entry == null)
			{
				throw new Exception($"Addressable : can't add {assetPath} to group {groupName}");
			}

			entry.address = addrPath;
			if (!string.IsNullOrEmpty(label))
			{
				if (bAddLabel)
				{
					entry.SetLabel(label, true, true, true);
				}
				else
				{
					var labelList = entry.labels.ToList();
					foreach (var labelInList in labelList)
					{
						entry.SetLabel(labelInList, false, postEvent: true);
					}

					entry.SetLabel(label, true, true, true);
				}
			}

			Debug.LogFormat("<color=yellow>Set Address path Success.</color>\nAddrPath[{0}] - AssetPath[{1}] - GroupName [{2}] - Label [{3}]",
				addrPath, assetPath, groupName, label);

			//addrSetting.RemoveGroup
		}

		public static string GetAddressableAsset(string assetPath, string groupName)
		{
			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			if (string.IsNullOrEmpty(guid))
			{
				Debug.LogError($"Path ERROR : {assetPath}");
				return "";
			}

			var addrSetting = AddressableAssetSettingsDefaultObject.Settings;
			AddressableAssetGroup group = null;

			if (string.IsNullOrEmpty(groupName))
				group = addrSetting.DefaultGroup;
			else
				group = addrSetting.FindGroup(groupName);

			if (group is null)
			{
				var defaultGroup = addrSetting.DefaultGroup;
				group = addrSetting.CreateGroup(groupName, false, false, true,
					defaultGroup.Schemas, defaultGroup.SchemaTypes.ToArray());
			}

			if (group is null)
			{
				Debug.LogError($"Adressable Group [{groupName}] is invalid.");
				return "";
			}

			foreach (var entry in group.entries)
			{
				if (entry.AssetPath == assetPath) return entry.address;
			}

			return "";
		}

		public static bool IsAssetBundleSetting(string assetPath)
		{
			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			if (string.IsNullOrEmpty(guid))
			{
				Debug.LogError($"Path ERROR : {assetPath}");
				return false;
			}

			var addrSetting = AddressableAssetSettingsDefaultObject.Settings;
			var entry = addrSetting.FindAssetEntry(guid, true);

			return entry != null;
		}
	}
}
#endif
