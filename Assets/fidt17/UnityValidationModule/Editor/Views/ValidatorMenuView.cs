using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fidt17.UnityValidationModule.Editor.Views
{
	internal class ValidatorMenuView
	{
		internal class MenuElement
		{
			public readonly string Title;
			public readonly Action OnGUICallback;

			public MenuElement(string title, Action onGUICallback)
			{
				Title = title;
				OnGUICallback = onGUICallback;
			}
		}

		private int _activeElementIdx;
		private readonly List<MenuElement> _elements;
		private readonly string[] _elementNames;

		public ValidatorMenuView(List<MenuElement> elements, int defaultTabIdx)
		{
			_elements = elements;
			_elementNames = _elements.Select(x => x.Title).ToArray();
			_activeElementIdx = defaultTabIdx;
		}

		public void OnGUI()
		{
			if (_elements.Count == 0) return;

			try
			{
				_activeElementIdx = GUILayout.Toolbar(_activeElementIdx, _elementNames);
				GUILayout.Space(5);
				_elements[_activeElementIdx]?.OnGUICallback();
			}
			catch (Exception)
			{
				// ignore
			}
		}
	}
}