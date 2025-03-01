using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace ArchipelagoDebugClient.Models;

public record ObjectHierarchy
{
    public string Name { get; }
    public string Type { get; }
    public object? Value { get; }
    public bool IsRoot { get; }
    public bool Expanded { get; set; }
    public IReadOnlyList<ObjectHierarchy> Children { get; }

    public ObjectHierarchy(string name, JToken token, bool root = true)
    {
        Name = name;
        IsRoot = root;
        Expanded = false;
        if (token.Type == JTokenType.Array)
        {
            Type = "Array";
            Children = token.Children().Select((t, i) => new ObjectHierarchy(i.ToString(), t, false)).ToList();
            if (Children.Count == 0)
            {
                Value = "[]";
            }
        }
        else if (token.Type == JTokenType.Object)
        {
            Type = "Object";
            Children = token.Children<JProperty>().Select(t => new ObjectHierarchy(t.Name, t.Value, false)).ToList();
            if (Children.Count == 0)
            {
                Value = "{}";
            } 
        }
        else
        {
            Type = token.Type.ToString();
            Value = token.Value<object>();
            Children = [];
        }
    }

    public void CopyExpandedState(ObjectHierarchy orig)
    {
        Expanded = orig.Expanded;
        foreach (ObjectHierarchy child in orig.Children)
        {
            if (child.Expanded)
            {
                // that child is expanded, try to find a matching child in this one and propagate its expanded state also
                ObjectHierarchy? myChild = Children.FirstOrDefault(c => c.Name == child.Name);
                myChild?.CopyExpandedState(child);
            }
        }
    }

    public static IEnumerable<ObjectHierarchy> GetHierarchyLists(JObject obj)
    {
        foreach (JProperty prop in obj.Properties())
        {
            yield return new ObjectHierarchy(prop.Name, prop.Value);
        }
    }
}
