using System;
using System.Collections.Generic;
using System.Linq;

namespace CompositePattern.GenericCompositePattern
{
    /// <summary>
    /// Represents a generic composite structure that supports tree-like hierarchies.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the component.</typeparam>
    public class GenericComposite<T>
    {
        private readonly List<GenericComposite<T>> _children = new();
        private bool _canHaveChildren;

        /// <summary>
        /// Gets the data stored in this component.
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Gets the parent of this component, or null if it is the root.
        /// </summary>
        public GenericComposite<T> Parent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericComposite{T}"/> class.
        /// </summary>
        /// <param name="data">The data to store in this component.</param>
        /// <param name="canHaveChildren">Indicates whether this component can have children.</param>
        public GenericComposite(T data, bool canHaveChildren = true)
        {
            Data = data;
            _canHaveChildren = canHaveChildren;
        }

        /// <summary>
        /// Adds a child component to this component.
        /// </summary>
        /// <param name="genericComposite">The child component to add.</param>
        /// <exception cref="InvalidOperationException">Thrown if this component cannot have children.</exception>
        public void Add(GenericComposite<T> genericComposite)
        {
            if (!_canHaveChildren)
                throw new InvalidOperationException("This component cannot have children.");

            genericComposite.Parent = this;
            _children.Add(genericComposite);
        }

        /// <summary>
        /// Removes a child component from this component.
        /// </summary>
        /// <param name="genericComposite">The child component to remove.</param>
        /// <exception cref="InvalidOperationException">Thrown if this component cannot have children.</exception>
        public void Remove(GenericComposite<T> genericComposite)
        {
            if (!_canHaveChildren)
                throw new InvalidOperationException("This component cannot have children.");

            genericComposite.Parent = null;
            _children.Remove(genericComposite);
        }

        /// <summary>
        /// Sets whether this component can have children and clears children if not.
        /// </summary>
        /// <param name="canHaveChildren">Indicates whether this component can have children.</param>
        public void SetFinal(bool canHaveChildren)
        {
            _canHaveChildren = canHaveChildren;

            if (!_canHaveChildren)
                _children.Clear();
        }

        /// <summary>
        /// Gets the immediate children of this component.
        /// </summary>
        /// <returns>An enumerable of child components.</returns>
        public IEnumerable<GenericComposite<T>> GetChildren() => _canHaveChildren ? _children : Enumerable.Empty<GenericComposite<T>>();

        /// <summary>
        /// Gets the full hierarchical path from the root to this component.
        /// </summary>
        /// <returns>A string representing the path.</returns>
        public string GetPath()
        {
            if (Parent == null)
            {
                return "root";
            }

            int index = Parent._children.IndexOf(this);

            return $"{Parent.GetPath()} -> {index}.child";
        }

        /// <summary>
        /// Gets the depth of this component in the tree.
        /// </summary>
        /// <returns>The depth of this component.</returns>
        public int GetDepth()
        {
            return Parent == null ? 0 : 1 + Parent.GetDepth();
        }

        /// <summary>
        /// Finds a component based on a predicate and search type.
        /// </summary>
        /// <param name="predicate">The condition to match.</param>
        /// <param name="searchType">The search type (DepthFirst or BreadthFirst).</param>
        /// <returns>The first component that matches the condition, or null if none found.</returns>
        public GenericComposite<T> Find(Func<GenericComposite<T>, bool> predicate, SearchType searchType = SearchType.DepthFirst)
        {
            return searchType switch
            {
                SearchType.DepthFirst => FindDepthFirst(predicate),
                SearchType.BreadthFirst => FindBreadthFirst(predicate),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private GenericComposite<T> FindDepthFirst(Func<GenericComposite<T>, bool> predicate)
        {
            if (predicate(this))
                return this;

            foreach (var child in _children)
            {
                var result = child.FindDepthFirst(predicate);
                if (result != null)
                    return result;
            }

            return null;
        }

        private GenericComposite<T> FindBreadthFirst(Func<GenericComposite<T>, bool> predicate)
        {
            var queue = new Queue<GenericComposite<T>>();
            queue.Enqueue(this);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                if (predicate(current))
                    return current;

                foreach (var child in current._children)
                {
                    queue.Enqueue(child);
                }
            }

            return null;
        }

        /// <summary>
        /// Indicates whether this component is a leaf (has no children).
        /// </summary>
        public bool IsLeaf => !_canHaveChildren;

        /// <summary>
        /// Traverses the component hierarchy and performs an action on each component.
        /// </summary>
        /// <param name="action">The action to perform on each component.</param>
        /// <param name="traversalType">The type of traversal (PreOrder, PostOrder, LevelOrder).</param>
        public void Traverse(Action<GenericComposite<T>> action, TraversalType traversalType = TraversalType.PreOrder)
        {
            switch (traversalType)
            {
                case TraversalType.PreOrder:
                    TraversePreOrder(action);
                    break;
                case TraversalType.PostOrder:
                    TraversePostOrder(action);
                    break;
                case TraversalType.LevelOrder:
                    TraverseLevelOrder(action);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TraversePreOrder(Action<GenericComposite<T>> action)
        {
            action(this);
            foreach (var child in _children)
                child.TraversePreOrder(action);
        }

        private void TraversePostOrder(Action<GenericComposite<T>> action)
        {
            foreach (var child in _children)
                child.TraversePostOrder(action);
            action(this);
        }

        private void TraverseLevelOrder(Action<GenericComposite<T>> action)
        {
            var queue = new Queue<GenericComposite<T>>();
            queue.Enqueue(this);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                action(current);

                foreach (var child in current._children)
                    queue.Enqueue(child);
            }
        }
    }
    
    public enum SearchType
    {
        DepthFirst,
        BreadthFirst
    }

    public enum TraversalType
    {
        PreOrder,
        PostOrder,
        LevelOrder
    }

    public class TreeNode<T>
    {
        public T Data { get; set; }
        public List<TreeNode<T>> Children { get; set; }

        public void Method()
        {
            GenericComposite<int> root = new GenericComposite<int>(3);
        }
    }
}