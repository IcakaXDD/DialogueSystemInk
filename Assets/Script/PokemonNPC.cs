using UnityEngine;

public class PokemonNPC : MonoBehaviour
{
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color choiseColor1 = Color.red;
    [SerializeField] private Color choiseColor2= Color.green;
    [SerializeField] private Color choiseColor3 = Color.blue;

    public Material material;


    private void Update()
    {
        string pokemonName = ((Ink.Runtime.StringValue)DialogManager.GetInstance().GetVariableState("bliga_name")).value;

        switch(pokemonName)
        {
            case "":
                material.color = defaultColor;
                break;
            case "Ico":
                material.color = choiseColor1;
                break;
            case "Vesko":
                material.color = choiseColor2;
                break;
            case "Misho":
                material.color = choiseColor3;
                break;
            default:
                Debug.LogWarning("Pokemon name not hadled by switch statemnet: "+pokemonName);
                break;
        }
    }
}
