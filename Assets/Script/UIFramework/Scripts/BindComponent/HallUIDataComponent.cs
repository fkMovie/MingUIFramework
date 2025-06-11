/*------------------------------------
 *Title: UI自动化生成脚本工具  
 *Author: ZiMing  
 *Date:  2025/6/9 8:47:03
 *Description: 组件需以 [Text]组件名 格式命名，右键窗口物体--->生成数据组件脚本 
 *注意：以下代码均由脚本生成，任何手动修改将有可能被下次生成覆盖，尽量避免再次生成！
 -------------------------------------*/
using UnityEngine.UI;
using UnityEngine;

namespace Ming_UIFramework
{
	public class HallUIDataComponent : MonoBehaviour
	{
		 public Button ChatButton;

		 public Button SettingButton;

		 public Button HomeButton;

		public void InitComponent(UIBase target)
		{
		        //事件绑定
		        HallUI m_UI=(HallUI)target;
		        target.AddButtonClickListener(ChatButton,m_UI.OnChatButtonClick);
		        target.AddButtonClickListener(SettingButton,m_UI.OnSettingButtonClick);
		        target.AddButtonClickListener(HomeButton,m_UI.OnHomeButtonClick);
		}
	}
}
