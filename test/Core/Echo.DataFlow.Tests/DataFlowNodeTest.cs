using System;
using System.Collections.Generic;
using Xunit;

namespace Echo.DataFlow.Tests
{
    public class DataFlowNodeTest
    {
        [Fact]
        public void ConstructorShouldSetContents()
        {
            var node = new DataFlowNode<int>(1, 2);
            Assert.Equal(2, node.Contents);
        }

        [Fact]
        public void AddNodeToGraphShouldSetParentGraph()
        {
            var dfg = new DataFlowGraph<int>();
            var node = new DataFlowNode<int>(1, 2);
            dfg.Nodes.Add(node);
            Assert.Same(dfg, node.ParentGraph);
        }

        [Fact]
        public void RemoveNodeFromGraphShouldUnsetParentGraph()
        {
            var dfg = new DataFlowGraph<int>();
            var node = new DataFlowNode<int>(1, 2);
            dfg.Nodes.Add(node);
            dfg.Nodes.Remove(node);
            Assert.Null(node.ParentGraph);
        }
        
        [Fact]
        public void AddStackDependencyShouldSetDependant()
        {
            var dfg = new DataFlowGraph<int>();
            var n0 = dfg.Nodes.Add(0, 0);
            var n1 = dfg.Nodes.Add(1, 1);

            var dependency = new DataDependency<int>(n0);
            n1.StackDependencies.Add(dependency);
            
            Assert.Same(n1, dependency.Dependant);
        }

        [Fact]
        public void RemoveStackDependencyShouldUnsetDependant()
        {
            var dfg = new DataFlowGraph<int>();
            var n0 = dfg.Nodes.Add(0, 0);
            var n1 = dfg.Nodes.Add(1, 1);

            var symbolicValue = new DataDependency<int>(n0);
            n1.StackDependencies.Add(symbolicValue);
            n1.StackDependencies.Remove(symbolicValue);
            Assert.Null(symbolicValue.Dependant);
        }

        [Fact]
        public void AddStackDependencyShouldAddToDependants()
        {
            var dfg = new DataFlowGraph<int>();
            var n0 = dfg.Nodes.Add(0, 0);
            var n1 = dfg.Nodes.Add(1, 1);
            var n2 = dfg.Nodes.Add(2, 2);

            var dependency1 = new DataDependency<int>(n0);
            n1.StackDependencies.Add(dependency1);
            var dependency2 = new DataDependency<int>(n0);
            n2.StackDependencies.Add(dependency2);

            Assert.Equal(new HashSet<IDataFlowNode>
            {
                n1, n2
            }, new HashSet<IDataFlowNode>(n0.GetDependants()));
        }

        [Fact]
        public void RemoveStackDependencyShouldAddToDependants()
        {
            var dfg = new DataFlowGraph<int>();
            var n0 = dfg.Nodes.Add(0, 0);
            var n1 = dfg.Nodes.Add(1, 1);
            var n2 = dfg.Nodes.Add(2, 2);

            var dependency1 = new DataDependency<int>(n0);
            n1.StackDependencies.Add(dependency1);
            var dependency2 = new DataDependency<int>(n0);
            n2.StackDependencies.Add(dependency2);

            n1.StackDependencies.Remove(dependency1);

            Assert.Equal(new HashSet<IDataFlowNode>
            {
                n2
            }, new HashSet<IDataFlowNode>(n0.GetDependants()));
        }

        [Fact]
        public void AddDependencyToAnotherGraphShouldThrow()
        {
            var dfg1 = new DataFlowGraph<int>();
            var n1 = dfg1.Nodes.Add(1, 0);
            
            var dfg2 = new DataFlowGraph<int>();
            var n2 = dfg2.Nodes.Add(2, 0);

            Assert.Throws<ArgumentException>(() =>
                n1.StackDependencies.Add(new DataDependency<int>(n2)));
        }

        [Fact]
        public void AddDataSourceToAnotherGraphShouldThrow()
        {
            var dfg1 = new DataFlowGraph<int>();
            var n1 = dfg1.Nodes.Add(1, 0);
            
            var dfg2 = new DataFlowGraph<int>();
            var n2 = dfg2.Nodes.Add(2, 0);

            n1.StackDependencies.Add(new DataDependency<int>());
            Assert.Throws<ArgumentException>(() => n1.StackDependencies[0].DataSources.Add(n2));
        }
    }
}