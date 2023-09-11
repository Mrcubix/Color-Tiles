using Godot;

namespace ColorTiles.Entities
{
	public partial class ColorTile
	{
		public const int TILEMAP_WIDTH = 3;
		public const int TILEMAP_HEIGHT = 4;
		public const int TILEMAP_BACKGROUND_START = 9;

		public Vector2I Position { get; set; }
		public Vector2I AtlasCoordinates { get; set; }

		public ColorTile() { }

		public ColorTile(Vector2I position)
		{
			Position = position;
		}

		public ColorTile(Vector2I position, Vector2I atlasCoordinates)
		{
			Position = position;
			AtlasCoordinates = atlasCoordinates;
		}

		/// <summary>
		///   Generates a random tile with a random color.
		/// </summary>
		/// <param name="position"></param>
		/// <returns>a tile with a random color</returns>
		public static ColorTile GenerateRandomTile(Vector2I position)
		{
            return new ColorTile(position)
            {
                AtlasCoordinates = new Vector2I(
					GD.RandRange(0, TILEMAP_WIDTH),
					GD.RandRange(0, TILEMAP_HEIGHT)
				)
            };
		}
	}
}
