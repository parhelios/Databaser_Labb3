using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataAccess.Entities;

namespace Front.Models;

public class QuizModel : INotifyPropertyChanged
{
    public string Id { get; set; }

    private string _title;

    public string Title 
    {
        get { return _title; }
        set
        {
            _title = value; 
            OnPropertyChanged();
        }
    }

    private string _description;

    public string Description
    {
        get { return _description; }
        set
        {
            _description = value; 
            OnPropertyChanged();
        }
    }

    private List<Category> _categories;

    public List<Category> Categories       
    {
        get { return _categories; }
        set
        {
            _categories = value; 
            OnPropertyChanged();
        }
    }

    private List<Question> _questions;

    public List<Question> Questions
    {
        get { return _questions; }
        set
        {
            _questions = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}