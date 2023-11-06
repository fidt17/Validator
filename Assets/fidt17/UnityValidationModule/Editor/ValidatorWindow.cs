using System;
using System.Collections.Generic;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Editor.Views;
using UnityEditor;
using UnityEngine;

namespace fidt17.UnityValidationModule.Editor
{
	public class ValidatorWindow : EditorWindow
	{
		private bool _isSetup;
		private ValidatorMenuView _menuView;
		private ValidatorMainView _mainView;
		private ValidatorSettingsView _settingsView;

		[MenuItem("Tools/Validator/Run Validator &v")]
		private static void RunValidator()
		{
			if (Application.isPlaying) return;

			var window = ShowWindow();
			window._mainView.StartValidationOperation();
		}
        
		[MenuItem("Tools/Validator/Validator Window")]
		private static ValidatorWindow ShowWindow()
		{
			if (Application.isPlaying) return null;

			var window = GetWindow<ValidatorWindow>("Validator");
			window.minSize = ValidatorEditorConstants.ValidatorWindowSize;
			window.maxSize = ValidatorEditorConstants.ValidatorWindowSize;
			window.Show();
			
			Validator.Reset();
			window.Setup();
			
			return window;
		}

		private void Setup()
		{
			_isSetup = false;

			try
			{
				_mainView = new ValidatorMainView(this, ValidatorSettings.GetSettings());
				_settingsView = new ValidatorSettingsView();
				_menuView = new ValidatorMenuView(new List<ValidatorMenuView.MenuElement>
				{
					new ValidatorMenuView.MenuElement("Main", () => _mainView.OnGUI()),
					new ValidatorMenuView.MenuElement("Settings", () => _settingsView.OnGUI()),
				}, 0);
			}
			catch (Exception e)
			{
				Debug.LogError($"ValidatorWindow exception: {e}");
			}

			_isSetup = true;
		}

        
		private void OnGUI()
		{
			if (!_isSetup || _menuView == null)
			{
				Setup();
				return;
			}
            
			_menuView.OnGUI();
		}
	}
}