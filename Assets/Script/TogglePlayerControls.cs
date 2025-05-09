using UnityEngine;
using StarterAssets;

public class TogglePlayerControls : MonoBehaviour
{
    public void SetInputState(bool enabled)
    {
        GetComponent<CharacterController>().enabled = enabled;
        GetComponent<ThirdPersonController>().enabled = enabled;
        GetComponent<Animator>().enabled = enabled;
    }

    public void EnableControls()
    {
        SetInputState(true);
    }

    public void DisableControls()
    {
        SetInputState(false);
    }
}
