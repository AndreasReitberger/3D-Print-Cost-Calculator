using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.ViewModels.Helix
{
    public class AttachedNodeViewModel : ViewModelBase
    {
        bool selected = false;
        public bool Selected
        {
            set
            {
                if (SetValue(ref selected, value))
                {
                    if (node is MeshNode m)
                    {
                        m.PostEffects = value ? $"highlight[color:#FFFF00]" : "";
                        foreach (var n in node.TraverseUp())
                        {
                            if (n.Tag is AttachedNodeViewModel vm)
                            {
                                vm.Expanded = true;
                            }
                        }
                    }
                }
            }
            get => selected;
        }

        bool expanded = false;
        public bool Expanded
        {
            set => SetValue(ref expanded, value);
            get => expanded;
        }

        public bool IsAnimationNode { get => node.IsAnimationNode; }

        public string Name { get => node.Name; }

        SceneNode node;

        public AttachedNodeViewModel(SceneNode node)
        {
            this.node = node;
            node.Tag = this;
        }
    }
}
