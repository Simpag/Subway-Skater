using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance { set; get; }

    //Level spawning

    //Pooling
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longblocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();
    public List<Piece> pieces = new List<Piece>(); //All of the pieces in the pool

    public Piece GetPiece(PieceType _pieceType, int _visualIndex)
    {
        Piece p = pieces.Find(x => x.type == _pieceType && x.visualIndex == _visualIndex && !x.gameObject.activeSelf);

        if (p == null)
        {
            //If all gameobjects are in use, spawn another one
            GameObject go = null;
            switch (_pieceType)
            {
                case PieceType.ramp:
                    go = ramps[_visualIndex].gameObject;
                    break;

                case PieceType.longblock:
                    go = longblocks[_visualIndex].gameObject;
                    break;

                case PieceType.jump:
                    go = jumps[_visualIndex].gameObject;
                    break;

                case PieceType.slide:
                    go = slides[_visualIndex].gameObject;
                    break;
            }

            go = Instantiate(go);
            p = go.GetComponent<Piece>();
            pieces.Add(p);
        }
        
        return p;
    }
}
