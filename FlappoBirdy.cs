using Godot;
using System;
using System.Collections.Generic;

public partial class FlappoBirdy : Node2D
{
    const int MINGAP = 120;
    const int MAXGAP = 300;
    const float PIPE_SPEED = 120;

    float speed = 1.25f;
    float pipeSpawnRate = 2.5f;
    float pipeTimer;

    PackedScene pipescn;
    readonly List<Pipe> pipes = new();
    Node2D pipeContainer;

    Sprite2D BG1;
    Sprite2D BG2;

    AudioStreamPlayer pointSound;
    Label pointLabel;

    AudioStreamPlayer deathSound;

    int points;
    bool playing;

    DateTime lastdeath;

    public override void _Ready()
    {
        pipeContainer = GetNode<Node2D>("PipeContainer");
        pointSound = GetNode<AudioStreamPlayer>("PointSound");
        deathSound = GetNode<AudioStreamPlayer>("DeathSound");
        pointLabel = GetNode<Label>("Points");
        BG1 = GetNode<Sprite2D>("BG1");
        BG2 = GetNode<Sprite2D>("BG2");
        pipescn = GD.Load<PackedScene>("res://Pipe.tscn");
        CreatePipe();
        lastdeath = DateTime.MinValue;
    }
    public void CreatePipe()
    {
        Pipe pipe = pipescn.Instantiate<Pipe>();
        pipe.Position = new Vector2(DisplayServer.WindowGetSize().X + 40, Random.Shared.Next(100, DisplayServer.WindowGetSize().Y - 100));
        pipe.SetGap(Random.Shared.Next(MINGAP, MAXGAP));
        pipes.Add(pipe);
        pipeContainer.AddChild(pipe);
        GD.Print($"Created pipe at {pipe.Position.X}, {pipe.Position.Y}");
    }

    public void GainPoint()
    {
        points++;
        pointSound.Play();
        pointLabel.Text = points.ToString();
    }

    public void BeginGame()
    {
        if (lastdeath.AddSeconds(1) > DateTime.Now) return;
        playing = true;
        points = 0;
        pipeTimer = 0;
        for (int i = pipes.Count - 1; i >= 0; i--)
        {
            GD.Print($"Destroying pipe at {pipes[i].Position.X}, {pipes[i].Position.Y}");
            pipes[i].Free();
            pipes.RemoveAt(i);
        }
        CreatePipe();
        Bird b = GetNode<Bird>("Bird");
        b.Position = GetNode<Marker2D>("BirdStartPos").Position;
        b.Visible = true;
        GetNode<Node2D>("StartScreen").Visible = false;
        GetNode<Node2D>("EndScreen").Visible = false;
        pointLabel.Text = points.ToString();
        pointLabel.Visible = true;
    }

    public void EndGame()
    {
        lastdeath = DateTime.Now;
        playing = false;
        Bird b = GetNode<Bird>("Bird");
        b.Visible = false;
        GetNode<Node2D>("EndScreen").Visible = true;
        GetNode<Label>("EndScreen/Points").Text = $"Game Over\n\nYour Score is\n{points}";
        pointLabel.Visible = false;
        deathSound.Play();
    }

	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("jump") && !playing)
        {
            BeginGame();
        }
        pipeTimer += (float)delta;
        if (pipeTimer > pipeSpawnRate)
        {
            pipeTimer = 0;
            CreatePipe();
        }
        for (int i = pipes.Count - 1; i >= 0; i--)
        {
            if (pipes[i].Position.X < -40)
            {
                GD.Print($"Destroying pipe at {pipes[i].Position.X}, {pipes[i].Position.Y}");
                pipes[i].Free();
                pipes.RemoveAt(i);
            }
            else
            {
                pipes[i].Position -= new Vector2(PIPE_SPEED, 0) * (float)delta;
            }
        }

        Vector2 bgdelta = new Vector2(-20, 0) * (float)delta;
        BG1.Position += bgdelta;
        BG2.Position += bgdelta;
        if (BG1.Position.X < -810) BG1.Position += new Vector2(804*2, 0);
        if (BG2.Position.X < -810) BG2.Position += new Vector2(804*2, 0);
	}
}
