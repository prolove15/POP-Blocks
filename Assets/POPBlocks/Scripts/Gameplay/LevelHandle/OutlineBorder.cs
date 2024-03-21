// // ©2015 - 2023 Candy Smith
// // All rights reserved
// // Redistribution of this software is strictly not allowed.
// // Copy of this software can be obtained from unity asset store only.
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

using POPBlocks.Scripts.Items;
using UnityEngine;

namespace POPBlocks.Scripts.LevelHandle
{
    /// <summary>
    /// Outline around the field
    /// </summary>
    public class OutlineBorder
    {
        private int maxRows;
        private int maxCols;
        private Field _fieldBoard;

        public OutlineBorder(int maxRows, int maxCols, Field fieldBoard)
        {
            this.maxRows = maxRows;
            this.maxCols = maxCols;
            _fieldBoard = fieldBoard;
            GenerateOutline();
        }

        void GenerateOutline()
        {
            int row = 0;
            int col = 0;
            for (row = 0; row < maxRows; row++)
            {
                //down
                SetOutline(col, row, 0);
            }

            row = maxRows - 1;
            for (col = 0; col < maxCols; col++)
            {
                //right
                SetOutline(col, row, 90);
            }

            col = maxCols - 1;
            for (row = maxRows - 1; row >= 0; row--)
            {
                //up
                SetOutline(col, row, 180);
            }

            row = 0;
            for (col = maxCols - 1; col >= 0; col--)
            {
                //left
                SetOutline(col, row, 270);
            }

            col = 0;
            for (row = 1; row < maxRows - 1; row++)
            {
                for (col = 1; col < maxCols - 1; col++)
                {
                    //  if (GetSquare(col, row).type == BlocksTypes.None)
                    SetOutline(col, row, 0);
                }
            }
        }


