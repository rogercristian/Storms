using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNPC : MonoBehaviour
{
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color choiceOneColor = Color.red;
    [SerializeField] private Color choiceTwotColor = Color.green;
    [SerializeField] private Color choiceThreetColor = Color.blue;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    private void Update()
    {
        string choiceName = ((Ink.Runtime.StringValue)DialogueManager.GetInstance().GetVariableState("nome_cor")).value;

        switch (choiceName)
        {
            case "":
                spriteRenderer.color = defaultColor;
                break;
            case "Vermelho":
                spriteRenderer.color = choiceOneColor;
                break;
            case "Verde":
                spriteRenderer.color = choiceTwotColor;

                break;
            case "Azul":
                spriteRenderer.color = choiceThreetColor;
                break;
            default:
                Debug.LogWarning("no name by switch statement" + choiceName);
                break;
        }
    }

}
