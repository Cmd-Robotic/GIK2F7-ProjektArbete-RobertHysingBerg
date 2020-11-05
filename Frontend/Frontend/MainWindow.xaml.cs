using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<GameInfo> Games;
        private GameApiHandler gameApiHandler;
        public MainWindow()
        {
            InitializeComponent();
            Games = new ObservableCollection<GameInfo>();
            gameApiHandler = new GameApiHandler("http://localhost:5000/games/");
            Load_Games();
        }

        private void Game_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Game_List.SelectedItem != null)
            {
                GameInfo selectedGame = (GameInfo)Game_List.SelectedItem;
                Selected_Game_Game_Name.Text = selectedGame.name;
                Selected_Game_Game_Id.Text = selectedGame.id.ToString();
                Selected_Game_Game_Score.Text = selectedGame.grade.ToString();
                Selected_Game_Game_Description.Text = selectedGame.description;
            }
        }

        /*private void Detailed_Game_Info_Button_Click(object sender, RoutedEventArgs e)
        {
            Queries.Visibility = (Visibility)2;
            Get_Detailed_Info_Box.Visibility = 0;
        }*/

        private void Delete_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Game_List.SelectedItem != null)
            {
                GameInfo GameDelete = (GameInfo)Game_List.SelectedItem;
                Delete_Game_Game_Id.Text = GameDelete.id.ToString();
                Queries.Visibility = (Visibility)2;
                Delete_Game_Box.Visibility = 0;
            }
            else
            {
                Error_Message.Text = "Please select a game to be deleted from the list!";
            }
        }

        private void Add_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            Queries.Visibility = (Visibility)2;
            Add_Game_Box.Visibility = 0;
        }

        private void Update_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Game_List.SelectedItem != null)
            {
                GameInfo GameUpdate = (GameInfo)Game_List.SelectedItem;
                Update_Game_Game_Id.Text = GameUpdate.id.ToString();
                Update_Game_Game_Name.Text = GameUpdate.name;
                Update_Game_Game_Rating.Text = GameUpdate.grade.ToString();
                Update_Game_Game_Description.Text = GameUpdate.description;
                Queries.Visibility = (Visibility)2;
                Update_Game_Box.Visibility = 0;
            }
            else
            {
                Error_Message.Text = "Please select a game from the list before trying to update it!";
            }
        }

        private async void Load_Image_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Game_List.SelectedItem != null)
            {
                GameInfo game = (GameInfo)Game_List.SelectedItem;
                Load_Game_Image_Box.Header = game.name + " Image";
                var httpResponse = await gameApiHandler.GetGameImage(game.id);
                if (httpResponse.IsSuccessStatusCode)
                {
                    BitmapImage image = new BitmapImage();
                    var imageData = await httpResponse.Content.ReadAsByteArrayAsync();
                    using (MemoryStream memoryStream = new MemoryStream(imageData))
                    {   //Hade lite problem med att bilden inte laddades med bara en rad så sökte runt på webben och hittade att jag disposade memstreamen innan den hunnit läsa den
                        //lade till några extra rader som säger till den att läsa innan den går vidare o nu fungerar det
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = memoryStream;
                        image.EndInit();
                    }
                    Load_Game_Image_Game_Image.Source = image;
                    Queries.Visibility = (Visibility)2;
                    Error_Message.Visibility = (Visibility)2;
                    Load_Game_Image_Box.Visibility = 0;
                }
                else if ((int)httpResponse.StatusCode > 499)
                {
                    Error_Message.Text = "Internal server error, Image does not exist or is corrupted";
                }
                else
                {
                    Error_Message.Text = "Some kind of error has occured, probably related to connectivity issues. Is the server on and/or are you connected to the internet?";
                }
            }
            else
            {
                Error_Message.Text = "Please select a game from the list before loading it's image!";
            }
        }

        private void Upload_Game_Image_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Game_List.SelectedItem != null)
            {
                Upload_Game_Image_Box.Visibility = 0;
                Queries.Visibility = (Visibility)2;
                Error_Message.Visibility = (Visibility)2;
                GameInfo game = (GameInfo)Game_List.SelectedItem;
                Upload_Game_Image_Game_Id.Text = game.id.ToString();
            }
            else
            {
                Error_Message.Text = "Please select a game in the list before trying to upload an image!";
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Home_Screen();
        }
        //A method to return the screen to the home screen
        private void Home_Screen()
        {
            Queries.Visibility = 0;
            Main_Info.Visibility = 0;
            Error_Message.Visibility = 0;
            Update_Game_Box.Visibility = (Visibility)2;
            Add_Game_Box.Visibility = (Visibility)2;
            Delete_Game_Box.Visibility = (Visibility)2;
            Load_Game_Image_Box.Visibility = (Visibility)2;
            Upload_Game_Image_Box.Visibility = (Visibility)2;
            Error_Message.Text = "";
        }

        /* Old code commented out since it's no longer needed
         * private void Get_Detailed_Info_Submit_Click(object sender, RoutedEventArgs e)
        {
            int gameId;
            if (int.TryParse(Get_Detailed_Info_Game_Id.Text, out gameId))
            {
                GameInfo game = gameApiHandler.GetGame(gameId);
                Get_Detailed_Info_Box.Visibility = (Visibility)2;
                Queries.Visibility = 0;
                if (game.name != null && game.name != "")
                {
                    Detailed_Game_Info_Name.Text = game.name;
                    Detailed_Game_Info_Id.Text = game.id.ToString();
                    Detailed_Game_Info_Grade.Text = game.grade.ToString();
                    Detailed_Game_Info_Description.Text = game.description;
                    Main_Info.Visibility = (Visibility)2;
                    Detailed_Info.Visibility = 0;
                }
                else
                {
                    Error_Message.Text = "No game matching that id was found in database";

                }
            }
            else
            {
                Error_Message.Text = "That Id is not a number!";
            }
        }*/

        private async void Delete_Game_Submit_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Delete_Game_Game_Id.Text, out int gameId))
            {
                await gameApiHandler.DeleteGame(gameId);
                Load_Games();
                Error_Message.Text = "If a game with that Id existed, it no longer does!";
                Home_Screen();
            }
            else
            {
                Error_Message.Text = "That Id is not a number!";
            }
        }

        private async void Add_Game_Submit_Click(object sender, RoutedEventArgs e)
        {
            GameInfo game = new GameInfo();
            if (Add_Game_Game_Name.Text == "")
            {
                Error_Message.Text = "The game must have a name!";
            }
            else if (Add_Game_Game_Description.Text == "")
            {
                Error_Message.Text = "At least have some short description!";
            }
            else if (int.TryParse(Add_Game_Game_Rating.Text, out int grade) != true)
            {
                Error_Message.Text = "Did you misstype the grade?";
            }
            else if (grade < 0 || grade > 100)
            {
                Error_Message.Text = "Invalid value for grade. Did you enter a negative number or a number above 100?";
            }
            else
            {
                game.name = Add_Game_Game_Name.Text;
                game.description = Add_Game_Game_Description.Text;
                game.grade = grade;
                await gameApiHandler.AddGame(game);
                Games.Add(game);
                Load_Games();
                Error_Message.Text = "Game has been added to database!";
                Home_Screen();
            }
        }

        private async void Update_Game_Submit_Click(object sender, RoutedEventArgs e)
        {
            GameInfo game = new GameInfo();
            if (int.TryParse(Update_Game_Game_Rating.Text, out int tempInt) == true)
            {
                game.grade = tempInt;
            }
            else
            {
                game.grade = -1;
            }
            if (int.TryParse(Update_Game_Game_Id.Text, out tempInt) != true)
            {
                Error_Message.Text = "Invalid Id, is that even a number? How did we even get here? You shouldn't even be able to edit that?";
            }
            else if (game.grade < 0 || game.grade > 100)
            {
                Error_Message.Text = "Invalid rating. Did you misstype it, enter a negative number or a number above 100?";
            }
            else
            {
                game.id = tempInt;
                game.name = Update_Game_Game_Name.Text;
                game.description = Update_Game_Game_Description.Text;
                await gameApiHandler.UpdateGame(game);
                Load_Games();
                Error_Message.Text = "The game has been updated!";
                Home_Screen();
            }
        }

        private void Load_All_Games_Button_Click(object sender, RoutedEventArgs e)
        {
            Load_Games();
            Error_Message.Text = "Games have been loaded!";
        }
        private void Load_Games()
        {
            Games = gameApiHandler.GetAllGames();
            Game_List.ItemsSource = Games;
        }

        private void Upload_Image_Clear_Image_Button_Click(object sender, RoutedEventArgs e)
        {
            Upload_Game_Image_Game_Image.Source = null;
        }

        private void Upload_Image_Load_Image_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Select an image",
                Filter = "Supported image files|*.jpeg;*.jpg;*.png;*.gif"
            };
            if (openFile.ShowDialog() == true)
            {
                Upload_Game_Image_Game_Image.Source = new BitmapImage(new Uri(openFile.FileName));
            }
        }

        private void Upload_Image_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Home_Screen();
        }

        private async void Upload_Image_Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Upload_Game_Image_Game_Id.Text, out int GameId))
            {
                if (Upload_Game_Image_Game_Image.Source != null)
                {
                    await gameApiHandler.PostGameImage(GameId, Upload_Game_Image_Game_Image.Source.ToString().Remove(0, 8));
                    Home_Screen();
                    Upload_Game_Image_Game_Image.Source = null;
                }
                else
                {
                    Home_Screen();
                    Error_Message.Text = "You can't post a file that doesn't exist!";
                }
            }
            else
            {
                Error_Message.Text = "Invalid Id, how did we get here?";
            }
        }
    }
}
