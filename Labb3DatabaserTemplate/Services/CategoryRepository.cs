using Common.DTO;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccess.Services;

public class CategoryRepository
{
    public readonly IMongoCollection<Category> _categories;

    public CategoryRepository()
    {
        var databaseName = "db_labb3";

        var client = new MongoClient($"mongodb+srv://admin:SEq1X2pxF62349lI@cluster01.koulmbi.mongodb.net/");
        var database = client.GetDatabase(databaseName);
        _categories = database.GetCollection<Category>("category_db", new MongoCollectionSettings { AssignIdOnInsert = true });
    }

    public List<CategoryRecord> GetAllCategories()
    {
        var filter = Builders<Category>.Filter.Empty;

        var allCategories = _categories
            .Find(filter).ToList()
            .Select(c => new CategoryRecord(
                               c.Id.ToString(),
                               c.InternalId,
                               c.CategoryName
            )
            );

        return allCategories.ToList();
    }

    public Category GetCategoryByName(string name)
    {
        var filter = Builders<Category>.Filter.Eq("CategoryName", name);
        var category = _categories.Find(filter).FirstOrDefault();

        return category;
    }

    public Category GetCategoryById(string id)
    {

        //TODO fixa
        var filter = Builders<Category>.Filter.Eq("_id", ObjectId.Parse(id));
        var category = _categories.Find(filter).FirstOrDefault();

        return category;
    }

    public void AddCategory(CategoryRecord categoryRecord)
    {
        var category = new Category
        {
            CategoryName = categoryRecord.categoryName,
            InternalId = categoryRecord.internalId
        };

        _categories.InsertOne(category);
    }

    public void DeleteCategory(string selectedId)
    {
        var filter = Builders<Category>.Filter.Eq("_id", ObjectId.Parse(selectedId));

        if (filter is null)
        {
            return;
        }

        _categories.DeleteOne(filter);
    }
}