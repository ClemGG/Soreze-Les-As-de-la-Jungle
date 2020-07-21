using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// the SliderPuzzle Class is the base class that can be used to create an in-game slider puzzle
//
// the normal procedure would be to create a custom SliderPuzzle subclass
// and override the following base functions :
//
// - PuzzleStart(); 						: is called when new puzzle is started
// - StartSlide(GameObject piece); 		    : is called when a piece starts to slide
// - EndSlide(GameObject piece);		    : is called when a piece ends sliding
// - PuzzleSolved(int moves, float time);	: is called when the puzzle has been solved
// 
// (see DemoSliderPuzzle class included with the product Demo)
//
public class SliderPuzzle : MonoBehaviour {

    // ---------------------------------------------------------------------------------------------------------
    // public (published) attributes - these can be set after adding the 
    // script to a container (like a cube primitive)
    // ---------------------------------------------------------------------------------------------------------

    public enum spot { TopLeft, TopRight, BottomLeft, BottomRight };

    public Texture image = null;                // will contain the jigsaw projected picture
    public Vector2 size = new Vector2(5,5);     // how many pieces will this puzzle have (x,y)
    public spot emptySpot = spot.BottomRight;   // location of the 'empty' spot
    public bool showGrid = true;                // display 'helper' puzzle matrix 
    public float spacing = 0.05f;               // how much room between pieces ( 0.01f - 0.05f is normaly a good value )
    public float slideSpeed = 0.75f;            // how fast do we slide in seconds

	
    // ---------------------------------------------------------------------------------------------------------
    // protected attributes (accessable in 'custom' subclass)
    // ---------------------------------------------------------------------------------------------------------

    // number of pieces of current puzzle
    protected int pieceCount                    
    {
        get
        {
            return (int)(size.x * size.y);
        }
    }

    protected Vector2 emptySpotVector
    {
        get
        {
            switch( emptySpot )
            {
                case spot.TopLeft:
                    return new Vector2(1,1);
                case spot.TopRight:
                    return new Vector2(size.x,1);
                case spot.BottomLeft:
                    return new Vector2(1,size.y);
                case spot.BottomRight:
                    return new Vector2(size.x,size.y);
            }
            return Vector2.zero;
        }
    }

	
	// ---------------------------------------------------------------------------------------------------------
    // private attributes
    // ---------------------------------------------------------------------------------------------------------

    private SliderMain main = null;	
    private bool checkedOnce = false;			
    private int puzzleMode = 0;					
    private GameObject grid = null;		
    private GameObject piecesContainer = null;	
	private GameObject pieceCache = null;
    protected int puzzleMoves = 0;
    protected int puzzleTicks = 0;
    private bool restarting = false;
    
    private GameObject slidingPiece = null;
    private bool sliding = false;

    private Vector2 checkSize;
    private Vector2 checkContainerSize;
    private float checkSpacing;
    private spot checkSpot;

    private Hashtable piecesLookup = new Hashtable();
    public Hashtable piecePositions = new Hashtable();
    public Hashtable pieceCorrectPositions = new Hashtable();

    private ArrayList pieces = new ArrayList();
    private Vector2 empty;
    private Vector2 slideFrom;
    private Vector3 v3From;
    private Vector3 v3To;
    private float slidingProgress = 0.0f;
    private GameObject emptyPiece;


    // ---------------------------------------------------------------------------------------------------------
    // virtual methods
    // ---------------------------------------------------------------------------------------------------------
	
	// This will be called when a new puzzle has been started
	// you need to override this method when you create your 'custom' subclass
	protected virtual void PuzzleStart()
    {
    }
		
	// This will be called when a 'loose' puzzle piece has been selected (start drag)
	// you need to override this method when you create your 'custom' subclass
    protected virtual void StartSlide(GameObject piece)
    {
    }
	
		
	// This will be called when a puzzle piece has been released (stop drag)
	// you need to override this method when you create your 'custom' subclass
    protected virtual void EndSlide(GameObject piece)
    {
    }
			
		
	// This will be called when the puzzle has been solved
	// you need to override this method when you create your 'custom' subclass
    public virtual void PuzzleSolved(int moves, float time)
    {
    }








    protected EpreuveTaquin epreuve;




    // ---------------------------------------------------------------------------------------------------------
    // methods
    // ---------------------------------------------------------------------------------------------------------

    // restart the current puzzle
    public void Restart()
    {
		// set indicator that puzzle has to be restarted - this is picked up in the Update() - process
        restarting = true;
    }

    // Use this for initialization
    void Start()
    {

        epreuve = (EpreuveTaquin)Epreuve.instance;
    }

