using Godot;

public partial class Bird : Area2D
{
    [Signal]
    public delegate void HitPipeEventHandler();

    [Signal]
    public delegate void GainPointEventHandler();

    const float GRAV = 15f;
    const float JUMPVEL = 750f;

    float velocity;

    AudioStreamPlayer jumpsound;

	public override void _Ready()
	{
        jumpsound = GetNode<AudioStreamPlayer>("JumpSound");
	}

	public override void _Process(double delta)
	{
        if (!Visible) return;
        if (Input.IsActionJustPressed("jump"))
        {
            // GD.Print("JUMP");
            velocity -= JUMPVEL * (float)delta;
            jumpsound.Play();
        }
        velocity += GRAV * (float)delta;
        velocity = Mathf.Clamp(velocity, -100, 100);
        Position = new Vector2(Position.X, Position.Y + velocity);
	}

    public void OnEnterArea(Area2D area)
    {
        if (!Visible) return;
        if(area.IsInGroup("pipes"))
        {
            EmitSignal(SignalName.HitPipe);
        }
        if(area.IsInGroup("pointgate"))
        {
            EmitSignal(SignalName.GainPoint);
        }
    }
    public void OnVisibilityChanged()
    {
        velocity = 0;
    }
}
