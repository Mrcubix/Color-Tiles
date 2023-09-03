
using System;
using System.Collections.Generic;
using ColorTiles.Entities;
using ColorTiles.UX.Menus;
using Godot;


namespace ColorTiles.UX
{
	public partial class Board : TileMap
	{
		public const int OFFSET_X = 3;
		public const int OFFSET_Y = 0;
		public static readonly Vector2I Offset = new(OFFSET_X, OFFSET_Y);

		public ColorTile[,] Rows { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public bool IsEnabled { get; set; } = false;

		public event EventHandler<int> MatchesFound;
		public event EventHandler OnPenalty;

		#region Initialization

		public Board() { }

		public Board(int width, int height)
		{
			InitializeEmptyGrid(width, height);
		}

		public Board(ColorTile[,] rows)
		{
			Rows = rows;
			Width = rows.GetLength(0);
			Height = rows.GetLength(1);
		}

		public void InitializeEmptyGrid(int width, int height)
		{
			Rows = new ColorTile[width, height];
			Width = width;
			Height = height;
		}

		#endregion

		#region Overrides

		public override void _Input(InputEvent @event)
		{
			base._Input(@event);

			if (!IsEnabled)
				return;

			if (@event is InputEventMouseButton mouseEvent)
			{
				if (mouseEvent.ButtonIndex == MouseButton.Left)
				{
					if (mouseEvent.Pressed)
					{
						Vector2 position = GetGlobalMousePosition();
						OnBoardPressed(position);
					}
				}
			}
			if (@event is InputEventScreenTouch touchEvent)
			{
				if (touchEvent.Pressed)
				{
					Vector2 position = GetCanvasTransform().BasisXformInv(touchEvent.Position);
					OnBoardPressed(position);
				}
			}
		}

		#endregion

		#region Events Callbacks

		public void OnBoardPressed(Vector2 position)
		{
			var tileMapPosition = LocalToMap(position);
#if DEBUG
			Console.WriteLine($"Clicked at {tileMapPosition}");
			GD.Print($"Clicked at {tileMapPosition}");
#endif

			// the click wasn't within the bounds of the board
			if (tileMapPosition.X < 0 || tileMapPosition.X >= Width || tileMapPosition.Y < 0 || tileMapPosition.Y >= Height)
				return;

			var tile = Rows[tileMapPosition.X, tileMapPosition.Y];

			if (tile == null)
			{
				//GD.Print($"Empty Tile at {Position} was clicked!");

				// get the adjacent tiles
				ColorTile[] res = GetClosestAdjacentTiles(tileMapPosition);

				// check for matches
				int matchCount = RemoveMatches(res);

				if (matchCount == 0)
					OnPenalty?.Invoke(this, EventArgs.Empty);
				else
					MatchesFound?.Invoke(this, matchCount);
			}
		}

		#endregion

		/// <summary>
		/// Returns the closest adjacent 4 tiles to the given position.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public ColorTile[] GetClosestAdjacentTiles(Vector2I position)
		{
			var adjacentTiles = new ColorTile[4];

			int currentX;
			int currentY;

			// left
			if (position.X > 0)
			{
				currentX = position.X - 1;

				// we shift to the left until we find a tile
				do
				{
					adjacentTiles[0] = Rows[currentX--, position.Y];
				} while (adjacentTiles[0] == null && currentX >= 0);
			}

			// right
			if (position.X < Width - 1)
			{
				currentX = position.X + 1;

				// we shift to the right until we find a tile
				do
				{
					adjacentTiles[1] = Rows[currentX++, position.Y];
				} while (adjacentTiles[1] == null && currentX < Width);
			}

			// top
			if (position.Y > 0)
			{
				currentY = position.Y - 1;

				// we shift to the top until we find a tile
				do
				{
					adjacentTiles[2] = Rows[position.X, currentY--];
				} while (adjacentTiles[2] == null && currentY >= 0);
			}

			// bottom
			if (position.Y < Height - 1)
			{
				currentY = position.Y + 1;

				// we shift to the bottom until we find a tile
				do
				{
					adjacentTiles[3] = Rows[position.X, currentY++];
				} while (adjacentTiles[3] == null && currentY < Height);
			}

			return adjacentTiles;
		}

		/// <summary>
		/// If the crossing tiles are the same color, then they will be removed from the board.
		/// </summary>
		/// <param name="tiles"></param>
		/// <returns>The number of matches removed.</returns>
		public int RemoveMatches(ColorTile[] tiles)
		{
			List<ColorTile> subjectForRemoval = new();

			// check for matches
			foreach (var tile in tiles)
			{
				foreach (var otherTile in tiles)
				{
					if (subjectForRemoval.Contains(otherTile))
						continue;

					if (tile == otherTile)
						continue;

					if (tile == null || otherTile == null)
						continue;

					if (tile.AtlasCoordinates == otherTile.AtlasCoordinates)
					{
						subjectForRemoval.Add(otherTile);
					}
				}
			}

			// remove the subject tiles
			foreach (var tile in subjectForRemoval)
				RemoveTile(tile.Position);

			return subjectForRemoval.Count;
		}

		public void RemoveTile(Vector2I position)
		{
			Rows[position.X, position.Y] = null;
			SetCell(1, position);
			// tiles are not scene objects here, no need for queue_free()
		}

		/// <summary>
		///   Clears the board of all tiles.
		/// </summary>
		public void ClearBoard()
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Rows[x, y] = null;

					SetCell(1, new Vector2I(x, y));
				}
			}
		}

		#region Operators

		public ColorTile this[int x, int y]
		{
			get => Rows[x, y];
			set => Rows[x, y] = value;
		}

		public ColorTile this[Vector2 position]
		{
			get => Rows[(int)position.X, (int)position.Y];
			set => Rows[(int)position.X, (int)position.Y] = value;
		}

		#endregion
	}
}
