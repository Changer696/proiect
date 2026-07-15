using System;

namespace SmartFactorySimple
{
    
    public class UndoAction
    {
        public string Description { get; }
        public Action Undo { get; }

        public UndoAction(string description, Action undo)
        {
            Description = description;
            Undo = undo;
        }
    }

   
    public static class UndoManager
    {
        private static readonly StackRepository<UndoAction> _stack = new StackRepository<UndoAction>();

        public static bool HasUndo => !_stack.IsEmpty;

       
        public static void Register(string description, Action undo)
        {
            if (undo == null)
                return;

            _stack.Push(new UndoAction(description, undo));
        }

        public static bool TryUndoLast(out string description)
        {
            description = null;
            UndoAction action = _stack.Pop();
            if (action == null)
                return false;

            description = action.Description;

            try
            {
                action.Undo();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Undo failed: {ex.Message}");
                return false;
            }

            return true;
        }

        
        public static void Clear()
        {
            _stack.Clear();
        }
    }
}