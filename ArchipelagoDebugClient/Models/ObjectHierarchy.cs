using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace ArchipelagoDebugClient.Models;

public record ObjectHierarchy
{
    public string Name { get; }
    public string Type { get; }
    public object? Value { get; }
    public IReadOnlyList<ObjectHierarchy> Children { get; }

    public ObjectHierarchy(string name, JToken token)
    {
        Name = name;
        if (token.Type == JTokenType.Array)
        {
            Type = "Array";
            Children = token.Children().Select((t, i) => new ObjectHierarchy(i.ToString(), t)).ToList();
        }
        else if (token.Type == JTokenType.Object)
        {
            Type = "Object";
            Children = token.Children<JProperty>().Select(t => new ObjectHierarchy(t.Name, t.Value)).ToList();
        }
        else
        {
            Type = token.Type.ToString();
            Value = token.Value<object>();
            Children = [];
        }
    }

    public ObjectHierarchy(string name, string type, object? value)
    {
        Name = name;
        Type = type;
        Value = value;
        Children = [];
    }

    public ObjectHierarchy(string name, string type, IReadOnlyList<ObjectHierarchy> children)
    {
        Name= name;
        Type = type;
        Children = children;
    }

    public static IEnumerable<ObjectHierarchy> GetHierarchyLists(JObject obj)
    {
        foreach (JProperty prop in obj.Properties())
        {
            yield return new ObjectHierarchy(prop.Name, prop.Value);
        }
    }
}
