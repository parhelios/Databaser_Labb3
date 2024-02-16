using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataAccess.Entities;

namespace Front.Models;

public class QuestionModel : INotifyPropertyChanged
{
    public string Id { get; set; }
    public Category Category { get; set; }

    private string _questionText;

    public string QuestionText
    {
        get { return _questionText; }
        set
        {
            _questionText = value;
            OnPropertyChanged();
        }
    }

    private string _correctAnswer;

    public string CorrectAnswer
    {
        get { return _correctAnswer; }
        set
        {
            _correctAnswer = value; 
            OnPropertyChanged();
        }
    }

    private string[] _incorrectAnswers;

    public string[] IncorrectAnswer
    {
        get { return _incorrectAnswers; }
        set
        {
            _incorrectAnswers = value; 
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