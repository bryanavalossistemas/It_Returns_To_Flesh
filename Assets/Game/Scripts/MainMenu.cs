using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using static BehaviourPlus;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset optionsUXML;
    private VisualElement panelOptions;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        TemplateContainer container = optionsUXML.Instantiate();
        panelOptions = container.Q<VisualElement>("canvas");
        root.Add(panelOptions);
        SetPanelOptions();
        HideOptions();

        Button btnPlay = root.Q<Button>("play");
        Button btnOptions = root.Q<Button>("options");
        Button btnQuit = root.Q<Button>("quit");

        btnPlay.clicked += OnPlay;
        btnOptions.clicked += ShowOptions;
        btnQuit.clicked += OnQuit;

        void SetPanelOptions()
        {
            Button _btnCloseOptions = root.Q<Button>("btn_close");

            DropdownField _dropdownLanguage = root.Q<DropdownField>("drop_languages");
            _dropdownLanguage.choices = Core.LocaleNames.ToList();
            _dropdownLanguage.value = Core.CurrentLocaleName;
            _dropdownLanguage.RegisterValueChangedCallback(evt => core.ChangeLanguage(evt.newValue));

            // Sliders de volumen
            SliderInt _sliderMaster = root.Q<SliderInt>("vol_master");
            SliderInt _sliderMusic = root.Q<SliderInt>("vol_bgm");
            SliderInt _sliderSfx = root.Q<SliderInt>("vol_sfx");

            // Eventos de cambio de volumen
            _sliderMaster.RegisterValueChangedCallback(evt => audioManager.SetMasterVolume(evt.newValue));
            _sliderMusic.RegisterValueChangedCallback(evt => audioManager.SetMusicVolume(evt.newValue));
            _sliderSfx.RegisterValueChangedCallback(evt => audioManager.SetSfxVolume(evt.newValue));

            _btnCloseOptions.clicked += HideOptions;

            // Inicializar sliders con los valores actuales del AudioManager
            _sliderMaster.value = audioManager.masterVolume;
            _sliderMusic.value = audioManager.musicVolume;
            _sliderSfx.value = audioManager.sfxVolume;
        }
    }

    private void OnPlay() => gameManager.StartLevel(0);
    private void OnQuit() => core.QuitGame();
    private void ShowOptions() => panelOptions.style.display = DisplayStyle.Flex;
    private void HideOptions() => panelOptions.style.display = DisplayStyle.None;
}