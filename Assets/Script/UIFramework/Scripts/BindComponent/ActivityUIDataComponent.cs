/*------------------------------------
 *Title: UI自动化生成脚本工具  
 *Author: ZiMing  
 *Date:  2025/6/10 6:11:39
 *Description: 组件需以 [Text]组件名 格式命名，右键窗口物体--->生成数据组件脚本 
 *注意：以下代码均由脚本生成，任何手动修改将有可能被下次生成覆盖，尽量避免再次生成！
 -------------------------------------*/
using UnityEngine.UI;
using UnityEngine;

namespace Ming_UIFramework
{
	public class ActivityUIDataComponent : MonoBehaviour
	{
		 public Text titleText;

		 public Image mainImage;

		 public Text DesText;

		 public Button CloseButton;

		public void InitComponent(UIBase target)
		{
		        //事件绑定
		        ActivityUI m_UI=(ActivityUI)target;
		        target.AddButtonClickListener(CloseButton,m_UI.OnCloseButtonClick);
		}
	}
}
