using Echo.Core.Graphing;

namespace Echo.Ast
{
    /// <summary>
    /// Provides a base contract for all Ast nodes
    /// </summary>
    public abstract class AstNodeBase<TInstruction> : TreeNodeBase
    {
        /// <summary>
        /// Assigns the unique ID to the node
        /// </summary>
        /// <param name="id">The unique identifier</param>
        protected AstNodeBase(long id)
            : base(id) { }

        /// <summary>
        /// Implements the visitor pattern
        /// </summary>
        public abstract void Accept<TState>(IAstNodeVisitor<TInstruction, TState> visitor, TState state);

        /// <summary>
        /// Implements the visitor pattern
        /// </summary>
        public abstract TOut Accept<TState, TOut>(IAstNodeVisitor<TInstruction, TState, TOut> visitor, TState state);
    }
}