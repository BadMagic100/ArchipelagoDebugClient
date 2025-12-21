using ArchipelagoDebugClient.Models;
using ArchipelagoDebugClient.Services;
using DynamicData;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ArchipelagoDebugClient.ViewModels.DesignData;

public class SlotDataDesignData : SlotDataViewModel
{
    private record TestSlotDataModelInner(string Name, int Amount);
    private record TestSlotDataModel(string Test, TestSlotDataModelInner Inner, List<TestSlotDataModelInner> ManyInner);

    public SlotDataDesignData() : base(new SessionProvider())
    {
        JObject obj = JObject.FromObject(new TestSlotDataModel("Foo", new("Bar", 20), [new("Baz", 50), new("Biff", 100)]));
        SlotDataFields.AddRange(ObjectHierarchy.GetHierarchyLists(obj));
    }
}
