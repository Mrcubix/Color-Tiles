using Godot;

namespace ColorTiles.UX.Dialogs
{
	public partial class ErrorDialog : AcceptDialog
	{
		[Export]
		public ColorRect Background { get; set; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Hide();
		}

		public void OnError(object sender, string message)
		{
			Title = "An error has occurred";
			DialogText = message;
			OkButtonText = "Close";
			
			Background.Show();
			Show();

			Confirmed += Quit;
			Canceled += Quit;
		}

		private void Quit()
		{
			GetTree().Quit();
		}
	}
}