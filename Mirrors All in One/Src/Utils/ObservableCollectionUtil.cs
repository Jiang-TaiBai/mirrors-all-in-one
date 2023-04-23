using System.Collections.ObjectModel;
using System.Linq;

namespace Mirrors_All_in_One.Utils
{
    public abstract class ObservableCollectionUtil
    {
        public static void Swap<T>(ObservableCollection<T> collection, int i, int j)
        {
            if (i == j) return;
            if (i < 0 || i >= collection.Count) return;
            if (j < 0 || j >= collection.Count) return;
            var temp = collection.ElementAt(i);
            collection[i] = collection.ElementAt(j);
            collection[j] = temp;
        }
    }
}