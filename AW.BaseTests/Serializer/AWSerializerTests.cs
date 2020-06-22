using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using AW.Base.Serializer.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AW.Base.Serializer.Tests
{
    [TestClass()]
    public class AWSerializerTests
    {
        [Common.AWSerializable]
        public class Test
        {
            public DateTime Date { get; set; } = DateTime.Now;
            public double D { get; set; } = 2.09;

            public List<int> LI { get; set; } = new List<int>
            {
                1, 2, 3, 4, 5
            };

            public int[] AI { get; set; } = new int[2] { 1, 2 };

            public List<string> LS { get; set; } = new List<string>
            {
                "222", "asasa", "dwww"
            };

            public Dictionary<string, string> DS { get; set; } = new Dictionary<string, string>
            {
                { "s1", "asas" }, { "s2", "asasas" }, { "s3", "aghghsas" }
            };

            public ObservableCollection<string> Vs = new ObservableCollection<string>
            {
                "sdfghnm"
            };
        }

        [TestMethod()]
        public void SerializeTest()
        {
            Test test = new Test();
            string data = null;

            using (AWSerializer serializer = new AWSerializer())
            {
                data = serializer.Serialize(test);
            }

            test = null;

            using (AWSerializer serializer = new AWSerializer())
            {
                test = serializer.Deserialize<Test>(data);
            }

            Assert.IsTrue(test != null);
        }

        [TestMethod()]
        public void SerializeToFileTest()
        {
            Test test = new Test();
            string data = null;

            using (AWSerializer serializer = new AWSerializer())
            {
                data = serializer.Serialize(test);
            }

            SerializerHelper.SaveText(data, "fileName");

            data = null;
            test = null;

            data = SerializerHelper.LoadText("fileName");

            using (AWSerializer serializer = new AWSerializer())
            {
                test = serializer.Deserialize<Test>(data);
            }

            Assert.IsTrue(test != null);
        }

        [AWSerializable]
        public class Reference : IReference
        {
            public int ReferenceId { get; set; }
        }

        [AWSerializable]
        public class TestReference
        {
            public Reference F1 { get; set; } = new Reference();

            [AWReference]
            public List<Reference> References { get; set; } = new List<Reference>();

            public TestReference()
            {
                References.Add(F1);
                References.Add(F1);
                References.Add(F1);
                References.Add(F1);
            }
        }

        [TestMethod()]
        public void SerializeHardTest()
        {
            var test = new MenuGroupItemContext
            {
                Name = "pages",
                CreateItemHint = "add_page",
                CreateGroupHint = "add_folder",
                ViewRemove = false,
                ViewRename = false,
                IsOpen = true
            };
            test.AddItem(new MenuItemContext()
            {
                Name = "name"
            });
            string data = null;

            using (AWSerializer serializer = new AWSerializer())
            {
                data = serializer.Serialize(test);
            }

            SerializerHelper.SaveText(data, "fileName");

            data = null;
            test = null;

            data = SerializerHelper.LoadText("fileName");

            using (AWSerializer serializer = new AWSerializer())
            {
                test = serializer.Deserialize<MenuGroupItemContext>(data);
            }

            Assert.IsTrue(test != null);
        }

        public class BaseContext
        {
            public void Notify(string name = "") { }
        }

        public interface IMenuItem : IReference
        {
            Func<IMenuItem, bool> OnSelect { get; set; }
            Func<IMenuItem, bool> OnRemove { get; set; }
            Func<IMenuItem, string, bool> OnRename { get; set; }

            IMenuGroup Group { get; set; }

            int Left { get; set; }
            bool IsSelect { get; set; }

            bool ViewRemove { get; }
            bool ViewRename { get; }
            bool CanChangeGroup { get; }


            string Name { get; set; }

            object Content { get; set; }
            object Context { get; set; }

            IEnumerable<string> Names { get; }
        }

        public interface IMenuGroup : IMenuItem
        {
            Func<IMenuGroup, string, bool> OnCreateItem { get; set; }
            Func<IMenuGroup, string, bool> OnCreateGroup { get; set; }
            Func<IMenuItem, IMenuGroup, bool> OnChangeGroup { get; set; }

            bool IsOpen { get; set; }

            bool NeedSortItems { get; }
            bool ViewCreateItem { get; }
            bool ViewCreateGroup { get; }

            string CreateItemHint { get; set; }
            string CreateGroupHint { get; set; }

            ObservableCollection<IMenuItem> Source { get; }

            void AddItem(IMenuItem item);
            void RemoveItem(IMenuItem item);
            void Clear();
        }

        [AWSerializable]
        public class MenuItemContext : BaseContext, IMenuItem
        {
            [AWReference]
            public IMenuGroup Group { get; set; }

            [AWIgnore]
            public Func<IMenuItem, bool> OnSelect { get; set; }
            [AWIgnore]
            public Func<IMenuItem, bool> OnRemove { get; set; }
            [AWIgnore]
            public Func<IMenuItem, string, bool> OnRename { get; set; }

            public int Left { get; set; }

            private bool isSelect;
            public virtual bool IsSelect
            {
                get => isSelect;
                set
                {
                    isSelect = value;
                    Notify();
                }
            }

            public bool ViewRemove { get; set; } = true;
            public bool ViewRename { get; set; } = true;
            public bool CanChangeGroup { get; set; }


            private string name;
            public string Name
            {
                get => name;
                set
                {
                    name = value;
                    Notify();
                }
            }

            public object Content { get; set; }
            public object Context { get; set; }

            public virtual IEnumerable<string> Names => new List<string> { Name.ToLower() };

            public int ReferenceId { get; set; }
        }

        [AWSerializable]
        public class MenuGroupItemContext : MenuItemContext, IMenuGroup
        {
            public MenuGroupItemContext()
                => Group = this;

            [AWIgnore]
            public Func<IMenuGroup, string, bool> OnCreateItem { get; set; }
            [AWIgnore]
            public Func<IMenuGroup, string, bool> OnCreateGroup { get; set; }
            [AWIgnore]
            public Func<IMenuItem, IMenuGroup, bool> OnChangeGroup { get; set; }

            public bool IsOpen { get; set; }
            public override bool IsSelect
            {
                get => false;
                set
                {
                }
            }

            public bool NeedSortItems { get; set; } = true;

            public bool ViewCreateItem { get; set; } = true;
            public bool ViewCreateGroup { get; set; } = true;

            public string CreateItemHint { get; set; }
            public string CreateGroupHint { get; set; }

            public ObservableCollection<IMenuItem> Source { get; set; } = new ObservableCollection<IMenuItem>();


            public void AddItem(IMenuItem item)
            {
                item.Left = Left + 40;
                item.Group = this;

                item.OnRemove ??= OnRemove;
                item.OnRename ??= OnRename;
                item.OnSelect ??= OnSelect;

                if (item is IMenuGroup group)
                {
                    group.OnCreateItem ??= OnCreateItem;
                    group.OnCreateGroup ??= OnCreateGroup;
                    group.OnChangeGroup ??= OnChangeGroup;

                    group.CreateItemHint ??= CreateItemHint;
                    group.CreateGroupHint ??= CreateGroupHint;

                }
                Source.Add(item);

            }

            public void RemoveItem(IMenuItem item)
            {
                Source.Remove(item);
            }

            public void Clear()
            {
            }
        }
    }
}