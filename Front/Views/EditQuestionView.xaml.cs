using System.Windows;
using System.Windows.Controls;
using Common.DTO;
using DataAccess.Services;

namespace Front.Views
{
    /// <summary>
    /// Interaction logic for EditQuestionView.xaml
    /// </summary>
    public partial class EditQuestionView : UserControl
    {
        private readonly QuestionRepository _question;

        private readonly CategoryRepository _category;

        public static event Action QuestionsUpdated;

        public EditQuestionView()
        {
            InitializeComponent();

            DataContext = this;

            EditCategoryView.CategoriesUpdated += EditCategoryView_CategoriesUpdated;

            _question = new QuestionRepository();

            _category = new CategoryRepository();
            
            PopulateCategoriesCombobox();
        }

        private void EditCategoryView_CategoriesUpdated()
        {
            CategoriesCb.Items.Clear();

            PopulateCategoriesCombobox();
        }

        private void PopulateCategoriesCombobox()
        {
            var allCategories = _category.GetAllCategories();
            foreach (var c in allCategories)
            {
                CategoriesCb.Items.Add(c.categoryName);
            }
        }

        private void AddQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (QuestionTxt.Text == "" || Answer1Txt.Text == "" || Answer2Txt.Text == "" || Answer3Txt.Text == "")
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            if (CategoriesCb.SelectedItem == null)
            {
                MessageBox.Show("Please select a category");
                return;
            }

            if (Answer1Rb.IsChecked == false && Answer2Rb.IsChecked == false && Answer3Rb.IsChecked == false)
            {
                MessageBox.Show("Please select a correct answers");
                return;
            }

            var selectedAnswerIndex = 0; 

            if (Answer1Rb.IsChecked == true)
            {
                selectedAnswerIndex = 1;
            }
            else if (Answer2Rb.IsChecked == true)
            {
                selectedAnswerIndex = 2;
            }
            else if (Answer3Rb.IsChecked == true)
            {
                selectedAnswerIndex = 3;
            }

            var (correctAnswer, incorrectAnswers) = SortAnswers(selectedAnswerIndex);

            var questionRecord = new QuestionRecord(
                "",
                CategoriesCb.SelectedItem.ToString(), 
                QuestionTxt.Text, 
                correctAnswer, 
                incorrectAnswers
                );

            _question.AddQuestion(questionRecord);

            MessageBox.Show("Question added successfully");
            ClearAllFields();
            QuestionsUpdated.Invoke();
        }

        private (string selectedAnswer, string[] incorrectAnswers) SortAnswers(int selectedIndex)
        {
            var selectedAnswer = "";
            var incorrectAnswers = new string[2];

            switch (selectedIndex)
            {
                case 1:
                    selectedAnswer = Answer1Txt.Text;
                    incorrectAnswers[0] = Answer2Txt.Text;
                    incorrectAnswers[1] = Answer3Txt.Text;
                    break;
                case 2:
                    selectedAnswer = Answer2Txt.Text;
                    incorrectAnswers[0] = Answer1Txt.Text;
                    incorrectAnswers[1] = Answer3Txt.Text;
                    break;
                case 3:
                    selectedAnswer = Answer3Txt.Text;
                    incorrectAnswers[0] = Answer1Txt.Text;
                    incorrectAnswers[1] = Answer2Txt.Text;
                    break;
                default:
                    throw new ArgumentException("Invalid answer");
            }

            return (selectedAnswer, incorrectAnswers);
        }

        private void ClearFieldsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ClearAllFields();
        }

        private void ClearAllFields()
        {
            QuestionTxt.Clear();
            Answer1Txt.Clear();
            Answer2Txt.Clear();
            Answer3Txt.Clear();
        }
    }
}
