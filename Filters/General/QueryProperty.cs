using Filtering.Enums;

namespace Filtering.General
{
    class QueryProperty<TValue>
    {
        public string Key { get; set; }
        public TValue Value { get; set; }
        public ConcatenationOperation ConcatenationOperation { get; set; }
    }
}
