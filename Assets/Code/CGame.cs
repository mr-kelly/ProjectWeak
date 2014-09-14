using UnityEngine;
using System.Collections;

public class CGame : MonoBehaviour {

	// Use this for initialization
	void Awake()
    {
        CCosmosEngine.New(gameObject, new ICModule[] {
            
        }, null, AfterInit);
	}
	

    IEnumerator AfterInit()
    {
        CUIManager.Instance.OpenWindow<CUIHome>();
        yield break;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
