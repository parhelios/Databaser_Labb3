using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Services;
using Front.Models;

namespace Front.Views
{
    /// <summary>
    /// Interaction logic for EditQuizView.xaml
    /// </summary>
    public partial class EditQuizView : UserControl
    {
        private readonly QuestionRepository _question;

        private readonly QuizRepository _quiz;

        private readonly CategoryRepository _category;

        public ObservableCollection<QuestionModel> QuestionList { get; set; } = new();

        public ObservableCollection<QuizModel> QuizList { get; set; } = new();

        public ObservableCollection<QuestionModel> QuestionsInSelectedQuizList { get; set; } = new();

        public QuestionModel? SelectedQuestion { get; set; } = new();

        public QuizModel? SelectedQuiz { get; set; } = new();

        public QuestionModel? SelectedQuestionInSelectedQuiz { get; set; } = new();

        public EditQuizView()
        {
            InitializeComponent();

            DataContext = this;

            EditQuestionView.QuestionsUpdated += EditQuestionViewOnQuestionsUpdated;
            EditCategoryView.CategoriesUpdated += EditCategoryView_CategoriesUpdated;
            AddQuizView.QuizzesUpdated += AddQuizViewOnQuizzesUpdated;

            _question = new QuestionRepository();
            _quiz = new QuizRepository();
            _category = new CategoryRepository();

            PopulateQuestionsInDatabase();
            PopulateCategoriesCb();
            PopulateQuizzes();
        }

        private void AddQuizViewOnQuizzesUpdated()
        {
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
                    Id = q.id
                });
            }
        }

        private void EditCategoryView_CategoriesUpdated()
        {
            PopulateCategoriesCb();
        }

        private void PopulateCategoriesCb()
        {
            SortByCategoryCb.Items.Clear();
            var allCategories = _category.GetAllCategories();
            foreach (var c in allCategories)
            {
                SortByCategoryCb.Items.Add(c.categoryName);
            }
        }

        private void EditQuestionViewOnQuestionsUpdated()
        {
            QuestionList.Clear();

            PopulateQuestionsInDatabase();
        }

        private void PopulateQuestionsInDatabase()
        {
            QuestionList.Clear();
            var allQuestions = _question.GetAllQuestions();

            foreach (var questions in allQuestions)
            {
                QuestionList.Add(new QuestionModel
                {
                    QuestionText = questions.questionText,
                    Id = questions.id,
                });
            }
        }

        private void PopulateQuestionsInQuizList()
        {
            QuestionsInSelectedQuizList.Clear();

            var questionsInSelectedQuiz = _quiz.DisplayQuestionsInQuiz(SelectedQuiz.Id);

            if (questionsInSelectedQuiz is null)
            {
                return;
            }

            foreach (var q in questionsInSelectedQuiz)
            {
                QuestionsInSelectedQuizList.Add(new QuestionModel
                {
                    QuestionText = q.questionText,
                    Id = q.id
                });
            }
        }

        private void QuizDatabaseList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedQuiz is null)
            {
                return;
            }

            PopulateQuestionsInQuizList();
        }

        private void AddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedQuiz is null || SelectedQuestion is null)
            {
                return;
            }

            _quiz.AddQuestionToSelectedQuiz(SelectedQuiz.Id, SelectedQuestion.Id);

            PopulateQuestionsInQuizList();
        }

        private void RemoveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedQuiz is null || SelectedQuestion is null)
            {
                return;
            }

            _quiz.RemoveQuestionFromSelectedQuiz(SelectedQuiz.Id, SelectedQuestionInSelectedQuiz.Id);

            PopulateQuestionsInQuizList();
        }

        private void TheRollBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://www.youtube.com/watch?v=dQw4w9WgXcQ", UseShellExecute = true });
        }


        private void SortByCategoryCb_OnDropDownClosed(object? sender, EventArgs e)
        {
            if (SortByCategoryCb.SelectedItem is null)
            {
                return;
            }
            var sortedQuestions = new List<QuestionModel>();

            var selectedCategory = SortByCategoryCb.SelectedItem.ToString();

            var allQuestions = _question.GetAllQuestions();

            foreach (var q in allQuestions)
            {
                if (q.category == selectedCategory)
                {
                    sortedQuestions.Add(new QuestionModel()
                    {
                        QuestionText = q.questionText
                    });   
                }
            }

            QuestionList.Clear();

            foreach (var q in sortedQuestions)
            {
                QuestionList.Add(q);
            }
        }

        private void ResetSortingBtn_OnClick(object sender, RoutedEventArgs e)
        {
            QuestionList.Clear();

            PopulateQuestionsInDatabase();
        }

        private void SearchForQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (SearchForQuestionTxt.Text == "")
            {
                MessageBox.Show("Please enter a question to search for");
                return;
            }

            var sortedQuestion = new List<QuestionModel>();

            var search = SearchForQuestionTxt.Text;

            var allQuestions = _question.GetAllQuestions();

            foreach (var q in allQuestions)
            {
                if (q.questionText.ToLower().Contains(search.ToLower()))
                {
                    sortedQuestion.Add(new QuestionModel()
                    {
                        QuestionText = q.questionText
                    });
                }
            }

            if (sortedQuestion.Count == 0)
            {
                MessageBox.Show("No questions found");
                return;
            }

            QuestionList.Clear();

            foreach (var q in sortedQuestion)
            {
                QuestionList.Add(q);
            }

            SearchForQuestionTxt.Clear();
        }
    }
}
