using System.Windows;
using Front.Models;
using Front.Views;

namespace Front
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PreGameView.GameStarted += PreGameViewOnGameStarted;
            GameView.GameFinished += GameViewOnGameFinished;
            
            PreGameTab.Visibility = Visibility.Visible;
            GameTab.Visibility = Visibility.Collapsed;
            EditQuizTab.Visibility = Visibility.Visible;
            EditQuestionTab.Visibility = Visibility.Visible;
            EditCategoryTab.Visibility = Visibility.Visible;
            AddQuizTab.Visibility = Visibility.Visible;

            PreGameTab.IsSelected = true;
        }

        private void GameViewOnGameFinished()
        {
            PreGameTab.Visibility = Visibility.Visible;
            GameTab.Visibility = Visibility.Collapsed;
            EditQuizTab.Visibility = Visibility.Visible;
            EditQuestionTab.Visibility = Visibility.Visible;
            EditCategoryTab.Visibility = Visibility.Visible;
            AddQuizTab.Visibility = Visibility.Visible;

            PreGameTab.IsSelected = true;
        }

        private void PreGameViewOnGameStarted(QuizModel obj)
        {
            PreGameTab.Visibility = Visibility.Collapsed;
            GameTab.Visibility = Visibility.Visible;
            EditQuizTab.Visibility = Visibility.Collapsed;
            EditQuestionTab.Visibility = Visibility.Collapsed;
            EditCategoryTab.Visibility = Visibility.Collapsed;
            AddQuizTab.Visibility = Visibility.Collapsed;

            GameTab.IsSelected = true;
        }
    }
}