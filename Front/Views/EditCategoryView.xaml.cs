using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Common.DTO;
using DataAccess.Services;
using Front.Models;

namespace Front.Views
{
    /// <summary>
    /// Interaction logic for EditCategoryView.xaml
    /// </summary>
    public partial class EditCategoryView : UserControl
    {
        private readonly QuestionRepository _question;

        private readonly CategoryRepository _category;

        public ObservableCollection<CategoryModel> CategoriesInDb { get; set; } = new();

        public CategoryModel? SelectedCategory { get; set; } = new();

        public static event Action CategoriesUpdated;

        public EditCategoryView()
        {
            InitializeComponent();

            DataContext = this;

            _question = new QuestionRepository();
            _category = new CategoryRepository();
            
            PopulateCategoriesInDb();
        }

        private void PopulateCategoriesInDb()
        {
            CategoriesInDb.Clear();
            var categories = _category.GetAllCategories();

            foreach (var category in categories)
            {
                CategoriesInDb.Add(new CategoryModel()
                {
                    Id = category.id,
                    InternalId = category.internalId,
                    CategoryName = category.categoryName
                });
            }
        }

        private void AddCategoryBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (CategoryTxt.Text == "")
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            var numberOfCategories = _category.GetAllCategories().Count();

            var categoryRecord = new CategoryRecord(
                id: "",
                internalId: numberOfCategories,
                categoryName: CategoryTxt.Text
            );

            _category.AddCategory(categoryRecord);

            MessageBox.Show("Category added successfully");

            CategoriesInDb.Clear();
            PopulateCategoriesInDb();
            CategoriesUpdated.Invoke();
            CategoryTxt.Clear();
        }

        private void ClearFieldsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CategoryTxt.Clear();
        }

        private void DeleteCategoryBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var message = "Are you sure?";
            var caption = "Delete category";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);

            if (result.Equals(MessageBoxResult.No))
            {
                return;
            }

            if (CategoriesInDbList.SelectedItem is CategoryModel selected)
            {
                _category.DeleteCategory(selected.Id);
            }

            CategoriesUpdated.Invoke();
            PopulateCategoriesInDb();
            MessageBox.Show("Category deleted successfully");
        }
    }
}
