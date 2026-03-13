using System.Runtime.InteropServices;

namespace Styx.WoWInternals
{
    public static class GameInput
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static int ToVirtualKey(InputMouseButton button)
        {
            switch (button)
            {
                case InputMouseButton.Left:    return 0x01; // VK_LBUTTON
                case InputMouseButton.Right:   return 0x02; // VK_RBUTTON
                case InputMouseButton.Middle:  return 0x04; // VK_MBUTTON
                case InputMouseButton.Button4: return 0x05; // VK_XBUTTON1
                case InputMouseButton.Button5: return 0x06; // VK_XBUTTON2
                default:                       return 0;
            }
        }

        public static bool IsMouseButtonDown(InputMouseButton button)
        {
            int vk = ToVirtualKey(button);
            if (vk == 0)
                return false;
            return (GetAsyncKeyState(vk) & unchecked((short)0x8000)) != 0;
        }
    }
}
