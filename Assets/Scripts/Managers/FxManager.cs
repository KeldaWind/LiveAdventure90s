using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    public static FxManager Instance;

    public FxPoolParameters[] allGameFx = new FxPoolParameters[0];

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        SetUpDictionary();
    }

    Dictionary<string, FxObject> allFxDictionary = new Dictionary<string, FxObject>();
    
    public void SetUpDictionary()
    {
        allFxDictionary = new Dictionary<string, FxObject>();
        foreach (FxPoolParameters fxParams in allGameFx)
        {
            if (fxParams.fxObject && fxParams.fxTag != "" && !allFxDictionary.ContainsKey(fxParams.fxTag))
            {
                allFxDictionary.Add(fxParams.fxTag, fxParams.fxObject);
            }
        }
    }

    public void PlayFx(string fxTag, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        if (allFxDictionary.ContainsKey(fxTag))
        {
            FxObject fxObj = Instantiate(allFxDictionary[fxTag], position, rotation);
            fxObj.transform.localScale = scale;

            fxObj.PlayFx(); 
        }
    }
}

[System.Serializable]
public struct FxPoolParameters 
{
    public string fxTag;
    public FxObject fxObject;
}