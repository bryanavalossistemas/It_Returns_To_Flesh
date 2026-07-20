using UnityEngine;
using UnityEngine.UI;
using static BehaviourPlus;

public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    
    void Start()
    {
    
    }
    void Update()
    {
        rellenoBarraVida.fillAmount = (float)gameManager.HP / gameManager.MaxHP;
    }
}
