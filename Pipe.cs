using Godot;
using System;

public partial class Pipe : Node2D
{
    public void SetGap(float gap)
    {
        GetNode<Area2D>("PipeLower").Position = new Vector2(0, gap/2);
        GetNode<Area2D>("PipeUpper").Position = new Vector2(0, -gap/2);
    }
}
