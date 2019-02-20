using Gamekit3D;
using UnityEngine;

public class RaiseTerrainTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    //void OnTriggerEnter(Collider other)
    //{
    //    RaiseTerrain raiseTerrain = other.gameObject.GetComponent<RaiseTerrain>();

    //    if (raiseTerrain != null)
    //    {
    //        //raiseTerrain.StartRaising(5);
    //        raiseTerrain.gameObject.GetComponent<BoxCollider>().isTrigger = false;
    //    }
    //}

    void OnTriggerExit(Collider other)
    {
        RaiseTerrain raiseTerrain = other.gameObject.GetComponent<RaiseTerrain>();

        if (raiseTerrain != null)
        {
            raiseTerrain.gameObject.GetComponent<BoxCollider>().isTrigger = false;
        }
    }
}
