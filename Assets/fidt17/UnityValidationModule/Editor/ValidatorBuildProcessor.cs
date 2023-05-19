using System;
using System.Collections.Generic;
using System.Linq;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace fidt17.UnityValidationModule.Editor
{
	public class ValidatorBuildProcessor : IPreprocessBuildWithReport
	{
		public int callbackOrder { get; }
		
		public void OnPreprocessBuild(BuildReport report)
		{
			var settings = ValidatorSettings.GetSettings();
			
			if (!settings.RunValidatorOnBuild) return;
			Debug.Log("Run prebuild validator");
			
			var openScenes = new List<Scene>();
			Func<IEnumerable<ValidationResult>> validationFunction = () => Validator.ValidateEverything(openScenes);

			var results = validationFunction.Invoke().ToList();
			var clearResults = new List<ValidationResult>();
			var failedCount = 0;
			foreach (var validationResult in results)
			{
				if (validationResult.Result || 
				    clearResults.Exists(x => x.TargetContext == validationResult.TargetContext && x.Message == validationResult.Message)) continue;
				
				if (validationResult.Result == false)
				{
					failedCount += 1;
				}
				clearResults.Add(validationResult);
			}

			if (failedCount != 0)
			{
				var exception = new BuildFailedException($"Validator found {failedCount} errors. Fix them before building. " +
				                                         "\nYou can open validator window by pressing Alt-V or by path /Tools/Validator");
				if (settings.StopBuildIfValidatorFails)
				{
					throw exception;
				}

				Debug.LogError(exception);
			}
		}
	}
}