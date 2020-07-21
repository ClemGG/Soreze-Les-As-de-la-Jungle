// IMPORTANT ! 
// To run the Javascript Sample you need to move the /Jigsaw/Standard Assets folder into the {root}/ of your project.,
// The reason for this is because the JigsawPuzzle class we are extending is a C# class and has to be pre-compiled.
//  This file was commented to avoid import package compiler errors.
// the DemoSlidingPuzzle Class is a sub class of SliderPuzzle that can be used to create an ingame puzzle
// The following base functions can be overridden :
//
// - PuzzleStart(); 						: is called when new puzzle is started
// - StartSlide(piece:GameObject); 		: is called when a 'loose' puzzle piece is selected (start drag)
// - EndSlide(piece:GameObject);		: is called when a 'loose' puzzle piece is released (stop drag)
// - PuzzleSolved(moves:int, time:float);	: is called when the puzzle has been solved
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

/*

public class JSDemoSlidingPuzzle extends SliderPuzzle
{
	// is true when the puzzle is solved
	public function get solved():boolean
	{
			return _solved;
	}
	
	// contains the move count if the puzzle is solved
	public function get moves():int
	{
			return _moves;
	}
	
	// contains the puzzle time if the puzzle is solved
	public function get time():float
	{
			return _time;
	}
	
	private var _solved:boolean = false;
	private var _moves:int = 0;
	private var _time:float = 0;
		
	// Update is called once per frame
	new function Update ():void {
        // call inherited JigsawPuzzle.Update();
        super.Update();
	}
	
	// PuzzleStart is called when a new puzzle is started
	override function PuzzleStart():void
	{
		_solved = false;
	}

    // ActivatePiece is called when one clicks - and hold mouse left button
    // on a (loose) puzzle piece.
    override function StartSlide(piece:GameObject):void
    {
    }

    // DeativatePiece is called when one releases left mouse button
    // and a puzzle piece was active
    protected function EndSlide(piece:GameObject):void
    {
    }

	// PiecePlace is called when a puzzle piece is fit on the correct spot 
    protected function PuzzleSolved(moves:int , time:float):void
    {
        _solved = true;
		_moves = moves;
		_time = time;
    }
	
}

*/