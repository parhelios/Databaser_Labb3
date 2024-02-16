using Front.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Services;

namespace Front.Views
{
    /// <summary>
    /// Interaction logic for PreGameView.xaml
    /// </summary>
    public partial class PreGameView : UserControl
    {
        private readonly QuestionRepository _question;

        private readonly QuizRepository _quiz;

        private readonly CategoryRepository _category;

        public ObservableCollection<QuestionModel> QuestionsInQuiz { get; set; } = new();

        public ObservableCollection<QuizModel> AvailableQuizzes { get; set; } = new();

        public ObservableCollection<CategoryModel> CategoriesInSelectedQuiz { get; set; } = new();

        public QuizModel? SelectedQuiz { get; set; } = new();

        public QuestionModel? SelectedQuestionInSelectedQuiz { get; set; } = new();

        public QuestionModel? SelectedCategoryInSelectedQuiz { get; set; } = new();

        public static event Action<QuizModel> GameStarted;

        public PreGameView()
        {
            InitializeComponent();

            DataContext = this;

            AddQuizView.QuizzesUpdated += AddQuizViewOnQuizzesUpdated;

            _question = new QuestionRepository();
            _quiz = new QuizRepository();
            _category = new CategoryRepository();

            PopulateQuizzes();
        }

        private void AddQuizViewOnQuizzesUpdated()
        {
            PopulateQuizzes();
        }

        private void PopulateQuizzes()
        {
            AvailableQuizzes.Clear();
            var allQuizzes = _quiz.GetAllQuizzes();

            foreach (var q in allQuizzes)
            {
                AvailableQuizzes.Add(new QuizModel
                {
                    Title = q.title,
                    Description = q.description,
                    Id = q.id
                });
            }
        }

        private void AvailableQuizzesList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedQuiz is null)
            {
                return;
            }

            QuestionsInQuiz.Clear();
            CategoriesInSelectedQuiz.Clear();

            var questionsInSelectedQuiz = _quiz.DisplayQuestionsInQuiz(SelectedQuiz.Id);

            if (questionsInSelectedQuiz is null)
            {
                return;
            }

            foreach (var q in questionsInSelectedQuiz)
            {
                QuestionsInQuiz.Add(new QuestionModel
                {
                    QuestionText = q.questionText,
                    Id = q.id
                });

                var category = _category.GetCategoryByName(q.category);

                if (category != null && CategoriesInSelectedQuiz.All(c => c.Id != category.Id.ToString()))
                {
                    CategoriesInSelectedQuiz.Add(new CategoryModel()
                    {
                        Id = category.Id.ToString(),
                        InternalId = category.InternalId,
                        CategoryName = category.CategoryName
                    });
                }
            }
            QuizDescriptionTb.Text = SelectedQuiz.Description;
        }

        private void StartGameBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedQuiz.Id is null)
            {
                MessageBox.Show("No quiz was selected");
                return;
            }

            GameStarted.Invoke(SelectedQuiz);
        }

        private void QuitGameBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var message = "Are you sure?";
            var caption = "Quit game";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);

            if (result.Equals(MessageBoxResult.No))
            {
                return;
            }

            Application.Current.Shutdown();
        }
    }
}
