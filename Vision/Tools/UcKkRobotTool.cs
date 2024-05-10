using System.ComponentModel;
using System.Windows.Forms;

using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcKkRobotTool : UserControl
    {
        public UcKkRobotTool(KkRobotCalibTool tool)
        {
            InitializeComponent();
            _tool = tool;
        }

        private readonly KkRobotCalibTool _tool;


        private void UcKRTool_Load(object sender, System.EventArgs e)
        {
            if (_tool != null)
            {
                cogCalibNPointToNPointEditV21.Subject = _tool.CalibTool;
            }
        }
    }
}
