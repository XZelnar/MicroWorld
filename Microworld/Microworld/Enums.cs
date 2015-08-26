using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld
{
    public enum PortState
    {
        Input = 1,
        Output = 2,
        Both = 3
    }

    /// <summary>
    /// Uses same bit for same direction. I.E. DownLeft == Down | Left == LeftDown
    /// </summary>
    public enum Direction
    {
        None = 0,
        Left = 1,
        Up = 2,
        Right = 4,
        Down = 8,
        LeftUp = 3,
        UpLeft = 3,
        UpRight = 6,
        RightUp = 6,
        RightDown = 12,
        DownRight = 12,
        DownLeft = 9,
        LeftDown = 9
    }

    public enum DirectionStrict
    {
        Left = 1,
        Up = 2,
        Right = 4,
        Down = 8
    }

    public enum Side
    {
        None = 0,

        Left = 1,
        Up = 2,
        Right = 4,
        Down = 8,

        LeftUp = 3,
        UpLeft = 3,
        LeftRight = 5,
        RightLeft = 5,
        DownLeft = 9,
        LeftDown = 9,
        UpRight = 6,
        RightUp = 6,
        UpDown = 10,
        DownUp = 10,
        RightDown = 12,
        DownRight = 12,

        LeftUpRight = 7,
        LeftRightUp = 7,
        RightLeftUp = 7,
        RightUpLeft = 7,
        UpLeftRight = 7,
        UpRightLeft = 7,

        UpRightDown = 14,
        UpDownRight = 14,
        RightUpDown = 14,
        RightDownUp = 14,
        DownUpRight = 14,
        DownRightUp = 14,

        LeftRightDown = 13,
        LeftDownRight = 13,
        RightLeftDown = 13,
        RightDownLeft = 13,
        DownLeftRight = 13,
        DownRightLeft = 13,

        LeftUpDown = 11,
        LeftDownUp = 11,
        UpLeftDown = 11,
        UpDownLeft = 11,
        DownLeftUp = 11,
        DownUpLeft = 11,

        All = 15
    }

    public enum MagnetPole
    {
        None = 0,
        N = 1,
        S = 2
    }
}
