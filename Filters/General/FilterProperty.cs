using Filtering.Enums;

namespace Filtering.General
{
    class FilterProperty<TOperation>
    {
        public string Key { get; set; }
        public dynamic Value { get; set; }
        public TOperation Operation { get; set; }
        public ConcatenationOperation ConcatenationOperation { get; set; }
    }
}
