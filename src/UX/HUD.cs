using Godot;
using System;

public partial class HUD : Control
{
	private double _time;
	private int _score;

	public const int SIZE_X = 720;
	public const int SIZE_Y = 30;

	[Export]
	public Label ScoreLabel { get; set; }
	[Export]
	public ColorRect TimerBar { get; set; }
	[Export]
	public Button ResetButton { get; set; }

	public double InitialTime { get; set; }
	public int InitialScore { get; set; }

	public event EventHandler ResetClicked = null!;

	public double Time
	{
		get => _time;
		set
		{
			_time = value;

			if (TimerBar == null)
				return;

			TimeChanged?.Invoke(this, value);
		}
	}
	public int Score
	{
		get => _score;
		set
		{
			_score = value;

			if (ScoreLabel == null)
				return;

			ScoreLabel.Text = value.ToString();
			ScoreChanged?.Invoke(this, value);
		}
	}

	public bool IsActive { get; set; } = false;

	public event EventHandler<int> ScoreChanged = null!;
	public event EventHandler<double> TimeChanged = null!;
	public event EventHandler TimeExpired = null!;

	#region Constructors

	public HUD()
	{
		InitialScore = 0;
		InitialTime = 120;
		Score = 0;
		Time = 120;
		IsActive = true;
	}

	public HUD(int score, int time)
	{
		InitialScore = score;
		InitialTime = time;
		Score = score;
		Time = time;
		IsActive = true;
	}

	public HUD(int time)
	{
		InitialScore = 0;
		InitialTime = time;
		Score = 0;
		Time = time;
		IsActive = true;
	}

	#endregion

	#region Overridden Methods

	public override void _Ready()
	{
		ScoreLabel.Text = Score.ToString();
		TimerBar.Size = new Vector2(SIZE_X, SIZE_Y);
	}

	public override void _Process(double delta)
	{
		if (Time <= 0)
		{
			if (IsActive)
			{
				TimeExpired?.Invoke(this, EventArgs.Empty);
				IsActive = false;
			}
			else
				return;
		}


		Time -= delta;
		TimerBar.Size = new Vector2((float)(Time / 120) * SIZE_X, SIZE_Y);
	}

	#endregion

	/// <summary>
	///   Resets the HUD to its initial values.
	/// </summary>
	/// <remarks>
	///  This method is called when the game is restarted.
	/// </remarks>
	public void Reset()
	{
		Score = InitialScore;
		Time = InitialTime;
		IsActive = true;
	}

	public void OnResetButtonPressed()
	{
		ResetClicked?.Invoke(this, EventArgs.Empty);
	}
}
