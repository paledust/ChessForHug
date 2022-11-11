/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public bool Moved = false;
    public bool Promotion = false;
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
    //Normal Pawn Movement before reaching bottom line
        if(!Promotion){
            List<Vector2Int> locations = new List<Vector2Int>();

            int forwardDirection = ChessManager.Instance.currentPlayer.forward;

            Vector2Int forward = new Vector2Int(gridPoint.x, gridPoint.y + forwardDirection);
            if(ChessManager.Instance.PieceAtGrid(forward) == false){
                locations.Add(forward);
            }

            if(!Moved){
                Vector2Int forward2 = new Vector2Int(gridPoint.x, gridPoint.y + forwardDirection * 2);
                if(ChessManager.Instance.PieceAtGrid(forward2) == false){
                    locations.Add(forward2);
                }
            }

            Vector2Int forwardRight = new Vector2Int(gridPoint.x + 1, gridPoint.y + forwardDirection);
            if(ChessManager.Instance.PieceAtGrid(forwardRight)){
                locations.Add(forwardRight);
            }

            Vector2Int forwardLeft = new Vector2Int(gridPoint.x - 1, gridPoint.y + forwardDirection);
            if(ChessManager.Instance.PieceAtGrid(forwardLeft)){
                locations.Add(forwardLeft);
            }
            return locations;
        }
    //Queen movement After reaching bottom line
        else{
            List<Vector2Int> locations = new List<Vector2Int>();
            List<Vector2Int> directions = new List<Vector2Int>(BishopDirections);
            directions.AddRange(RookDirections);

            foreach(Vector2Int dir in directions){
                for(int i = 1; i<8;i++){
                    Vector2Int nextGridPoint = new Vector2Int(gridPoint.x + i * dir.x, gridPoint.y + i * dir.y);
                    locations.Add(nextGridPoint);
                    if(ChessManager.Instance.PieceAtGrid(nextGridPoint)){
                        break;
                    }
                }
            }
            
            return locations;
        }
    }
}
