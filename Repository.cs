using System.Collections.Generic;
using System.Linq;

public class Repository<T> where T : class
{
    protected List<T> _items = new List<T>();

    public int Count => _items.Count;

    // Adds an item to the repository; returns false for null.
    public virtual bool Add(T item)
    {
        if (item == null)
            return false;

        _items.Add(item);
        return true;
    }

    // Removes an item from the repository.
    public virtual bool Remove(T item)
    {
        return _items.Remove(item);
    }

    // Returns a shallow copy list of all repository items.
    public List<T> GetAll()
    {
        return _items.ToList();
    }

    // Finds the first item matching the predicate or null.
    public T Find(System.Func<T, bool> predicate) => _items.FirstOrDefault(predicate);

    // Checks if any item matches the predicate.
    public bool Exists(System.Func<T, bool> predicate) => _items.Any(predicate);

    // Removes the first item matching the predicate.
    public bool RemoveWhere(System.Func<T, bool> predicate)
    {
        var item = _items.FirstOrDefault(predicate);
        return item != null && Remove(item);
    }

    // Returns all items that match the predicate.
    public List<T> GetWhere(System.Func<T, bool> predicate) => _items.Where(predicate).ToList();

    // Removes all items from the repository.
    public void Clear()
    {
        _items.Clear();
    }
}


public class RepositoryWithId<T> : Repository<T> where T : class, IIdentifiable
{
    // Finds an item by its identifier.
    public T FindById(string id) => _items.FirstOrDefault(x => x.Id == id);

    // Returns whether an item with the given id exists.
    public bool ExistsById(string id) => _items.Any(x => x.Id == id);

    // Removes an item by id.
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

    // Pushes an item onto the stack.
    public void Push(T item)
    {
        if (item != null)
            _items.Push(item);
    }

    // Pops and returns the top item, or null if empty.
    public T Pop()
    {
        return _items.Count > 0 ? _items.Pop() : null;
    }

    // Peeks at the top item without removing it, or returns null if empty.
    public T Peek()
    {
        return _items.Count > 0 ? _items.Peek() : null;
    }

    // Clears the stack.
    public void Clear()
    {
        _items.Clear();
    }

    public bool IsEmpty => !_items.Any();

    // Returns all items in stack order as a list.
    public List<T> GetAll()
    {
        return _items.ToList();
    }
}


public class QueueRepository<T> where T : class
{
    private Queue<T> _items = new Queue<T>();

    public int Count => _items.Count;

    // Enqueues an item.
    public void Enqueue(T item)
    {
        if (item != null)
            _items.Enqueue(item);
    }

    // Dequeues and returns an item, or null if empty.
    public T Dequeue()
    {
        return _items.Count > 0 ? _items.Dequeue() : null;
    }

    // Peeks at the next item to be dequeued, or null if empty.
    public T Peek()
    {
        return _items.Count > 0 ? _items.Peek() : null;
    }

    // Clears the queue.
    public void Clear()
    {
        _items.Clear();
    }

    // Returns whether the queue is empty.
    public bool IsEmpty => !_items.Any();
}

public class HashSetRepository<T> where T : class
{
    private HashSet<T> _items = new HashSet<T>();

    public int Count => _items.Count;

    // Adds an item to the hashset repository.
    public bool Add(T item)
    {
        return item != null && _items.Add(item);
    }

    // Removes an item from the hashset repository.
    public bool Remove(T item)
    {
        return _items.Remove(item);
    }

    // Returns all items as a list.
    public List<T> GetAll()
    {
        return _items.ToList();
    }

    // Checks whether the repository contains the specified item.
    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    // Clears the hashset repository.
    public void Clear()
    {
        _items.Clear();
    }

    // Returns whether the repository is empty.
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
