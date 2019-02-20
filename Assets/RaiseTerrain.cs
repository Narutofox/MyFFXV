using UnityEngine;

public class RaiseTerrain : MonoBehaviour
{
    private Terrain _terrain;
    private int _hmWidth; // heightmap width
    private int _hmHeight; // heightmap height
    private bool _startRaising;

    private int _posXInTerrain; // position of the game object in terrain width (x axis)
    private int _posYInTerrain; // position of the game object in terrain height (y axis)

    readonly int size = 50; // the diameter of terrain portion that will raise under the game object
    public float DesiredHeight = 0; // the height we want that portion of terrain to be

    private float[,] _originalHeights;

    private void OnDestroy()
    {
       // this._terrain.terrainData.SetHeights(0, 0, this._originalHeights);
    }

    private void Start()
    {
        _terrain = Terrain.activeTerrain;
        _hmWidth = _terrain.terrainData.heightmapWidth;
        _hmHeight = _terrain.terrainData.heightmapHeight;
        _startRaising = false;
        this._originalHeights = this._terrain.terrainData.GetHeights(
            0, 0, this._terrain.terrainData.heightmapWidth, this._terrain.terrainData.heightmapHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if (_startRaising)
        {
            Raise();
        }
    }

    public void StartRaising(float desiredHeight = 0)
    {
        if (desiredHeight > 0)
        {
            DesiredHeight = desiredHeight;
           
        }

        _startRaising = true;
    }

    private void Raise()
    {
        
        // get the normalized position of this game object relative to the terrain
        Vector3 tempCoord = (transform.position - _terrain.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / _terrain.terrainData.size.x;
        coord.y = tempCoord.y / _terrain.terrainData.size.y;
        coord.z = tempCoord.z / _terrain.terrainData.size.z;

        // get the position of the terrain heightmap where this game object is
        _posXInTerrain = (int)(coord.x * _hmWidth);
        _posYInTerrain = (int)(coord.z * _hmHeight);

        // we set an offset so that all the raising terrain is under this game object
        int offset = size / 2;

        // get the heights of the terrain under this game object
        float[,] heights = _terrain.terrainData.GetHeights(_posXInTerrain - offset, _posYInTerrain - offset, size, size);

        // we set each sample of the terrain in the size to the desired height
        for (int i = size - 1; i >= 0; i--)
        {
            for (int j = 0; j < size; j++)
            {
                heights[i, j] = DesiredHeight;
            }
        }

        // go raising the terrain slowly
        DesiredHeight += Time.deltaTime;

        // set the new height
        _terrain.terrainData.SetHeights(_posXInTerrain - offset, _posYInTerrain - offset, heights);

        if (_terrain.terrainData.GetHeight(_posXInTerrain - offset, _posYInTerrain - offset) >= DesiredHeight)
        {
            _startRaising = false;
        }
    }
}
