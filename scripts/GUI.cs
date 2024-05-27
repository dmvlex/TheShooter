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
    public Label ResultLabel { get; set; }
    [Export]
    public Label PointsLabel { get; set; }
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

	private void SetHudVisibility(bool visible)
	{
		PointsOutput.Visible = visible;
        Warning.Visible = visible;
        AmmoOutput.Visible = visible;
        MagsOutput.Visible = visible;
		SetWarningVisibility(visible);
		PointsLabel.Visible = visible;
    }

	private void ResetGame()
	{
		CurrentAmmo = 0;
		Points = 0;
		AmmoMags = 6;
	}

	private void SetResultVisibility(bool visible)
	{
        ResultLabel.Visible = visible;
		if (visible)
		{
            ResultLabel.Text = "";
            ResultLabel.Text = $"Your score: {Points} \n";
            ResultLabel.Text += "Press [Q] to try again\nPress [ESC] to quit\n";
        }
    }

	private void GetGameResult()
	{
		if (AmmoMags == 0 && CurrentAmmo == 0)
		{
			SetHudVisibility(false);
			SetResultVisibility(true);

			if (Input.IsActionJustPressed("Restart"))
			{
				SetResultVisibility(false);
                SetHudVisibility(true);
                ResetGame();
			}
		}
		else
		{
			SetHudVisibility(true);
		}
	}
	private void SetWarningVisibility(bool visible)
	{
		if (visible)
		{
            if (CurrentAmmo > 0) Warning.Visible = false;
            else Warning.Visible = true;
		}
		else
		{
			Warning.Visible = visible;
		}
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
		GetGameResult();

        PrintMags();

		PointsOutput.Text = Points.ToString();
		AmmoOutput.Text = $"{CurrentAmmo}/{MaxAmmo}";
	}
}
