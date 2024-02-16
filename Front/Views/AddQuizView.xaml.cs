using DataAccess.Services;
using Front.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Common.DTO;

namespace Front.Views
{
    /// <summary>
    /// Interaction logic for AddQuizView.xaml
    /// </summary>
    public partial class AddQuizView : UserControl
    {
        private readonly QuizRepository _quiz;

        public ObservableCollection<QuizModel> QuizList { get; set; } = new();

        public QuizModel? SelectedQuiz { get; set; } = new();

        public static event Action QuizzesUpdated;

        public AddQuizView()
        {
            InitializeComponent();

            DataContext = this;

            _quiz = new QuizRepository();
            PopulateQuizzes();
        }

        private void PopulateQuizzes()
        {
            QuizList.Clear();
            var allQuizzes = _quiz.GetAllQuizzes();

            foreach (var q in allQuizzes)
            {
                QuizList.Add(new QuizModel
                {
                    Title = q.title,
                    Description = q.description,
                    Id = q.id
                });
            }
        }

        private void AddQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (QuizTxt.Text == "" || QuizDescriptionTxt.Text == "")
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            var quizRecord = new QuizRecord(
                id: "",
                title: QuizTxt.Text,
                description: QuizDescriptionTxt.Text,
                questions: new string[0],
                categories: new string[0]
            );

            _quiz.CreateQuiz(quizRecord);

            MessageBox.Show("Quiz added successfully");

            QuizzesUpdated.Invoke();
            QuizTxt.Clear();
            QuizDescriptionTxt.Clear();
            QuizList.Clear();
            PopulateQuizzes();
        }

        private void ClearFieldsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            QuizTxt.Clear();
            QuizDescriptionTxt.Clear();
        }

        private void DeleteQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var message = "Are you sure?";
            var caption = "Delete quiz";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);

            if (result.Equals(MessageBoxResult.No))
            {
                return;
            }

            if (QuizzesInDbList.SelectedItem is QuizModel selected)
            {
                _quiz.DeleteQuiz(selected.Id);
            }
            QuizzesUpdated.Invoke();
            PopulateQuizzes();
            MessageBox.Show("Quiz deleted successfully");
        }
    }
}
