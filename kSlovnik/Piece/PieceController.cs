using kSlovnik.Board;
using kSlovnik.Controls;
using kSlovnik.Generic;
using kSlovnik.Player;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Piece
{
    public static class PieceController
    {
        #region Manual
        public const int MoveIntervalMilliseconds = 1000 / Constants.FPS;
        private static DateTime LastMoveTime = DateTime.MinValue;
        
        private static bool Dragging = false;
        private static Point DragStart = Point.Empty;
        private static bool WasOnBoard = false;
        private static int PreviousBoardIndex = -1;

        public static void MakeMovable(this Control control)
        {
            control.MouseDown += OnMouseDown;
            control.MouseUp += OnMouseUp;
            control.MouseMove += OnMouseMove;
        }

        public static void OnMouseDown(object sender, MouseEventArgs e)
        {
            var control = sender as HandSlot;
            if (control == null) return;

            // Play click sound
            Resources.SoundController.Click.Play();

            // Get position on the board
            var boardSlot = BoardController.GetSlotAtPoint(control.GetCenterLocation());
            if (boardSlot != null)
            {
                if(boardSlot.PendingPiece != null)
                {
                    boardSlot.PendingPiece.CurrentBoardSlot = null;
                    boardSlot.PendingPiece = null;

                    WasOnBoard = true;
                    PreviousBoardIndex = (int)boardSlot.Tag;
                }
                Sidebar.SidebarController.RenderWords();
            }

            // On right click - return to hand
            if (e.Button == MouseButtons.Right)
            {
                control.ReturnToHand();
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                // Start dragging
                Dragging = true;
                DragStart = new Point(e.X, e.Y);
                control.Capture = true;
                control.BringToFront();
                control.IsInHand = false;
            }
        }

        public static void OnMouseMove(object sender, MouseEventArgs e)
        {
            var now = DateTime.Now;

            if ((now - LastMoveTime).TotalMilliseconds >= MoveIntervalMilliseconds)
            {
                LastMoveTime = now;
                var control = sender as Control;

                if (Dragging)
                {
                    control.Location = e.Location.Plus(control.Location).Minus(DragStart);
                }
            }
        }

        public static void OnMouseUp(object sender, MouseEventArgs e)
        {
            var control = sender as HandSlot;
            if (control == null) return;
            if (e.Button != MouseButtons.Left) return;

            // Stop dragging
            Dragging = false;
            control.Capture = false;
            // Play click sound
            Resources.SoundController.Click.Play();

            Slot targetSlot = BoardController.GetSlotAtPoint(control.GetCenterLocation());

            if (targetSlot == null)
            {
                targetSlot = HandController.GetSlotAtPoint(control.GetCenterLocation());
            }

            bool validTargetFound = false;
            if (targetSlot is BoardSlot boardSlot)
            {
                // Target is a board slot
                if (boardSlot.IsFilled == false)
                {
                    validTargetFound = true;

                    if(boardSlot.PendingPiece == null)
                    {
                        control.PlaceOnBoard(boardSlot);
                    }
                    else
                    {
                        if (WasOnBoard)
                        {
                            boardSlot.PendingPiece.PlaceOnBoard(BoardController.GetSlotAtIndex(PreviousBoardIndex));
                        }
                        else
                        {
                            boardSlot.PendingPiece.ReturnToHand();
                        }
                        control.PlaceOnBoard(boardSlot);
                    }
                }
            }
            else if (targetSlot is HandSlot handSlot)
            {
                if (handSlot.IsInHand)
                {
                    // Target is a hand slot in hand - swap positions
                    control.ReturnToHand();
                    HandController.SlidePieces(control, handSlot);
                }
                else
                {
                    HandController.SwapPieces(control, handSlot);
                }
            }

            if (validTargetFound == false)
            {
                control.ReturnToHand();
            }

            Sidebar.SidebarController.RenderWords();

            WasOnBoard = false;
            PreviousBoardIndex = -1;

    }
        #endregion

        #region Automated
        public static void MoveTile(byte tileIndex, byte boardIndex)
        {
            // Get current cursor position
            MouseOperations.MousePoint startPos = MouseOperations.GetCursorPosition();
            //userpb1.Top = 300;
            //userpb1.Left = 800;

            Form form = Program.MainView;
            int formX = form.DesktopLocation.X;
            int formY = form.DesktopLocation.Y;
            // Tile 1
            MouseOperations.SetCursorPosition(formX + 810 + tileIndex * 49, formY + 350);
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            MouseOperations.SetCursorPosition(formX + 30 + boardIndex % 15 * 50, formY + 80 + boardIndex / 15 * 50);
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            Application.DoEvents();

            MouseOperations.SetCursorPosition(startPos);
        }

        public static class MouseOperations
        {
            [Flags]
            public enum MouseEventFlags
            {
                LeftDown = 0x00000002,
                LeftUp = 0x00000004,
                MiddleDown = 0x00000020,
                MiddleUp = 0x00000040,
                Move = 0x00000001,
                Absolute = 0x00008000,
                RightDown = 0x00000008,
                RightUp = 0x00000010
            }

            [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool SetCursorPos(int X, int Y);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetCursorPos(out MousePoint lpMousePoint);

            [DllImport("user32.dll")]
            private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

            public static void SetCursorPosition(int X, int Y)
            {
                SetCursorPos(X, Y);
            }

            public static void SetCursorPosition(MousePoint point)
            {
                SetCursorPos(point.X, point.Y);
            }

            public static MousePoint GetCursorPosition()
            {
                MousePoint currentMousePoint;
                var gotPoint = GetCursorPos(out currentMousePoint);
                if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
                return currentMousePoint;
            }

            public static void MouseEvent(MouseEventFlags value)
            {
                MousePoint position = GetCursorPosition();

                mouse_event
                    ((int)value,
                     position.X,
                     position.Y,
                     0,
                     0)
                    ;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MousePoint
            {
                public int X;
                public int Y;

                public MousePoint(int x, int y)
                {
                    X = x;
                    Y = y;
                }
            }
        }
        #endregion
    }
}