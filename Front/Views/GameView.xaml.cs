using Front.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Services;

namespace Front.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        private readonly QuizRepository _quiz;

        private readonly CategoryRepository _category;

        public ObservableCollection<QuestionModel> QuestionsInActiveQuiz { get; set; } = new();

        private List<string> AnsweredQuestionsInActiveQuiz { get; set; } = new();

        private int _quizQuestionCounter;

        private int _nextQuestion;

        private int _activeQuizScore;

        private Random random = new Random();

        public static event Action GameFinished;

        public GameView()
        {
            InitializeComponent();
            PreGameView.GameStarted += PreGameViewOnGameStarted;

            DataContext = this;

            _quiz = new QuizRepository();
            _category = new CategoryRepository();
        }

        private void PreGameViewOnGameStarted(QuizModel obj)
        {
            var activeQuiz = _quiz.DisplayQuestionsInQuiz(obj.Id);

            CurrentQuizBlock.Text = obj.Title;

            foreach (var q in activeQuiz)
            {
                var category = _category.GetCategoryByName(q.category);

                QuestionsInActiveQuiz.Add(new QuestionModel
                {
                    QuestionText = q.questionText,
                    CorrectAnswer = q.correctAnswer,
                    IncorrectAnswer = q.incorrectAnswers,
                    Id = q.id,
                    Category = category
                });
            }

            SetupNextQuestion();
        }

        private void SetupNextQuestion()
        {
            CurrentScoreBlock.Text = _activeQuizScore.ToString();

            if (_quizQuestionCounter == QuestionsInActiveQuiz.Count)
            {
                MessageBox.Show($"Game finished! \nYour total score: {_activeQuizScore}/{QuestionsInActiveQuiz.Count}");

                GameFinished.Invoke();

                _activeQuizScore = 0;
                _quizQuestionCounter = 0;
                _nextQuestion = 0;
                QuestionsInActiveQuiz.Clear();
                AnsweredQuestionsInActiveQuiz.Clear();
                return;
            }

            _nextQuestion = random.Next(0, QuestionsInActiveQuiz.Count);
            var currentQuestion = QuestionTxtBlock.Text = QuestionsInActiveQuiz[_nextQuestion].QuestionText;

            if (AnsweredQuestionsInActiveQuiz.Contains(currentQuestion))
            {
                SetupNextQuestion();
                return;
            }

            AnsweredQuestionsInActiveQuiz.Add(currentQuestion);

            var answers = new List<string>
            {
                QuestionsInActiveQuiz[_nextQuestion].CorrectAnswer,
                QuestionsInActiveQuiz[_nextQuestion].IncorrectAnswer[0],
                QuestionsInActiveQuiz[_nextQuestion].IncorrectAnswer[1]
            };

            // Fisher-Yates shuffle
            int n = answers.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                string value = answers[k];
                answers[k] = answers[n];
                answers[n] = value;
            }

            Option1Rb.Content = answers[0];
            Option2Rb.Content = answers[1];
            Option3Rb.Content = answers[2];
        }

        private void CheckQuestionResult(int questionNumber)
        {

            if (Option1Rb.Content.Equals(QuestionsInActiveQuiz[questionNumber].CorrectAnswer))
            {
                if (Option1Rb.IsChecked == true)
                {
                    _activeQuizScore += 1;
                    _quizQuestionCounter += 1;
                    return;
                }
            }
            if (Option2Rb.Content.Equals(QuestionsInActiveQuiz[questionNumber].CorrectAnswer))
            {
                if (Option2Rb.IsChecked == true)
                {
                    _activeQuizScore += 1;
                    _quizQuestionCounter += 1;
                    return;
                }
            }
            if (Option3Rb.Content.Equals(QuestionsInActiveQuiz[questionNumber].CorrectAnswer))
            {
                if (Option3Rb.IsChecked == true)
                {
                    _activeQuizScore += 1;
                    _quizQuestionCounter += 1;
                    return;
                }
            }

            _quizQuestionCounter += 1;
            return;
        }

        private void NextQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (Option1Rb.IsChecked == false && Option2Rb.IsChecked == false && Option3Rb.IsChecked == false)
            {
                MessageBox.Show("Please select an answer");
                return;
            }

            CheckQuestionResult(_nextQuestion);

            SetupNextQuestion();
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
