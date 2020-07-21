using UnityEngine;
using System.Collections;

// the DemoSlidingPuzzle Class is a sub class of SliderPuzzle that can be used to create an ingame puzzle
// The following base functions can be overridden :
//
// - PuzzleStart(); 						: is called when new puzzle is started
// - StartSlide(GameObject piece); 		: is called when a 'loose' puzzle piece is selected (start drag)
// - EndSlide(GameObject piece);		: is called when a 'loose' puzzle piece is released (stop drag)
// - PuzzleSolved(int moves, float time);	: is called when the puzzle has been solved
// 
// To Create an Ingame Puzzle
//
// 1. Add The SliderMain prefab to the current scene
// 2. Create a puzzle cube primitive - the puzzle will be place on the 'forward' side of this cube
// 3. Set the right dimensions ( width/height/Thickness = scale x/y/z) of your puzzle
// 4. Add your 'custom' SlidingPuzzle subclass to your puzzle 'cube' game object
// 5. adjust the settings of your puzzle
//		-	image 			: 	will contain the jigsaw projected picture
// 		-	size			:	how many pieces will this puzzle have (x,y)
//		-	showGrid		:   display 'helper' puzzle matrix 
//
public class DemoSlidingPuzzle : SliderPuzzle
{


    // is true when the puzzle is solved
    public bool solved
	{
		get
		{
			return _solved;
		}
	}
	
	// contains the move count if the puzzle is solved
	public int moves
	{
		get
		{
			return _moves;
		}
	}
	
	// contains the puzzle time if the puzzle is solved
	public float time
	{
		get
		{
			return _time;
		}
	}
	
	bool _solved = false;
	int _moves = 0;
	float _time = 0.0f;


	
	// Update is called once per frame
	new void Update () {
        // call inherited JigsawPuzzle.Update();
        base.Update();
	}
	
	// PuzzleStart is called when a new puzzle is started
	protected override void PuzzleStart()
	{
		_solved = false;
	}

    // ActivatePiece is called when one clicks - and hold mouse left button
    // on a (loose) puzzle piece.
    protected override void StartSlide(GameObject piece)
    {
    }

    // DeativatePiece is called when one releases left mouse button
    // and a puzzle piece was active
    protected override void EndSlide(GameObject piece)
    {
    }

    // PiecePlace is called when a puzzle piece is fit on the correct spot 
    public override void PuzzleSolved(int moves, float time)
    {
        _solved = true;
        _moves = moves;
        _time = time;

        epreuve.DisplayVictory(_moves, _time);

    }
    
    // PiecePlace is called when a puzzle piece is fit on the correct spot 
    public void PuzzleSolved()
    {
        _solved = true;
        _moves = puzzleMoves;
        _time = (System.Environment.TickCount - puzzleTicks) / 1000;

        epreuve.DisplayVictory(_moves, _time);

    }

}