        void SetOutline(int col, int row, float zRot)
        {
            Square square = _fieldBoard.GetSquare(col, row, true);
            if (!(square is null) && square.type != BlocksTypes.None)
            {
                if (row == 0 || col == 0 || col == maxCols - 1 || row == maxRows - 1)
                {
                    var outline = Object.Instantiate(_fieldBoard.outline3, square.transform);
                    // outline.transform.position += (Vector3) square.GetWorldPosition()+ Vector3.left * sideOffset;

                    // outline.transform.localRotation = Quaternion.Euler(0, 0, zRot);
                    // if (zRot == 0)
                    //     outline.transform.position += Vector3.zero + Vector3.left * sideOffset;
                    // if (zRot == 90)
                    //     outline.transform.position += Vector3.zero + Vector3.down * sideOffset;
                    // if (zRot == 180)
                    //     outline.transform.position += Vector3.zero + Vector3.right * sideOffset;
                    // if (zRot == 270)
                    //     outline.transform.position += Vector3.zero + Vector3.up * sideOffset;
                    // if (row == 0 && col == 0)
                    // {
                    //     //top left
                    //     outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                    //     outline.transform.localPosition = Vector3.zero + Vector3.left * cornerOffset + Vector3.up * cornerOffset;
                    // }
                    //
                    // if (row == 0 && col == maxCols - 1)
                    // {
                    //     //top right
                    //     outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    //     outline.transform.localPosition = Vector3.zero + Vector3.right * cornerOffset + Vector3.up * cornerOffset;
                    // }
                    //
                    // if (row == maxRows - 1 && col == 0)
                    // {
                    //     //bottom left
                    //     outline.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    //     outline.transform.localPosition = Vector3.zero + Vector3.left * cornerOffset + Vector3.down * cornerOffset;
                    // }
                    //
                    // if (row == maxRows - 1 && col == maxCols - 1)
                    // {
                    //     //bottom right
                    //     outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    //     outline.transform.localPosition = Vector3.zero + Vector3.right * cornerOffset + Vector3.down * cornerOffset;
                    // }

                }
                // else
                // {
                //     //top left
                //     var outline = Object.Instantiate(_fieldBoard.outline3, square.transform);
                //     if (_fieldBoard.GetSquare(col - 1, row - 1, true).type == BlocksTypes.None && _fieldBoard.GetSquare(col, row - 1, true).type == BlocksTypes.None &&
                //         _fieldBoard.GetSquare(col - 1, row, true).type == BlocksTypes.None)
                //     {
                //         outline.transform.localPosition = Vector3.zero + Vector3.left * cornerOffset + Vector3.up * cornerOffset;
                //         outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                //     }
                //
                //     //top right
                //     if (_fieldBoard.GetSquare(col + 1, row - 1, true).type == BlocksTypes.None && _fieldBoard.GetSquare(col, row - 1, true).type == BlocksTypes.None &&
                //         _fieldBoard.GetSquare(col + 1, row, true).type == BlocksTypes.None)
                //     {
                //         outline.transform.localPosition = Vector3.zero + Vector3.right * cornerOffset + Vector3.up * cornerOffset;
                //         outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                //     }
                //
                //     //bottom left
                //     if (_fieldBoard.GetSquare(col - 1, row + 1, true).type == BlocksTypes.None && _fieldBoard.GetSquare(col, row + 1, true).type == BlocksTypes.None &&
                //         _fieldBoard.GetSquare(col - 1, row, true).type == BlocksTypes.None)
                //     {
                //         outline.transform.localPosition = Vector3.zero + Vector3.left * cornerOffset + Vector3.down * cornerOffset;
                //         outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
                //     }
                //
                //     //bottom right
                //     if (_fieldBoard.GetSquare(col + 1, row + 1, true).type == BlocksTypes.None && _fieldBoard.GetSquare(col, row + 1, true).type == BlocksTypes.None &&
                //         _fieldBoard.GetSquare(col + 1, row, true).type == BlocksTypes.None)
                //     {
                //         outline.transform.localPosition = Vector3.zero + Vector3.right * cornerOffset + Vector3.down * cornerOffset;
                //         outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                //     }
                //
                //     outline.transform.position += (Vector3) square.GetWorldPosition();
                // }
            }
            // else
            // {
            //     bool corner = false;
            //     var outline = Object.Instantiate(_fieldBoard.outline2, square.transform);
            //     if (_fieldBoard.GetSquare(col - 1, row, true).type != BlocksTypes.None && _fieldBoard.GetSquare(col, row - 1, true).type != BlocksTypes.None)
            //     {
            //         outline.transform.localPosition = Vector3.zero;
            //         outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //         corner = true;
            //     }
            //
            //     if (_fieldBoard.GetSquare(col + 1, row, true).type != BlocksTypes.None && _fieldBoard.GetSquare(col, row + 1, true).type != BlocksTypes.None)
            //     {
            //         outline.transform.localPosition = Vector3.zero;
            //         outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
            //         corner = true;
            //     }
            //
            //     if (_fieldBoard.GetSquare(col + 1, row, true).type != BlocksTypes.None && _fieldBoard.GetSquare(col, row - 1, true).type != BlocksTypes.None)
            //     {
            //         outline.transform.localPosition = Vector3.zero;
            //         outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
            //         corner = true;
            //     }
            //
            //     if (_fieldBoard.GetSquare(col - 1, row, true).type != BlocksTypes.None && _fieldBoard.GetSquare(col, row + 1, true).type != BlocksTypes.None)
            //     {
            //         outline.transform.localPosition = Vector3.zero;
            //         outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
            //         corner = true;
            //     }
            //
            //
            //     if (!corner)
            //     {
            //         outline = Object.Instantiate(_fieldBoard.outline1, square.transform);
            //         if (_fieldBoard.GetSquare(col, row - 1, true).type != BlocksTypes.None)
            //         {
            //             outline.transform.localPosition = Vector3.zero + Vector3.up * 0.395f;
            //             outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
            //         }
            //
            //         if (_fieldBoard.GetSquare(col, row + 1, true).type != BlocksTypes.None)
            //         {
            //             outline.transform.localPosition = Vector3.zero + Vector3.down * 0.395f;
            //             outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
            //         }
            //
            //         if (_fieldBoard.GetSquare(col - 1, row, true).type != BlocksTypes.None)
            //         {
            //             outline.transform.localPosition = Vector3.zero + Vector3.left * 0.395f;
            //             outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //         }
            //
            //         if (_fieldBoard.GetSquare(col + 1, row, true).type != BlocksTypes.None)
            //         {
            //             outline.transform.localPosition = Vector3.zero + Vector3.right * 0.395f;
            //             outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //         }
            //     }
            // }
        }
    }
}