using System;
using ColorTiles.Entities;
using ColorTiles.UX;
using ColorTiles.UX.Controls;
using ColorTiles.UX.Dialogs;
using ColorTiles.UX.Menus;
using Godot;

namespace ColorTiles
{
    public partial class GameLogic : Node2D
    {
        private const int BOARD_WIDTH = 23;
        private const int BOARD_HEIGHT = 15;
        private const int TILE_OF_COLOR = 20;

        private const int TIME = 120;
        private const int PENALTY = 10;

        [Export]
        public Board Board { get; set; }
        [Export]
        public MainMenu MainMenu { get; set; }
        [Export]
        public GameOverScreen GameOverScreen { get; set; }
        [Export]
        public HUD HUD { get; set; }
        [Export]
        public ErrorDialog ErrorDialog { get; set; }

        public PlayButton PlayButton => MainMenu.PlayButton;

        public override void _Ready()
        {
            if (!CheckAssociations())
                return;

            SubscribeToEvents();

            // Show the MainMenu
            MainMenu.Show();

            // Hide the GameOverScreen
            GameOverScreen.Hide();

            // Hide the HUD
            HUD.Hide();
            
            // Initialize the Grid
            Board.InitializeEmptyGrid(BOARD_WIDTH, BOARD_HEIGHT);
        }

        public override void _Notification(int what)
        {
            if (what == NotificationWMCloseRequest || what == NotificationWMGoBackRequest)
            {
                Unload();
            }
        }

        #region Events Callbacks

        private void OnPlayButtonPressed(object sender, EventArgs e)
        {
            // Initialize the Grid
            Board.InitializeEmptyGrid(BOARD_WIDTH, BOARD_HEIGHT);

            // Generate the board
            GenerateBoard();

            // Place the newly generated tiles on the Grid
            foreach (var tile in Board.Rows)
            {
                if (tile != null)
                {
                    Board.SetCell(1, tile.Position, 0, tile.AtlasCoordinates);
                }
            }

            HUD.Reset();
            HUD.Show();
        }

        private void OnPlayAgainClicked(object sender, EventArgs e)
        {
            // Hide the GameOverScreen
            GameOverScreen.Hide();

            // Clear the board
            Board.ClearBoard();

            // Show the MainMenu
            MainMenu.Show();
        }

        private void OnQuitClicked(object sender, EventArgs e)
        {
            GetTree().Quit();
        }

        private void OnMatchesFound(object sender, int matchesCount)
        {
            HUD.Score += matchesCount;
        }

        private void OnPenalty(object sender, EventArgs e)
        {
            HUD.Time -= PENALTY;
        }

        private void OnTimeExpired(object sender, EventArgs e)
        {
            // Hide the HUD
            HUD.Hide();

            GameOverScreen.ScoreLabel.Text = HUD.Score.ToString();

            // Show the GameOverScreen
            GameOverScreen.Show();
        }

        #endregion

        #region Further Initialization

        /// <summary>
        ///   Check if some exported variables have been associated in the editor.
        /// </summary>
        /// <returns>true if all the exported variables have been associated, false otherwise</returns>
        public bool CheckAssociations()
        {
            if (Board == null)
            {
                ErrorDialog.OnError(this, "No Board was associated with the Game Element or there was an error during the Board's initialization.");
                return false;
            }

            if (MainMenu == null)
            {
                ErrorDialog.OnError(this, "No MainMenu was associated with the Game Element or there was an error during the MainMenu's initialization.");
                return false;
            }

            if (GameOverScreen == null)
            {
                ErrorDialog.OnError(this, "No GameOverScreen was associated with the Game Element or there was an error during the GameOverScreen's initialization.");
                return false;
            }

            return true;
        }

        /// <summary>
        ///   Subscribe to various events used within the game.
        /// </summary>
        public void SubscribeToEvents()
        {
            // Subscribe to the PlayButton's event
            PlayButton.PlayButtonPressed += OnPlayButtonPressed;

            // Subscribe to the GameOverScreen's event
            GameOverScreen.PlayAgainClicked += OnPlayAgainClicked;

            // Subscribe to the Board's event
            Board.MatchesFound += OnMatchesFound;
            Board.OnPenalty += OnPenalty;

            // Subscribe to the HUD's event
            HUD.TimeExpired += OnTimeExpired;

            // Subscribe to the GameOverScreen's event
            GameOverScreen.PlayAgainClicked += OnPlayAgainClicked;
            GameOverScreen.QuitClicked += OnQuitClicked;
        }

        /// <summary>
        ///   Generate the board with random tiles. <br/>
        ///   For each color in the atlas, generate <see cref="TILE_OF_COLOR"/> number of tiles.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///    The board is generated by generating a random position and checking if the position is empty. <br/>
        ///    If the position is empty, generate a random tile and place it on the board. <br/>
        ///  </para>
        /// </remarks>
        public void GenerateBoard()
        {
            // for every colors in the atlas (ColorTile.TILEMAP_WIDTH * ColorTile.TILEMAP_HEIGHT)
            for (int i = 0; i < ColorTile.TILEMAP_WIDTH; i++)
            {
                for (int j = 0; j < ColorTile.TILEMAP_HEIGHT; j++)
                {
                    var tilesToGenerate = TILE_OF_COLOR;

                    // while there are still tiles to generate
                    while (tilesToGenerate > 0)
                    {
                        // generate a random position
                        var randomPosition = new Vector2I(
                            GD.RandRange(0, BOARD_WIDTH - 1), 
                            GD.RandRange(0, BOARD_HEIGHT - 1)
                        );

                        // if the position is empty
                        if (Board[randomPosition] == null)
                        {
                            // generate a random tile
                            Board[randomPosition] = new ColorTile(randomPosition, new Vector2I(i, j));

                            // decrement the tiles to generate
                            tilesToGenerate--;
                        }
                    }
                }
            }
        }

        #endregion

        #region De-initialization

        /// <summary>
        ///  Unload the GameLogic.
        ///  <para>
        ///  Unsubscribe from all events and clear the board.
        ///  </para>
        /// </summary>
        public void Unload()
        {
            // unsub from all events
            Unsubscribe();

            Board.ClearBoard();
        }

        /// <summary>
        ///   Unsubscribe from all events.
        /// </summary>
        public void Unsubscribe()
        {
            // Unsubscribe from the PlayButton's event
            PlayButton.PlayButtonPressed -= OnPlayButtonPressed;

            // Unsubscribe from the Board's event
            Board.MatchesFound -= OnMatchesFound;
            Board.OnPenalty -= OnPenalty;

            // Unsubscribe from the HUD's event
            HUD.TimeExpired -= OnTimeExpired;

            // Unsubscribe from the GameOverScreen's event
            GameOverScreen.PlayAgainClicked -= OnPlayAgainClicked;
            GameOverScreen.QuitClicked -= OnQuitClicked;
        }

        #endregion
    }
}