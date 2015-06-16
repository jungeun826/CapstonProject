using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; //이벤트 발생위한 네임스페이스
using System.Windows.Forms; //현재 마우스위치를 위한 네임스페이스


namespace HandGesture
{
    static class ApiController
    {
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int KEYBOARD_DOWN = 0x00;
        public const int KEYBOARD_UP = 0x02;
        public static readonly int MOVE = 0x00000001;

        /// <summary>
        /// 마우스 이벤트를 발생 시킨다.
        /// dwData랑 dwExtraInfo는 뭔지 나도 모름요
        /// </summary>
        /// <param name="dwFlags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dwData"></param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll")]
        static public extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData = 0, int dwExtraInfo = 0);

        static public void mouse_event(uint dwFlags)
        {
            mouse_event(dwFlags, (uint)Control.MousePosition.X, (uint)Control.MousePosition.Y);
        }
        /// <summary>
        /// 마우스커서 위치 변경하는녀석임
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static public extern int SetCursorPos(int x, int y);

        static public void MoveCursorPos(int x, int y)
        {
            SetCursorPos(Control.MousePosition.X + x, Control.MousePosition.Y + y);
        }

        static public void MoveCursorPos(int x, int y, int ratio)
        {
            MoveCursorPos(x * ratio, y * ratio);
        }

        /// <summary>
        /// 키보드이벤트를 발생시킨다.
        /// </summary>
        /// <param name="vk">ex) Keys.Left</param>
        /// <param name="scan">스캔코드, 그냥 0써서 넘어가면됨</param>
        /// <param name="flags">0x00, 0x02 각각 누름과 뗌</param>
        /// <param name="extraInfo">일반키보드이므로 0 쓰세얌</param>
        [DllImport("user32.dll")]
        public static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo);

    }
}
