namespace SqlTools.Models
{
    public class CheckBoxEntry<T>
    {
        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public T Value { get; set; }
    }

    public class CheckBoxEntry
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}