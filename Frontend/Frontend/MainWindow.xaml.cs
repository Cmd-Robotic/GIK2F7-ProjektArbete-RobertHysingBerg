using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<GameInfo> Games = new ObservableCollection<GameInfo>();
        private GameApiHandler gameApiHandler = new GameApiHandler("http://localhost:5000/games/");
        public MainWindow()
        {
            InitializeComponent();
            Game_List.ItemsSource = Games;
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

        private void Detailed_Game_Info_Button_Click(object sender, RoutedEventArgs e)
        {
            Queries.Visibility = (Visibility)2;
            Get_Detailed_Info_Box.Visibility = 0;
        }

        private void Delete_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            Queries.Visibility = (Visibility)2;
            Delete_Game_Box.Visibility = 0;
        }

        private void Add_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            Queries.Visibility = (Visibility)2;
            Add_Game_Box.Visibility = 0;
        }

        private void Update_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            Queries.Visibility = (Visibility)2;
            Update_Game_Box.Visibility = 0;
        }

        /*private void Upload_Game_Image_Button_Click(object sender, RoutedEventArgs e)
        {

        }*/

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Add_Game_Box.Visibility = (Visibility)2;
            Delete_Game_Box.Visibility = (Visibility)2;
            Get_Detailed_Info_Box.Visibility = (Visibility)2;
            Update_Game_Box.Visibility = (Visibility)2;
            Detailed_Info.Visibility = (Visibility)2;
            Queries.Visibility = 0;
            Main_Info.Visibility = 0;
        }

        private void Get_Detailed_Info_Submit_Click(object sender, RoutedEventArgs e)
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
        }

        private void Delete_Game_Submit_Click(object sender, RoutedEventArgs e)
        {
            int gameId;
            if (int.TryParse(Delete_Game_Game_Id.Text, out gameId))
            {
                gameApiHandler.DeleteGame(gameId);
                Games = gameApiHandler.GetAllGames();
                Game_List.ItemsSource = Games;
                Error_Message.Text = "If a game with that Id existed, it no longer does!";
            }
            else
            {
                Error_Message.Text = "That Id is not a number!";
            }
        }

        private void Add_Game_Submit_Click(object sender, RoutedEventArgs e)
        {
            GameInfo game = new GameInfo();
            int grade;
            if (Add_Game_Game_Name.Text == "")
            {
                Error_Message.Text = "The game must have a name!";
            }
            else if (Add_Game_Game_Description.Text == "")
            {
                Error_Message.Text = "At least have some short description!";
            }
            else if (int.TryParse(Add_Game_Game_Rating.Text, out grade) != true)
            {
                Error_Message.Text = "Did you misstype the grade?";
            }
            else
            {
                game.name = Add_Game_Game_Name.Text;
                game.description = Add_Game_Game_Description.Text;
                game.grade = grade;
                gameApiHandler.AddGame(game);
                Games.Add(game);
                Games = gameApiHandler.GetAllGames();
                Game_List.ItemsSource = Games;
                Error_Message.Text = "Game has been added to database!";
            }
        }

        private void Update_Game_Submit_Click(object sender, RoutedEventArgs e)
        {
            GameInfo game = new GameInfo();
            int tempInt;
            if (int.TryParse(Update_Game_Game_Rating.Text, out tempInt) != true)
            {
                Error_Message.Text = "Invalid rating, did you misstype it?";
            }
            else
            {
                game.grade = tempInt;
            }
            if (int.TryParse(Update_Game_Game_Id.Text, out tempInt) != true)
            {
                Error_Message.Text = "Invalid Id, is that even a number?";
            }
            else
            {
                game.id = tempInt;
                game.name = Update_Game_Game_Name.Text;
                game.description = Update_Game_Game_Description.Text;
                gameApiHandler.UpdateGame(game);
                Games = gameApiHandler.GetAllGames();
                Game_List.ItemsSource = Games;
                Error_Message.Text = "The game has been updated!";
            }
        }

        private void Load_All_Games_Button_Click(object sender, RoutedEventArgs e)
        {
            Games = gameApiHandler.GetAllGames();
            Game_List.ItemsSource = Games;
            Error_Message.Text = "Games have been loaded!";
        }
    }
}
