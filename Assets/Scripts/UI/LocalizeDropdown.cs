using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequireComponent(typeof(TMP_Dropdown))]
[AddComponentMenu("Localization/Localize dropdown")]
[ExecuteInEditMode]
public class LocalizeDropdown : MonoBehaviour {

    public List<LocalizedDropdownOption> options;

    public int selectedOptionIndex = 0;

    private TMP_Dropdown Dropdown => GetComponent<TMP_Dropdown>();

    private IEnumerator Start() {
        yield return PopulateDropdown();
    }

    private void OnEnable() {
        LocalizationSettings.SelectedLocaleChanged += UpdateDropdownOptions;
    }

    private void OnDisable() {
        LocalizationSettings.SelectedLocaleChanged -= UpdateDropdownOptions;
    }

    private IEnumerator PopulateDropdown() {
        Dropdown.ClearOptions();
        Dropdown.onValueChanged.RemoveListener(UpdateSelectedOptionIndex);
        for(int i = 0; i < options.Count; i++) {
            LocalizedDropdownOption option = options[i];
            string localizedText = string.Empty;
            Sprite localizedSprite = null;
            if(!option.text.IsEmpty) {
                AsyncOperationHandle<string> localizedTextHandle = option.text.GetLocalizedStringAsync();
                yield return localizedTextHandle;
                localizedText = localizedTextHandle.Result;
                if(i == selectedOptionIndex) {
                    UpdateSelectedText(localizedText);
                }
            }

            if(!option.sprite.IsEmpty) {
                AsyncOperationHandle<Sprite> localizedSpriteHandle = option.sprite.LoadAssetAsync();
                yield return localizedSpriteHandle;
                localizedSprite = localizedSpriteHandle.Result;
                if(i == selectedOptionIndex) {
                    UpdateSelectedSprite(localizedSprite);
                }
            }
            Dropdown.options.Add(new TMP_Dropdown.OptionData(localizedText, localizedSprite, Color.white));
        }
        Dropdown.value = selectedOptionIndex;
        Dropdown.onValueChanged.AddListener(UpdateSelectedOptionIndex);
    }

    private void UpdateDropdownOptions(Locale locale) {
        for(int i = 0; i < Dropdown.options.Count; i++) {
            int optionI = i;
            LocalizedDropdownOption option = options[i];
            if(!option.text.IsEmpty) {
                AsyncOperationHandle<string> localizedTextHandle = option.text.GetLocalizedStringAsync(locale);
                localizedTextHandle.Completed += (handle) => {
                    Dropdown.options[optionI].text = handle.Result;
                    if(optionI == selectedOptionIndex) {
                        UpdateSelectedText(handle.Result);
                    }
                };
            }
            if(!option.sprite.IsEmpty) {
                AsyncOperationHandle<Sprite> localizedSpriteHandle = option.sprite.LoadAssetAsync();
                localizedSpriteHandle.Completed += (handle) => {
                    Dropdown.options[optionI].image = localizedSpriteHandle.Result;
                    if(optionI == selectedOptionIndex) {
                        UpdateSelectedSprite(localizedSpriteHandle.Result);
                    }
                };
            }
        }
    }

    private void UpdateSelectedOptionIndex(int index) {
        selectedOptionIndex = index;
    }

    private void UpdateSelectedText(string text) {
        if(Dropdown.captionText) {
            Dropdown.captionText.text = text;
        }
    }

    private void UpdateSelectedSprite(Sprite sprite) {
        if(Dropdown.captionImage) {
            Dropdown.captionImage.sprite = sprite;
        }
    }
}

[System.Serializable]
public class LocalizedDropdownOption {
    public LocalizedString text;
    public LocalizedSprite sprite;
}
