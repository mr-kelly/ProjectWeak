using UnityEngine;
using System.Collections;
using Umeng;

public class CGame : MonoBehaviour {

	// Use this for initialization
	void Awake()
    {

        GA.StartWithAppKeyAndChannelId("541d93effd98c518d0031a4d", "App");
        
        CCosmosEngine.New(gameObject, new ICModule[] {
            
        }, null, AfterInitModules);
	}
	

    
    IEnumerator AfterInitModules()
    {
        CUIManager.Instance.OpenWindow<CUIHome>();
        yield break;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
