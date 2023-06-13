using UnityEditor;
using UnityEngine;

namespace fidt17.UnityValidationModule.Editor.Views
{
	internal class ValidatorSettingsView
	{
		private readonly ValidatorSettings _settings;

		public ValidatorSettingsView()
		{
			_settings = ValidatorSettings.GetSettings();
		}

		public void OnGUI()
		{
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.Space(3);
				if (_settings == null)
				{
					EditorGUILayout.LabelField("Could not find settings file", new GUIStyle(GUI.skin.label)
					{
						fontSize = 12
					});
				}
				else
				{
					EditorGUILayout.LabelField("Build Settings", new GUIStyle(GUI.skin.label)
					{
						fontSize = 13
					});
			
					EditorGUILayout.Space(5);
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("Run Validator on build", new GUIStyle(GUI.skin.label)
						{
							fontSize = 12,
						});
						var runValidatorOnBuild = EditorGUILayout.Toggle(_settings.RunValidatorOnBuild, new GUIStyle(GUI.skin.toggle)
						{
							alignment = TextAnchor.MiddleLeft
						});
						if (runValidatorOnBuild != _settings.RunValidatorOnBuild)
						{
							_settings.RunValidatorOnBuild = runValidatorOnBuild;
							EditorUtility.SetDirty(_settings);
							AssetDatabase.SaveAssets();
						}
			
					}
					EditorGUILayout.EndHorizontal();
				
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("Stop build on Validator failure", new GUIStyle(GUI.skin.label)
						{
							fontSize = 12,
						});
						var stopBuildOnValidatorFailure = EditorGUILayout.Toggle(_settings.StopBuildIfValidatorFails, new GUIStyle(GUI.skin.toggle)
						{
							alignment = TextAnchor.MiddleLeft
						});
						if (stopBuildOnValidatorFailure != _settings.StopBuildIfValidatorFails)
						{
							_settings.StopBuildIfValidatorFails = stopBuildOnValidatorFailure;
							EditorUtility.SetDirty(_settings);
							AssetDatabase.SaveAssets();
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
		}
	}
}