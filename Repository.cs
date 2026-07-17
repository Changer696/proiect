using System.Collections.Generic;
using System.Linq;

public class Repository<T> where T : class
{
    protected List<T> _items = new List<T>();

    public int Count => _items.Count;

    public virtual bool Add(T item)
    {
        if (item == null)
            return false;

        _items.Add(item);
        return true;
    }

    public virtual bool Remove(T item)
    {
        return _items.Remove(item);
    }

    public List<T> GetAll()
    {
        return _items.ToList();
    }

    public T Find(System.Func<T, bool> predicate) => _items.FirstOrDefault(predicate);

    public bool Exists(System.Func<T, bool> predicate) => _items.Any(predicate);

    public bool RemoveWhere(System.Func<T, bool> predicate)
    {
        var item = _items.FirstOrDefault(predicate);
        return item != null && Remove(item);
    }

    public List<T> GetWhere(System.Func<T, bool> predicate) => _items.Where(predicate).ToList();

    public void Clear()
    {
        _items.Clear();
    }
}


public class RepositoryWithId<T> : Repository<T> where T : class, IIdentifiable
{
    public T FindById(string id) => _items.FirstOrDefault(x => x.Id == id);

    public bool ExistsById(string id) => _items.Any(x => x.Id == id);

    public bool RemoveById(string id)
    {
        var item = FindById(id);
        return item != null && Remove(item);
    }
}


public interface IIdentifiable
{
    string Id { get; }
}


public class StackRepository<T> where T : class
{
    private Stack<T> _items = new Stack<T>();

    public int Count => _items.Count;

    public void Push(T item)
    {
        if (item != null)
            _items.Push(item);
    }

    public T Pop()
    {
        return _items.Count > 0 ? _items.Pop() : null;
    }

    public T Peek()
    {
        return _items.Count > 0 ? _items.Peek() : null;
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool IsEmpty => !_items.Any();

    public List<T> GetAll()
    {
        return _items.ToList();
    }
}


public class QueueRepository<T> where T : class
{
    private Queue<T> _items = new Queue<T>();

    public int Count => _items.Count;

    public void Enqueue(T item)
    {
        if (item != null)
            _items.Enqueue(item);
    }

    public T Dequeue()
    {
        return _items.Count > 0 ? _items.Dequeue() : null;
    }

    public T Peek()
    {
        return _items.Count > 0 ? _items.Peek() : null;
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool IsEmpty => !_items.Any();
}

public class HashSetRepository<T> where T : class
{
    private HashSet<T> _items = new HashSet<T>();

    public int Count => _items.Count;

    public bool Add(T item)
    {
        return item != null && _items.Add(item);
    }

    public bool Remove(T item)
    {
        return _items.Remove(item);
    }

    public List<T> GetAll()
    {
        return _items.ToList();
    }

    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool IsEmpty => !_items.Any();
}

public class DictionaryRepository<TKey, TValue> where TValue : class
{
    private Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue>();

    public int Count => _items.Count;

    public bool Add(TKey key, TValue value)
    {
        if (value == null || _items.ContainsKey(key))
            return false;
        _items[key] = value;
        return true;
    }

    public bool Remove(TKey key)
    {
        return _items.Remove(key);
    }

    public TValue GetByKey(TKey key)
    {
        if (_items.ContainsKey(key))
            return _items[key];
        return null;
    }

    public bool ContainsKey(TKey key)
    {
        return _items.ContainsKey(key);
    }

    public List<TValue> GetAll()
    {
        return _items.Values.ToList();
    }

    public List<TKey> GetAllKeys()
    {
        return _items.Keys.ToList();
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool IsEmpty => !_items.Any();
    
}
