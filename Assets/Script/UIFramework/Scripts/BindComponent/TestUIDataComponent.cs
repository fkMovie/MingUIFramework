/*------------------------------------
 *Title: UI自动化生成脚本工具  
 *Author: ZiMing  
 *Date:  2025/6/8 8:47:36
 *Description: 组件需以 [Text]组件名 格式命名，右键窗口物体--->生成数据组件脚本 
 *注意：以下代码均由脚本生成，任何手动修改将有可能被下次生成覆盖，尽量避免再次生成！
 -------------------------------------*/
using UnityEngine.UI;
using UnityEngine;

namespace Ming_UIFramework
{
	public class TestUIDataComponent : MonoBehaviour
	{
		 public Image testImage;

		 public Text titleText;

		 public InputField pwdInputField;

		 public Button TestButton;

		 public Button CloseButton;

		public void InitComponent(UIBase target)
		{
		        //事件绑定
		        TestUI m_UI=(TestUI)target;
		        target.AddInputFieldListener(pwdInputField,m_UI.OnpwdInputChange,m_UI.OnpwdInputEnd);
		        target.AddButtonClickListener(TestButton,m_UI.OnTestButtonClick);
		        target.AddButtonClickListener(CloseButton,m_UI.OnCloseButtonClick);
		}
	}
}
