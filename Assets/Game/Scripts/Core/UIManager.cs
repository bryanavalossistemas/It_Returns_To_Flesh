using EditorAttributes;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BehaviourPlus;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Sprite lockedIcon;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] public Color _normalColor = Color.white, _dangerColor = Color.red,  _selectedColor = Color.green;
    [SerializeField, TypeFilter(typeof(IColor))] private Component[] skillButtons;
    private UnityEngine.UI.Image[] skillImages;
    private Sprite[] originalIcons;
    private IColor[] skills;
    private TextMeshProUGUI[] skillTexts;

    void Awake() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDestroy() => SceneManager.sceneLoaded -= SceneLoaded;

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (skills != null) ClearUI();
    }

   void Start() 
    {
        skills = skillButtons.Select(s => s.GetComponent<IColor>()).ToArray();
        skillImages = new UnityEngine.UI.Image[skillButtons.Length];
        originalIcons = new Sprite[skillButtons.Length];
        skillTexts = new TextMeshProUGUI[skillButtons.Length];
        for (int i = 0; i < skillButtons.Length; i++)
        {
            // Extraemos el componente Image nativo de cada botón
            skillImages[i] = skillButtons[i].GetComponent<UnityEngine.UI.Image>();
            
            // Guardamos el icono original que dejaste configurado en tu Prefab
            if (skillImages[i] != null)
            {
                originalIcons[i] = skillImages[i].sprite;
            }
            skillTexts[i] = skillButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }
        UpdateSkillsUI();
    }

    public void ClearUI()
    {
        // En lugar de pintarlos todos normales, evaluamos si están bloqueados o no
        UpdateSkillsUI();
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        //for (int i = 0; i < skills.Length; i++) skills[i].SetColor(GetColor(i));
        UpdateSkillsUI();
    }
    public void UpdateSkillsUI()
    {
        if (skills == null || gameManager.phaseSO == null) return;

        for (int i = 0; i < skills.Length; i++) 
    {
        // Revisamos si la habilidad está desbloqueada en el nivel actual
        bool isUnlocked = gameManager.phaseSO.unlockedSkills[i];
        if (skillImages[i] != null)
            {
                skillImages[i].sprite = isUnlocked ? originalIcons[i] : lockedIcon;
            }
        skills[i].SetColor(GetColor(i));
        
        // 2. Bloqueamos el componente Button de Unity para matar sus estados visuales automáticos
        Button unityButton = skillButtons[i].GetComponent<Button>();
        if (unityButton != null)
        {
            unityButton.interactable = isUnlocked;
        }
        
        if (skillTexts[i] != null)
            {
                skillTexts[i].gameObject.SetActive(isUnlocked);
            }
    }
    }
    private Color GetColor(int i)
    {
        if (gameManager.selectedSkill == i) return _selectedColor;
        return _normalColor;
    }
    public void ResetSkillHighlight()
    {
        gameManager.selectedSkill = -1;
        //if (FleshRipper.SelectedRipper != null) EvaluateSkills(FleshRipper.SelectedRipper.Health);
        //else ClearUI();
    }

    public void SelectButton(int pos)
    {
        // SOLUCIÓN AL BUG DEL CLICK: Verificamos si la habilidad está bloqueada.
        // Si está bloqueada (false) en el LevelSO actual, abortamos la función con "return" y no hace nada.
        if (gameManager.phaseSO != null && !gameManager.phaseSO.unlockedSkills[pos]) 
        {
            return; 
        }

        gameManager.selectedSkill = pos;
        UpdateSkillsUI(); // Actualizamos todos los colores para que se note la selección
        skills[pos].PaintSelected();
    }
    /*public void NextButtonGO()
    {
        currentButtonGO = currentButtonGO.FindSelectableOnDown();
        currentButtonGO.Select();
    }*/
}