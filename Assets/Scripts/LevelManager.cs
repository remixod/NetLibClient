﻿using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static void Load(string name)
    {
        Loom.DispatchToMainThread(() => {
            GameObject go = new GameObject("LevelManager");
            LevelManager instance = go.AddComponent<LevelManager>();
            instance.StartCoroutine(instance.InnerLoad(name));
        });
    }

    IEnumerator InnerLoad(string name)
    {
        //load transition scene
        Object.DontDestroyOnLoad(this.gameObject);
        //Application.LoadLevel("Demo Loading Scene");

        //wait one frame (for rendering, etc.)
        yield return null;

        //load the target scene
        Application.LoadLevel(name);
        Destroy(this.gameObject);
    }
}
