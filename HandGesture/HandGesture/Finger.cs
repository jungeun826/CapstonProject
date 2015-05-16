
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandGesture
{
    class Finger
    {
        [Flags]
        public enum FingerType
        {
            ForeFinger,
            IndexFinger,
            MiddleFinger,
            RingFinger,
            LittleFinger,
        }

        private int _fingerCnt = 0;
        public int FingerCnt { get { return _fingerCnt; } }

        private FingerType _fingerPos;
        public FingerType FingerPos { get { return _fingerPos; } }
    }
}
