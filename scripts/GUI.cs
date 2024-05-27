using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class GUI : Control
{
	[Export]
	public int Points { get; set; } = 0;
	[Export]
	public int MaxAmmo { get; set; } = 30;
    [Export]
    public int AmmoMags { get; set; } = 5;
    public int CurrentAmmo { get; set; }
	[Export]
	public Label PointsOutput { get; set; }
    [Export]
    public Label Warning { get; set; }
    [Export]
    public Label AmmoOutput { get; set; }
    [Export]
    public Label MagsOutput { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		CurrentAmmo = MaxAmmo;
		Warning.Visible = false;
	}

	private void PrintMags()
	{
        MagsOutput.Text = "";
        for (int i = 0; i < AmmoMags; i++)
		{
			MagsOutput.Text += "|";
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (CurrentAmmo > 0)
		{
            Warning.Visible = false;
        }else
		{
			Warning.Visible = true;
		}

		PrintMags();

		PointsOutput.Text = Points.ToString();
		AmmoOutput.Text = $"{CurrentAmmo}/{MaxAmmo}";
	}
}
