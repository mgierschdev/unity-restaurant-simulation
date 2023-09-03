using Util.PathFinding;

namespace Util.Collections
{
    interface IBaseGameCollections
    {
        int GetSize();
        bool IsEmpty();
        PathNode Peek();
        PathNode Poll();
        void Add(PathNode node);
    }
}