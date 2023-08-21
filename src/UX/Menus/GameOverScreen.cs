using Godot;
using System;

namespace ColorTiles.UX.Menus
{
	public partial class GameOverScreen : VBoxContainer
	{
		[Export]
		public Label ScoreLabel { get; set; }
		[Export]
		public Button PlayAgainButton { get; set; }

		public event EventHandler PlayAgainClicked = null!;
		public event EventHandler QuitClicked = null!;

		public override void _Ready()
		{
			Hide();
		}

		public void OnPlayAgainButtonPressed()
		{
			PlayAgainClicked?.Invoke(this, EventArgs.Empty);
		}

		public void OnQuitButtonPressed()
		{
			QuitClicked?.Invoke(this, EventArgs.Empty);
		}
	}
}