using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementChange
{
    private readonly Vector2Int origin;
    private readonly Vector2Int destination;

    public MovementChange(Vector2Int origin, Vector2Int destination) {
        this.origin = origin; 
        this.destination = destination;
    }

    private MovementChange()
    {
        this.origin = Vector2Int.one * int.MinValue;
        this.destination = Vector2Int.one * int.MinValue;
    }

    public static MovementChange Empty()
    {
        return new MovementChange();
    }

    public Vector2Int getOrigin() {
        return origin;
    }

    public Vector2Int getDestination() {
        return destination;
    }

    public bool Equals(MovementChange other)
    {
        if (other == null || other.GetType() != typeof(MovementChange))
        {
            return false;
        } else if (ReferenceEquals(other, this))
        {
            return true;
        } else
        {
            return other.origin == this.origin && other.destination == this.destination;
        }
    }

    public override string ToString()
    {
        return base.ToString() + string.Format(" ({0}, {1})", origin, destination);
    }
}
