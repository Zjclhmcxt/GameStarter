using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Gamelauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Gameload();
            myGames.ItemsSource = Games;
        }
        public List<Game> Games { get; set; }
        public void Gameload()
        {
            Games = new List<Game>();
            string[] lines = File.ReadAllLines("games.txt");    
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 2)
                {
                    Games.Add(new Game { Name = parts[0], Path = parts[1] });
                }
            }
        }
        public class Game
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {

            if (myGames.SelectedItem is Game selectedGame)
            {
                Process.Start(selectedGame.Path);
            }
            else
            {
                MessageBox.Show("未选择游戏！");
            }
        }

        private void AddGame(object sender, RoutedEventArgs e)
        {
            var gameName = gameNameTextBox.Text;
            var gamePath = gamePathTextBox.Text;

            if (string.IsNullOrWhiteSpace(gameName) || string.IsNullOrWhiteSpace(gamePath))
            {
                MessageBox.Show("游戏名和路径不能为空！");
                return;
            }

            // 添加游戏到列表
            Games.Add(new Game { Name = gameName, Path = gamePath });

            // 更新文件
            File.AppendAllText("games.txt", $"{gameName},{gamePath}\n");

            // 清空输入框
            gameNameTextBox.Clear();
            gamePathTextBox.Clear();

            // 重新加载游戏列表
            myGames.ItemsSource = null;
            myGames.ItemsSource = Games;

            MessageBox.Show("游戏已添加！");
        }

        private void openTxt(object sender, RoutedEventArgs e)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo("games.txt") { UseShellExecute = true },
                EnableRaisingEvents = true
            };
            process.Exited += Process_Exited;
            process.Start();
            
        }
        private void Process_Exited(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 刷新ComboBox数据
                Gameload();
                myGames.ItemsSource = null;
                myGames.ItemsSource = Games;
            });

            // 取消订阅事件
            var process = sender as Process;
            if (process != null)
            {
                process.Exited -= Process_Exited;
            }
        }
    }
}