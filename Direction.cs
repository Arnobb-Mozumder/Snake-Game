﻿namespace Snakee
{
    public class Direction
    {
        public static readonly Direction Left = new Direction(0, -1);
        public static readonly Direction Right = new Direction(0, 1);
        public static readonly Direction Up = new Direction(-1, 0);
        public static readonly Direction Down = new Direction(1, 0);

        public int RowOffset { get; }
        public int ColOffset { get; }

        public Direction(int rowOffset, int colOffset)
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }

        public Direction Opposite()
        {
            return new Direction(-RowOffset, -ColOffset);
        }

        public override bool Equals(object? obj)
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColOffset == direction.ColOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        public static bool operator ==(Direction? left, Direction? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Direction? left, Direction? right)
        {
            return !(left == right);
        }
    }
}