    // Update is called once per frame
    protected void Update () {

        if (epreuve.EpreuveFinished)
            return;

        if (main == null)
        {
            // main puzzle initialization
            if (!checkedOnce)
            {
                // check ONCE if JigsawMain Script is found on JisawMain Prefab
                GameObject go = GameObject.Find("SliderMain");
                if (go == null)
                {
                    Debug.LogError("SliderMain (prefab) GameObject not added to scene!");
                    checkedOnce = true;
                    return;
                }
				// get JigsawMain class for piece prototype access
                main = go.GetComponent("SliderMain") as SliderMain;
                // check if main is initialized correctly so isValid should be true
                if (main != null)
                    if (!main.isValid)
                    {
                        Debug.LogError("SliderMain (prefab) GameObject is not valid!");
                        main = null;
                        checkedOnce = true;
                        return;
                    }

                // initialization of this puzzle
				// create horizontal and vertical lines
                SetGrid();
				// create pieces of this puzzle
                SetPieces(false);
            }
            else
                // puzzle system is invalid so return;
                return;
        }
        else
        {
			// JigsawMain was found and is valid so we can go on with our puzzle	
            // Lets first check if base puzzle settings have been changed like size or the top-left piece so we have to force a restart
            if (!Vector2.Equals(size, checkSize) || checkSpacing != spacing || restarting || checkSpot!=emptySpot)
            {
                // base puzzle settings have been changed so reset lines, sample image and pieces.
                SetGrid();
                SetPieces(true);
                // restart puzzle - so puzzleMode to 0
                puzzleMode = 0;
            }

            // check if lines have to be shown/hidden
			if (grid!=null && grid.activeSelf != showGrid) grid.SetActive(showGrid);

			// Puzzle control
            switch (puzzleMode)
            {
                case 0:     // puzzle initialization
                    if (pieceCount == 0) return;				
					// we have pieces so scatter them around
                   	ScramblePuzzle();
							
                    // starting to puzzle so reset move count and puzzleTime
                    puzzleMoves = 0;
                    puzzleTicks = System.Environment.TickCount;
                    restarting = false;
					// call overriden PuzzleStart function
					PuzzleStart();
					// Puzzle control to next step
					puzzleMode++;
                    break;
				
                case 1:     // we are puzzling
                    if (!sliding)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), Vector3.Distance(Camera.main.transform.position, transform.position) * 2, 1 << main.layerMask);
                            if (hits.Length > 0)
                            {
                                GameObject piece = hits[0].collider.gameObject;
                                Vector2 pos = (Vector2)piecePositions[piece.name];
                                if (((empty.x == pos.x - 1 || empty.x == pos.x + 1) && empty.y == pos.y) ||
                                    ((empty.y == pos.y - 1 || empty.y == pos.y + 1) && empty.x == pos.x))
                                {
                                    // set where we are sliding from
                                    slideFrom = pos;
                                    slidingPiece = piece;
                                    v3From = PuzzlePosition(slideFrom);
                                    v3To = PuzzlePosition(empty);
                                    // start sliding
                                    StartSlide(slidingPiece);
                                    sliding = true;
                                    slidingProgress = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        // add elapsed time to sliding progress
                        slidingProgress += Time.deltaTime/slideSpeed;
                        if (slidingProgress > 1) slidingProgress = 1;
                        // move sliding piece to right position
                        slidingPiece.transform.position = Vector3.Lerp(v3From, v3To, slidingProgress);
                        if (slidingProgress == 1)
                        {
                            // end sliding
                            EndSlide(slidingPiece);
                            // new empty spot is where we came from
                            piecePositions[slidingPiece.name] = empty;
                            empty = slideFrom;
                            sliding = false;

                            puzzleMoves++;

                            bool solved = true;
                            if (Vector2.Equals(piecePositions[slidingPiece.name],pieceCorrectPositions[slidingPiece.name]))
                            {
                                for (int p=0; p<pieces.Count; p++)
                                {
                                    GameObject piece = pieces[p] as GameObject;
                                    if (piece.activeSelf && piece != slidingPiece)
                                    {
                                        if (!Vector2.Equals(piecePositions[piece.name],pieceCorrectPositions[piece.name]))
                                        {
                                            solved = false;
                                            break;
                                        }
                                    }
                                }

                                if (solved)
                                {
                                    // show empty spot piece
                                    emptyPiece.transform.position = PuzzlePosition(empty);
									emptyPiece.SetActive(true);
                                    // puzzle is solved so call overridden PuzzleSolved function
                                    PuzzleSolved(puzzleMoves, (System.Environment.TickCount - puzzleTicks) / 1000);
									puzzleMode++;
                                }

                            }

                        }
                    }

                    break;
                case 2:     // puzzle is Done - this a kind of sleep state.
                    break;                        
            }
        }	
	}


	// create line matrix to display 
    protected void SetGrid()
    {

        if (grid != null)
        {
			grid.SetActive(false);
            Destroy(grid);
        }

        grid = GameObject.CreatePrimitive(PrimitiveType.Cube);
        grid.name = "grid";
        // set the right scale (z = very thin) rotation and position so it will cover the puzzl
        grid.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0.0001F);
        grid.transform.rotation = transform.rotation;
        // add grid to puzzle
        grid.transform.parent = gameObject.transform;
        // set 'transparent' material to grid
        grid.GetComponent<Renderer>().material = main.grid;
        // move this 'thin' cube so that it floats just above the puzzle
        grid.transform.position = transform.position + transform.forward * ((transform.localScale.z / 2) + 0.005F);
        // scale the texture in relation to specified size
        grid.GetComponent<Renderer>().material.mainTextureScale = new Vector2(size.x, size.y);
        // set the right offset in relation to the specified size and the specified topLeftPiece
		grid.SetActive(false);
        
        checkSize = size;
        checkSpacing = spacing;
        checkSpot = emptySpot;
    }
	   
	// scatter the pieces and place them randomly around the puzzle
    private void ScramblePuzzle()
    {
        Vector2 lastpos = Vector2.zero;
        // we are gonna simulate lots of moves to scramble the puzzle
        for (int p = 0; p < pieceCount * 10; p++)
        {
            Vector2 pos = Vector2.zero;
            do
            {
                // determine randomly which side we are gonne slide
                int ps = (int)Mathf.Floor(Random.value * 4);
                switch (ps)
                {
                    case 0: // up
                        pos.y = empty.y - 1;
                        if (pos.y == 0) pos.y = empty.y + 1;
                        pos.x = empty.x;
                        break;
                    case 1: // right
                        pos.x = empty.x + 1;
                        if (pos.x == size.x + 1) pos.x = empty.x - 1;
                        pos.y = empty.y;
                        break;
                    case 2: // down
                        pos.y = empty.y + 1;
                        if (pos.y == size.y + 1) pos.y = empty.y - 1;
                        pos.x = empty.x;
                        break;
                    case 3: // left
                        pos.x = empty.x - 1;
                        if (pos.x == 0) pos.x = empty.x + 1;
                        pos.y = empty.y;
                        break;
                }

            } while (Vector2.Equals(pos, lastpos)); // not allowed to slide back previous move

            // perform swap
            // get objects
            GameObject piece = piecesLookup[pos.x + "-" + pos.y] as GameObject;
            emptyPiece = piecesLookup[empty.x + "-" + empty.y] as GameObject;
            // set new position of puzzle piece
            piece.transform.position = PuzzlePosition(empty);
            piecePositions[piece.name] = empty;
            // swap lookup table
            piecesLookup[empty.x + "-" + empty.y] = piece;
            piecesLookup[pos.x + "-" + pos.y] = emptyPiece;
            // swap vectors
            lastpos = empty;
            empty = pos;
        }
    }

    // create piece containers
    private void CreateContainers()
    {
        if (piecesContainer != null) GameObject.Destroy(piecesContainer);
		if (pieceCache != null) GameObject.Destroy(pieceCache);
		
		// piecesContainer will hold all 'loose' scattered pieces
        piecesContainer = new GameObject("piecesContainer");
        piecesContainer.transform.parent = gameObject.transform;
        piecesContainer.transform.rotation = transform.rotation;
        piecesContainer.transform.localScale = transform.localScale;
        piecesContainer.transform.position = transform.position;
		
		// pieceCache will hold all pieces that were created but no longer
		// are used on current puzzle - but can re-use after resize or restart
		pieceCache = new GameObject("pieceCache");
        pieceCache.transform.parent = gameObject.transform;
        pieceCache.transform.rotation = transform.rotation;
        pieceCache.transform.localScale = transform.localScale;
        pieceCache.transform.position = transform.position;
    }

    // calculates the right puzzle piece position of a matrix position
    private Vector3 PuzzlePosition(Vector2 pos)
    {
        float dX = transform.localScale.x / size.x;
        float dY = transform.localScale.y / size.y;

        Vector3 tl = transform.position +
            ((transform.localScale.x / 2) * transform.right) +
            ((transform.localScale.y / 2) * transform.up) +
            ((transform.localScale.z / 2) * transform.forward);

        // move this 'thin' cube so that it floats just above the puzzle on the right spot
        return tl +
            ((((pos.x - 1) * dX) + (dX / 2)) * transform.right * -1) +
            ((((pos.y - 1) * dY) + (dY / 2)) * transform.up * -1);

    }
	
	
	// initialize specific piece with right scale, position and texture (scale/offset)
    private void InitPiece(GameObject puzzlePiece, Vector2 pos)
    {

        // set the right scale and  rotation and position
        puzzlePiece.transform.parent = null;
        puzzlePiece.transform.localScale = new Vector3(transform.localScale.x / (size.x / (1 - spacing)), transform.localScale.y / (size.y / (1 - spacing)), transform.localScale.z / 10);
        puzzlePiece.transform.rotation = transform.rotation;        // add grid to puzzle
        puzzlePiece.transform.parent = piecesContainer.transform;
        puzzlePiece.transform.position = PuzzlePosition(pos);

        // set material to puzzle
        puzzlePiece.GetComponent<Renderer>().material = main.puzzle;

        // scale the texture in relation to specified size
        // base texture calculation variables
        float tsx = 1 / (size.x / (1 - spacing));
        float dsx = (size.x / (1 - spacing)) / size.x;
        float itsx = tsx * (1 - (1 - spacing));
        float tsy = 1 / (size.y / (1 - spacing));
        float dsy = (size.y / (1 - spacing)) / size.y;
        float itsy = tsy * (1 - (1 - spacing));

        // set the right shrunken piece texture scale
        puzzlePiece.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tsx, tsy);
        // set the right offset in relation to the specified size and piece shrink factor
        puzzlePiece.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(tsx * ((pos.x - 1) * dsx) + (itsx/2), (tsy * (pos.y * dsy) * -1) + (itsy/2) ); 
    }
	
	// create a new piece from a specific prototype on a specific position with a specific piece type
    private GameObject CreateNewPiece()
    {
        GameObject puzzlePiece = GameObject.CreatePrimitive(PrimitiveType.Cube);        
        // add to specific layer for fast future RayCasting
        puzzlePiece.layer = main.layerMask;
        return puzzlePiece;
    }

	// Create or set (initialize) all pieces of the current puzzle
    private void SetPieces(bool recreate)
    {
        main.puzzle.mainTexture = image;

        // we have to have a valid size
        if (size.x <= 1 || size.y <= 1) return;

        if (!recreate)
			// only create piece containers the first time
            CreateContainers();
        else
        {
            // remove all active pieces from puzzle
            while (pieces.Count>0)
            {
                GameObject p = pieces[0] as GameObject;
				// create pieces array and positions
                pieces.Remove(p);
                piecePositions.Remove(p.name);
				p.SetActive(false);
				// add piece to cache for re-use
                p.transform.parent = pieceCache.transform;
            }
        }
				
		// loop vertical rows of the puzzle
        for (int y = 1; y <= size.y; y++)
        {
			// loop horizontal columns of the puzzle
            for (int x = 1; x <= size.x; x++)
            {
				// check if specific piece was created earlier
                GameObject puzzlePiece = piecesLookup[""+x+"-"+y] as GameObject;
                if (puzzlePiece!=null)
                {
					// a created piece has been found that can be used
                    puzzlePiece.name = "" + x + "-" + y;
                    // add puzzlePiece to this puzzle's pieces
                    InitPiece(puzzlePiece, new Vector2(x,y));
                    pieces.Add(puzzlePiece);
                }
                else
                {
                    // create a new puzzlePiece
                    puzzlePiece = CreateNewPiece();
                    puzzlePiece.name = "" + x + "-" + y;
                    if (puzzlePiece != null)
                    {
                        // add puzzlePiece to this puzzle's pieces and to lookup table
                        InitPiece(puzzlePiece, new Vector2(x, y));
                        piecesLookup.Add(puzzlePiece.name, puzzlePiece);
                        pieces.Add(puzzlePiece);
                    }
                }

                piecePositions[puzzlePiece.name] = new Vector2(x, y);
                pieceCorrectPositions[puzzlePiece.name] = new Vector2(x, y);
                if (!Vector2.Equals(new Vector2(x, y), emptySpotVector))
					puzzlePiece.SetActive(true);
                else
					puzzlePiece.SetActive(false);
            }
        }

        empty = emptySpotVector;

    }


    public List<GameObject> GetAllIncorrectPieces()
    {
        List<GameObject> pieces = new List<GameObject>();

        for (int i = 0; i < piecesContainer.transform.childCount; i++)
        {
            Transform t = piecesContainer.transform.GetChild(i);
            if (!Equals(piecePositions[t.name], pieceCorrectPositions[t.name]))
            {
                pieces.Add(t.gameObject);
            }
        }

        return pieces;
    }

}
