using System;
using Godot;

namespace ColorTiles.UX.Controls
{
	public partial class PlayButton : TextureButton
	{
		public event EventHandler PlayButtonPressed;

		public void OnPlayButtonPressed()
		{
			// Hide the MainMenu
			GetParent<VBoxContainer>().Hide();

			// Invoke the event
			PlayButtonPressed?.Invoke(this, EventArgs.Empty);
		}
	}
}
