using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiiInputMapper.HIDEmulators
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using WindowsInput.Events;
    using static WindowsInput.Native.SystemMetrics;

    public class Mouse
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            OnVirtualDesktop = 0x00004000,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        bool leftState = false;
        bool rightState = false;
        bool middleState = false;
        bool scrollUpState = false;
        bool scrollDownState = false;
        Rectangle virtualScreen = SystemInformation.VirtualScreen;

        public void Stop()
        {
            MouseLeft(false);
            MouseRight(false);
            MouseMiddle(false);
        }

        public void MoveRel(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void MoveRel(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public void MoveAbs(int x, int y)
        {

            int mic_x = (int)System.Math.Round(x * 65536.0 / virtualScreen.Width);
            // Mickey Y coordinate
            int mic_y = (int)System.Math.Round(y * 65536.0 / virtualScreen.Height);

            MouseEvent(MouseEventFlags.Absolute | MouseEventFlags.Move | MouseEventFlags.OnVirtualDesktop, mic_x, mic_y);
        }

        public MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public void MouseLeftClick()
        {
            MouseEvent(MouseEventFlags.LeftDown | MouseEventFlags.LeftUp);
        }

        public void MouseRightClick()
        {
            MouseEvent(MouseEventFlags.LeftDown | MouseEventFlags.LeftUp);
        }
        public void MouseMiddleClick()
        {
            MouseEvent(MouseEventFlags.LeftDown | MouseEventFlags.LeftUp);
        }

        public void MouseLeft(bool state)
        {
            if (state && !leftState)
                MouseEvent(MouseEventFlags.LeftDown);
            if (!state && leftState)
                MouseEvent(MouseEventFlags.LeftUp);
            leftState = state;
        }

        public void MouseRight(bool state)
        {
            if (state && !rightState)
                MouseEvent(MouseEventFlags.RightDown);
            if (!state && rightState)
                MouseEvent(MouseEventFlags.RightUp);
            rightState = state;
        }
        public void MouseMiddle(bool state)
        {
            if (state && !middleState)
                MouseEvent(MouseEventFlags.MiddleUp);
            if (!state && middleState)
                MouseEvent(MouseEventFlags.MiddleUp);
            middleState = state;
        }


        public void MouseScrollUp(bool state)
        {
            //if (state && !scrollUpState)
            //    MouseEvent(MouseEventFlags.);
            //if (!state && scrollUpState)
            //    MouseEvent(MouseEventFlags.RightUp);
            //rightState = state;
        }

        public void MouseEvent(MouseEventFlags value)
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

        public void MouseEvent(MouseEventFlags value, MousePoint position)
        {
            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }
        public void MouseEvent(MouseEventFlags value, int x, int y)
        {
            mouse_event
                ((int)value,
                 x,
                 y,
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
            override public string ToString()
            {
                return $"X: {X}, Y: {Y}";
            }
        }
    }
}
