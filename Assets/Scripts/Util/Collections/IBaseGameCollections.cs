interface IBaseGameCollections
{
    int GetSize();
    bool IsEmpty();
    PathNode Peek();
    PathNode Poll();
    void Add(PathNode node);
}