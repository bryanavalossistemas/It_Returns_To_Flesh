using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement panelOptions;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;

        Button btnPlay = root.Q<Button>("btn-play");
        Button btnOptions = root.Q<Button>("btn-opt");
        Button btnQuit = root.Q<Button>("btn-quit");

        btnPlay.clicked += OnPlay;
        //btnOptions.clicked += () => panelOptions.RemoveFromClassList("hidden");
        btnQuit.clicked += OnQuit;

        //SetPanelOptions();

        async void SetPanelOptions()
        {
            panelOptions = root.Q<VisualElement>("panel-options");
            Button _btnClosePopup = root.Q<Button>("btn-close-popup");

            DropdownField _dropdownLanguage = root.Q<DropdownField>("dropdown-language");
            _dropdownLanguage.choices = Core.LocaleNames.ToList();
            _dropdownLanguage.value = Core.CurrentLocaleName;
            _dropdownLanguage.RegisterValueChangedCallback(evt => Core.ChangeLanguage(evt.newValue));

            // Sliders de volumen
            Slider _sliderMaster = root.Q<Slider>("vol_master");
            Slider _sliderMusic = root.Q<Slider>("vol_music");
            Slider _sliderSfx = root.Q<Slider>("vol_sfx");

            // Eventos de cambio de volumen
            _sliderMaster.RegisterValueChangedCallback(evt => AudioManager.SetMasterVolume(evt.newValue));
            _sliderMusic.RegisterValueChangedCallback(evt => AudioManager.SetMusicVolume(evt.newValue));
            _sliderSfx.RegisterValueChangedCallback(evt => AudioManager.SetSfxVolume(evt.newValue));

            _btnClosePopup.clicked += HidePopup;
            HidePopup(); // Ocultar al inicio

            await Task.Yield();
            // Inicializar sliders con los valores actuales del AudioManager
            _sliderMaster.value = AudioManager.masterVolume;
            _sliderMusic.value = AudioManager.musicVolume;
            _sliderSfx.value = AudioManager.sfxVolume;
        }
    }

    private void OnPlay() => Core.ChangeScene(2);
    private void OnQuit() => Core.QuitGame();
    private void ShowOptions() => panelOptions.style.display = DisplayStyle.Flex;
    private void HideOptions() => panelOptions.style.display = DisplayStyle.None;
    private void HidePopup() => panelOptions.AddToClassList("hidden");
}