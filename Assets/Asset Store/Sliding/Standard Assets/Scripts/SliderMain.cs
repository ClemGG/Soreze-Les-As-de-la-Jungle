using UnityEngine;
using System.Collections;

// The JigsawMain Class is used to access the (blender - fbx) imported piece prototypes and 
// to access materials for lines, sample image, scattered pieces and piece bases.
//
// You need to attach this script to an object in your scene. easiest way is to add the JigsawMain prefab
// to you scene.
//
public class SliderMain: MonoBehaviour {

    // ---------------------------------------------------------------------------------------------------------
    // public attributes
    // ---------------------------------------------------------------------------------------------------------
    public Material grid = null;		    //	material for grid
    public Material puzzle = null;		    //	material for puzzle
    public int layerMask = 31;				//	default layer mask for quick RayCasting
	
	// is true if this class has been initialized correctly
    public bool isValid
    {
        get
        {
            return _isValid;
        }
    }


    // ---------------------------------------------------------------------------------------------------------
    // private attributes
    // ---------------------------------------------------------------------------------------------------------
    private bool _isValid = false;


    // ---------------------------------------------------------------------------------------------------------
    // methods
    // ---------------------------------------------------------------------------------------------------------

	// Use this for initialization
	void Start () {
        _isValid = true;
    }
	
}
