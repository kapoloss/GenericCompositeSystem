using System.Linq;

namespace CompositePattern.GenericCompositePattern;

public static class TreeHelper
{
    public static GenericComposite<T> CreateTreeFromJson<T>(TreeNode<T> jsonNode)
    {
        var root = new GenericComposite<T>(jsonNode.Data);

        if (jsonNode.Children == null) return root;

        foreach (var childNode in jsonNode.Children)
            root.Add(CreateTreeFromJson(childNode));

        return root;
    }

    public static TreeNode<T> ToJson<T>(GenericComposite<T> genericComposite)
    {
        return new TreeNode<T>
        {
            Data = genericComposite.Data,
            Children = genericComposite.GetChildren().Select(ToJson).ToList()
        };
    }
}
