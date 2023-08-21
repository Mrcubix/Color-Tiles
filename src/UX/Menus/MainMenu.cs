using ColorTiles.UX.Controls;
using Godot;

namespace ColorTiles.UX.Menus
{
	public partial class MainMenu : VBoxContainer
	{
		[Export]
		public PlayButton PlayButton { get; set; }
	}
}