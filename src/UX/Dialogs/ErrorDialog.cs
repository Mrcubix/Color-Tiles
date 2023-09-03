using Godot;

namespace ColorTiles.UX.Dialogs
{
	public partial class ErrorDialog : AcceptDialog
	{
		[Export]
		public ColorRect Background { get; set; }

		public Control Container;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Container = GetParent<Control>();
			Hide();
		}

		public void OnError(object sender, string message)
		{
			Title = "An error has occurred";
			DialogText = message;
			OkButtonText = "Close";
			
			Container.Show();
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