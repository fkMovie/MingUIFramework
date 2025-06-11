/*------------------------------------
 *Title: UI自动化生成脚本工具  
 *Author: ZiMing  
 *Date:  2025/6/9 9:17:10
 *Description: 组件需以 [Text]组件名 格式命名，右键窗口物体--->生成组件查找脚本 
 *注意：以下代码均由脚本生成，任何手动修改将有可能被下次生成覆盖，尽量避免再次生成！
 -------------------------------------*/
using UnityEngine.UI;
using UnityEngine;

namespace Ming_UIFramework
{
	public class SettingUIComponent
	{
		 public Slider SoundSlider;

		 public Slider SFXSlider;

		 public Dropdown resolutionDropdown;

		 public Toggle muteToggle;

		 public Button CloseButton;

		public void InitComponent(UIBase target)
		{
		        //组件查找
		        SoundSlider = target.transform.Find("UIContent/BaseMap/[Slider]Sound").GetComponent<Slider>();
		        SFXSlider = target.transform.Find("UIContent/BaseMap/[Slider]SFX").GetComponent<Slider>();
		        resolutionDropdown = target.transform.Find("UIContent/BaseMap/[Dropdown]resolution").GetComponent<Dropdown>();
		        muteToggle = target.transform.Find("UIContent/BaseMap/[Toggle]mute").GetComponent<Toggle>();
		        CloseButton = target.transform.Find("UIContent/[Button]Close").GetComponent<Button>();
	
	
		        //事件绑定
		        SettingUI m_UI=(SettingUI)target;
		        target.AddToggleClickListener(muteToggle,m_UI.OnmuteToggleChange);
		        target.AddButtonClickListener(CloseButton,m_UI.OnCloseButtonClick);
		}
	}
}
