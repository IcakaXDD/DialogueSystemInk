using UnityEngine;
using Ink.Runtime;
using UnityEngine.SceneManagement;

public class InkExternalFunctions 
{
    public void Bind(Story story,Animator emoteAnimator)
    {
        story.BindExternalFunction("playEmote", (string emoteName) => PlayEmote(emoteName,emoteAnimator));
        //story.BindExternalFunction("enable_controls",(string target) => EnableControls(target));
        //story.BindExternalFunction("disable_controls", (string target) => DisableControls(target));
        
    }
    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("playEmote");
    }

    public void PlayEmote(string emoteName,Animator emoteAnimator)
    {
        if (emoteAnimator != null)
        {
            emoteAnimator.Play(emoteName);
        }
        else
        {
            Debug.LogWarning("Tried to play emote, but emote animator was not initialized when entering dialogue mode.");
        }
    }

    public static void EnableControls(string target)
    {
        var togglePlayerControls = FindComponentOnObject<TogglePlayerControls>(target);
        if (togglePlayerControls == null) return;
        togglePlayerControls.EnableControls();
    }

    public static void DisableControls(string target)
    {
        var togglePlayerControls = FindComponentOnObject<TogglePlayerControls>(target);
        if (togglePlayerControls == null) return;
        togglePlayerControls.DisableControls();
    }

    private static T FindComponentOnObject<T>(string name) where T : Component
    {
        GameObject gameObject = FindGameObject(name);
        if (gameObject != null)
        {
            return gameObject.GetComponent<T>();
        }
        Debug.LogError(string.Format("GameObject {0} has no component of the type {1}", name, typeof(T).Name));
        return null;
    }
    private static GameObject FindGameObject(string name)
    {
        GameObject result = GameObject.Find(name);
        if (result == null)
        {
            Debug.LogError(string.Format("GameObject {0} can't be found", name));
        }
        return result;
    }
}
