using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fidt17.UnityValidationModule.Editor.ContextParsers;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;

namespace fidt17.UnityValidationModule.Editor
{
    public class ValidatorWindow : EditorWindow
    {
        private static ValidationScopeEnum _scope = ValidationScopeEnum.ActiveScene;
        private static List<BaseResultParser> _resultParsers;

        private static int _currentResultsPage;
        private static int _totalValidations;
        private static int _processedResults;
        private static bool _validationRunning;
        
        private static int _adaptiveYieldStep = 10;
        private const int _yieldStepIncrease = 5;
        
        private Vector2 _scrollPosition;

        [MenuItem("Tools/Validator/Run Validator &v")]
        private static void RunValidator()
        {
            if (Application.isPlaying || _validationRunning) return;

            var window = ShowWindow();
            window.StartValidationOperation();
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

            return window;
        }

        private void OnDestroy()
        {
            _validationRunning = false;
        }

        #region Validation Process
        
        private async void StartValidationOperation()
        {
            if (_validationRunning) return;

            _validationRunning = true;

            _totalValidations = 0;
            _processedResults = 0;
            _currentResultsPage = 0;
            _adaptiveYieldStep = 10;
            
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            var validationResults = new List<ValidationResult>();
            var newlyOpenedScenes = new List<Scene>();

            Func<IEnumerable<ValidationResult>> getVLFunction = null;
            switch (_scope)
            {
                case ValidationScopeEnum.ActiveScene:
                    getVLFunction = Validator.ValidateActiveScene;
                    break;
                
                case ValidationScopeEnum.InBuildScenes:
                    getVLFunction = () => Validator.ValidateInBuildScenes(newlyOpenedScenes, closeScenes: false);
                    break;
                
                case ValidationScopeEnum.ProjectAssets:
                    getVLFunction = Validator.ValidateProjectAssets;
                    break;
                
                case ValidationScopeEnum.Everything:
                    getVLFunction = () => Validator.ValidateEverything(newlyOpenedScenes, closeScenes: false);
                    break;
            }


            //float t = Time.realtimeSinceStartup;
            int yieldCounter = 0;
            foreach (var validationResult in getVLFunction.Invoke())
            {
                _totalValidations++;
                validationResults.Add(validationResult);

                if (yieldCounter++ > _adaptiveYieldStep)
                {
                    yieldCounter = 0;
                    _adaptiveYieldStep += _yieldStepIncrease;
                    Repaint();
                    await Task.Yield();
                }
            }
            //Debug.Log($"Total: {Time.realtimeSinceStartup - t}");

            await ProcessResults(validationResults);
            
            newlyOpenedScenes.ForEach(x => EditorSceneManager.CloseScene(x, true));
            
            _validationRunning = false;
            Repaint();
        }
        
        private async Task ProcessResults(List<ValidationResult> validationResults)
        {
            int yieldCounter = 0;
            //remove duplicate results
            var clearResults = new List<ValidationResult>();
            foreach (var validationResult in validationResults)
            {
                if (validationResult.Result || clearResults.Exists(x => x.TargetContext == validationResult.TargetContext && x.Message == validationResult.Message))
                {
                    _processedResults++;
                    if (yieldCounter++ > _adaptiveYieldStep)
                    {
                        yieldCounter = 0;
                        _adaptiveYieldStep += _yieldStepIncrease;
                        Repaint();
                        await Task.Yield();
                    }
                }
                else
                {
                    clearResults.Add(validationResult);
                }
            }

            _resultParsers = new List<BaseResultParser>();
            for (var resultIdx = 0; resultIdx < clearResults.Count; resultIdx++)
            {
                _resultParsers.Add(ReflectionExtensions.GetContextParserFor(clearResults[resultIdx]));

                _processedResults++;

                if (yieldCounter++ > _adaptiveYieldStep)
                {
                    yieldCounter = 0;
                    _adaptiveYieldStep += _yieldStepIncrease;
                    Repaint();
                    await Task.Yield();
                }
            }
        }
        
        #endregion
        
        private void OnGUI()
        {
            DrawValidationSettingsPanel();
            DrawResults();
        }

        private void DrawValidationSettingsPanel()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            _scope = (ValidationScopeEnum)EditorGUILayout.EnumPopup(_scope);

            EditorGUILayout.EndVertical();

            EditorGUI.BeginDisabledGroup(_validationRunning);
            var wasRunValidatorButtonPressed = GUILayout.Button("Run validator", new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 350
            });
            EditorGUI.EndDisabledGroup();

            if (wasRunValidatorButtonPressed)
            {
                StartValidationOperation();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            DrawValidationThreadProgress();
        }

        private void DrawValidationThreadProgress()
        {
            if (_validationRunning == false) return;

            var r = EditorGUILayout.BeginVertical();

            if (_processedResults == 0)
            {
                EditorGUILayout.LabelField($"Locating... [{_totalValidations}]", new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.UpperCenter,
                    fontSize = 13
                });   
            }
            else
            {
                EditorGUILayout.LabelField($"Processing... [{(_processedResults / (float) _totalValidations * 100):n2}%]", new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.UpperCenter,
                    fontSize = 13
                });
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawResults()
        {
            if (_validationRunning) return;
            if (_resultParsers == null) return;
            
            EditorGUILayout.BeginVertical(ValidatorEditorUtils.GetBackgroundStyle(ValidatorEditorConstants.LightDarkBackgroundColor));

                if (_resultParsers.Count == 0)
                {
                    EditorGUILayout.LabelField($"All Clear!", new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.UpperCenter,
                        fontSize = 16
                    });
                    EditorGUILayout.Space(5);
                }
                else
                {
                    EditorGUILayout.LabelField($"Found {_resultParsers.Count} errors.", new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.UpperCenter,
                        fontSize = 16
                    });
                    EditorGUILayout.Space(5);
                        
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                    
                    for (int i = _currentResultsPage * ValidatorEditorConstants.MaxResultsForPage;
                         i < _currentResultsPage * ValidatorEditorConstants.MaxResultsForPage + ValidatorEditorConstants.MaxResultsForPage;
                         i++)
                    {
                        if (i >= _resultParsers.Count) break;
                        _resultParsers[i].GetDrawer().DrawValidationResult();
                    }

                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUI.BeginDisabledGroup(_currentResultsPage <= 0);

                        if (GUILayout.Button((Texture)Resources.Load(ValidatorEditorConstants.PreviousPageButtonTextureName), new GUIStyle(GUI.skin.button)
                            {
                                fixedHeight = 25,
                                fixedWidth = 50,
                            }))
                        {
                            _currentResultsPage--;
                        }   
                    
                    EditorGUI.EndDisabledGroup();
                    
                    EditorGUILayout.LabelField($"Page {_currentResultsPage + 1} out of {_resultParsers.Count / ValidatorEditorConstants.MaxResultsForPage + 1}", new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.UpperCenter,
                        fontSize = 14
                    });

                    EditorGUI.BeginDisabledGroup(_currentResultsPage >= _resultParsers.Count / ValidatorEditorConstants.MaxResultsForPage);
                    
                        if ((GUILayout.Button((Texture)Resources.Load(ValidatorEditorConstants.NextPageButtonTextureName), new GUIStyle(GUI.skin.button)
                            {
                                fixedHeight = 25,
                                fixedWidth = 50,
                            })))
                        {
                            _currentResultsPage++;
                        }
                        
                    EditorGUI.EndDisabledGroup();
                        
                    EditorGUILayout.EndHorizontal();
                }

            EditorGUILayout.EndVertical();
        }
    }
}